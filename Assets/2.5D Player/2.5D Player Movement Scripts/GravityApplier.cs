using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(FinalMovementComplier))]
[RequireComponent(typeof(CharacterController))]
public class GravityApplier2D : MonoBehaviour
{
    [Header("Ground Check settings")]
    public float rayCastDistance = 2f;//I can change its length if I want to lower the character
    public LayerMask groundLayer;//to specify what is the ground layer so it doesn't hit other objects

    //private
    Vector3 _orgin;

    [Header("Gravity settings")]
    public float gravity = -9.81f;

    [Tooltip("This is the gravity that will be applied when the player is grounded, it is used to snap the player to the ground and prevent it from floating when grounded")]
    public float snapGravity = 0f;

    #region Modules
    FinalMovementComplier _final;
    CharacterController _characterController;
    #endregion

    //for debugging purpose only:
    RaycastHit hit;
    Renderer _hitRenderer;

    #region exposed so it can be used again
    public bool IsGrounded2D { get; private set; }
    public bool HasSunken2D { get; private set; } = false;
    #endregion

    private void Awake()
    {
        _final = GetComponent<FinalMovementComplier>();
        _characterController = GetComponent<CharacterController>();

    }



    #region Ground Check

    void GroundCheck()
    {
        /// Converts the local 'center' offset into a real-time world position
        _orgin = transform.TransformPoint(_characterController.center);

        //Raycast is a line that is casted in a direction and checks if it hits something,
        //it returns true if it hits something and false if it doesn't
        IsGrounded2D = Physics.Raycast(_orgin, Vector3.down, out hit, rayCastDistance, groundLayer);

        //I will have to change this to ignore layers so it doesn't hit the player i
        //tself and also to ignore the trigger colliders

        SunkenCheck();
    }


    //Cast another ray shorter than actual ground check to see if the player has sunken if yes
    //then apply reverse snap gravity to snap the player to the ground and prevent it from sunking underground
    void SunkenCheck()
    {
        float sunkenRayCastDistance = rayCastDistance*0.9f;

        HasSunken2D = Physics.Raycast(_orgin, Vector3.down, sunkenRayCastDistance, groundLayer);
    }

    

    #endregion

    #region apply gravity

    void ApplyGravity()
    {
        if (!IsGrounded2D)
        {
            _final.AddToFinalMovement(new(0, gravity, 0));
        }

        else if (HasSunken2D)
        {
            _final.AddToFinalMovement(new(0, Mathf.Max(-gravity,1.9f), 0));
        }

        else
        {
            _final.AddToFinalMovement(new(0, snapGravity, 0));
        }
    }
    #endregion


    #region debug utils

    void debuging()
    {
        Color color = HasSunken2D? Color.red: Color.yellow;

        Debug.DrawRay(_orgin, Vector3.down * rayCastDistance, Color.blue);
        if (IsGrounded2D)
        {
            _hitRenderer = hit.collider.GetComponent<Renderer>();
            _hitRenderer.material.color = color;
        }
        
        else if(_hitRenderer != null)
        {
            _hitRenderer.material.color = Color.white;
            _hitRenderer = null;
        }

    }


    #endregion
    void Update()
    {
        GroundCheck();
        ApplyGravity();
        //debuging();
    }

}
