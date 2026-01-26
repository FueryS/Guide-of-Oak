using UnityEngine;

public class GravityApplier : MonoBehaviour
{
    #region modules
    CharacterController controller;
    #endregion

    #region public variables
    public Vector3 drag;
    public float appliedGravity;
    [Header("Ground Check Settings")]
    public float shellOffset = 0.1f; // Starts the check slightly inside the capsule
    public float groundDistance = 0.2f; // How far below the feet to check
    public LayerMask groundMask; // Set this to "Everything" except the Player layer
    #endregion

    #region private variables
    public float Gravity { get; private set; }
    public Vector3 FinalFall { get; private set; }
    public bool IsGrounded { get; private set; }
    #endregion

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Gravity = -20;
    }

    private void FixedUpdate()
    {
        GroundCheck();
        GravityReseter();
        FinalFall = (new Vector3(0, appliedGravity, 0) - drag) * Time.fixedDeltaTime;
        controller.Move(FinalFall);
    }

    void GravityReseter()
    {
        // FIX: Only reset to -3 if grounded AND we aren't currently jumping upwards
        // If appliedGravity > 0, it means PlayerMover just set a jump value.
        if (IsGrounded && appliedGravity <= 0)
        {
            appliedGravity = -3;
        }
        else if (appliedGravity > Gravity)
        {
            // This handles the falling acceleration
            appliedGravity -= 1f; // Reduced from 3 to 1 for a smoother arc, adjust as needed
        }
    }

    void GroundCheck()
    {
        // 1. Calculate the 'Bottom' center of the sphere
        // We start slightly ABOVE the actual floor (shellOffset) so the sphere has room to move down
        float radius = controller.radius * 0.9f; // Slightly smaller than the player to avoid hitting walls
        Vector3 vHeight = new(0, (controller.height * 0.5f) - radius + shellOffset, 0);
        Vector3 origin = transform.position + controller.center - vHeight;

        // 2. The SphereCast
        // origin: center of the sphere
        // radius: width of the check
        // Vector3.down: direction
        // groundDistance: how far to move the sphere down
        IsGrounded = Physics.SphereCast(origin, radius, Vector3.down, out _, groundDistance, groundMask);

        // DEBUG: Visualizes the sphere in the scene view
        Debug.DrawRay(origin, Vector3.down * groundDistance, IsGrounded ? Color.green : Color.red);
    }
}