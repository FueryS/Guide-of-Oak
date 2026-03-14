using System.Collections;
using UnityEngine;

/// <summary>
/// Enemy performs a single attack then returns to chasing.
///
/// Timeline within one attack:
///   [0 ──── attackHitboxActiveDuration]  hitbox isLive = true  (player can be hit / parried)
///   [attackHitboxActiveDuration ── attackDuration]  hitbox isLive = false
///   [attackDuration + attackCooldown]  → back to ChaseState
///
/// On enter: plays the attack AudioResource and sets the enemy's color to attackColor.
/// On exit:  restores the enemy's default color.
///
/// The coroutine is owned by EnemyStateManager (a MonoBehaviour) since state
/// class instances are plain C# objects with no coroutine support of their own.
/// </summary>
public class EnemyAttackState : EnemyBaseState
{
    // -------------------------------------------------------------------------
    #region Private Variables

    private Coroutine _attackRoutine;

    #endregion
    // -------------------------------------------------------------------------
    #region Enter / Exit

    public override void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("[Enemy] Attack");

        // ── Play attack sound ─────────────────────────────────────────────────
        // AudioResource is assigned on the AudioSource via .resource then played.
        // Guards ensure missing components never throw.
        if (enemy.audioSource != null && enemy.attackSound != null)
        {
            enemy.audioSource.resource = enemy.attackSound;
            enemy.audioSource.Play();
        }

        // ── Flash attack color ────────────────────────────────────────────────
        if (enemy.enemyRenderer != null)
            enemy.enemyRenderer.material.color = enemy.attackColor;

        // Stop any leftover routine from a previous interrupted attack
        if (_attackRoutine != null)
            enemy.StopCoroutine(_attackRoutine);

        _attackRoutine = enemy.StartCoroutine(AttackSequence(enemy));
    }

    public override void ExitState(EnemyStateManager enemy)
    {
        // Restore the default color regardless of how the state was exited
        if (enemy.enemyRenderer != null)
            enemy.enemyRenderer.material.color = enemy.defaultColor;

        // Ensure the hitbox is always deactivated when leaving this state
        if (enemy.attackHitbox != null)
            enemy.attackHitbox.isLive = false;

        if (_attackRoutine != null)
        {
            enemy.StopCoroutine(_attackRoutine);
            _attackRoutine = null;
        }
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Update

    public override void UpdateState(EnemyStateManager enemy)
    {
        // The coroutine drives this state — nothing to poll each frame.
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Attack Sequence Coroutine

    /// <summary>
    /// Drives the full attack lifecycle:
    ///   activate hitbox → wait → deactivate hitbox → cooldown → return to chase.
    /// </summary>
    private IEnumerator AttackSequence(EnemyStateManager enemy)
    {
        // ── Phase 1: Hitbox active window ─────────────────────────────────────
        if (enemy.attackHitbox != null)
            enemy.attackHitbox.isLive = true;

        yield return new WaitForSeconds(enemy.attackHitboxActiveDuration);

        // ── Phase 2: Hitbox inactive — rest of attack animation ───────────────
        if (enemy.attackHitbox != null)
            enemy.attackHitbox.isLive = false;

        float remainingAttackTime = enemy.attackDuration - enemy.attackHitboxActiveDuration;
        if (remainingAttackTime > 0f)
            yield return new WaitForSeconds(remainingAttackTime);

        // ── Phase 3: Cooldown before next attack ──────────────────────────────
        yield return new WaitForSeconds(enemy.attackCooldown);

        _attackRoutine = null;

        // Return to chase — the chase state will re-enter attack if still in range
        enemy.SwitchState(enemy.chaseState);
    }

    #endregion
    // -------------------------------------------------------------------------
}