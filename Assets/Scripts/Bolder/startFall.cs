using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startFall : MonoBehaviour
{
    public bool allowFall = false;
    [SerializeField][Tooltip("Job 3 cinema guide")] CinemaGuide cm;


    private void OnTriggerEnter(Collider other)
    {
        

        if (other.CompareTag("Damaging"))
        {
            if (allowFall)
            {
                other.transform.SetParent(null);

                FixedJoint joint = other.GetComponent<FixedJoint>();

                if (joint != null)
                {
                    Destroy(joint);
                }

                other.GetComponent<FallTo>().startFalling = true;
                cm.switchNow = true;

            }
        }
    }
}
