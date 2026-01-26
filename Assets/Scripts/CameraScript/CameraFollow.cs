using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // Assign the parent or player manually or via inspector
    [SerializeField] private float distance = 5f;
    [SerializeField] private float smoothSpeed = 5f;

    private Vector3 desiredOffset;

    void Start()
    {
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
        // Update offset in case the target rotates
        desiredOffset = -target.forward * distance;

        // Desired camera position
        Vector3 desiredPosition = target.position + desiredOffset;

        // Smoothly interpolate to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Optional: Look at the target
        transform.LookAt(target);
    }
}