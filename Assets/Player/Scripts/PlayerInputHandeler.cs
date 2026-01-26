using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    //controls is the object for PlayerControls.cs which is the generated script of the input system
    private PlayerControls controls;
    /*This creates a public variable which can be set privately but can be accessed publicly 
    I am using this because exposing the set to whole game is un necessary as it will be modified in this code only
    Vector2 because the input system was also defined in vector2*/
    public Vector2 moveInput { get; private set; }
    public Vector2 lookInput { get; private set; }
    public bool sprintHeld { get; private set; }
    public bool interactHeld { get; private set; }
    public bool spaceHeld { get; private set; }
    public bool spaceReleased { get; private set; }
   

    //public bool spaceHeld { get; private set; }


    //These are the event, An event is not a function — it's a notification mechanism
    //Other scripts can subscribe to these events and are notified when these events are invoked
    public event Action OnJumpPressed;
    public event Action OnJumpReleased;
    public event Action OnDashPressed;
    public event Action OnDashReleased;
    public event Action OnSlidePressed;

    private void Awake()
    {
        controls = new PlayerControls();

        // Movement
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        /*controls=object Move is an input action defined in the .inputactions asset of Player, perfomed is triggered when input is active 
         and canceled is triggered right as the input stream stops ie. input is inactive so we set Vector2 to zero removing the movement*/

        // Look
        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        // Sprint
        controls.Player.Sprint.performed += ctx => sprintHeld = true;
        controls.Player.Sprint.canceled += ctx => sprintHeld = false;

        //Slide event
        controls.Player.Sprint.started += ctx=> OnSlidePressed?.Invoke();

        // Jump Events
        controls.Player.Jump.started += ctx => OnJumpPressed?.Invoke();
        controls.Player.Jump.canceled += ctx => OnJumpReleased?.Invoke();

        //Notify the subsciribed events when action is taken

        // Dash Events
        controls.Player.Dash.started += ctx => OnDashPressed?.Invoke();
        controls.Player.Dash.canceled += ctx => OnDashReleased?.Invoke();//same

        //Interact 
        controls.Player.Interact.performed += ctx => interactHeld = true;
        controls.Player.Interact.canceled += ctx => interactHeld = false;

        // Space (Fly Camera Up)
        controls.Player.Jump.performed += ctx => spaceHeld = true;
        controls.Player.Jump.canceled += ctx => spaceHeld = false;
        /* "Space" must be defined in your InputActions asset under Player map.
           This allows you to hold space to fly the camera upward. */

    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
    /*These ensure your input system is active only when the GameObject is enabled.
     Without this, inputs might leak or persist across scenes.*/
}
