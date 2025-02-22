using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;


//卡牌效果抽象类
public abstract class Effect : ScriptableObject
{
    public int value;
    public EffectTargetType targetType;//效果应用对象（单体/群体）

    //执行效果
    public abstract void Execute(CharacterBase from, CharacterBase target);

}
