using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    #region modules

    CharacterController m_controller;
    GravityApplier m_applier;
    PlayerInputHandler m_inputHandler;

    #endregion

    #region public

    [Header("Basic Movement")]

    public float runSpeed = 10;
    public float strafeSpeed = 4;

    [Header("Jump Settings")]
    public float maxJumpCount = 2;
    public float jumpSpeed;

    [Header("Slide Properties")]
    public float slideSpeed = 10;
    public float slideCooldown = 1.5f;
    public float slideDuration = 0.5f;
    public float slideControllerHeight = 1f;
    public float slideControllerCenter = 0.5f;


    public Vector3 playerMoveFinal;

    #endregion

    #region private

    Vector3 playerMove;

    public bool canSlide { get; private set; }
    float _slideSpeed;
    float _DefaultControllerCenter;
    float _DefaultControllerHeight;

    float _jumpCount;

    #endregion

    private void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_applier = GetComponent<GravityApplier>();
        m_inputHandler = GetComponent<PlayerInputHandler>();

        //Default values
        _DefaultControllerCenter = m_controller.center.y;
        _DefaultControllerHeight = m_controller.height;

        canSlide = true;

        //Subscribe events
        m_inputHandler.OnSlidePressed += PlayerSlide;
        m_inputHandler.OnJumpReleased += jumpReleased;
    }

    private void Update()
    {
        Jump();
        PlayerMove();

        Vector3 slideValue = new(0, 0, _slideSpeed);
        playerMoveFinal = (playerMove + slideValue) * Time.deltaTime;
        //Move runner
        m_controller.Move(playerMoveFinal);
    }

    #region Jump
    void Jump()
    {
        // 1. Reset jump count when grounded
        if (m_applier.IsGrounded && m_applier.appliedGravity <= 0)
        {
            _jumpCount = 0;
        }

        // 2. Check for the initial press (not held) to prevent "infinite" floating
        if (m_inputHandler.spaceHeld) // Assuming you have a 'spacePressed' bool for GetKeyDown
        {
            if (_jumpCount < maxJumpCount)
            {
                Debug.Log("Jump Performed: " + _jumpCount);
                m_applier.appliedGravity = jumpSpeed;
                _jumpCount++;
            }
        }
    }

    void jumpReleased()
    {
        _jumpCount++;
    }

    #endregion

    void PlayerMove()
    {
        float movex = 0;
        Vector2 moveIp = m_inputHandler.moveInput;

        movex = moveIp.x * strafeSpeed;

        playerMove = new(movex, 0, runSpeed);
    }

    #region slide

    void PlayerSlide()
    {
        if (!m_applier.IsGrounded && !canSlide) { Debug.Log("Cant Slide"); return; }
        StartCoroutine(PlayerSlidePerform());
        StartCoroutine(PlayerSlideCoolDown());
    }

    IEnumerator PlayerSlidePerform()
    {
        //disable slide
        canSlide = false;

        //increase the speed
        _slideSpeed = slideSpeed;

        //Shrink the character
        m_controller.height = slideControllerHeight;
        m_controller.center = new Vector3(0, slideControllerCenter, 0);

        yield return new WaitForSeconds(slideDuration);

        //Unshrink the character
        m_controller.height = _DefaultControllerHeight;
        m_controller.center = new Vector3(0, _DefaultControllerCenter, 0);

        //reset the speed
        _slideSpeed = 0;
    }

    IEnumerator PlayerSlideCoolDown()
    {
        yield return new WaitForSeconds(slideCooldown);

        //reset slide
        canSlide = true;
    }

    #endregion
}
