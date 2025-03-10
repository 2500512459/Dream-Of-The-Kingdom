using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class GameWinPanel : MonoBehaviour
{
    private VisualElement rootElement;
    private Button backToMapButton;
    private Button pickCardButton;

    [Header("¹ã²¥")]
    public ObjectEventSO loadMapEvent;
    public ObjectEventSO pickCardEvent;

    private void Awake()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        backToMapButton = rootElement.Q<Button>("BackToMapButton");
        pickCardButton = rootElement.Q<Button>("PickCardButton");

        backToMapButton.clicked += OnBackToMapButtonClicked;
        pickCardButton.clicked += OnPickCardButtonClicked;
    }

    private void OnPickCardButtonClicked()
    {
        pickCardEvent.RaiseEvent(null, this);
    }

    private void OnBackToMapButtonClicked()
    {
        loadMapEvent.RaiseEvent(null, this);
    }

    public void OnFinishPickCardEvent()
    {
        pickCardButton.style.display = DisplayStyle.None;
    }
}
