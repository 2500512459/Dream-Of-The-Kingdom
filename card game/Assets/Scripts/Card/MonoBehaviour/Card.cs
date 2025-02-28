using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Card : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    [Header("组件")]
    public SpriteRenderer cardSprite;           // 显示卡牌图像的精灵渲染器
    public TextMeshPro costText, descriptionText, typeText, cardName;// 显示卡牌信息的文本

    public CardDataSO cardData;// 该卡牌的数据对象，包含卡牌的各种属性

    [Header("鼠标触发前的原始数据")]
    public Vector3 originalPosition;   //原始位置
    public Quaternion originalRotation;//原始角度
    public int originalLayerOrder;     //原始层级

    public bool isAnimating;//是否在动画中
    public bool isAvailable;//是否可以打出
    public Player player;

    [Header("广播事件")]
    public ObjectEventSO discardCardEvent;// 用于广播卡牌被丢弃的事件
    public IntEventSO costEvent;          // 用于广播卡牌费用的变化
    public void Start()
    {
        Init(cardData);
    }

    // 使用卡牌数据初始化卡牌的各项属性
    public void Init(CardDataSO data)
    {
        cardData = data;// 赋值卡牌数据
        cardSprite.sprite = data.cardImage;// 设置卡牌的图像
        costText.text = data.cost.ToString();// 设置卡牌的费用文本
        descriptionText.text = data.description;// 设置卡牌描述文本
        cardName.text = data.cardName;// 设置卡牌名称文本

        // 根据卡牌类型设置类型文本
        typeText.text = data.cardType switch
        {
            CardType.Attack => "攻击",// 攻击类型
            CardType.Abilities => "技能",// 技能类型
            CardType.Defense => "防御",// 防御类型
            _ => throw new System.NotImplementedException(),
        };

        // 查找并赋值玩家对象
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    // 更新卡牌的位置和旋转角度
    public void UpdataPositionRotation(Vector3 Position, Quaternion Rotation)
    {
        originalPosition = Position;// 更新原始位置
        originalRotation = Rotation;// 更新原始旋转
        originalLayerOrder = GetComponent<SortingGroup>().sortingOrder;// 更新原始的层级排序
    }

    // 鼠标移入卡牌时触发的事件
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAnimating)   // 如果卡牌没有在执行动画
        {
            // 设定卡牌的新位置，使其浮动
            //transform.position = originalPosition + Vector3.up;// 这样无法保证卡牌高度一致
            Vector3 newPosition = transform.position;
            newPosition.y = -3.5f;//默认高度为-3.5f保证卡牌高度一致
            transform.position = newPosition;
            transform.rotation = Quaternion.identity;
            // 提高卡牌的层级，使其显示在上面
            GetComponent<SortingGroup>().sortingOrder = 20;
        }
    }

    // 鼠标移出卡牌时触发的事件
    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        if (!isAnimating)   // 如果卡牌没有在执行动画
        {
            RestCardTransform();    // 恢复卡牌的原始位置和旋转
        } 
    }

    // 恢复卡牌的原始位置和旋转角度
    public void RestCardTransform()
    {
        transform.SetLocalPositionAndRotation(originalPosition, originalRotation);
        GetComponent<SortingGroup>().sortingOrder = originalLayerOrder; // 恢复卡牌的层级
    }

    // 执行卡牌的效果（例如攻击、防御等）
    public void ExecuteCardEffect(CharacterBase from, CharacterBase target)
    {
        // 广播扣除卡牌费用并丢弃卡牌的事件
        costEvent.RaiseEvent(cardData.cost, this);
        discardCardEvent.RaiseEvent(this, this);

        // 执行卡牌上定义的所有效果
        foreach (var effect in cardData.effects)
        {
            effect.Execute(from, target);// 执行卡牌的每个效果
        }
    }
    /// <summary>
    /// 更新卡牌状态，检查卡牌是否可以打出（例如检查玩家的能量）
    /// </summary>
    public void UpdataCardState()
    {
        // 判断是否可以打出卡牌：检查玩家当前的能量是否足够
        isAvailable = player.CurrentMana >= cardData.cost;

        // 根据卡牌是否可以打出设置费用文本的颜色
        costText.color = isAvailable ? Color.green : Color.red;
    }
}
