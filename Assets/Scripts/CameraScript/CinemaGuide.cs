using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

/*
 * This script is used to manage the cinmatic camera
 * the only job of this is to add values to the CinematicCamFollow script depending on many of its own idenpendent conditions
 * then just enable    
 */
public class CinemaGuide : MonoBehaviour
{
    //------------ For external factors -----------------
    
    [Tooltip("CinematicCamFollow")]
    [SerializeField] CinematicCamFollow ccf;

    [Tooltip("Camera")]
    [SerializeField] Camera cam;

    [Tooltip("CameraManager")]
    [SerializeField] CameraManager camManager;

    [Header("Next Target settings")]
    [Tooltip("Enable only if the next target is supposed to be another object")]
    public bool isObject;
    [SerializeField] Transform nextTarget;
    public Vector3 nextPos;
    [SerializeField] Vector3 offset;

    [Header("Look at settings")]
    [Tooltip("Enable if the look at target is an object")]
    public bool lookIsObject;
    public bool lookAt;
    [SerializeField] Transform lookTarget;
    public Vector3 lookPos;

    [Header("Next transition settings")]
    [SerializeField] float startSpeed;
    [SerializeField] float endSpeed;
    [SerializeField] string ease;

    [Header("Job details")]
    [SerializeField]
    [Tooltip("0:switch to cinema and teleport \n1:start destination \n2:Intermediate destination \n3:switch to main when reached")]
    int chainJob;
    //[Tooltip("Decides if the stay duration is dependent on an external event or not")]
    //public bool hasTrigger;//I dont think I will need this one
    [Tooltip("Duration of stay")]
    public float stayTime;
    [Tooltip("If trigger then activate this to make the transition")]
    public bool triggered;

    // job 1 settings
    [Header("Job 1 (if it is job1)")]
    [Tooltip("Enable to start the countdown of the movement of camera")]
    public bool triggeredBefore;
    [Tooltip("Enable to set the Lookat a certain position")]
    public bool lookAtBefore;
    [Tooltip("Enable to copy position of a certain object for look at")]
    public bool lookAtBeforeIsObject;
    public Vector3 lookAtBeforePos;
    [Tooltip("Add the object here to set look at")]
    [SerializeField] Transform lookAtBeforeObj;
    public float startSpeedBefore;
    public float endSpeedBefore;
    [Tooltip("Set the ease motion for before")]
    public string easeBefore;

    // job 3 settings
    [Header("Job 3 (if its job 3)")]
    [Tooltip("Initiates the countdown to switch the camera to main")]
    public bool switchNow;
    [Tooltip("Delay before switching to main camera")]
    public float delay;

    // job  settings
    [Header("Job 0 (if its job 0)")]
    [Tooltip("Initiates the countdown to switch the camera to cinema")]
    public bool j0_switchNow;
    [Tooltip("Delay before switching to cinema camera")]
    public float j0_delay;
    [Tooltip("Next camera guide")]
    [SerializeField] CinemaGuide g_job1;

    //----------- For internal logic ---------------
    [Header("Trigger Settings")]
    public Vector3 boxColliderSize = new Vector3(1f, 1f, 1f);

    private BoxCollider triggerCollider;

    private void Awake()
    {
        // 1. Get the BoxCollider component reference
        triggerCollider = GetComponent<BoxCollider>();

        if (triggerCollider != null)
        {
            // 2. Adjust the center of the Box Collider using the defined offset.
            // This shifts the trigger volume relative to the object's pivot.
            triggerCollider.isTrigger = true;
            //if(chainJob==1) triggerCollider.center = offset;

            // Note: The Box Collider's size should be set in the inspector 
            // to be large enough to contain the camera.
        }
        else
        {
            Debug.LogError("CinemaGuide requires a BoxCollider component to function.");
        }
    }
    private void Update()
    {
        job1();
        //StartCoroutine(job3());
        StartCoroutine(job0());
        //Do not add offset its broken and useless I plan to remove it in future
        offset = Vector3.zero;
    }

    IEnumerator job0()
    {
        if (j0_switchNow & chainJob == 0) 
        {
            yield return new WaitForSeconds(j0_delay);
            cam.transform.position = transform.position;
            camManager.cinema = true;
            g_job1.triggered = true;
            g_job1.triggeredBefore = true;
            Destroy(gameObject);
        }
        yield break;
    }
    void job1()
    {
        //validation checks before executing the script
        if (chainJob != 1) return;
        if (!triggeredBefore) return;

        //When its Triggered bring the camera to its location
        ccf.target = transform.position; 
        ccf.enableLookAt = lookAt;    
        ccf.lookAt = lookAtBeforeIsObject ? lookAtBeforeObj.position : lookAtBeforePos;
        ccf.easer = ease;
        ccf.move = true;
        ccf.offset = offset;
        triggeredBefore = false;
    }

    //<summery>
    //After reaching at the position the code much check for the camera at its position the start the counter or check the trigger to 
    //initiate the movement to next position
    //</summery>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Cinema"))
        {

            if (triggered)
            {
                //Debug.Log("The cam should start moving now");

                if(chainJob == 2) Invoke("job2", stayTime);
                if(chainJob == 3) StartCoroutine(job3());

                

            }
        }
    }

    void job2()
    {
        ccf.target = isObject ? nextTarget.position : nextPos;
        ccf.enableLookAt = lookAt;
        ccf.lookAt = lookAtBeforeIsObject ? lookTarget.position : lookPos;
        ccf.move = true;
        triggered = false;
    }

    IEnumerator job3()
    {
        
        // Conditions to stop the coroutine immediately 
        if (!switchNow) yield break; 

        // If conditions are met, execution continues here:
        yield return new WaitForSeconds(delay);
        camManager.cinema = false;
        triggered = false;
    }

}
