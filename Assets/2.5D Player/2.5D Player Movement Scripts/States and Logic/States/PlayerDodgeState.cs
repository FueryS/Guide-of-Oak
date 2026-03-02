using System.Collections;
using UnityEngine;

public class PlayerDodgeState : PlayerBaseState
{
    public float dashTime=0.1f;
    public float dashSpeed=20;
    public float dashCooldown=1f;

    public bool canDash = true;

    PlayerStateManager playerRef;
    public override void EnterState(PlayerStateManager player)
    {
        //If the cooldown is still running just switch back same logic as jump is grounded
        if (!canDash)
        {
            Switch(player);
            return;
        }
        Debug.Log("dashing");
        player.StartCoroutine(DashCooldown());
        playerRef = player;
        player.StartCoroutine(DashStart());

    }

    public override void UpdateState(PlayerStateManager player)
    {
        float direction = (player.transform.localEulerAngles.y == 0)? 1:-1;
        player.movementComplier.AddToFinalMovement(new(0, 0, dashSpeed*direction));
    }

    IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }    
    
    IEnumerator DashStart()
    {
        yield return new WaitForSeconds(dashTime);
        Switch(playerRef);
    }

    void Switch(PlayerStateManager player)
    {
        if (player.inputHandler.moveInput.x == 0)
            player.SwitchState(player.idleState);
        else if (!player.inputHandler.sprintHeld)
            player.SwitchState(player.walkState);
        else
            player.SwitchState(player.runState);
    }

    public override void ExitState(PlayerStateManager player)
    {
        
    }
}
