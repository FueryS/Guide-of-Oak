using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationHandelerBolderSwing : MonoBehaviour
{
    Animator anim;
    public float startFrame;
    public int offset;
    public bool pauseAnimation;
    public float animationSpeed;
    public string animationName;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (animationName == null) animationName = "bolderSwing";
    }

    private void Start()
    {
        anim.Play(animationName,offset,startFrame);
    }

    private void Update()
    {
        if (pauseAnimation) anim.speed = 0;
        else anim.speed = animationSpeed;
    }

}
