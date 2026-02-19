using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using System.Collections;


public class Stats : MonoBehaviour
{
    float _health;

    [ContextMenuItem("Damage Player","_Damage")]
    [ContextMenuItem("Heal Player","_Heal")]
    [SerializeField] float HP;
    [SerializeField] float _iframeTime = 0.5f;

    bool _iframe= false;
    bool _isDead = false;

    private void Awake()
    {
        if (HP < 1)
        {
            Debug.LogWarning("Value of HP is 0 or less nigga");
        }

        _health = HP;
    }

    [ContextMenu("Load HP")]
    void _LoadHp()
    {
        _health = HP;
    }


    
    public void DeathEvent()
    {
        if (_isDead) return; // If we're already dying, don't die again!
        _isDead = true;

        foreach (var smr in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            smr.enabled = false;
        }

        Invoke("ReloadScene", 0.5f);
    }

    public void ReloadScene()
    {
        GameLoader.Instance.ReloadScene();
    }

    public void Damage(float d)
    {
        if (_iframe) return;
        this._health -= d;
        if (_health < 1) DeathEvent();
        StartCoroutine("Iframe");
    }
    public void Damage(float d, bool ignoreIframe)
    {
        _iframe = !ignoreIframe;
        if (_iframe) return;
        this._health -= d;
        if (_health < 1) DeathEvent();
        StartCoroutine("Iframe");
    }

    IEnumerator Iframe()
    {
        _iframe = true;
        yield return new WaitForSeconds(_iframeTime);
        _iframe = false;
    }

    private void _Damage()
    {
        this._health -= 10;
        if (_health < 1 ) DeathEvent(); ;
    }

    public void Heal(float heal)
    {
        this._health += heal;
        clampHealth();
    }
    private void _Heal()
    {
        this._health += 10;
        clampHealth();
    }

    public void OverHeal(float heal) 
    {
        this._health += heal;
    }
    public void clampHealth()
    {
        this._health = Mathf.Clamp(_health, 0f, HP);
    }

    public void InstaKill()
    {
        this._health = 0;
        DeathEvent();
    }

    public void SetHP(float hp)
    {
        this.HP = hp;
    }

    public float GetHp()
    {
        return this._health;
    }
    public float GetHealth()
    {
        return this._health;
    }

}
