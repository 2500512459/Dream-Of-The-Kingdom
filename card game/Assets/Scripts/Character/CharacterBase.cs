using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 角色基类
public class CharacterBase : MonoBehaviour
{
    public int maxHp;  // 最大生命值
    public IntVariable hp;  // 生命值变量（包含当前值和最大值）
    public IntVariable defense;  // 防御力变量
    public int CurrentHP { get => hp.currentValue; set => hp.SetValue(value);}  // 当前生命值
    public int MaxHP { get => hp.maxValue;} // 最大生命值
    protected Animator animator;  // 动画控制器
    public bool isDead;  // 是否死亡

    public GameObject buff;  // 增益效果对象
    public GameObject debuff;  // 弱化效果对象

    // 在Awake中获取组件（如动画控制器）
    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // 在Start中初始化生命值和防御值
    protected virtual void Start()
    {
        hp.maxValue = maxHp;  // 设置最大生命值
        CurrentHP = MaxHP;  // 初始化当前生命值为最大生命值

        ResetDefense();  // 重置防御值
    }

    // 受到伤害时的处理
    public virtual void TakeDamage(int damage)
    {
        // 计算实际伤害（伤害减去防御值，确保不为负）
        var currentDamage = (damage - defense.currentValue) >= 0 ? (damage - defense.currentValue) : 0;
        // 计算剩余防御力（如果防御力大于伤害，防御力会减少）
        var currentDefense = (damage - defense.currentValue) >= 0 ? 0 : (defense.currentValue - damage);

        // 更新防御力
        defense.SetValue(currentDefense);

        // 处理当前生命值
        if (CurrentHP > damage)
        {
            CurrentHP -= currentDamage;  // 受到伤害后减少生命值
        }
        else
        {
            CurrentHP = 0;  // 死亡时，生命值为0
            // 设置角色为死亡状态
            isDead = true;
        }
    }

    // 增加防御值
    public void UpdataDefense(int amount)
    {
        var value = defense.currentValue + amount;  // 防御力增加
        defense.SetValue(value);  // 更新防御力
    }

    // 重置防御值为0
    public void ResetDefense()
    {
        defense.SetValue(0);  // 防御力清零
    }

    // 治疗生命值
    public void HealHealth(int amount)
    {
        CurrentHP += amount;  // 增加生命值
        CurrentHP = Mathf.Min(CurrentHP, MaxHP);  // 确保生命值不超过最大生命值

        buff.SetActive(true);  // 激活增益效果
    }
}