using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GamePlayPanel : MonoBehaviour
{
    private VisualElement rootElement;
    private Label energyAmountLabel, drawAmountLabel, discardAmountLabel, turnLabel;
    private Button endTurnButton;
    private void OnEnable()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        //这里添加你的UI元素和事件处理代码
        energyAmountLabel = rootElement.Q<Label>("EnergyAmount");
        drawAmountLabel = rootElement.Q<Label>("DrawAmount");
        discardAmountLabel = rootElement.Q<Label>("DiscardAmount");
        turnLabel = rootElement.Q<Label>("TurnLabel");
        endTurnButton = rootElement.Q<Button>("EndTurn");

        energyAmountLabel.text = "0";
        drawAmountLabel.text = "0";
        discardAmountLabel.text = "0";
        turnLabel.text = "游戏开始";
    }

    public void UpdataDrawAmount(int amount)
    {
        drawAmountLabel.text = amount.ToString();
    }
    public void UpdataDiscardAmount(int amount)
    {
        discardAmountLabel.text = amount.ToString();
    }
}
