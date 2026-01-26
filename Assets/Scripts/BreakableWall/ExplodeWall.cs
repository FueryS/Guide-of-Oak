using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class ExplodeWall : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] animationHandelerBolderSwing anim;
    BoxCollider box;

    [SerializeField] float time;

    AudioManager _AM;
    private void Awake()
    {
        ps = GetComponentInChildren<ParticleSystem>();
        box = GetComponent<BoxCollider>();

        _AM = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioManager>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Bolder")
        {
            if (anim.pauseAnimation == false)
            {
                setRB();
                ps.Play();
                Destroy(box);
            }
        }
    }
    void setRB()
    {

        // Get all Rigidbody components in children of breakableWall
        Rigidbody[] childRigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in childRigidbodies)
        {
            rb.isKinematic = false;
           
        }
        Invoke("DestroyChildren", time);
        _AM.PlayBolderBreakWall();

    }

    void DestroyChildren()
    {
  
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject); 
        }
    }
}
