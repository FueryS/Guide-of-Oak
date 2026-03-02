using UnityEngine;

public class PlayerWalkingForwardState : PlayerBaseState
{
    public string walk = "Walk";
    public float walkSpeed = 3.5f;

    PlayerStateManager playerRef;

    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Player is entering walking");
        player.animator.SetTrigger(walk);
        playerRef = player;

        player.inputHandler.OnJumpPressed += OnJumpPressed;
        player.inputHandler.OnDashPressed += OnDashPressed;
    }

    public override void UpdateState(PlayerStateManager player)
    {
        #region state switing
        if (player.inputHandler.moveInput == Vector2.zero) player.SwitchState(player.idleState);
        if (player.inputHandler.sprintHeld) player.SwitchState(player.runState);
        #endregion

        MoveForward(player);
        player.runState.ManageRotation(player, player.inputHandler.moveInput);
    }

    void MoveForward(PlayerStateManager player)
    {
        Vector2 moveInput = player.inputHandler.moveInput;
        float targetVelocity = moveInput.x * walkSpeed;
        player.movementComplier.AddToFinalMovement(new(0, 0, targetVelocity));
    }

    public override void ExitState(PlayerStateManager player)
    {
        playerRef.inputHandler.OnJumpPressed -= OnJumpPressed;
        playerRef.inputHandler.OnDashPressed -= OnDashPressed;
    }


    void OnJumpPressed()
    {
        playerRef.SwitchState(playerRef.jumpState);
    }
    void OnDashPressed()
    {
        playerRef.SwitchState(playerRef.dodgeState);
    }


}
