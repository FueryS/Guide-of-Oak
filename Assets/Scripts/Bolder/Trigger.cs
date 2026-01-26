using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] GameObject bolder;
    //[SerializeField] GameObject beakableWall;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("collided with: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            //setRB();
            bolder.GetComponent<animationHandelerBolderSwing>().pauseAnimation = false;
        }
    }

    
}
