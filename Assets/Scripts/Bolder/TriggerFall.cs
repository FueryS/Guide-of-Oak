using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFall : MonoBehaviour
{
    public Transform bolder;
    public startFall startFall;
    [SerializeField] CinemaGuide cm;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("collided with: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            startFall.allowFall = true;
            cm.j0_switchNow = true;
        }
    }
}