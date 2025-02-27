using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public int maxHp;
    public IntVariable hp;
    public IntVariable defense;
    public int CurrentHP { get => hp.currentValue; set => hp.SetValue(value);}
    public int MaxHP { get => hp.maxValue;}
    protected Animator animator;
    public bool isDead;

    public GameObject buff;
    public GameObject debuff;
    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        hp.maxValue = maxHp;
        CurrentHP = MaxHP;

        ResetDefense();
    }

    public virtual void TakeDamage(int damage)
    {
        var currentDamage = (damage - defense.currentValue) >= 0 ? (damage - defense.currentValue) : 0;
        var currentDefense = (damage - defense.currentValue) >= 0 ? 0 : (defense.currentValue - damage);
        defense.SetValue(currentDefense);
        if (CurrentHP > damage)
        {
            CurrentHP -= currentDamage;
        }
        else
        {
            CurrentHP = 0;
            // À¿Õˆ
            isDead = true;
        }
    }

    public void UpdataDefense(int amount)
    {
        var value = defense.currentValue + amount;
        defense.SetValue(value);
    }
    public void ResetDefense()
    {
        defense.SetValue(0);
    }

    public void HealHealth(int amount)
    {
        CurrentHP += amount;
        CurrentHP = Mathf.Min(CurrentHP, MaxHP);

        buff.SetActive(true);
    }
}
