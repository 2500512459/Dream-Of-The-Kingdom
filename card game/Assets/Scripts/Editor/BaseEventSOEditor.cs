using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseEventSO<>))]
public class BaseEventSOEditor<T> : Editor
{
    private BaseEventSO<T> baseEventSO;
    private void OnEnable()
    {
        if(baseEventSO == null)
            baseEventSO = (BaseEventSO<T>)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("订阅数量: " + GetListeners().Count);

        foreach (var listener in GetListeners())
        {
            EditorGUILayout.LabelField(listener.ToString());//显示监听者名称
        }
    }

    //返回所有监听者
    private List<MonoBehaviour> GetListeners()
    {
        List<MonoBehaviour> listeners = new();

        if (baseEventSO == null || baseEventSO.onEventRaised == null)
            return listeners;

        var subscribers = baseEventSO.onEventRaised.GetInvocationList();

        foreach( var subscriber in subscribers ) {
            var obj = subscriber.Target as MonoBehaviour;
            if(!listeners.Contains(obj))
                listeners.Add(obj);

        }
        return listeners;
    }
}
