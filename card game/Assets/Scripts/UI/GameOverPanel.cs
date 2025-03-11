using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverPanel : MonoBehaviour
{
    private VisualElement rootElement;
    private Button backToStartButton;
    public ObjectEventSO loadMenuEvent;

    private void OnEnable()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        backToStartButton = rootElement.Q<Button>("BackToStartButton");

        backToStartButton.clicked += BackToStart;
    }

    private void BackToStart()
    {
        loadMenuEvent.RaiseEvent(null, this);
    }
}
