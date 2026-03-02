using System;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public float rotationFactor = 180f;
    public float xPlanePosition = 0f;

    Transform _playerTransformChache;

    void Awake()
    {
        _playerTransformChache = transform;
    }

    [ContextMenu("Fix Rotation")]
    void FixRotation()
    {
        float currentYRotation = transform.localEulerAngles.y;

        float fixedRotation = Mathf.Round(currentYRotation / rotationFactor) * rotationFactor;

        _playerTransformChache.localRotation = Quaternion.Euler(0f, fixedRotation, 0f);

        Debug.Log($"Read: {currentYRotation}, Calculated Snap: {fixedRotation}");
    }

    void TurnAroundStart()
    {

    }


    private void FixedUpdate()
    {
        Vector3 currentPosition = transform.position;
        _playerTransformChache.position = new Vector3(xPlanePosition, currentPosition.y, currentPosition.z);
    }
}

