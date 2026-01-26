using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parkour : MonoBehaviour
{
    // --- References ---
    //private PlayerMovement PM;
    private CharacterController controller;
    //private PlayerInputHandler PI;
    [SerializeField] GameObject player;

    [Header("State tracker")]
    public bool ledgeDetected;

    [Header("Ledge detect")]
    public float detectDistance = 0.3f;
    public LayerMask walkableLayer; // Assign "Walkable" layer in Inspector

    private void Awake()
    {
        controller = GetComponentInParent<CharacterController>();
        //PI = GetComponentInParent<PlayerInputHandler>();
        //PM = GetComponentInParent<PlayerMovement>();
    }

    //private void Update()
    //{
    //    ledgeDetected = ledgeDetect();
    //}

    public bool LedgeDetect()
    {
        // Sync rotation
        gameObject.transform.rotation = player.transform.rotation;


        // SphereCast returns true if it hits something on the specified layer
        bool hasHit = Physics.SphereCast(transform.position, controller.radius, transform.forward, out RaycastHit hit, detectDistance, walkableLayer);

        // Visual Debug
        Debug.DrawRay(transform.position, transform.forward * detectDistance, hasHit ? Color.green : Color.red);

        return hasHit;
    }

}