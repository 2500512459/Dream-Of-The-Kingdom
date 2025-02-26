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
}
