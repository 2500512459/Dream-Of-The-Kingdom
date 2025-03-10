using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class PickCardPanel : MonoBehaviour
{
    public CardManager cardManager;
    private VisualElement rootElement;
    public VisualTreeAsset cardTemplate;
    private VisualElement cardContainer;
    private CardDataSO currentCardData;

    private List<Button> cardButtons = new();

    private Button confirmButton;

    [Header("广播")]
    public ObjectEventSO finishPickCardEvent;
    private void OnEnable()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        cardContainer = rootElement.Q<VisualElement>("Container");
        confirmButton = rootElement.Q<Button>("ConfirmButton");

        confirmButton.clicked += OnConfirmButtonClicked;

        for (int i = 0; i < 3; i++)
        {
            var card = cardTemplate.Instantiate();
            var data = cardManager.GetNewCardData();
            //初始化
            InitCard(card, data);
            var cardButton = card.Q<Button>(name: "Card");
            cardContainer.Add(card);
            cardButtons.Add(cardButton);
            cardButton.clicked += () => OnCardClicked(cardButton, data);
        }
    }

    private void OnConfirmButtonClicked()
    {
        cardManager.UnlockCard(currentCardData);
        finishPickCardEvent.RaiseEvent(null, this);
    }

    private void OnCardClicked(Button cardButton, CardDataSO data)
    {
        currentCardData = data;
        //Debug.Log(currentCardData.cardName);
        for (int i = 0; i < cardButtons.Count; i++)
        {
            if (cardButtons[i] == cardButton)
                cardButtons[i].SetEnabled(false);
            else
                cardButtons[i].SetEnabled(true);
        }
    }

    public void InitCard(VisualElement card, CardDataSO cardData)
    {
        card.dataSource = cardData;

        var cardSpriteElement = card.Q<VisualElement>(name: "CardSprite");
        var cardCost = card.Q<Label>(name: "EnergyCost");
        var cardDescription = card.Q<Label>(name: "CardDescription");
        var cardType = card.Q<Label>(name: "CardType");
        var cardName = card.Q<Label>(name: "CardName");

        cardSpriteElement.style.backgroundImage = new StyleBackground(cardData.cardImage);
        cardName.text = cardData.cardName;
        cardCost.text = cardData.cost.ToString();
        cardDescription.text = cardData.description;
        cardType.text = cardData.cardType switch
        {
            CardType.Attack => "攻击",
            CardType.Abilities => "能力",
            CardType.Defense => "技能",
            _ => throw new System.NotImplementedException(),
        };
    }
}
