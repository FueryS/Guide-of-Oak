using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    PlayerStateManager playerRef;
    GravityApplier2D GravityApplier;

   

    public string jumpStart = "JumpStart";
    public float maxJumpDuration = 1f;
    public float jumpStrength = 12f;
    public float minJumpDuration = 20f;
    public float airJumpAmount = 1f;

    Coroutine _endJumpCoroutine;
    bool _endJump = false;
    bool _switchNow = false;

    public void AssignVariables(string jumpStart,float maxJumpDuration, float jumpStrength, float minJumpDuration)
    {
        this.jumpStart = jumpStart;
        this.maxJumpDuration = maxJumpDuration;
        this.jumpStrength = jumpStrength;
        this.minJumpDuration = minJumpDuration;
    }

    public override void EnterState(PlayerStateManager player)
    {
        playerRef = player;
        GravityApplier = player.gravityApplier;

        if (!GravityApplier.IsGrounded2D)
        {
            Switch(player);
            return;
        }

        Debug.Log("Enter Jump State");
        player.inputHandler.OnJumpReleased += OnJumpRelease;

        player.animator.SetTrigger(jumpStart);

        _endJumpCoroutine = player.StartCoroutine(EndJump());

        _switchNow = false;
        _endJump = false;
        player.StartCoroutine(MinJump());
        
    }

    public override void UpdateState(PlayerStateManager player)
    {
        player.movementComplier.AddToFinalMovement(new(0,jumpStrength,0));
    }

    IEnumerator EndJump()
    {
        yield return new WaitForSeconds(maxJumpDuration);

        Debug.LogWarning("Coroutine Ended");
        Switch(playerRef);
    }
    IEnumerator MinJump()
    {
        yield return new WaitForSeconds(minJumpDuration);
        _endJump = true;
        if (_switchNow) Switch(playerRef);
    }

    void OnJumpRelease()
    {
        _switchNow = true;
        Switch(playerRef);
    }

    void Switch(PlayerStateManager player)
    {
        if (!_endJump) return;

        if (player.inputHandler.moveInput.x == 0)
            player.SwitchState(player.idleState);
        else if (!player.inputHandler.sprintHeld)
            player.SwitchState(player.walkState);
        else
            player.SwitchState(player.runState);
    }

    public override void ExitState(PlayerStateManager player)
    {
        player.inputHandler.OnJumpReleased -= OnJumpRelease;

        if (_endJumpCoroutine != null)
        {
            playerRef.StopCoroutine(_endJumpCoroutine);
            //Debug.Log("Coroutine destroyed");
        }
            
    }

}
