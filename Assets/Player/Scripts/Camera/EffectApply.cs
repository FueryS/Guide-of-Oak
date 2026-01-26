using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CamEffects))]
public class EffectApply : MonoBehaviour
{

    PlayerMovement pm;
    CamEffects effects;


    [Header("Shake Values")]
    [SerializeField] float walkShake = 0.5f;
    [SerializeField] float runShake = 2f;
    [Header("Shake Frequency")]
    [SerializeField] int walkShakeF = 6;
    [SerializeField] int runShakeF = 20;

    [Header("Push Back")]
    public bool focused;
    [SerializeField][Tooltip("Used for exploring")] float pushValueRun = -1.2f;
    [SerializeField]
    [Tooltip("Used for my bitch ass obsticles which for some reason I am the only one comfortable with the camera angle of other just cant do shit So i have to perogram a few more lines of code")] 
    float pushWhenFocusedWalk = -1.8f;
    [SerializeField]
    [Tooltip("same but for running")] float pushWhenFocusedRun = 1.3f;
    [SerializeField]
    [Tooltip("same but idle")] float pushWhenFocusedIdle = -1.5f;
    
    void Start()
    {
        if (pm == null) pm = GetComponentInParent<PlayerMovement>();

        if (effects == null) effects = GetComponent<CamEffects>();
    }

    // Update is called once per frame
    void Update()

    {
        CamShaker();
        RunEffect();
        WalkEffectFocused();
        IdleEffectWhenFocused();
    }
    void CamShaker()
    {
        float v = pm.isWalking ? walkShake : (pm.isRunning ? runShake : 0f);
        int s = pm.isWalking ? walkShakeF : runShakeF;
        effects.ApplyShake(v,s);
    }

    void RunEffect()
    {
        if (!pm.isRunning) return;
        effects.PushBack(focused?pushWhenFocusedRun : pushValueRun);
    }

    void WalkEffectFocused()
    {
        if (focused & pm.isWalking) effects.PushBack(pushWhenFocusedWalk);
    }

    void IdleEffectWhenFocused()
    {
        if (focused & pm.isIdle) effects.PushBack(pushWhenFocusedIdle);
    }
}
