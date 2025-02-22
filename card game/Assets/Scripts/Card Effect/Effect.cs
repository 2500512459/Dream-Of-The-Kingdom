using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;


//����Ч��������
public abstract class Effect : ScriptableObject
{
    public int value;
    public EffectTargetType targetType;//Ч��Ӧ�ö��󣨵���/Ⱥ�壩

    //ִ��Ч��
    public abstract void Execute(CharacterBase from, CharacterBase target);

}
