using System.Collections;
using UnityEngine;

public class PlayerRunningForwardState : PlayerBaseState
{
    #region public used for tweaking
    public float runSpeed = 8f;
    public float walkSpeed = 3.5f;
    public float turnRunSpeed = 3f;
    public float inputBuffer = 0.2f;

    public string turnRun = "TurnRun";
    public string run = "Run";
    #endregion

    #region private used for internal logic
    float _moveDirectionChache;
    float _velocityBuffer;
    float _targedVelocity;
    Coroutine switchRoutine;
    #endregion

    PlayerInputHandler inputHandler;
    PlayerStateManager playerRef;

    Vector2 _moveInput;

    public override void EnterState(PlayerStateManager player)
    {
        //Debug.Log("Player is entering running");

        inputHandler = player.inputHandler;
        playerRef = player;
        _moveInput = inputHandler.moveInput;

        _velocityBuffer = 0;

        player.animator.SetBool(run, true);

        player.inputHandler.OnJumpPressed += OnJumpPressed;
        player.inputHandler.OnDashPressed += OnDashPressed;
        player.inputHandler.OnAttackPressed += OnAttackPressed;
    }

    public override void UpdateState(PlayerStateManager player)
    {
        #region state switing
        //switch to idle
        if (player.inputHandler.moveInput == Vector2.zero)
        {
            if (switchRoutine == null)
                switchRoutine = player.StartCoroutine(Switch(player, player.idleState));
        }
        //switch to walk
        else if (player.inputHandler.moveInput != Vector2.zero && !player.inputHandler.sprintHeld)
        {
            if (switchRoutine == null)
                switchRoutine = player.StartCoroutine(Switch(player, player.walkState));
        }
        else
        {
            if (switchRoutine != null)
            {
                player.StopCoroutine(switchRoutine);
                switchRoutine = null;
            }
        }

        #endregion

        MoveForward(player);
        ManageRotation(player, _moveInput);
    }

    void MoveForward(PlayerStateManager player)
    {
        _moveInput = player.inputHandler.moveInput;
        _targedVelocity = _moveInput.x * runSpeed;

        float t = turnRunSpeed * Time.deltaTime;
        _velocityBuffer = Mathf.Lerp(_velocityBuffer, _targedVelocity, t);

        //Debug.Log("Velocity Buffer: " + _velocityBuffer);

        player.movementComplier.AddToFinalMovement(new(0, 0, _velocityBuffer));
    }


    public void ManageRotation(PlayerStateManager player, Vector2 _moveInput)
    {
        float moveDirection;
        moveDirection = ((_moveInput.x == 0) ? player.transform.localRotation.eulerAngles.y : ((_moveInput.x > 0) ? 0 : 180));
        player.transform.localRotation = Quaternion.Euler(0, moveDirection, 0);
        //Debug.Log("Move Direction: " + moveDirection);
    }

    IEnumerator Switch(PlayerStateManager player, PlayerBaseState state)
    {
        yield return new WaitForSeconds(inputBuffer);

        switchRoutine = null;

        if (_moveInput == Vector2.zero || !player.inputHandler.sprintHeld)
        {
            player.SwitchState(state);
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        if (switchRoutine != null)
        {
            player.StopCoroutine(switchRoutine);
            switchRoutine = null;
        }

        player.animator.SetBool(run, false);
        playerRef.inputHandler.OnJumpPressed -= OnJumpPressed;
        playerRef.inputHandler.OnDashPressed -= OnDashPressed;
        playerRef.inputHandler.OnAttackPressed -= OnAttackPressed;
    }

    #region subscriptions
    void OnJumpPressed()
    {
        playerRef.SwitchState(playerRef.jumpState);
    }
    void OnDashPressed()
    {
        playerRef.SwitchState(playerRef.dodgeState);
    }
    void OnAttackPressed()
    {
        playerRef.SwitchState(playerRef.attackState);
    }
    #endregion
}
