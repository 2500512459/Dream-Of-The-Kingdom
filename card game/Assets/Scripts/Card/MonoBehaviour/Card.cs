using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Card : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    [Header("组件")]
    public SpriteRenderer cardSprite;
    public TextMeshPro costText, descriptionText, typeText, cardName;

    public CardDataSO cardData;

    [Header("鼠标触发前的原始数据")]
    public Vector3 originalPosition;   //原始位置
    public Quaternion originalRotation;//原始角度
    public int originalLayerOrder;     //原始层级

    public bool isAnimating;//是否在动画中

    public Player player;
    [Header("广播事件")]
    public ObjectEventSO discardCardEvent;
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
            CardType.Attack => "攻击",
            CardType.Defense => "技能",
            CardType.Abilities => "防御",
            _ => throw new System.NotImplementedException(),
        };
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    //更新卡牌的位置和旋转
    public void UpdataPositionRotation(Vector3 Position, Quaternion Rotation)
    {
        originalPosition = Position;
        originalRotation = Rotation;
        originalLayerOrder = GetComponent<SortingGroup>().sortingOrder;
    }
    //鼠标移入卡牌
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            //transform.position = originalPosition + Vector3.up;
            Vector3 newPosition = transform.position;
            newPosition.y = -3.5f;//默认高度为-3.5f保证卡牌高度一致
            transform.position = newPosition;
            transform.rotation = Quaternion.identity;
            GetComponent<SortingGroup>().sortingOrder = 20;
        }
    }
    //鼠标离开卡牌
    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        if (!isAnimating)
            RestCardTransform();
    }
    //恢复卡牌的位置和旋转
    public void RestCardTransform()
    {
        transform.SetLocalPositionAndRotation(originalPosition, originalRotation);
        GetComponent<SortingGroup>().sortingOrder = originalLayerOrder;
    }

    //执行卡牌
    public void ExecuteCardEffect(CharacterBase from, CharacterBase target)
    {
        //TODO:减少对应能量，通知回收卡牌
        discardCardEvent.RaiseEvent(this, this);
        //执行这张卡牌的所有效果
        foreach (var effect in cardData.effects)
        {
            effect.Execute(from, target);
        }
    }
}
