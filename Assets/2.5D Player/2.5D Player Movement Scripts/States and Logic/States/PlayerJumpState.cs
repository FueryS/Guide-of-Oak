using UnityEngine;

/// <summary>
/// Handles all jump behaviour for the player.
///
/// Jump Phases (internal enum, no separate substate classes needed):
///   WhileJump     – button held, upward force is actively applied
///   WhileRelease  – button released, upward velocity decays to 0, then gravity takes over
///   WhileAirJump  – same as WhileJump but triggered mid-air using an air-jump charge
///
/// Exit condition: state only switches back to ground states once GravityApplier.IsGrounded2D
/// is true AND the character has already left the ground at least once (prevents instant exit
/// on the same frame the jump started).
/// </summary>
public class PlayerJumpState : PlayerBaseState
{
    // -------------------------------------------------------------------------
    #region Public Variables — tweak in Inspector via PlayerStateManager or here

    AnimationEvents AnimEvent;

    [Header("Jump Force")]
    /// <summary>Upward velocity applied every frame while the jump button is held.</summary>
    public float jumpStrength = 12f;

    /// <summary>
    /// Minimum upward velocity guaranteed even on a tap (instant release).
    /// Enforced when the player releases before minJumpDuration has elapsed.
    /// </summary>
    public float minJumpStrength = 4f;

    /// <summary>Maximum time (seconds) the upward force can be applied while holding.</summary>
    public float maxJumpDuration = 0.4f;

    /// <summary>
    /// Minimum time (seconds) the upward force is applied regardless of early release.
    /// Ensures the character always clears a small height on a tap.
    /// </summary>
    public float minJumpDuration = 0.1f;

    /// <summary>
    /// How fast (units/s²) the upward velocity decays toward 0 after the button is released.
    /// Higher value = snappier arc cut; lower value = floatier feel.
    /// </summary>
    public float releaseDecaySpeed = 8f;

    [Header("Air Movement")]
    /// <summary>Target horizontal (Z-axis) speed while airborne.</summary>
    public float airMoveSpeed = 6f;

    /// <summary>Lerp factor for reaching airMoveSpeed — controls responsiveness in air.</summary>
    public float airMoveLerpSpeed = 3f;

    [Header("Air Jump")]
    /// <summary>Number of extra jumps allowed before landing again.</summary>
    public int maxAirJumps = 1;

    [Header("Animator Parameters")]
    public string jumpTrigger = "JumpStart";
    public string airJumpTrigger = "AirJump";

    #endregion
    // -------------------------------------------------------------------------
    #region Private Variables — internal state tracking

    /// <summary>Current jump phase. Drives which update logic runs each frame.</summary>
    private enum JumpPhase { WhileJump, WhileRelease, WhileAirJump }
    private JumpPhase _currentPhase;

    // Cached references set on EnterState
    private PlayerStateManager _playerRef;
    private GravityApplier2D _gravityApplier;

    // Jump force tracking
    private float _currentVerticalStrength; // Upward velocity being added to FinalMovement
    private float _jumpTimer;               // How long the current jump phase has been active
    private bool _minJumpComplete;         // True once minJumpDuration has elapsed
    private bool _jumpReleasedEarly;       // True if button released before minJumpDuration

    // Air movement tracking
    private float _airMoveVelocity;         // Smoothed horizontal velocity buffer

    // Grounding guard — prevents same-frame exit when jump starts on the ground
    private bool _hasLeftGround;

    // Air jump charge — reset on each ground jump, consumed per air jump
    private int _airJumpsRemaining;

    #endregion
    // -------------------------------------------------------------------------
    #region Assign Variables — called from ValuesAssigner or any external script

    /// <summary>
    /// Sets all tuneable jump variables from an external source (e.g. ValuesAssigner).
    /// Call this in Start() after the state has been constructed so values are ready
    /// before the first EnterState is invoked.
    /// </summary>
    public void AssignVariables(
        string jumpTrigger,
        string airJumpTrigger,
        float jumpStrength,
        float minJumpStrength,
        float maxJumpDuration,
        float minJumpDuration,
        float releaseDecaySpeed,
        float airMoveSpeed,
        float airMoveLerpSpeed,
        int maxAirJumps,
        AnimationEvents AnimEvent)
    {
        this.jumpTrigger = jumpTrigger;
        this.airJumpTrigger = airJumpTrigger;
        this.jumpStrength = jumpStrength;
        this.minJumpStrength = minJumpStrength;
        this.maxJumpDuration = maxJumpDuration;
        this.minJumpDuration = minJumpDuration;
        this.releaseDecaySpeed = releaseDecaySpeed;
        this.airMoveSpeed = airMoveSpeed;
        this.airMoveLerpSpeed = airMoveLerpSpeed;
        this.maxAirJumps = maxAirJumps;
        this.AnimEvent = AnimEvent;
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Enter State

    public override void EnterState(PlayerStateManager player)
    {
        _playerRef = player;
        _gravityApplier = player.gravityApplier;

        // Subscribe to jump input events
        player.inputHandler.OnJumpReleased += OnJumpReleased;
        player.inputHandler.OnJumpPressed += OnJumpPressed;

        if (!_gravityApplier.IsGrounded2D)
        {
            // ── Entered while airborne ──────────────────────────────────────
            // This happens when another state calls SwitchState(jumpState) while
            // already in the air (e.g. a future wall-jump) OR via OnJumpPressed
            // below which calls InitiateAirJump directly. Guard for both cases.
            if (_airJumpsRemaining > 0)
            {
                InitiateAirJump(player);
            }
            else
            {
                // No charges left — enter a dead release phase so state stays
                // alive until grounded, letting gravity handle everything.
                _currentPhase = JumpPhase.WhileRelease;
                _currentVerticalStrength = 0f;
            }

            _hasLeftGround = true; // Already airborne
        }
        else
        {
            // ── Ground jump ─────────────────────────────────────────────────
            _airJumpsRemaining = maxAirJumps; // Reset air-jump charges on landing
            InitiateGroundJump(player);
            _hasLeftGround = false;           // Will flip to true once CC lifts off
        }
    }

    /// <summary>Sets up a fresh ground jump: phase, velocity, timer, and animation.</summary>
    private void InitiateGroundJump(PlayerStateManager player)
    {
        _currentPhase = JumpPhase.WhileJump;
        _currentVerticalStrength = jumpStrength;
        _jumpTimer = 0f;
        _minJumpComplete = false;
        _jumpReleasedEarly = false;
        _airMoveVelocity = 0f;

        player.animator.SetTrigger(jumpTrigger);
        //Debug.Log("[JumpState] Ground Jump initiated.");
    }

    /// <summary>Consumes one air-jump charge and restarts the jump arc mid-air.</summary>
    private void InitiateAirJump(PlayerStateManager player)
    {
        _airJumpsRemaining--;

        _currentPhase = JumpPhase.WhileAirJump;
        _currentVerticalStrength = jumpStrength;
        _jumpTimer = 0f;
        _minJumpComplete = false;
        _jumpReleasedEarly = false;
        // Note: _airMoveVelocity intentionally NOT reset — preserves momentum feel

        player.animator.SetTrigger(airJumpTrigger);
        //Debug.Log($"[JumpState] Air Jump initiated. Remaining charges: {_airJumpsRemaining}");
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Update State

    public override void UpdateState(PlayerStateManager player)
    {
        // ── Grounding guard ──────────────────────────────────────────────────
        // Track the moment we become airborne so we don't exit on frame 0.
        if (!_hasLeftGround && !_gravityApplier.IsGrounded2D)
            _hasLeftGround = true;

        // ── Landing check ────────────────────────────────────────────────────
        // Only valid once the character has actually left the ground.
        if (_hasLeftGround && _gravityApplier.IsGrounded2D)
        {
            ExitToGroundState(player);
            return;
        }

        // ── Phase update ─────────────────────────────────────────────────────
        switch (_currentPhase)
        {
            case JumpPhase.WhileJump:
            case JumpPhase.WhileAirJump:
                UpdateActiveJumpPhase();
                break;

            case JumpPhase.WhileRelease:
                UpdateReleasePhase();
                break;
        }

        // ── Apply vertical movement ──────────────────────────────────────────
        // Only add positive upward force; gravity script handles the downward pull.
        if (_currentVerticalStrength > 0f)
            player.movementComplier.AddToFinalMovement(new Vector3(0f, _currentVerticalStrength, 0f));

        // ── Apply horizontal (air) movement ──────────────────────────────────
        HandleAirMovement(player);
    }

    // ── Phase: WhileJump / WhileAirJump ──────────────────────────────────────
    /// <summary>
    /// Advances the jump timer and handles transitions:
    ///   • After minJumpDuration  → honour any early release request.
    ///   • After maxJumpDuration  → force-transition to release.
    /// </summary>
    private void UpdateActiveJumpPhase()
    {
        _jumpTimer += Time.deltaTime;

        // Minimum duration gate
        if (!_minJumpComplete && _jumpTimer >= minJumpDuration)
        {
            _minJumpComplete = true;

            // Player released before minimum was met — now that it's met, cut the jump
            if (_jumpReleasedEarly)
            {
                TransitionToRelease();
                return;
            }
        }

        // Maximum duration reached — cap the jump even if button is still held
        if (_jumpTimer >= maxJumpDuration)
            TransitionToRelease();
    }

    // ── Phase: WhileRelease ──────────────────────────────────────────────────
    /// <summary>
    /// Decays upward velocity toward 0 each frame. Once 0, this method is a no-op
    /// and gravity in GravityApplier drives the fall until the state exits on landing.
    /// </summary>
    private void UpdateReleasePhase()
    {
        if (_currentVerticalStrength > 0f)
            _currentVerticalStrength = Mathf.MoveTowards(
                _currentVerticalStrength, 0f, releaseDecaySpeed * Time.deltaTime);
    }

    // ── Transition helper ────────────────────────────────────────────────────
    /// <summary>
    /// Switches to WhileRelease. Guarantees _currentVerticalStrength is at least
    /// minJumpStrength so a tap always produces a visible jump arc.
    /// </summary>
    private void TransitionToRelease()
    {
        if (_currentVerticalStrength < minJumpStrength)
            _currentVerticalStrength = minJumpStrength;

        _currentPhase = JumpPhase.WhileRelease;
        //Debug.Log("[JumpState] → WhileRelease phase.");
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Air Movement & Rotation

    /// <summary>
    /// Applies smoothed horizontal movement on the Z-axis and handles air rotation.
    /// The character will NOT rotate when moving in the direction opposite to its current facing.
    /// </summary>
    private void HandleAirMovement(PlayerStateManager player)
    {
        Vector2 moveInput = player.inputHandler.moveInput;
        float targetSpeed = moveInput.x * airMoveSpeed;

        // Smooth the horizontal velocity for a floaty-but-responsive air feel
        _airMoveVelocity = Mathf.Lerp(_airMoveVelocity, targetSpeed, airMoveLerpSpeed * Time.deltaTime);

        player.movementComplier.AddToFinalMovement(new Vector3(0f, 0f, _airMoveVelocity));

        // Rotation — restricted in air (see method below)
        HandleAirRotation(player, moveInput);
    }

    /// <summary>
    /// Mirrors ground rotation logic with one key difference:
    /// the character will only rotate toward a direction if it is ALREADY facing that direction.
    /// Moving "against" the current facing preserves the look direction (no mid-air flip).
    /// </summary>
    private void HandleAirRotation(PlayerStateManager player, Vector2 moveInput)
    {
        if (moveInput.x == 0f) return;

        float eulerY = player.transform.localRotation.eulerAngles.y;
        bool facingRight = eulerY < 90f || eulerY > 270f; // 0° = right, 180° = left
        bool movingRight = moveInput.x > 0f;

        // Only rotate when input direction matches current facing direction
        if (facingRight == movingRight)
            player.transform.localRotation = Quaternion.Euler(0f, movingRight ? 0f : 180f, 0f);
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Input Callbacks

    /// <summary>
    /// Called when the player releases the jump button.
    /// If minJumpDuration has passed → transition to release immediately.
    /// If not → flag for deferred transition once the minimum is met.
    /// </summary>
    private void OnJumpReleased()
    {
        if (_currentPhase != JumpPhase.WhileJump && _currentPhase != JumpPhase.WhileAirJump)
            return;

        if (_minJumpComplete)
            TransitionToRelease();
        else
            _jumpReleasedEarly = true; // UpdateActiveJumpPhase will call TransitionToRelease later
    }

    /// <summary>
    /// Called when the player presses jump again.
    /// Triggers an air jump if charges remain and the character is airborne.
    /// </summary>
    private void OnJumpPressed()
    {
        if (!_gravityApplier.IsGrounded2D && _airJumpsRemaining > 0)
            InitiateAirJump(_playerRef);
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Exit State

    /// <summary>
    /// Determines which ground state to transition into based on current move input.
    /// Mirrors the same pattern used in other states (idle / walk / run).
    /// </summary>
    private void ExitToGroundState(PlayerStateManager player)
    {
        AnimEvent.PlayFootSteps();
        Vector2 moveInput = player.inputHandler.moveInput;

        if (moveInput.x == 0f)
            player.SwitchState(player.idleState);
        else if (!player.inputHandler.sprintHeld)
            player.SwitchState(player.walkState);
        else
            player.SwitchState(player.runState);
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Always unsubscribe to prevent ghost callbacks
        player.inputHandler.OnJumpReleased -= OnJumpReleased;
        player.inputHandler.OnJumpPressed -= OnJumpPressed;

        // Zero out vertical force so no stale value bleeds into the next state
        _currentVerticalStrength = 0f;



        //Debug.Log("[JumpState] Exited.");
    }

    #endregion
    // -------------------------------------------------------------------------
}