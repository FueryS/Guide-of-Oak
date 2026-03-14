using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageEnemy : MonoBehaviour
{
    [Serializable]
    public struct DamageData
    {
        public string attackName;
        public int damage;
    }

    [SerializeField]
    private List<DamageData> damageEntries = new List<DamageData>()
    {
        new DamageData { attackName = "Great Sword Slash", damage = 10 },
        new DamageData { attackName = "Great Sword Kick", damage = 20 }
    };

    private Dictionary<string, int> damageAmountDisctionary = new Dictionary<string, int>();

    [SerializeField] Animator Animator;

    private void Awake()
    {
        foreach (var entry in damageEntries)
        {
            damageAmountDisctionary[entry.attackName] = entry.damage;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyStatManager enemy = other.GetComponent<EnemyStatManager>();

        if (enemy == null)
        {   
            return;
        }

        string currentAttack = Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (damageAmountDisctionary.TryGetValue(currentAttack, out int damageAmount))
        {
            enemy.AcceptDamage(damageAmount);
        }
        else
        {
            Debug.LogWarning($"No damage amount found for attack: {currentAttack}");
        }
    }
}