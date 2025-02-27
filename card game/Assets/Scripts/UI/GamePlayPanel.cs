using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GamePlayPanel : MonoBehaviour
{
    private VisualElement rootElement;
    private Label energyAmountLabel, drawAmountLabel, discardAmountLabel, turnLabel;
    private Button endTurnButton;

    private VisualElement defenseElement;
    private Label defenseAmountLabel;

    [Header("广播")]
    public ObjectEventSO playerTurnEnd;
    private void OnEnable()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        //这里添加你的UI元素和事件处理代码
        energyAmountLabel = rootElement.Q<Label>("EnergyAmount");
        drawAmountLabel = rootElement.Q<Label>("DrawAmount");
        discardAmountLabel = rootElement.Q<Label>("DiscardAmount");
        turnLabel = rootElement.Q<Label>("TurnLabel");
        endTurnButton = rootElement.Q<Button>("EndTurn");

        endTurnButton.clicked += OnEndTurnButtenClicked;

        energyAmountLabel.text = "0";
        drawAmountLabel.text = "0";
        discardAmountLabel.text = "0";
        turnLabel.text = "游戏开始";
    }

    private void OnEndTurnButtenClicked()
    {
        playerTurnEnd.RaiseEvent(null, this);
    }

    public void UpdataDrawAmount(int amount)
    {
        drawAmountLabel.text = amount.ToString();
    }
    public void UpdataDiscardAmount(int amount)
    {
        discardAmountLabel.text = amount.ToString();
    }
    public void UpdataEnergyAmount(int amount)
    {
        energyAmountLabel.text = amount.ToString();
    }
    public void OnEnemyTurnBegin()
    {
        endTurnButton.SetEnabled(false);
        turnLabel.text = "敌人回合";
        turnLabel.style.color = new StyleColor(Color.red);
    }

    public void OnPlayerTurnBegin()
    {
        endTurnButton.SetEnabled(true);
        turnLabel.text = "你的回合";
        turnLabel.style.color = new StyleColor(Color.white);
    }


}
