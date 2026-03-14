using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Central hub for the enemy AI.
/// Holds all state instances, shared references, and tunable values.
/// Exposes IsInAttackState() for the player's parry detection logic.
/// </summary>
public class EnemyStateManager : MonoBehaviour
{
    // -------------------------------------------------------------------------
    #region States

    [HideInInspector] public EnemyIdleState idleState = new EnemyIdleState();
    [HideInInspector] public EnemyChaseState chaseState = new EnemyChaseState();
    [HideInInspector] public EnemyAttackState attackState = new EnemyAttackState();

    private EnemyBaseState _currentState;

    #endregion
    // -------------------------------------------------------------------------
    #region References — set automatically on Awake / Start

    [HideInInspector] public EnemyStatManager statManager;
    [HideInInspector] public EnemyAttackHitbox attackHitbox;
    [HideInInspector] public AudioSource audioSource;

    /// <summary>
    /// Renderer used for attack color flash.
    /// Searches the root first, then children — handles meshes parented under the enemy root.
    /// </summary>
    [HideInInspector] public Renderer enemyRenderer;

    /// <summary>
    /// Cached reference to the player Transform for distance checks and movement.
    /// Populated in Start() — NOT Awake() — so cross-object lookup is safe.
    /// Retried every frame in idle if still null (handles runtime-spawned players).
    /// </summary>
    [HideInInspector] public Transform playerTransform;

    /// <summary>Cached default material color, restored when the attack state exits.</summary>
    [HideInInspector] public Color defaultColor;

    #endregion
    // -------------------------------------------------------------------------
    #region Inspector Tuning

    [Header("Detection")]
    /// <summary>Distance at which the enemy notices the player and begins chasing.</summary>
    public float detectionRange = 8f;

    [Header("Movement")]
    /// <summary>Movement speed along the Z axis while chasing.</summary>
    public float moveSpeed = 3f;

    [Header("Attack")]
    /// <summary>Distance at which the enemy stops chasing and starts attacking.</summary>
    public float attackRange = 1.5f;

    /// <summary>
    /// Total duration of the attack state (seconds).
    /// The hitbox is live for attackHitboxActiveDuration of this window, starting from the beginning.
    /// </summary>
    public float attackDuration = 1.2f;

    /// <summary>How long the hitbox stays active within one attack.</summary>
    public float attackHitboxActiveDuration = 0.4f;

    /// <summary>Cooldown (seconds) before the enemy can attack again after finishing an attack.</summary>
    public float attackCooldown = 1f;

    [Header("Attack Feedback")]
    /// <summary>Color the enemy flashes when entering the attack state.</summary>
    public Color attackColor = Color.red;

    /// <summary>
    /// AudioResource asset played when the enemy begins an attack.
    /// Requires Unity 6+. Assign in the Inspector.
    /// The AudioSource component on this GameObject is used for playback.
    /// </summary>
    public AudioResource attackSound;

    #endregion
    // -------------------------------------------------------------------------
    #region Unity Lifecycle

    private void Awake()
    {
        // ── Self-contained component lookups — safe in Awake ─────────────────
        // These only look at this GameObject and its children, so order doesn't matter.
        statManager = GetComponent<EnemyStatManager>();
        attackHitbox = GetComponentInChildren<EnemyAttackHitbox>();
        audioSource = GetComponent<AudioSource>();

        // Renderer is often on a child mesh object, not the root
        enemyRenderer = GetComponent<Renderer>() ?? GetComponentInChildren<Renderer>();

        if (enemyRenderer != null)
            // sharedMaterial avoids instantiating a new material just to read the color
            defaultColor = enemyRenderer.sharedMaterial.color;
        else
            Debug.LogWarning("[EnemyStateManager] No Renderer found — attack color flash disabled.");

        if (audioSource == null)
            Debug.LogWarning("[EnemyStateManager] No AudioSource found — attack sound disabled.");
    }

    private void Start()
    {
        // ── Cross-object lookup in Start, NOT Awake ───────────────────────────
        // All Awake() calls across every GameObject are guaranteed to complete
        // before any Start() runs. Doing FindGameObjectWithTag in Awake risks
        // running before the player GameObject has initialised, returning null
        // and silently locking the enemy in idle forever.
        TryFindPlayer();

        _currentState = idleState;
        _currentState.EnterState(this);
    }

    private void Update()
    {
        _currentState.UpdateState(this);
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Player Lookup

    /// <summary>
    /// Attempts to find and cache the player Transform by tag.
    /// Called from Start() and retried each frame in EnemyIdleState as a fallback
    /// for runtime-spawned players that don't exist at scene load.
    /// </summary>
    public void TryFindPlayer()
    {
        if (playerTransform != null) return; // Already found, skip

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            Debug.Log("[EnemyStateManager] Player found and cached.");
        }
        else
        {
            Debug.LogWarning("[EnemyStateManager] Player not found — ensure the player GameObject " +
                             "is tagged 'Player' in the Inspector.");
        }
    }

    #endregion
    // -------------------------------------------------------------------------
    #region State Switching

    public void SwitchState(EnemyBaseState newState)
    {
        _currentState.ExitState(this);
        _currentState = newState;
        _currentState.EnterState(this);
    }

    #endregion
    // -------------------------------------------------------------------------
    #region Public Queries — used by external scripts (e.g. PlayerParryState)

    /// <summary>
    /// Returns true if the enemy is currently in the attack state.
    /// Called by PlayerParryState.CheckSuccessParry() each frame.
    /// </summary>
    public bool IsInAttackState() => _currentState == attackState;

    /// <summary>Distance between this enemy and the player along the Z axis.</summary>
    public float DistanceToPlayerZ()
    {
        if (playerTransform == null) return float.MaxValue;
        return Mathf.Abs(transform.position.z - playerTransform.position.z);
    }

    /// <summary>Flat 3D distance to the player (used for detection radius).</summary>
    public float DistanceToPlayer()
    {
        if (playerTransform == null) return float.MaxValue;
        return Vector3.Distance(transform.position, playerTransform.position);
    }

    #endregion
    // -------------------------------------------------------------------------
}