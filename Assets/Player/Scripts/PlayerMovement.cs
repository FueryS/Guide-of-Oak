using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerMovement : MonoBehaviour
{
    // --- Objects ---
    CharacterController controller;
    PlayerInputHandler input;
    Parkour pr; // Reference to the parkour script for ledge/wall detection

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;
    public float resistance = 1;

    [Header("Jump Settings")]
    // Replaced "Charge" settings with "Variable Height" settings
    [SerializeField] private float maxJumpHeight = 3f; // Height reached if button is HELD
    [SerializeField] private float wallJumpHeight = 2.5f; // Height for the wall/ledge jump
    [SerializeField] private int maxJumps = 2;

    // --- Physics State ---
    private Vector3 velocity;
    private bool isGrounded;
    private bool wasGrounded;
    private int jumpsRemaining;

    [Header("Trackers for effects")] 
    public bool isRunning;
    public bool isWalking;
    public bool isDashing;
    public bool isIdle;
    public Vector3 Move;

    public bool hasJumped;

    public bool isTouchingWall;
    private Vector3 wallNormal;

    //for wall run
    public bool doWallRun=false;
    public float wr_friction=1;
    // This variable tracks if the player is physically holding the jump button
    // We need this for the "Variable Jump Height" logic (Hollow Knight style)
    private bool isJumpHeld = false;

    [Header("Exposed variables")]
    public bool canDash = true;
    public bool canMultiJump = true;
    public bool canJump = true;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private float lastDashTime = -999f;

    [Header("Debugging")]
    [SerializeField] private bool debugMoves = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputHandler>();
        pr = GetComponentInChildren<Parkour>();

        // As we are using events for movement we will be calling these functions on actions
        input.OnJumpPressed += HandleJumpPress;   // Trigger jump immediately
        input.OnJumpReleased += HandleJumpRelease;// Track when button is let go
        input.OnDashPressed += HandleDash;

        // Resets the jump count in case if i had made an error of setting it to 0
        if (maxJumps < 1) maxJumps = 1;
        jumpsRemaining = maxJumps;

        if (wr_friction > 1) Debug.LogWarning($"<color=red>value {wr_friction} is not supported make sure its intentional\nIt will likely get ignored</color>");
    }

    private void Update()
    {
        // Check if player is on ground 
        HandleGroundCheck();
        // handle wll check
        UpdateWallContactSphereCast();
        // Handle player movement (Walk/Sprint)
        HandleMovement();
        // Apply gravity (Handle physics and variable jump arc)
        ApplyGravity();
        //Debug.Log(velocity);

        //if(wr_friction <1) wr_friction = 1;avoid deviding by 0
    }

    // ----------------- Ground Check -----------------
    private void HandleGroundCheck()
    {
        // This calls an inbuilt function called isGrounded to see if its grounded or not
        isGrounded = controller.isGrounded;

        // Just landed (was in air, now on ground)
        if (!wasGrounded && isGrounded)
        {
            isTouchingWall = false;// RESET WALL TRACKER on ground contact
            jumpsRemaining = maxJumps; // Reset all jumps on landing
        }

        // Just left the ground (walked or fell off a ledge)
        if (wasGrounded && !isGrounded)
        {
            // Consume base jump so only air jumps remain
            // If we have multi-jump enabled, set jumps to (max - 1)
            if (canMultiJump && jumpsRemaining == maxJumps)
                jumpsRemaining = Mathf.Max(1, maxJumps - 1);
        }

        // Have rested (Physics cleanup)
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep player snapped to ground
            SetDash(true);
        }

        // Keep track of player state for next frame
        wasGrounded = isGrounded;
    }

    // ----------------- Movement -----------------
    private void HandleMovement()
    {
        /* input.moveInput is the part of playerInputHandeler.cs which checks up/down/left/right 
           and returns a vector 2 */
        Vector2 moveInput = input.moveInput;

        // Determine the player direction of movement and store it in move variable
        // Multiply input strength with local x and z axis (y is 0)
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Ensure the actual movement speed is the intended (sprint or walking) action
        float targetSpeed = 0;
        if (input.sprintHeld && input.moveInput != Vector2.zero)
        {
            targetSpeed = sprintSpeed;
            isRunning = true;
            isWalking = false;
            isIdle = false;
        }
        else if (input.moveInput != Vector2.zero)
        {
            targetSpeed = walkSpeed;
            isWalking = true;
            isRunning = false;
            isIdle = false;
        }
        else
        {
            isWalking = false;
            isRunning = false;
            isIdle = true;
        }

        Move = resistance * targetSpeed * move;
        // Move is the inbuilt controller method its physics based uses vector 3
        controller.Move(Move * Time.deltaTime);

        if (debugMoves) Debug.Log("move:" + move);
    }

    // ----------------- Jump Handling -----------------

    // New function to house the requested SphereCast logic
    private void UpdateWallContactSphereCast()
    {
        // RaycastHit variable is required by SphereCast

        // Perform SphereCast using pr.detectDistance as requested
        bool hitForward = Physics.SphereCast(
            // Use center of character's height for a robust check
            transform.position + (Vector3.up / 2f),
            controller.radius,
            transform.forward,
            out RaycastHit hit, // Required output parameter
            pr.detectDistance,
            pr.walkableLayer
        );

        // Update the tracker variables
        isTouchingWall = hitForward;
        if (hitForward)
        {
            wallNormal = hit.normal;
        }
        // Debug.DrawRay(transform.position, transform.forward * pr.detectDistance, hitForward ? Color.magenta : Color.cyan);
    }

    // Called immediately when Space is pressed
    private void HandleJumpPress()
    {
        if (!canJump) return;

        isJumpHeld = true; // Set flag: Player is holding the button
       

        // 1. WALL JUMP CHECK
        // Replaced Physics.CheckSphere with the built-in controller collision tracker
        bool hitWall = isTouchingWall;


        // Logic: If we are in the air (!isGrounded), touching a wall (hitWall), 
        // BUT the parkour script says it's NOT a climbable ledge (!pr.ledgeDetect())...
        // Then perform a Wall Jump.
        if (!isGrounded && hitWall && !pr.LedgeDetect())
        {
            PerformJump(wallJumpHeight);
            // Optional: Apply velocity *away* from the wall for better feel
            // Vector3 wallJumpVelocity = wallNormal * 5f;
            // velocity.x = wallJumpVelocity.x;
            // velocity.z = wallJumpVelocity.z;
            return; // Exit so we don't do a normal jump too
        }

        // 2. STANDARD JUMP
        // Can jump if grounded, or has remaining air jumps
        bool validJump = (isGrounded) || (!isGrounded && canMultiJump && jumpsRemaining > 0);

        if (validJump)
        {
            PerformJump(maxJumpHeight);
            jumpsRemaining--;
        }
    }

    // Called when Space is released
    private void HandleJumpRelease()
    {
        // We update the flag. This triggers the gravity change in ApplyGravity()
        isJumpHeld = false;
    }

    // Calculates the physics force needed to reach specific height
    private void PerformJump(float targetHeight)
    {
        hasJumped = true;
        // Standard physics formula: v = sqrt(h * -2 * g)
        velocity.y = Mathf.Sqrt(targetHeight * -2f * gravity);
    }

    // ----------------- Gravity -----------------
    private void ApplyGravity()
    {
        float l_gravity = gravity;
        if (doWallRun & velocity.y < 0)
        {
            l_gravity = gravity/wr_friction;
        }
        // --- HOLLOW KNIGHT STYLE VARIABLE JUMP ---
        // Logic: If the player is moving UP (velocity.y > 0)
        // AND they have let go of the jump button (!isJumpHeld)...
        
        if (velocity.y > 0 && !isJumpHeld)
        {
            // Apply MUCH heavier gravity (3x) to cut the jump arc short.
            // This creates the "Short Hop" vs "Long Jump" feel.
            velocity.y += (l_gravity * 3f) * Time.deltaTime;
        }
        else
        {
            // Otherwise apply normal gravity
            velocity.y += l_gravity * Time.deltaTime;
        }

        // Applies a downward force and keeps the track of the collision during that
        CollisionFlags flag = controller.Move(velocity * Time.deltaTime);

        if ((flag & CollisionFlags.Above) != 0)
        {
            // If flag exist and an above collision is detected (Bonk Head), reset velocity
            velocity.y = 0;
        }
    }

    // ----------------- Dash Handeling ----------------
    private void HandleDash() // handles things that will happen when dash is called
    {
        // This keeps track of the cooldown
        if (isDashing || Time.time < lastDashTime + dashCooldown)
            return; // still cooling down or already dashing

        StartCoroutine(DashRoutine()); // StartCoroutine is used to start IEnumerators 
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        lastDashTime = Time.time; // Keep track of dash initiation time for cooldown logic

        Vector2 moveInput = input.moveInput; // take movement input for dash direction
        Vector3 dashDirection = transform.forward; // set a default forward direction

        // If player is pressing movement keys, dash in that direction
        if (moveInput.sqrMagnitude > 0.1f)
            // sqrMagnitude is x*x+y*y instead of mathf.Sqrt used in .magnitude (better for performance)
            dashDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;

        float dashSpeed = dashDistance / dashDuration;
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            // Move the character in a direction with speed
            controller.Move(dashSpeed * Time.deltaTime * dashDirection);
            yield return null; // wait till next frame
        }

        isDashing = false; // set dash flag to false
    }

    // ---------------- Utilities -----------------
    public void SetJump(int jumpCount) => jumpsRemaining = jumpCount; //for more flexiblity

    public void ResetJump() => SetJump(maxJumps);   //for external uses
    
    public void SetDash(bool value) => canDash = value;
    public float GetGravity() { return gravity; }
}