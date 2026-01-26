using UnityEngine;

public class ForceManagement : MonoBehaviour
{
    CharacterController characterController;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public Vector3 impact = Vector3.zero;
    void Update()
    {
        if (impact.magnitude > 0.2f) characterController.Move(impact * Time.deltaTime);
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime); // Consumes the force
    }
    public void AddForce(Vector3 dir, float magnitude) => impact += dir.normalized * magnitude;
}

