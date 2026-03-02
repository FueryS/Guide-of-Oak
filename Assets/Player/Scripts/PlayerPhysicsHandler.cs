using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class PlayerPhysicsHandler : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float groundCheckDistance = 1.1f; // Slightly more than half player height
    [SerializeField] private float forwardCheckDistance = 1.0f;
    [SerializeField] private LayerMask groundLayer;

    // We use a List of a custom struct because Dictionaries aren't serializable 
    // in the Inspector and OrderBy in Update is slow.
    [System.Serializable]
    public struct SlopeThreshold
    {
        public float angle;
        public string actionName;
        public float resistance;
        public bool canJump;
    }

    [Header("Slope Settings")]
    public List<SlopeThreshold> slopeSettings = new List<SlopeThreshold>
    {
        new SlopeThreshold { angle = 0f,  actionName = "none", resistance = 1.0f, canJump = true },
        new SlopeThreshold { angle = 20f, actionName = "slowIncrease", resistance = 0.8f, canJump = true },
        new SlopeThreshold { angle = 30f, actionName = "intendedIncrease", resistance = 0.6f, canJump = true },
        new SlopeThreshold { angle = 50f, actionName = "punishing", resistance = 0.3f, canJump = true },
        new SlopeThreshold { angle = 56f, actionName = "disableJump", resistance = 0.0f, canJump = false }
    };

    [Header("Debugging")]
    [SerializeField] private bool showDebugLogs = true;

    private PlayerMovement PM;
    private CharacterController controller;
    private PlayerInputHandler PI;

    private void Awake()
    {
        PM = GetComponent<PlayerMovement>();
        controller = GetComponent<CharacterController>();
        PI = GetComponent<PlayerInputHandler>();

        // Sort settings once at start to save performance
        slopeSettings = slopeSettings.OrderByDescending(s => s.angle).ToList();
    }

    private void Update()
    {
        EvaluatePhysics();
    }

    private void EvaluatePhysics()
    {
        RaycastHit hit;
        Vector3 moveDir = GetMovementDirection();
        float detectedAngle = 0f;

        // 1. Check Forward (Anticipation)
        // We check forward first so the player "feels" the slope as they hit it.
        bool hitForward = Physics.Raycast(transform.position + Vector3.up * 0.5f, moveDir, out hit, forwardCheckDistance, groundLayer);

        if (hitForward)
        {
            detectedAngle = Vector3.Angle(hit.normal, Vector3.up);
        }
        else
        {
            // 2. Check Ground (Current state)
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, groundCheckDistance, groundLayer))
            {
                detectedAngle = Vector3.Angle(hit.normal, Vector3.up);
            }
        }
        // Inside EvaluatePhysics...
        Vector3 currentNormal = hit.normal;
        ApplySlopeLogic(detectedAngle, currentNormal, moveDir);

        // Visual Debugging
        if (showDebugLogs)
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, moveDir * forwardCheckDistance, hitForward ? Color.red : Color.green);
        }
    }

    private void ApplySlopeLogic(float angle, Vector3 slopeNormal, Vector3 moveDir)
{
    // Access the grounded state from the CharacterController reference
    bool isGrounded = controller.isGrounded; 

    // Determine if we are moving UPHILL 
    float moveDirectionDot = Vector3.Dot(moveDir, slopeNormal);
    bool isMovingUphill = moveDirectionDot < 0;

    // If we aren't touching the ground, OR we are moving downhill, reset resistance
    if (!isGrounded || !isMovingUphill || angle < 10f) 
    {
        PM.resistance = 1.0f;
        PM.canJump = true;
        return;
    }

        // Otherwise, find the threshold for UPHILL movement
        foreach (var settings in slopeSettings)
        {
            if (angle >= settings.angle)
            {
                PM.resistance = settings.resistance;
                PM.canJump = settings.canJump;

                if (showDebugLogs)
                    Debug.Log($"Uphill Detected! Angle: {angle:F1}° | Resistance: {settings.resistance}");

                return;
            }
        }
    }

    private Vector3 GetMovementDirection()
    {
        if (PI == null || PI.moveInput.sqrMagnitude < 0.01f)
            return transform.forward; // Default to forward if standing still

        Vector3 dir = (transform.right * PI.moveInput.x + transform.forward * PI.moveInput.y).normalized;
        return dir;
    }
}