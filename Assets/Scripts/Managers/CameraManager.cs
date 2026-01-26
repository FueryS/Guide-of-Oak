using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField][Tooltip("Define The main camera")] Camera gameplayCam;
    [SerializeField][Tooltip("Define The cinematic camera")] Camera cinematicCam;

    [Header("State managers")]
    [Tooltip("Enable to switch to cinema else stay at gameplay")] public bool cinema;


    void Awake()
    {
        gameplayCam.enabled = true;
        cinematicCam.enabled = false;
    }

    private void Update()
    {
        if (cinematicCam == null || gameplayCam == null)
        {
            Debug.LogWarning("Camera manger has null camera!!"); return;
        }

        if (cinema)
        {
            SwitchToCinematic();
        }
        else
        {
            SwitchToGameplay();
        }
    }

    public void SwitchToCinematic()
    {
        gameplayCam.enabled = false;
        cinematicCam.enabled = true;
    }

    public void SwitchToGameplay()
    {
        cinematicCam.enabled = false;
        gameplayCam.enabled = true;
    }
}