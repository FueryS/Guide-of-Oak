using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(FinalMovementComplier))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;

    public float jumpStrength = 4f;
    public float jumpDuration = 1.0f;
    public int airJumpsCount = 1;

    [Header("Animator variable names")]
    public string runTrigger = "Run";
    public string idleTrigger = "Idle";
    public string turnRunTrigger = "TurnRun";

    //refrences
    PlayerInputHandler inputHandler;
    FinalMovementComplier movementComplier;
    GravityApplier2D gravityApplier;
    Animator animator;

    #region private used for internal refrencing
    Vector2 _moveInput;
    #endregion

    #region private used for internal logic

    float _moveInputXBuffer;
    bool _jumpBuffer;
    int _airJumpsLeft;

    #endregion


    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        movementComplier = GetComponent<FinalMovementComplier>();
        animator = GetComponentInChildren<Animator>();
        gravityApplier = GetComponent<GravityApplier2D>();
        
        //Chache data
        _airJumpsLeft = airJumpsCount;
    }

    private void Start()
    {
        inputHandler.OnJumpPressed += JumpPressed;

    }

    #region Movement
    void MoveSideways()
    {
        _moveInput = inputHandler.moveInput;

        float moveSpeed = _moveInput.x * (inputHandler.sprintHeld ? sprintSpeed : walkSpeed);


        float moveDirection;
        moveDirection = ((_moveInput.x == 0) ? transform.localRotation.eulerAngles.y : ((_moveInput.x > 0) ? 0 : 180));

        //If the diraction changed trigger the turn run animation
        //if (moveDirection != _moveInputXBuffer) TriggerTurnRun();

        transform.localRotation = Quaternion.Euler(0, moveDirection, 0);

        if (moveSpeed != 0)
        {
            StartRun(true);
        }
        else
        {
            StartRun(false);
        }




        Vector3 movement = new Vector3(0, 0, moveSpeed);
        movementComplier.AddToFinalMovement(movement);


        //remember the current direction in this cycle
        _moveInputXBuffer = moveDirection;
    }


    #region Jump

    //---------- JumpPressed logic ------------------
    void JumpPressed()
    {
        //Check if its grounded if yes the start the jumpstarted courutine
        if (gravityApplier.IsGrounded2D)
        {
            StartCoroutine("JumpStarted");
        }
        else if (airJumpsCount > 0) 
        {
            StartCoroutine("JumpStarted");
            _airJumpsLeft--;
        }

    }

    IEnumerator JumpStarted()
    {
        _jumpBuffer = true;
        yield return new WaitForSeconds(jumpDuration);
        _jumpBuffer = false;
    }

    void ExecuteJump()
    {
        if (_jumpBuffer)
        {
            movementComplier.AddToFinalMovement(new(0, jumpStrength, 0));
        }

        if (gravityApplier.IsGrounded2D)
        {
            _airJumpsLeft = airJumpsCount;
        }
    }



    #endregion
    #endregion

    #region animation utils

    void StartRun(bool value)
    {
        animator.SetBool(idleTrigger, !value);
        animator.SetBool(runTrigger, value);
    }

    void TriggerTurnRun()
    {
        animator.SetTrigger(turnRunTrigger);
    }

    #endregion


    private void Update()
    {
        MoveSideways();
        ExecuteJump();
    }
}
