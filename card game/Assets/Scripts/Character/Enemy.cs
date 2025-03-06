using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人类，继承自 CharacterBase，处理敌人行为和回合逻辑
/// </summary>
public class Enemy : CharacterBase
{
    // 敌人行动数据，存储可用的行动信息
    public EnemyActionDataSO actionDataSO;

    // 当前敌人选定的行动
    public EnemyAction currentAction;

    // 玩家引用，用于敌人攻击时的目标
    protected Player player;

    /// <summary>
    /// Awake 方法，在对象初始化时调用
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        // 通过标签查找场景中的 Player 组件
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    /// <summary>
    /// Start 方法，在游戏开始时调用
    /// </summary>
    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// 当玩家回合开始时调用，随机选择敌人的下一个行动
    /// </summary>
    public virtual void OnPlayerTurnBegin()
    {
        var randomIndex = Random.Range(0, actionDataSO.actions.Count);
        currentAction = actionDataSO.actions[randomIndex]; // 随机选择一个行动
    }

    /// <summary>
    /// 当敌人回合开始时调用，根据当前行动的目标类型执行相应的操作
    /// </summary>
    public virtual void OnEnemyTurnBegin()
    {
        switch (currentAction.effect.targetType)
        {
            case EffectTargetType.Self:
                Skill(); // 如果目标是自己，则执行技能
                break;
            case EffectTargetType.Target:
                Attack(); // 如果目标是敌人，则执行攻击
                break;
            case EffectTargetType.All:
                // 目前未实现对所有目标的行动
                break;
        }
    }

    /// <summary>
    /// 执行技能（作用于自身）
    /// </summary>
    public void Skill()
    {
        StartCoroutine(ProcessDelayAction("skill"));
    }

    /// <summary>
    /// 执行攻击（作用于玩家）
    /// </summary>
    public void Attack()
    {
        StartCoroutine(ProcessDelayAction("attack"));
    }

    /// <summary>
    /// 处理延迟执行的动作（如攻击或技能），等待动画播放到一定时间后执行效果
    /// </summary>
    /// <param name="actionName">要执行的动作名称</param>
    IEnumerator ProcessDelayAction(string actionName)
    {
        animator.SetTrigger(actionName); // 触发相应的动画

        // 等待动画播放到一定时间后再执行实际效果，避免动画未播放完毕就结算
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.1f // 确保动画已经开始播放
            && animator.IsInTransition(0) // 检测是否在动画过渡阶段
            && animator.GetCurrentAnimatorStateInfo(0).IsName(actionName)); // 确保当前动画是指定的动作

        // 根据动作类型执行不同的效果
        if (actionName == "attack")
            currentAction.effect.Execute(this, player); // 攻击时，作用于玩家
        else if (actionName == "skill")
            currentAction.effect.Execute(this, this); // 技能时，作用于自身
    }
}
