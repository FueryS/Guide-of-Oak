using UnityEngine;

public class AnimationHandle : MonoBehaviour
{
    //components
    Animator anim;
    PlayerMovement pm;
    AnimationStateControl state;
    PlayerInputHandler pi;

    [Header("Custom variables")]
    public float walkrun = 1f;
    public float idlewalk = 0f;

    //variables
    //[Header("aniamtion Variable names")]

    private void Awake()
    {
        anim = GetComponent<Animator>();
        pm = GetComponentInParent<PlayerMovement>();
        state = GetComponent<AnimationStateControl>();
        pi = GetComponentInParent<PlayerInputHandler>();
        
    }
    void Update()
    {
        Mover(pi.moveInput);
        checkRun();
        checkJump();
    }

    public void Mover(Vector2 m)
    {
        float currentX = state.getVx();
        float currentY = state.getVy();

        float smoothX = Mathf.Lerp(currentX, m.x, Time.deltaTime * 10f);
        float smoothY = Mathf.Lerp(currentY, m.y, Time.deltaTime * 10f);

        state.setVx(smoothX);
        state.setVy(smoothY);
    }
    public void checkRun() {
        state.setShift(pi.sprintHeld);
    }

    public void checkJump()
    {
        state.setJumpAnim(pm.hasJumped);
        if (pm.hasJumped) pm.hasJumped = false;//reset
    }

    

}
