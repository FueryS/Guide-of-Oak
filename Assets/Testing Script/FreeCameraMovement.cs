using UnityEngine;

public class FreeCameraMovement : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float sensitivity = 2f;
    #endregion

    #region Private Variables
    float _yaw;
    float _pitch;

    PlayerInputHandler m_ip; // Reference to PlayerInputHandler
    #endregion

    #region Unity Methods
    void Start()
    {
        m_ip = GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        CameraRotate();
        CameraMove();
    }
    #endregion

    #region Camera Movement
    void CameraMove()
    {
        Vector3 move = Vector3.zero;

        // Horizontal movement (camera-relative)
        Vector2 input = m_ip.moveInput;
        move += transform.forward * input.y;
        move += transform.right * input.x;

        // Vertical movement
        if (m_ip.spaceHeld)          // Fly upward when Space is held
            move += Vector3.up;

        if (m_ip.sprintHeld)         // Move downward when Sprint is held
            move += Vector3.down;

        transform.position += move * moveSpeed * Time.deltaTime;
    }

    void CameraRotate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float mouseX = m_ip.lookInput.x * sensitivity;
        float mouseY = m_ip.lookInput.y * sensitivity;

        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, -90f, 90f);
        _yaw += mouseX;

        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }
    #endregion
}