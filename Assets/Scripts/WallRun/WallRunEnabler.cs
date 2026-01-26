using System;
using UnityEngine;

public class WallRunEnabler : MonoBehaviour
{
    [SerializeField] string gametag = "Player";
    [SerializeField] float friction = 3;

    PlayerMovement pm;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(gametag))
        {
            try
            {
                if (friction < 1)
                {
                    Debug.LogWarning($"<color=yellow>[WallRunEnabler]</color> the value of friction is invalid {friction} reseting to 1");
                    friction = 1;
                }
                pm = other.GetComponent<PlayerMovement>();
                pm.doWallRun = true;
                pm.wr_friction = friction;
                pm.SetJump(1);
                pm.SetDash(true);
            }
            catch (NullReferenceException)
            {

                Debug.LogWarning($"<color=yellow>[WallRunEnabler]</color> {other.name} does not have PlayerMovement script");
                return;
            }
            catch (Exception e) { Debug.LogError(e); return; }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (pm != null & other.CompareTag(gametag))
        {
            pm.doWallRun = false;
        }
    }
}
