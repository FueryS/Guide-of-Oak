using UnityEngine;

/// <summary>
/// Enemy stands still and polls for the player entering detection range.
/// Transitions to ChaseState once the player is within detectionRange.
///
/// Also retries the player lookup each frame while playerTransform is null.
/// This handles the edge case where the player spawns after the enemy.
/// </summary>
public class EnemyIdleState : EnemyBaseState
{
    // -------------------------------------------------------------------------
    #region Enter / Exit

    public override void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("[Enemy] Idle");
    }

    public override void ExitState(EnemyStateManager enemy) { }

    #endregion
    // -------------------------------------------------------------------------
    #region Update

    public override void UpdateState(EnemyStateManager enemy)
    {
        // ── Player reference missing — retry the lookup ───────────────────────
        // If Start() ran before the player existed (e.g. spawned at runtime),
        // keep trying each frame until it's found before doing anything else.
        if (enemy.playerTransform == null)
        {
            enemy.TryFindPlayer();
            return;
        }

        if (enemy.DistanceToPlayer() <= enemy.detectionRange)
            enemy.SwitchState(enemy.chaseState);
    }

    #endregion
    // -------------------------------------------------------------------------
}