using UnityEngine;

/// <summary>
/// Enemy moves toward the player along the Z axis (2.5D).
/// Faces the player by rotating 0° or 180° on Y — same pattern as the player.
/// Transitions:
///   → AttackState  when within attackRange
///   → IdleState    when player leaves detectionRange
/// </summary>
public class EnemyChaseState : EnemyBaseState
{
    // -------------------------------------------------------------------------
    #region Enter / Exit

    public override void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("[Enemy] Chase");
    }

    public override void ExitState(EnemyStateManager enemy) { }

    #endregion
    // -------------------------------------------------------------------------
    #region Update

    public override void UpdateState(EnemyStateManager enemy)
    {
        if (enemy.playerTransform == null) return;

        float distanceToPlayer = enemy.DistanceToPlayer();

        // ── Lost the player ───────────────────────────────────────────────────
        if (distanceToPlayer > enemy.detectionRange)
        {
            enemy.SwitchState(enemy.idleState);
            return;
        }

        // ── Close enough to attack ────────────────────────────────────────────
        if (enemy.DistanceToPlayerZ() <= enemy.attackRange)
        {
            enemy.SwitchState(enemy.attackState);
            return;
        }

        // ── Move along Z axis toward the player ───────────────────────────────
        float direction = Mathf.Sign(enemy.playerTransform.position.z - enemy.transform.position.z);
        enemy.transform.Translate(new Vector3(0f, 0f, direction * enemy.moveSpeed * Time.deltaTime),
                                  Space.World);

        // ── Rotate to face movement direction (0° = +Z, 180° = −Z) ───────────
        enemy.transform.localRotation = Quaternion.Euler(0f, direction > 0f ? 0f : 180f, 0f);
    }

    #endregion
    // -------------------------------------------------------------------------
}