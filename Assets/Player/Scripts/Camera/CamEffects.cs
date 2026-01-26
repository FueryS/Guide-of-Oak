using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamEffects : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeAmplitude = 0.05f; // base strength of shake
    [SerializeField] private float smoothness = 1f;        // how smoothly it blends back

    private Vector3 originalLocalPos;
    private float shakeTimer;

    void Start()
    {
        originalLocalPos = transform.localPosition;
    }

    /// <summary>
    /// Call this every frame with a float v (e.g., player speed).
    /// Higher v = stronger shake. Mild AAA-style vertical bobbing.
    /// </summary>
    public void ApplyShake(float v, int shakeFrequency)
    {
        if (v <= 0.01f)
        {
            // Reset to original position when idle
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalLocalPos, Time.deltaTime * smoothness);
            return;
        }

        shakeTimer += Time.deltaTime * shakeFrequency;

        // Vertical sinusoidal shake (up/down bobbing)
        float offsetY = Mathf.Sin(shakeTimer) * shakeAmplitude * (v);

        // Apply shake only in Y direction, keep X/Z stable
        Vector3 targetPos = originalLocalPos + new Vector3(0f, offsetY, 0f);

        // Smoothly interpolate
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smoothness);
    }

    public void PushBack(float v)
    {
        Vector3 targetPos = originalLocalPos + (Vector3.forward * v);
        transform.localPosition= Vector3.Lerp(transform.localPosition, targetPos , Time.deltaTime * smoothness);
    }
}
   
