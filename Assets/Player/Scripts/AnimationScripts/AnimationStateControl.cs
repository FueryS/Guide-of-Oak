using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class AnimationStateControl : MonoBehaviour
{
    Animator animator;

    //animations
    [SerializeField] string vx = "VelocityX";
    [SerializeField] string vy = "VelocityY";
    [SerializeField] string shift = "ShiftHeld";
    [SerializeField] string jumped = "hasJumped";
    private void Awake()
    {
        animator = GetComponent<Animator>();

    }

    public void setVx(float v) { animator.SetFloat(vx, v); }
    public void setVy(float v) { animator.SetFloat(vy, v); }

    public float getVx() { return animator.GetFloat(vx); }

    public float getVy() { return animator.GetFloat(vy); }

    public void setShift(bool v) => animator.SetBool(shift, v);

    public void setJumpAnim(bool v) => animator.SetBool(jumped, v);
}
