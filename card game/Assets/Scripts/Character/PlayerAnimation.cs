using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // 引用 Player 组件，用于获取玩家的相关状态
    private Player player;
    // 引用 Animator 组件，用于控制角色动画
    private Animator animator;

    private void Awake()
    {
        // 获取当前 GameObject 上的 Player 组件
        player = GetComponent<Player>();
        // 获取子对象中的 Animator 组件
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        // 启用时播放“睡眠”动画，并设置 isSleep 变量为 true
        animator.Play("sleep");
        animator.SetBool("isSleep", true);
    }

    // 玩家回合开始时调用，取消睡眠和格挡动画状态
    public void PlayerTurnBeginAnimation()
    {
        animator.SetBool("isSleep", false);
        animator.SetBool("isParry", false);
    }

    // 玩家回合结束时调用，根据玩家的防御值决定动画状态
    public void PlayerTurnEndAnimation()
    {
        if (player.defense.currentValue > 0)
        {
            // 如果玩家有防御值，播放“格挡”动画
            animator.SetBool("isParry", true);
        }
        else
        {
            // 否则播放“睡眠”动画，并取消“格挡”动画
            animator.SetBool("isSleep", true);
            animator.SetBool("isParry", false);
        }
    }

    // 播放不同卡牌对应的动画
    public void PlayCardEvent(object obj)
    {
        // 将传入的对象转换为 Card 类型
        Card card = obj as Card;

        // 根据卡牌类型触发不同的动画
        switch (card.cardData.cardType)
        {
            case CardType.Attack:
                // 攻击类卡牌，触发“攻击”动画
                animator.SetTrigger("attack");
                break;
            case CardType.Defense:
            case CardType.Abilities:
                // 防御类和能力类卡牌，触发“技能”动画
                animator.SetTrigger("skill");
                break;
        }
    }

    public void SetSleepAnimation()
    {
        animator.Play("death");
    }
}
