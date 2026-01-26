using UnityEngine;

public class ScriptEnabler_startFall : MonoBehaviour
{
    [SerializeField] startFall startFall;
    //This script is not robust at all and is meant to be one time use only on specificly one singular bolder detecter do no use it on something else without a 
    //change

    void OnTriggerEnter(Collider a)
    {
        if (a.CompareTag("Damaging")) startFall.enabled = true;
    }

}
