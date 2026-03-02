using UnityEngine;

public class ValuesAssigner : MonoBehaviour
{
    PlayerStateManager stateManager;

    [Header("Jump Settins")]
    public string jumpStart = "JumpStart";
    public float maxJumpDuration = 1f;
    public float jumpStrength = 12f;
    public float minJumpDuration = 0.4f;

    private void Awake()
    {
        stateManager = GetComponent<PlayerStateManager>();
    }

    [ContextMenu("Start again")]
    private void Start()
    {
        stateManager.jumpState.AssignVariables(jumpStart,maxJumpDuration,jumpStrength,minJumpDuration);
    }
}
