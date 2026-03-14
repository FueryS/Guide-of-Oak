using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public string idle = "Idle";

    PlayerInputHandler inputHandler;
    PlayerStateManager playerRef;
    public override void EnterState(PlayerStateManager player)
    {
        playerRef = player;

        //Debug.Log("Player is Entering Idle");

        inputHandler = player.inputHandler;

        player.animator.SetBool(idle, true);

        player.inputHandler.OnJumpPressed += OnJumpPressed;
        player.inputHandler.OnDashPressed += OnDashPressed;
        player.inputHandler.OnAttackPressed += OnAttackPressed;

    }

    public override void UpdateState(PlayerStateManager player)
    {
        //Debug.Log("Player is Idling");

        #region state switing
        if (inputHandler.moveInput != Vector2.zero)
        {
            if (inputHandler.sprintHeld) player.SwitchState(player.runState);
            else player.SwitchState(player.walkState);
        }
        #endregion
    }

    public override void ExitState(PlayerStateManager player)
    {
        player.animator.SetBool(idle, false);
        player.inputHandler.OnJumpPressed -= OnJumpPressed;
        player.inputHandler.OnDashPressed -= OnDashPressed;
        player.inputHandler.OnAttackPressed -= OnAttackPressed;

    }


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




}
