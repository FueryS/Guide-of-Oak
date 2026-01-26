using UnityEngine;

public class ScriptEnabler_ScriptEnabler : MonoBehaviour
{
    [SerializeField] ScriptEnabler_startFall sc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cinema")) sc.enabled = true;
    }

}
