using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharacterBase
{
    public EnemyActionDataSO actionDataSO;
    public EnemyAction currentAction;

    protected Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public virtual void OnPlayerTurnBegin()
    {
        var randomIndex = Random.Range(0, actionDataSO.actions.Count);
        currentAction = actionDataSO.actions[randomIndex];
    }

    public virtual void OnEnemyTurnBegin()
    {
        switch (currentAction.effect.targetType)
        {
            case EffectTargetType.Self:
                Skill();
                break;
            case EffectTargetType.Target:
                Attack();
                break;
            case EffectTargetType.All:
                break;
        }
    }

    public void Skill()
    {
        StartCoroutine(ProcessDelayAction("skill"));
    }

    public void Attack()
    {
        StartCoroutine(ProcessDelayAction("attack"));
    }

    IEnumerator ProcessDelayAction(string actionName)
    {
        animator.SetTrigger(actionName);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.1f
                                         && animator.IsInTransition(0)
                                         && animator.GetCurrentAnimatorStateInfo(0).IsName(actionName));

        if(actionName == "attack")
            currentAction.effect.Execute(this, player);
        else if(actionName == "skill")
            currentAction.effect.Execute(this, this);
    }
}
