using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 休息房间UI控制面板，负责处理玩家在休息房间的交互逻辑
/// </summary>
public class RestRoomPanel : MonoBehaviour
{
    // ---------- UI元素 ----------
    private VisualElement rootElement;    // 根视觉元素
    private Button restButton;            // 休息按钮
    private Button backToMapButton;       // 返回地图按钮

    // ---------- 游戏逻辑 ----------
    public Effect restEffect;             // 休息时触发的效果
    private CharacterBase player;         // 玩家角色引用

    // ---------- 事件系统 ----------
    [Header("广播")]
    public ObjectEventSO loadMapEvent;    // 加载地图事件

    private void Start()
    {
        // 初始化UI元素
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        restButton = rootElement.Q<Button>("RestButton");
        backToMapButton = rootElement.Q<Button>("BackToMapButton");

        // 查找场景中的玩家对象（包含未激活对象）
        player = FindAnyObjectByType<Player>(FindObjectsInactive.Include);

        // 注册按钮点击事件
        restButton.clicked += OnRestButtonClicked;
        backToMapButton.clicked += OnBackToMapButtonClicked;
    }

    /// <summary>
    /// 返回地图按钮点击处理
    /// </summary>
    private void OnBackToMapButtonClicked()
    {
        // 触发加载地图事件
        loadMapEvent.RaiseEvent(null, this);
    }

    /// <summary>
    /// 休息按钮点击处理
    /// </summary>
    private void OnRestButtonClicked()
    {
        // 对玩家执行休息效果
        restEffect.Execute(player, null);

        // 禁用休息按钮防止重复点击
        restButton.SetEnabled(false);
    }
}
