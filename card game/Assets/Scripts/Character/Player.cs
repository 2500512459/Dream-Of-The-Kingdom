using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterBase
{
    public IntVariable playerMana;

    public int MaxMana;
    public int CurrentMana { get => playerMana.currentValue; set=>playerMana.SetValue(value);}

    private void OnEnable()
    {
        playerMana.maxValue = MaxMana;
        CurrentMana = playerMana.maxValue;//设置初始法力值
    }

    /// <summary>
    /// 监听事件函数，在新的回合开始时调用
    /// </summary>
    public void NewTurn()
    {
        CurrentMana = MaxMana;//新的回合重置法力值
    }

    /// <summary>
    /// 监听事件函数，在消耗法力值时调用
    /// </summary>
    /// <param name="cost"></param>
    public void UpdataMana(int cost)
    {
        CurrentMana -= cost;
        if (playerMana.currentValue <= 0)
        {
            CurrentMana = 0;
        }
    }

    public void NewGame()
    {
        isDead = false;
        CurrentHP = MaxHP;  // 初始化当前生命值为最大生命值
        buffRound.currentValue = 0;// 重置buffRound为0
        ResetDefense();  // 重置防御值
        CurrentMana = MaxMana;//新的回合重置法力值
    }
}
