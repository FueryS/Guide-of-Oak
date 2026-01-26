using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles slope-related player physics behavior:
/// - Detects slope angle and adjusts movement resistance accordingly.
/// - Disables jump on overly steep surfaces.
/// - Designed to work alongside PlayerMovement and PlayerInputHandler.
/// </summary>
[DefaultExecutionOrder(-10)]
public class PlayerPhysicsHandler : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float forwardCheckDistance = 2.3f;   // Max distance for forward slope checks

    [Header("Slope Thresholds")]
    // Defines slope-angle thresholds and corresponding actions
    public Dictionary<float, string> slopeActions = new Dictionary<float, string>
    {
        { 0f,  "none" },
        { 20f, "slowIncrease" },
        { 30f, "intendedIncrease" },
        { 50f, "punishing" },
        { 56f, "disableJump" }
    };

    [Header("Debugging Options")]
    [SerializeField] private bool debugHit = true;
    [SerializeField] private bool debugStand = true;
    [SerializeField] private bool debugRes = true;
    [SerializeField] private bool debugSlopeAction = true;
    [SerializeField] private bool debugVisual = true;

    // --- References ---
    private PlayerMovement PM;
    private CharacterController controller;
    private PlayerInputHandler PI;

    // --- Cached values ---
    private Vector3 slopeNormalStanding = Vector3.zero; // Normal of the surface player stands on
    public float steepestAngle;                        // Detected slope angle
    public string slopeAction = "none";                // Current action applied
    public LayerMask layer;

    private float g; // Cached gravity value for potential future use

    // -----------------------------------------------------------
    private void Awake()
    {
        // Cache all components
        PM = GetComponent<PlayerMovement>();
        controller = GetComponent<CharacterController>();
        PI = GetComponent<PlayerInputHandler>();

        // Store gravity for potential use in future mechanics
        g = PM.GetGravity();
    }

    // -----------------------------------------------------------
    private void Update()
    {
        CastRaySlopeCheck();
    }

    // -----------------------------------------------------------
    /// <summary>
    /// Casts a downward ray to detect the ground slope.
    /// If grounded, also casts a forward ray to detect slope changes ahead.
    /// </summary>
    private void CastRaySlopeCheck()
    {
        Vector3 down = Vector3.down;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, down, out hit, forwardCheckDistance,layer))
        {
            if (debugStand)
                Debug.Log($"Ground detected. Normal: {hit.normal}");
            if ((Mathf.Round(Vector3.Angle(hit.normal, Vector3.up)) > 19f))
                CastRaySlopeCheckForward(hit);
        }
        else
        {
            ApplySlopeAction(1f, true);
        }

        if (debugVisual)
            Debug.DrawRay(transform.position, down * forwardCheckDistance, Color.green);
    }

    // -----------------------------------------------------------
    /// <summary>
    /// Casts a ray in the player’s movement direction to check the slope ahead.
    /// </summary>
    void CastRaySlopeCheckForward(RaycastHit groundHit)
    {
        Vector2 moveInput = PI.moveInput;
        Vector3 forward = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        RaycastHit hit;


        if (Physics.SphereCast(transform.position + Vector3.up, controller.radius, forward, out hit, forwardCheckDistance))
        {
            if (debugHit)
                Debug.Log($"Forward obstacle detected. Normal: {hit.normal}");
        }
        else
        {
            slopeAction = "none";
        }

        CalculateSlopeEffect(hit);
        if (debugVisual)
            Debug.DrawRay(transform.position + Vector3.up, forward * forwardCheckDistance, Color.cyan);
    }

    // -----------------------------------------------------------
    /// <summary>
    /// Applies resistance and jump restrictions based on slope category.
    /// </summary>
    private void CalculateSlopeEffect(RaycastHit hit)
    {
        if (!controller.isGrounded) return;

        slopeNormalStanding = hit.normal;
        string currentState = CalculateState(hit);

        switch (currentState)
        {
            case "none":
                ApplySlopeAction(1f, true);
                break;
            case "slowIncrease":
                ApplySlopeAction(0.8f, true);
                break;
            case "intendedIncrease":
                ApplySlopeAction(0.6f, true);
                break;
            case "punishing":
                ApplySlopeAction(0.3f, true);
                break;
            case "disableJump":
                ApplySlopeAction(0.0f, false);
                break;
            default:
                Debug.LogWarning("Unexpected slope condition encountered.");
                break;
        }

        if (debugRes)
            Debug.Log($"Current Resistance: {PM.resistance}");
    }

    // -----------------------------------------------------------
    /// <summary>
    /// Calculates which slope category the player is currently on.
    /// </summary>
    private string CalculateState(RaycastHit hit)
    {
        float angle = Mathf.Round(Vector3.Angle(hit.normal, Vector3.up));
        steepestAngle = angle;

        foreach (var key in slopeActions.Keys.OrderBy(k => k))
        {
            if (steepestAngle >= key)
                slopeAction = slopeActions[key];
            else
                break;
        }

        if (debugSlopeAction)
            Debug.Log($"Slope angle: {steepestAngle:F2}, Action: {slopeAction}");

        return slopeAction;
    }

    // -----------------------------------------------------------
    /// <summary>
    /// Applies resistance multiplier and jump toggle.
    /// </summary>
    private void ApplySlopeAction(float resistanceMultiplier, bool canJump)
    {
        PM.resistance = resistanceMultiplier;
        PM.canJump = canJump;
    }
}
