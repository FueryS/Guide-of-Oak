using JetBrains.Annotations;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // Assign the parent or player manually or via inspector
    [SerializeField] private float distance = 5f;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float minHeightOffset = 2f;
    [SerializeField] private float maxHeightOffset = 10f;

    public bool is3DMode = true; // Toggle between 2.5D and 3D modes

    private System.Action updateLogic;


    private Vector3 desiredOffset;

    void Start()
    {
        updateLogic = UpdateFor2_5D; // Default to 2.5D mode
        if (is3DMode)
            updateLogic = UpdateFor3D;
        

        if (target == null)
        {
            target = transform.parent;
            if (target == null)
            {
                Debug.LogWarning("CameraFollow: No target assigned and no parent found.");
                enabled = false;
                return;
            }
        }

        // Initial offset behind the target
        desiredOffset = -target.forward * distance;
    }

    void LateUpdate()
    {
        updateLogic?.Invoke();
    }

    void UpdateFor3D()
    {
        // Update offset in case the target rotates
        desiredOffset = -target.forward * distance;

        // Desired camera position
        Vector3 desiredPosition = target.position + desiredOffset;

        // Smoothly interpolate to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Optional: Look at the target
        transform.LookAt(target);
    }

    void UpdateFor2_5D()
    {
        // Define the vertical bounds based on target position
        float lowBound = target.position.y + minHeightOffset;
        float highBound = target.position.y + maxHeightOffset;

        // Clamp the current camera Y so it stays within those bounds
        float clampedY = Mathf.Clamp(transform.position.y, lowBound, highBound);

        Vector3 desiredPosition = new Vector3(
            transform.position.x,
            clampedY,
            target.position.z + distance
        );

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }


}