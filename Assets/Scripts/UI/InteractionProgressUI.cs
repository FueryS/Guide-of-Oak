using System;
using UnityEngine;
using UnityEngine.UI; // Needed for Slider

/// <summary>
/// Handles interaction progress when the player is within a defined radius.
/// Requires a SphereCollider set as trigger on the same GameObject.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class InteractionProgressUI : MonoBehaviour
{

    public bool interacted=false;

    #region Public Variables
    [Tooltip("Radius within which the player can interact.")]
    public float interactionRadius = 3f;

    [Tooltip("Time (in seconds) the player must hold interact to succeed.")]
    public float interactionDelay = 2f;

    [Tooltip("Destroy this object after successful interaction. Used only for debugging DO NOT ENABLE WHEN BEING USED WITH PURPOSE.")]
    public bool destroyOnSuccess = false;

    [Tooltip("Add the gameObject that needs to be Destroyed here")]
    public GameObject canvas;

    [Tooltip("Tag used to identify the player in trigger checks.")]
    public string playerTag = "Player";
    #endregion

    #region Private Variables
    private Slider _slider;                // Local reference to the Slider component
    private float _currentProgress = 0f;   // Tracks current progress time
    private bool _playerInRange = false;   // Is the player inside the trigger radius?

    // Reference to PlayerInputHandler (non-local component)
    private PlayerInputHandler m_playerInputHandler;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        // Cache the Slider component on this GameObject
        _slider = GetComponent<Slider>();

        if (_slider == null)
        {
            Debug.LogError("No Slider component found on this GameObject!");
            enabled = false;
            return;
        }

        // Set slider max value to interactionDelay so time maps directly to progress
        _slider.maxValue = interactionDelay;

        // Ensure SphereCollider is set up correctly
        SphereCollider sphere = GetComponent<SphereCollider>();
        sphere.isTrigger = true;
        sphere.radius = interactionRadius;
    }

    private void Update()
    {
        if (_playerInRange && m_playerInputHandler != null)
        {
            if (m_playerInputHandler.interactHeld)
            {
                // Increase progress over time while interact is held
                _currentProgress += Time.deltaTime;
                _slider.value = _currentProgress;

                // Check for success
                if (_currentProgress >= interactionDelay)
                {
                    Debug.Log("Interaction successful!");
                    
                    interacted = true;

                    if (destroyOnSuccess)
                        Destroy(canvas);
                }
            }
            else
            {
                // Reset progress if interact is released
                ResetProgress();
            }
        }
        else
        {
            // Reset progress if player leaves range
            ResetProgress();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player
        if (other.CompareTag(playerTag))
        {
            m_playerInputHandler = other.GetComponent<PlayerInputHandler>();

            if (m_playerInputHandler != null)
                _playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerInRange = false;
            m_playerInputHandler = null;
            ResetProgress();
        }
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Resets progress and slider value.
    /// </summary>
    private void ResetProgress()
    {
        _currentProgress = 0f;
        if (_slider != null)
            _slider.value = 0f;
    }
    #endregion
}