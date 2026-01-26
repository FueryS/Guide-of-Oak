using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
/// <summary>
/// Handles 3rd Person Camera logic, switching between Locked (Combat) and Free (Orbit) modes.
/// </summary>
public class PlayerCameraController : MonoBehaviour
{
    #region Configuration
    [Header("References")]
    [Tooltip("The Transform of the character model to rotate.")]
    public Transform playerBody;

    [Header("Settings")]
    [Tooltip("Mouse sensitivity for look speed.")]
    [Range(0f, 1f)]
    public float sensitivity = 1f;

    [Tooltip("How high the camera can look up.")]
    [Range(-90f, 0f)]
    public float minVerticalAngle = -60f;

    [Tooltip("How low the camera can look down.")]
    [Range(0f, 90f)]
    public float maxVerticalAngle = 60f;

    [Header("State Flags")]
    [Tooltip("If true, the Player body rotates with the camera (God of War). If false, camera orbits freely (Elden Ring).")]
    public bool rotatePlayer = true;

    [Tooltip("If true, mouse input is ignored.")]
    public bool lockMouseInput = false;
    #endregion

    #region Private State
    private PlayerInputHandler input;
    private float xRotation = 0f; // Pitch (Up/Down)
    private float yRotation = 0f; // Yaw (Left/Right)
    #endregion

    void Start()
    {
        input = GetComponent<PlayerInputHandler>();

        // Initialize rotation to current facing direction to prevent snapping
        if (playerBody != null)
            yRotation = playerBody.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (lockMouseInput) return;

        HandleCameraRotation();
    }

    /// <summary>
    /// Calculates and applies rotation based on input and current mode (rotatePlayer).
    /// </summary>
    private void HandleCameraRotation()
    {
        // 1. Get Input
        float mouseX = input.lookInput.x * sensitivity;
        float mouseY = input.lookInput.y * sensitivity;

        // 2. Calculate Vertical Rotation (Pitch) - Always local to camera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        if (rotatePlayer)
        {
            // --- GOD OF WAR STYLE ---

            // Rotate the actual Player Body horizontally
            playerBody.Rotate(Vector3.up * mouseX);

            // Sync our internal yRotation tracker to the player's new rotation
            // This ensures smooth transitions if we toggle 'rotatePlayer' off later
            yRotation = playerBody.eulerAngles.y;

            // Apply vertical rotation to Camera locally
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        else
        {
            // --- ELDEN RING STYLE (Free Look) ---

            // Accumulate horizontal rotation internally (Orbit)
            yRotation += mouseX;

            // Apply BOTH rotations directly to the Camera in World Space
            // This allows the camera to orbit even if the player parent stays still
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
}