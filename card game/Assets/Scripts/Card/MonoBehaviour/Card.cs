using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("���")]
    public SpriteRenderer cardSprite;
    public TextMeshPro costText, descriptionText, typeText, cardName;

    public CardDataSO cardData;

    public void Start()
    {
        Init(cardData);
    }

    public void Init(CardDataSO data)
    {
        cardData = data;
        cardSprite.sprite = data.cardImage;
        costText.text = data.cost.ToString();
        descriptionText.text = data.description;
        cardName.text = data.cardName;
        typeText.text = data.cardType switch
        {
            CardType.Attack => "����",
            CardType.Defense => "����",
            CardType.Abilities => "����",
            _ => throw new System.NotImplementedException(),
        };
    }
}
