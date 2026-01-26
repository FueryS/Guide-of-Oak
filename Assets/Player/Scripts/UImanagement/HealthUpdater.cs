using System;
using UnityEngine;
using UnityEngine.UI;



public class HealthUpdater : MonoBehaviour
{
    [SerializeField] Stats m_stats;
    [SerializeField] Slider m_health;
    [SerializeField] Slider m_lerp;

    [ContextMenuItem("Load Health", "GetMaxHP")]
    float _maxHP;

    float _currentHP;

    [SerializeField][Range(0f, 0.1f)] float lerpSpeed = 0.05f;


    void Awake()
    {
        if (m_health == null || m_lerp == null)
        {
            Debug.LogWarning(
                "<color=#ffffff>[HealthUpdater]</color> value of <color=#ffffff>m_health</color> " +
                "or <color=#CDAC8B>m_lerp</color> is null\nModules are being auto-assigned and may lead to unintended behaviour"
            );
            Slider[] s = GetComponentsInChildren<Slider>();
            m_health = s[1];
            m_lerp = s[0];
            if (m_stats == null)
            {
                Debug.LogWarning("<color = #ffffff>[HealthUpdater]</color> value of m_stats is null");
                this.enabled = false;
            }
        }
    }

    void Start()
    {


        GetMaxHP();
    }

    void Update()
    {
        ShowCurrentHP();
    }

    void GetMaxHP()
    {
        _maxHP = m_stats.GetHp();
    }

    void ShowCurrentHP()
    {
        _currentHP = m_stats.GetHealth();
        if (m_health.value != _currentHP)
        {
            m_health.value = _currentHP;
        }

        if (m_lerp.value != m_health.value)
        {
            m_lerp.value = Mathf.Lerp(m_lerp.value, m_health.value, lerpSpeed);
        }

    }

}
