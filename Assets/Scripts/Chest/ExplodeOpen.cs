using System.Collections;
using System.Xml.Serialization;
using Unity.Mathematics;
using UnityEngine;

public class ExplodeOpen : MonoBehaviour
{
    #region Modules
    Rigidbody _rb;
    public InteractionProgressUI m_IPU;
    #endregion

    #region effects
    public ParticleSystem playParticle;
    public float waitTime;
    public string nextScene;
    #endregion

    #region Inspector
    [Tooltip("Force to apply relative to object orientation.\nX = sideways, Y = up/down, Z = forward/backward.")]
    public Vector3 applyForce;

    [Tooltip("Torque to apply relative to object orientation.\nX = roll, Y = pitch, Z = yaw.")]
    public Vector3 torque;
    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    #region Main Function
    private void Update()
    {
        if (m_IPU == null)
            return;

        // If interaction completed, trigger explosion and destoy the "E" canvas
        if (m_IPU.interacted)
        {
            Explode();
            Destroy(m_IPU.canvas);
        }
    }

    void Explode()
    {
        if (_rb == null)
        {
            Debug.LogError("Could not proceed with explosion: _rb is null");
            return;
        }

        _rb.isKinematic = false;

        // Build force relative to object’s facing direction
        Vector3 relativeForce =
            transform.right * applyForce.x +   // sideways
            transform.up * applyForce.y +   // up/down
            transform.forward * applyForce.z;    // forward/backward

        // Same for torque (rotations relative to orientation)
        Vector3 relativeTorque =
            transform.right * torque.x +
            transform.up * torque.y +
            transform.forward * torque.z;

        _rb.AddForce(relativeForce, ForceMode.Impulse);
        _rb.AddTorque(relativeTorque, ForceMode.Impulse);
        StartCoroutine("PlayParticle");

    }
    #endregion

    #region extra effects
    IEnumerator PlayParticle()
    {
        yield return new WaitForSeconds(waitTime);
        playParticle.Play();
        
        //set the next scene value
        setNextSceneValue();

        //wait for particle to finish
        yield return new WaitForSeconds(playParticle.main.duration);

        GameLoader.Instance.LoadNextScene();
    }


    // this will set the next scene value
    void setNextSceneValue()
    {
        GameLoader.nextScene = nextScene;
    }
    #endregion

}