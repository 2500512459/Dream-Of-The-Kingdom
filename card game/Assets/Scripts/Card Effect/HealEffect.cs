using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Card Effect/HealEffect")]
public class HealEffect : Effect
{
    public override void Execute(CharacterBase from, CharacterBase target)
    {
        if (EffectTargetType.Self == targetType)
            from.HealHealth(value);

        if(EffectTargetType.Target == targetType)
            target.HealHealth(value);
    }
}
