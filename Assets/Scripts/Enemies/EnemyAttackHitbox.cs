using UnityEngine;

/// <summary>
/// Attached to the enemy's hitbox collider (set as Trigger in the Inspector).
///
/// isLive is toggled by EnemyAttackState to open and close the damage window.
/// isLive is also read by PlayerParryState.CheckSuccessParry() to determine
/// whether the parry was timed correctly — do NOT rename or remove this field.
///
/// Damage is only applied when:
///   • isLive is true (we are inside the attack's active window)
///   • The collider that entered is tagged "Player"
///   • The player's Stats component is present
/// </summary>
public class EnemyAttackHitbox : MonoBehaviour
{
    // -------------------------------------------------------------------------
    #region Public Variables

    /// <summary>
    /// Controls whether this hitbox can deal damage.
    /// Toggled by EnemyAttackState — also read externally by PlayerParryState.
    /// DO NOT rename: PlayerParryState.CheckSuccessParry references this field directly.
    /// </summary>
    public bool isLive = false;

    /// <summary>Amount of damage dealt to the player on a valid hit.</summary>
    public float damageAmount = 10f;

    #endregion
    // -------------------------------------------------------------------------
    #region Collision

    private void OnTriggerStay(Collider other)
    {
        // Only process hits while the attack window is open
        if (!isLive) return;

        // Only damage the player
        if (!other.CompareTag("Player")) return;

        Stats playerStats = other.GetComponentInChildren<Stats>();
        if (playerStats == null) return;

        playerStats.Damage(damageAmount);
    }

    #endregion
    // -------------------------------------------------------------------------
}