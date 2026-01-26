using UnityEngine;

public class Headwobble : MonoBehaviour
{
    [Header("Settings")]
    public float walkingBobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;
    public float smooth = 10f;

    [Header("Tilt Settings")]
    public float maxTilt = 5f;      // How many degrees to tilt
    public float tiltSpeed = 10f;   // How fast to reach that tilt

    private float currentZ = 0f;

    [Header("References")]
    public CharacterController controller_m;
    public PlayerInputHandler input_m;

    private float defaultPosY = 0;
    private float timer = 0;

    void Start()
    {
        // Record the initial local Y position of the camera
        defaultPosY = transform.localPosition.y;
        controller_m = GetComponentInParent<CharacterController>();
        input_m = GetComponentInParent<PlayerInputHandler>();
    }

    void Update()
    {
        //Tilt the camera
        TiltCamera();

        // Check if the player is moving and grounded
        if ((Mathf.Abs(controller_m.velocity.x) > 0.1f || Mathf.Abs(controller_m.velocity.z) > 0.1f) & controller_m.isGrounded)
        {
            // Player is moving
            timer += Time.deltaTime * walkingBobbingSpeed;

            // Calculate the new local Y position using a Sine wave
            float newY = defaultPosY + Mathf.Sin(timer) * bobbingAmount;
            Vector3 targetPosition = new(transform.localPosition.x, newY, transform.localPosition.z);

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smooth);
        }
        else
        {
            // Player is idle - reset timer and move back to default position
            timer = 0;
            Vector3 targetPosition = new(transform.localPosition.x, defaultPosY, transform.localPosition.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smooth);
        }
    }

    void TiltCamera()
    {
        // 1. Calculate the target Z rotation
        // We multiply by -1 because usually, moving Right (positive X) 
        // should tilt the camera Left (negative Z) for a natural feel.
        float targetZ = -input_m.moveInput.x * maxTilt;

        // 2. Linearly Interpolate the Z value
        currentZ = Mathf.Lerp(currentZ, targetZ, Time.deltaTime * tiltSpeed);

        // 3. Apply the rotation while preserving current X and Y (Look rotation)
        // We use localEulerAngles to modify the 'Roll' specifically.
        Vector3 currentRot = transform.localEulerAngles;
        transform.localRotation = Quaternion.Euler(currentRot.x, currentRot.y, currentZ);
    }

}