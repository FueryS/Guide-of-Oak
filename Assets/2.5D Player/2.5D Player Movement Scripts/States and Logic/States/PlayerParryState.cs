using UnityEngine;

public class PlayerParryState : PlayerBaseState
{
    // -------------------------------------------------------------------------
    #region Public Variables

    [Header("Animator Parameters")]
    public string parryTrigger = "Parry";
    public string parryBool = "ParryBool";


    [Header("Parry advanced")]
    public LayerMask isEnemy;
    public Collider hitbox;

    public float parryRadius = 1.7f;
    public float parryOrginHeight = 1f;

    public float parryWindow = 0.5f;

    [Header("Parry Control")]
    /// <summary>
    /// Set this to true externally (e.g. from a hitbox or combat manager)
    /// when a successful parry is detected. Drives the ParryBool blend value.
    /// </summary>
    public bool flagSuccessParry = false;

    /// <summary>
    /// Normalised animation time (0–1) that must be reached before the state
    /// is allowed to exit. Prevents cancelling the parry animation early.
    /// </summary>
    public float parryExitThreshold = 0.8f;

    #endregion
    // -------------------------------------------------------------------------
    #region Private Variables

    private PlayerStateManager _playerRef;

    float _startTime;

    #endregion
    // -------------------------------------------------------------------------
    #region Enter State

    public override void EnterState(PlayerStateManager player)
    {
        _playerRef = player;

        //flagSuccessParry = false;

        player.animator.SetTrigger(parryTrigger);
        player.animator.SetFloat(parryBool, 0f);

        player.inputHandler.OnAttackPressed += OnAttackPressed;
        //CheckSuccessParry();

        //Initiate the parry Window
        _startTime = Time.time;
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Update State

    public override void UpdateState(PlayerStateManager player)
    {
        CheckSuccessParry();
        // Mirror the success flag into the animator blend value each frame
        player.animator.SetFloat(parryBool, flagSuccessParry ? 1f : 0f);

        // IsName checks the active Animator STATE name, not the clip name.
        // This is required for blend trees since GetCurrentAnimatorClipInfo returns
        // the name of the clip playing INSIDE the blend tree, not the blend tree itself.
        bool inParryState = player.animator.GetCurrentAnimatorStateInfo(0).IsName(parryTrigger);

        if (!inParryState) return; // Still transitioning into the parry state, wait.

        float normalizedTime = player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        bool thresholdMet = normalizedTime >= parryExitThreshold;

        if (thresholdMet)
            ExitToGroundState(player);
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Exit Helpers

    private void ExitToGroundState(PlayerStateManager player)
    {
        if (player.inputHandler.moveInput == Vector2.zero)
            player.SwitchState(player.idleState);
        else if (!player.inputHandler.sprintHeld)
            player.SwitchState(player.walkState);
        else
            player.SwitchState(player.runState);
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Exit State

    public override void ExitState(PlayerStateManager player)
    {
        player.animator.SetFloat(parryBool, 0f);
        player.inputHandler.OnAttackPressed -= OnAttackPressed;
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Input Callbacks

    private void OnAttackPressed()
    {
        _playerRef.SwitchState(_playerRef.attackState);
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Assign Variables

    public void AssignVariables(string parryTrigger, string parryBool,LayerMask IsEnemy, float parryExitThreshold = 0.8f)
    {
        this.parryTrigger = parryTrigger;
        this.parryBool = parryBool;
        this.parryExitThreshold = parryExitThreshold;
        this.isEnemy = IsEnemy;
    }

    #endregion
    // -------------------------------------------------------------------------

    #region Parry Detection

    /// <summary>
    /// Casts an overlap sphere to find nearby enemies.
    /// If an enemy is found whose attack state is active AND their hitbox is live,
    /// the parry is a success — flagSuccessParry is set to true.
    /// UpdateState reads this flag each frame and writes the animator blend value.
    /// </summary>
    void CheckSuccessParry()
    {
        if (Time.time-_startTime > parryWindow) return; // Parry window has expired, no need to check.

        // Cast overlap sphere on the enemy layer
        Collider[] hits = Physics.OverlapSphere(_playerRef.transform.position+new  Vector3(0,parryOrginHeight,0), parryRadius, isEnemy);

        // No enemies nearby — parry cannot succeed
        if (hits.Length == 0)
        {
            flagSuccessParry = false;
            return;
        }

        // Check each enemy in range
        foreach (Collider hit in hits)
        {
            bool enemyHitboxIsLive = hit.GetComponent<EnemyAttackHitbox>().isLive;

            if (enemyHitboxIsLive)
            {
                Debug.Log("<color=green>Successful Parry!</color>");
                flagSuccessParry = true;
                return; // One valid attacker is enough — no need to check the rest
            }
        }

        // No enemy met both conditions
        flagSuccessParry = false;
    }

    #endregion
}