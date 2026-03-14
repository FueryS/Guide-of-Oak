 using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    #region states
    PlayerBaseState currentState;
    [HideInInspector] public PlayerRunningForwardState runState = new PlayerRunningForwardState();
    [HideInInspector] public PlayerIdleState idleState = new PlayerIdleState();
    [HideInInspector] public PlayerWalkingForwardState walkState = new PlayerWalkingForwardState();
    [HideInInspector] public PlayerJumpState jumpState = new PlayerJumpState();
    [HideInInspector] public PlayerDodgeState dodgeState = new PlayerDodgeState();
    [HideInInspector] public PlayerAttackState attackState = new PlayerAttackState();
    [HideInInspector] public PlayerParryState parryState = new PlayerParryState();
    #endregion

    #region refrences
    [HideInInspector]public PlayerInputHandler inputHandler;
    [HideInInspector]public FinalMovementComplier movementComplier;
    [HideInInspector]public GravityApplier2D gravityApplier;

    [HideInInspector]public Animator animator;
    #endregion

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        movementComplier = GetComponent<FinalMovementComplier>();
        gravityApplier = GetComponent<GravityApplier2D>();

        //remember to recode this when there are more chidrens with animator
        animator = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        currentState = idleState;
        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this); 
    }

    public void SwitchState(PlayerBaseState state)
    {
        currentState.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }

    #region Subscription
    public void OnAttackPressed()
    {
        SwitchState(attackState);
    }

    public void OnJumpPressed()
    {
        SwitchState(jumpState);
    }
    public void OnDashPressed()
    {
        SwitchState(dodgeState);
    }
    #endregion
}
