using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefenseEffect", menuName = "Card Effect/DefenseEffect")]
public class DefenseEffect : Effect
{
    public override void Execute(CharacterBase from, CharacterBase target)
    {   
        if(EffectTargetType.Self == targetType)
            from.UpdataDefense(value);

        if (EffectTargetType.Target == targetType)
            target.UpdataDefense(value);
    }
}
