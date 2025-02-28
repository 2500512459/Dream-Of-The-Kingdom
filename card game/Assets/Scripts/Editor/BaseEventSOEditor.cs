using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// 使用CustomEditor特性声明这是一个自定义的编辑器类，作用于BaseEventSO<>泛型类型
[CustomEditor(typeof(BaseEventSO<>))]
public class BaseEventSOEditor<T> : Editor
{
    private BaseEventSO<T> baseEventSO;
    private void OnEnable()
    {
        if(baseEventSO == null)
            baseEventSO = (BaseEventSO<T>)target;
    }
    // 显示订阅者
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();// 先绘制默认的Inspector内容

        EditorGUILayout.LabelField("订阅数量: " + GetListeners().Count);

        // 遍历所有监听者并显示它们的名称
        foreach (var listener in GetListeners())
        {
            EditorGUILayout.LabelField(listener.ToString());//显示监听者名称
        }
    }

    /// <summary>
    /// 获取所有订阅该事件的监听者（MonoBehaviour组件）
    /// </summary>
    /// <returns>订阅者组件列表</returns>
    private List<MonoBehaviour> GetListeners()
    {
        List<MonoBehaviour> listeners = new();

        // 安全检查：当事件为空时返回空列表
        if (baseEventSO == null || baseEventSO.onEventRaised == null)
            return listeners;

        // 获取事件的所有订阅者（委托调用列表）
        var subscribers = baseEventSO.onEventRaised.GetInvocationList();

        // 遍历订阅者并转换为MonoBehaviour组件
        foreach ( var subscriber in subscribers ) {
            var obj = subscriber.Target as MonoBehaviour;
            if(!listeners.Contains(obj))
                listeners.Add(obj);

        }
        return listeners;
    }
}
