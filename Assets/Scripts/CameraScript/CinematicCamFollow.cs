using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Used for easy list searching

public class CinematicCamFollow : MonoBehaviour
{
    SetLookAt m_sla;
    [Header("State")]
    [Tooltip("Toggle this to true to trigger the movement.")]
    public bool move;

    [Header("Target Definition")]
    [Tooltip("Final destination position.")]
    public Vector3 target;
    [Tooltip("Offset applied to the target position.")]
    public Vector3 offset;

    [Header("Rotation Settings")]
    [Tooltip("If false, camera retains previous orientation.")]
    public bool enableLookAt = true;
    [Tooltip("Point to look at (if enabled).")]
    public Vector3 lookAt;

    [Header("Movement Guide")]
    public EaserBehaviourDefination easeSettings = new();
    public string easer = "Linear";
    public CameraSpeedProfile speedProfile = new();

    // Internal tracker for the active coroutine
    private Coroutine currentRoutine;

    private void Awake()
    {
        m_sla = GetComponent<SetLookAt>();
    }
    private void Update()
    {
        // CHANGED: Removed "!isMoving" check. 
        // We now allow new inputs to interrupt the current move.
        if (move)
        {
            move = false;
            MoveCamera();
        }
    }

    [ContextMenu("Trigger Move")]
    public void MoveCamera()
    {
        // 1. Interrupt existing movement
        if (currentRoutine != null) StopCoroutine(currentRoutine);

        // 2. Start fresh
        currentRoutine = StartCoroutine(MoveRoutine());
        m_sla.enabled = false;
    }

    private IEnumerator MoveRoutine()
    {
        transform.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);

        // We calculate initial distance to determine total duration
        float totalDistance = Vector3.Distance(startPos, target + offset);

        // Prevent divide by zero
        float avgSpeed = (speedProfile.startSpeed + speedProfile.endSpeed) / 2f;
        if (avgSpeed <= 0.01f) avgSpeed = 1f;

        float duration = totalDistance / avgSpeed;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float linearT = timer / duration;

            // CHANGED: Update the destination EVERY FRAME
            // This ensures if 'target' changes, the camera corrects course
            Vector3 finalPos = target + offset;

            // Get Curve
            AnimationCurve selectedCurve = easeSettings.GetCurveByName(easer);
            float easedT = selectedCurve.Evaluate(linearT);

            // Move
            transform.position = Vector3.Lerp(startPos, finalPos, easedT);

            // Rotate
            if (enableLookAt)
            {
                // CHANGED: Update Look Rotation every frame too
                Vector3 direction = (lookAt - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(startRot, targetRot, easedT);
                }
            }

            yield return null;
        }

        // Final Snap
        transform.position = target + offset;
        currentRoutine = null;
        m_sla.enabled = true;
    }
}

// --- Data Structures ---

[System.Serializable]
public class CameraSpeedProfile
{
    [Tooltip("Speed at the start of the path.")]
    public float startSpeed = 5f;
    [Tooltip("Speed at the end of the path.")]
    public float endSpeed = 5f;
}

[System.Serializable]
public class EaserProfile
{
    public string name;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
}

[System.Serializable]
public class EaserBehaviourDefination
{
    [Tooltip("List of named curves.")]
    public List<EaserProfile> profiles = new();

    /// <summary>
    /// Helper to find a curve by string name. Returns Linear if not found.
    /// </summary>
    public AnimationCurve GetCurveByName(string name)
    {
        foreach (var profile in profiles)
        {
            if (profile.name == name) return profile.curve;
        }

        Debug.LogWarning($"Curve '{name}' not found! Defaulting to Linear.");
        return AnimationCurve.Linear(0, 0, 1, 1);
    }
}