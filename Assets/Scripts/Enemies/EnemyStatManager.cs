using System.Collections;
using UnityEngine;

public class EnemyStatManager : MonoBehaviour
{
    public float IframesDuration = 0.5f; // Duration of invincibility frames after taking damage
    public float HitPoints = 100f; // Total health of the enemy

    #region Private Variables
    bool _invinsible = false;
    float _currentHP = 100f;
    #endregion

    #region Testing
    Renderer _renderer;

    public Color startColor = Color.red;
    public Color endColor = Color.black;
    Color iFrameColor = Color.white;

    float _lerpValue;
    #endregion


    public void Start()
    {
        _currentHP = HitPoints; // Initialize current HP to total hit points


        //----------- Testing -----------
        _renderer = GetComponent<Renderer>();
    }

    public void AcceptDamage(float damageAmount)
    {
        if (_invinsible) return; // If currently invincible, ignore damage
        _invinsible = true; 
        _currentHP -= Mathf.Min(damageAmount, _currentHP); // Reduce HP by damage amount, ensuring it doesn't go below zero

        //Start the Ifrmae
        StartCoroutine(IFrame());

        if (_currentHP <= 0) Destroy(gameObject);
    }

    IEnumerator IFrame()
    {
        DamageIndicator_testing();
        yield return new WaitForSeconds(IframesDuration);
        _invinsible = false;
        DamageIndicator_testing();
    }


    public void DamageIndicator_testing()
    {
        if (_invinsible) { _renderer.material.color = iFrameColor; return; }

        float _lerpValue = 1 - (_currentHP / HitPoints); // Calculate lerp value based on current HP
        _renderer.material.color = Color.Lerp( startColor,endColor, _lerpValue); // Lerp between start and end colors based on HP
    }

}
