using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [SerializeField] float damage = 200f;
    [SerializeField, Tooltip("Keep negative to deflect the boulder on impact")]
    float deflection = -3f;
    [SerializeField, Tooltip("Used to apply force on player")]
    float pushForce = 10f;

    Rigidbody rb;

    Vector3 lastPosition;
    Vector3 velocity; // units per second

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        deflection = -Mathf.Abs(deflection);
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        velocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        collision.gameObject.GetComponent<Stats>()?.Damage(damage);

        if (velocity.sqrMagnitude > 0.01f)
        {
            // Push player
            collision.gameObject.GetComponent<ForceManagement>().impact
                += velocity * pushForce;

            // Deflect this object
            rb.AddForce(velocity * deflection, ForceMode.Impulse);

            //Debug.Log("Impact:" + collision.gameObject.GetComponent<ForceManagement>().impact);
            //Debug.Log("Velocity:" + velocity);
            ////Debug.Break();
        }
    }
}
