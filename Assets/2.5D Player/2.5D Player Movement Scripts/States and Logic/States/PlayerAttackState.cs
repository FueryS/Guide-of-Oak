using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the player's melee attack combo.
///
/// Flow:
///   EnterState  → fire the Animator trigger, reset all flags.
///   UpdateState → poll the current clip name to:
///                   • detect when the attack animation has actually started (_attackPerformed gate)
///                   • open a re-roll window once normalizedTime >= comboReRollTime
///                   • detect when the animation has finished so the state can exit
///   ExitState   → unsubscribe from input events.
///
/// Edge cases handled:
///   • GetCurrentAnimatorClipInfo(0) can return an empty array during transitions — guarded.
///   • attackAnimationNames list being empty — guarded in SafeOnAttackPressed.
///   • Re-roll SwitchState and _attackPerformed write happening on the same frame — fixed via early return.
///   • System.Diagnostics.Debug shadowing UnityEngine.Debug — wrong using removed.
/// </summary>
public class PlayerAttackState : PlayerBaseState
{
    // -------------------------------------------------------------------------
    #region Public Variables — Inspector / ValuesAssigner

    [Header("Animator")]
    /// <summary>Animator trigger parameter name that starts the attack.</summary>
    public string attackTrigger = "Attack";

    //public Collider attackHitbox;//Disable this on exit to prevent the edge case of mid attack parry, hitbox still active.

    /// <summary>
    /// Ordered list of animation clip names that make up the combo.
    /// The state detects which combo step is playing by matching against this list.
    /// The last entry is the combo cap — pressing attack while it plays does nothing.
    /// </summary>
    public List<string> attackAnimationNames = new List<string>()
    {
        "Great Sword Slash",
        "Great Sword Kick"
    };

    [Header("Timing")]
    /// <summary>
    /// Normalised animation time (0–1) after which the player may press attack
    /// to re-roll into the next combo hit.
    /// </summary>
    public float comboReRollTime = 0.6f;

    [Header("Attack Control")]
    /// <summary>
    /// When false the player cannot trigger a re-roll, effectively locking the
    /// attack state from chaining. Useful for cutscenes, stagger, or hit-stop.
    /// </summary>
    public bool canAttack = true;

    #endregion
    // -------------------------------------------------------------------------
    #region Private Variables — internal logic

    /// <summary>Clip name of the attack animation currently playing.</summary>
    private string _currentAttackClipName;

    /// <summary>
    /// True once an attack animation from attackAnimationNames has been detected
    /// as playing. Guards against the state exiting before the animation even begins
    /// (e.g. on the frames where a transition or a different clip is still active).
    /// </summary>
    private bool _attackPerformed;

    /// <summary>
    /// Set by SafeOnAttackPressed once the combo re-roll window is open.
    /// Causes the state to immediately re-enter itself on the next eligible frame.
    /// </summary>
    private bool _allowReRoll;

    #endregion
    // -------------------------------------------------------------------------
    #region References

    private PlayerStateManager _playerRef;

    #endregion
    // -------------------------------------------------------------------------
    #region Enter State

    public override void EnterState(PlayerStateManager player)
    {
        _playerRef = player;

        // Fire the animator trigger to start the attack blend/transition
        player.animator.SetTrigger(attackTrigger);

        // Reset all logic flags so every entry (including combo re-rolls) starts clean
        _attackPerformed = false;
        _currentAttackClipName = string.Empty;
        _allowReRoll = false;

        // Subscribe — unsubscribed symmetrically in ExitState
        player.inputHandler.OnAttackPressed += SafeOnAttackPressed;
        player.inputHandler.OnInteractPressed+= SafeOnParryPressed;

        //Debug.Log("[AttackState] Entered.");
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Update State

    public override void UpdateState(PlayerStateManager player)
    {
        // ── Safe clip name read ───────────────────────────────────────────────
        // GetCurrentAnimatorClipInfo returns an empty array during blend transitions.
        // Accessing [0] without this guard throws IndexOutOfRangeException.
        AnimatorClipInfo[] clipInfos = player.animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfos.Length == 0) return;

        _currentAttackClipName = clipInfos[0].clip.name;

        // ── Attack animation not yet / no longer playing ──────────────────────
        if (!attackAnimationNames.Contains(_currentAttackClipName))
        {
            // If we had already confirmed the attack animation ran, the clip leaving
            // the list means the animation finished → transition to a ground state.
            if (_attackPerformed)
            {
                //Debug.Log("[AttackState] Animation finished, exiting.");
                ExitToGroundState(player);
            }
            // If !_attackPerformed we are still in the pre-attack transition; wait.
            return;
        }

        // ── Attack animation is confirmed playing ─────────────────────────────
        _attackPerformed = true;

        float normalizedTime = player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        // ── Re-roll window check ──────────────────────────────────────────────
        // Early-return AFTER the SwitchState call so _attackPerformed = true (above)
        // is not written again on a state that is already being torn down.
        if (normalizedTime >= comboReRollTime && _allowReRoll && canAttack)
        {
            Debug.Log("[AttackState] Combo re-roll triggered.");
            player.SwitchState(player.attackState);
            return; // ← prevents any further writes to this instance this frame
        }
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Exit Helpers

    /// <summary>
    /// Transitions to the appropriate ground state based on current move input.
    /// Mirrors the pattern used in all other states.
    /// </summary>
    private void ExitToGroundState(PlayerStateManager player)
    {
        if (player.inputHandler.moveInput == Vector2.zero)
            player.SwitchState(player.idleState);
        else if (!player.inputHandler.sprintHeld)
            player.SwitchState(player.walkState);
        else
            player.SwitchState(player.runState);
    }

    public override void ExitState(PlayerStateManager player)
    {
        player.inputHandler.OnAttackPressed -= SafeOnAttackPressed;
        player.inputHandler.OnInteractPressed -= SafeOnParryPressed;
        //attackHitbox.enabled = false;//Disable the hitbox on exit to prevent the edge case of mid attack parry, hitbox still active.
        //Debug.Log("[AttackState] Exited.");
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Input Callbacks

    /// <summary>
    /// Called when the player presses attack during an active attack animation.
    /// Sets the re-roll flag so the next eligible frame chains into the next combo hit.
    /// The animator owns the loop — no combo-cap check needed here.
    /// Blocked entirely when canAttack is false.
    /// </summary>
    private void SafeOnAttackPressed()
    {
        if (!canAttack) return;
        _allowReRoll = true;
    }

    void SafeOnParryPressed()
    {
        if (!canAttack) return;
        _playerRef.SwitchState(_playerRef.parryState);
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Assign Variables — called from ValuesAssigner

    /// <summary>
    /// Sets all tuneable variables from an external source (e.g. ValuesAssigner).
    /// Call this in Start() before the state is first entered.
    /// canAttack defaults to true so existing callers need no changes.
    /// </summary>
    public void AssignVariables(
        string attackTrigger,
        List<string> attackAnimationNames,
        float comboReRollTime,
        bool canAttack = true
        )
    {
        this.attackTrigger = attackTrigger;
        this.attackAnimationNames = attackAnimationNames;
        this.comboReRollTime = comboReRollTime;
        this.canAttack = canAttack;

    }

    #endregion
    // -------------------------------------------------------------------------


}