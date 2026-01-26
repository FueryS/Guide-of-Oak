using UnityEngine;

public class DashParticle : MonoBehaviour
{
    [SerializeField] private PlayerMovement p;
    [SerializeField] private ParticleSystem dashEffect;
    private bool wasDashing;

    private void Start()
    {
        wasDashing = false;
    }

    private void Update()
    {
        if (p == null || dashEffect == null) return;

        // Detect dash start
        if (p.isDashing && !wasDashing)
        {
            dashEffect.Play();
            //Debug.Log("Dash started → Particle played");
        }

        // Detect dash end
        if (!p.isDashing && wasDashing)
        {
            dashEffect.Stop();
            //Debug.Log("Dash ended → Particle stopped");
        }

        wasDashing = p.isDashing;
    }
}