using System.Collections.Generic;
using UnityEngine;

public class ValuesAssigner : MonoBehaviour
{
    PlayerStateManager stateManager;
    public AnimationEvents AnimEvent;

    #region Jump

    [Header("Jump Settings")]
    public string jumpTrigger = "JumpStart";
    public string airJumpTrigger = "AirJump";
    public float jumpStrength = 12f;
    public float minJumpStrength = 4f;
    public float maxJumpDuration = 0.4f;
    public float minJumpDuration = 0.1f;
    public float releaseDecaySpeed = 8f;

    [Header("Air Movement Settings")]
    public float airMoveSpeed = 6f;
    public float airMoveLerpSpeed = 3f;

    [Header("Air Jump Settings")]
    public int maxAirJumps = 1;
    #endregion

    #region Attack
    [Header("Attack Settings")]
    public string attackTrigger = "Attack";
    public List<string> attackAnimationNames = new List<string>()
    {
        "Great Sword Slash",
        "Great Sword Kick"
    };
    public float comboReRollTime = 0.6f;
    public GameObject attackHitbox;
    #endregion

    #region Defence
    [Header("Dodge Settings")]
    public AudioClip[] dashSFX;
    public AudioSource audioSource;

    float dashTime = 0.3f;
    float dashSpeed = 20f;
    float dashCooldown = 1f;


    [Header("Parry Settings")]
    public string parryTrigger = "Parry";
    public string parryBool = "ParryBool";
    public LayerMask isEnemy;
    #endregion

    private void Awake()
    {
        stateManager = GetComponent<PlayerStateManager>();
    }

    [ContextMenu("Reassign Values")]
    private void Start()
    {
        //--------------- jump --------------

        stateManager.jumpState.AssignVariables(
            jumpTrigger,
            airJumpTrigger,
            jumpStrength,
            minJumpStrength,
            maxJumpDuration,
            minJumpDuration,
            releaseDecaySpeed,
            airMoveSpeed,
            airMoveLerpSpeed,
            maxAirJumps,
            AnimEvent
        );

        //--------------- attack --------------

        stateManager.attackState.AssignVariables(
            attackTrigger,
            attackAnimationNames,
            comboReRollTime
            );

        //--------------- dodge --------------

        stateManager.dodgeState.ValueAssigner(
            audioSource,
            dashSFX,
            dashTime,
            dashSpeed,
            dashCooldown
            );


        //--------------- parry --------------

        stateManager.parryState.AssignVariables(
            parryTrigger,
            parryBool,
            isEnemy
            );

    }
}