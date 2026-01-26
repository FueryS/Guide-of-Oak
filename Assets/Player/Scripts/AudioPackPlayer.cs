using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class AudioPackPlayer : MonoBehaviour
{
    AudioManager _AM;
    CharacterController controller;
    PlayerMovement _PM;
     

    [Header("Ground Check Settings")]
    public float shellOffset = 0.1f; // Starts the check slightly inside the capsule
    public float groundDistance = 0.2f; // How far below the feet to check
    public LayerMask groundMask; // Set this to "Everything" except the Player layer

    private void Start()
    {
        _AM = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioManager>();
        controller = GetComponent<CharacterController>();
        _PM = GetComponent<PlayerMovement>();
        if (_AM == null) Debug.Log("KYS");
    }

    private void Update()
    {
    }

    void PlayFootSteps()
    {
        if (!GroundCheck()) return;
        if (_PM.isWalking||_PM.isRunning) _AM.PlayFootStep();

        
    }


    public bool GroundCheck()
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
        bool IsGrounded = Physics.SphereCast(origin, radius, Vector3.down, out _, groundDistance, groundMask);

        // DEBUG: Visualizes the sphere in the scene view
        Debug.DrawRay(origin, Vector3.down * groundDistance, IsGrounded ? Color.green : Color.red);

        return IsGrounded;
    }

}
