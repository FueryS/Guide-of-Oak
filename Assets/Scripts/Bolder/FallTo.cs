using UnityEngine;
using System.Collections;

/// <summary>
/// Moves the attached GameObject from its current position to a target Transform's position 
/// over time, adding customizable 3D curvature.
/// </summary>
public class FallTo : MonoBehaviour
{
    [Header("Target & Controls")]
    [Tooltip("The Transform (usually an Empty GameObject) that defines the final landing position.")]
    public Transform targetTransform;

    [Tooltip("Set this to TRUE to initiate the falling movement. It will automatically reset to FALSE when the target is reached.")]
    public bool startFalling = false;

    [Tooltip("The time (in seconds) the object takes to reach the target.")]
    public float fallDuration = 1.0f;

    [Header("Curvature Modifiers")]
    [Tooltip("Strength of the curve along the X-axis (Horizontal sway).")]
    [Range(0f, 10f)]
    public float xCurveStrength = 0.5f;

    [Tooltip("Strength of the curve along the Y-axis (Vertical arc/height of the fall).")]
    [Range(0f, 10f)]
    public float yCurveStrength = 3.0f;

    [Tooltip("Strength of the curve along the Z-axis (Forward/depth sway).")]
    [Range(0f, 10f)]
    public float zCurveStrength = 0.5f;

    private float timeElapsed = 0f;
    private Vector3 startPosition;
    private bool isFalling = false;

    void Update()
    {
        // --- 1. Check for the Start Trigger ---
        // If the user has set startFalling to true AND we are not currently falling, initiate the fall.
        if (startFalling && !isFalling && targetTransform != null)
        {
            // Consume the trigger and set internal state
            startFalling = false;
            isFalling = true;

            // Set starting conditions
            startPosition = transform.position;
            timeElapsed = 0f;

            // Early exit to wait for next frame to start movement
            return;
        }

        // Exit if we are not falling, the duration is invalid, or the target is missing
        if (!isFalling || fallDuration <= 0 || targetTransform == null)
        {
            return;
        }

        // Use the current position of the target object
        Vector3 finalTargetPosition = targetTransform.position;

        // --- 2. Movement Calculation ---

        timeElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(timeElapsed / fallDuration); // Normalized progress (0 to 1)

        // Base Linear Interpolation (straight line between start and target)
        Vector3 basePosition = Vector3.Lerp(startPosition, finalTargetPosition, t);

        // Calculate Curvature Offset: t * (1 - t) creates a smooth parabolic arc
        float offsetFactor = t * (1f - t);

        // Apply curvature based on the world axes
        float curveX = offsetFactor * xCurveStrength;
        float curveY = offsetFactor * yCurveStrength;
        float curveZ = offsetFactor * zCurveStrength;

        // Combine base position and curvature offset
        Vector3 finalPosition = basePosition;
        finalPosition.x += curveX;
        finalPosition.y += curveY;
        finalPosition.z += curveZ;

        // Apply the new position
        transform.position = finalPosition;

        // --- 3. Completion Check ---
        if (t >= 1.0f)
        {
            // Ensure the object snaps exactly to the target and stays there
            transform.position = finalTargetPosition;
            isFalling = false; // Stop the movement loop

            // startFalling remains false and is ready to be set true again for a new fall.
            Debug.Log(gameObject.name + " successfully reached the target.");
        }
    }
}