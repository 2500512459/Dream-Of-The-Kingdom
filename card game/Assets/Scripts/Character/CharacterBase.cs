using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public int maxHp;
    public IntVariable hp;
    public int CurrentHP { get => hp.currentValue; set => hp.SetValue(value);}
    public int MaxHP { get => hp.maxValue;}
    protected Animator animator;
    private bool isDead;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        hp.maxValue = maxHp;
        CurrentHP = MaxHP;
    }

    public virtual void TakeDamage(int damage)
    {
        if (CurrentHP > damage)
        {
            CurrentHP -= damage;
            Debug.Log("µ±Ç°ÑªÁ¿£º" + CurrentHP);
        }
        else
        {
            CurrentHP = 0;
            // ËÀÍö
            isDead = true;
        }
    }
}
