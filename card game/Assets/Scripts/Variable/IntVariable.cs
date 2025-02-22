using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IntVariable", menuName = "Variables/IntVariable")]
public class IntVariable : ScriptableObject
{
    public int maxValue;
    public int currentValue;
    public IntEventSO ValueChangedEvent;

    [TextArea]
    [SerializeField] private string description;

    // �㲥��Ҫ�ı��ֵ
    public void SetValue(int value)
    {
        currentValue = value;
        ValueChangedEvent.RaiseEvent(value, this);
    }
}
