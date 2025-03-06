using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

public class CardDeck : MonoBehaviour
{
    // 负责管理卡牌的实例
    public CardManager cardManager;
    // 负责卡牌布局管理的实例
    public CardLayoutManager cardLayoutManager;
    // 牌堆的位置
    public Vector3 deckPosition;

    // 抽牌堆：存储当前还未抽取的卡牌
    public List<CardDataSO> drawDeck = new List<CardDataSO>();
    // 弃牌堆：存储已经使用过的卡牌   
    public List<CardDataSO> discardDeck = new List<CardDataSO>();
    // 手牌列表：存储当前玩家手上的卡牌对象
    public List<Card> handCardObjectList = new List<Card>();      

    [Header("广播事件")]
    // 用于更新抽牌堆和弃牌堆数量的事件
    public IntEventSO drawCountEvent;
    public IntEventSO discardCountEvent;

    //测试
    private void Start()
    {
        InitializeDeck();
    }

    // 初始化牌堆，将所有卡牌加入抽牌堆，并洗牌
    public void InitializeDeck()
    {
        drawDeck.Clear();// 清空抽牌堆
        // 遍历当前卡组，将每种卡牌按数量加入抽牌堆
        foreach (var entry in cardManager.currentCardLibrary.cardLibraryList)
        {
            for (int i = 0; i < entry.amount; i++)
            {
                drawDeck.Add(entry.cardData);
            }
        }

        // 洗牌（改变牌堆顺序）
        ShuffleDeck();
    }

    // 测试抽牌功能，用于调试
    [ContextMenu("测试抽牌")]
    public void TestDrawCard()
    {
        DrawCard(1);
    }
    /// <summary>
    /// 事件函数，新回合开始时，抽牌
    /// </summary>
    public void NewTurnDrawCards()
    {
        DrawCard(4);
    }

    /// <summary>
    /// 抽牌逻辑函数，抽取指定数量的卡牌
    /// </summary>
    /// <param name="amount">要抽取的卡牌数量</param>
    public void DrawCard(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // 如果抽牌堆已空，则将弃牌堆中的卡牌重新加入抽牌堆
            if (drawDeck.Count == 0)
            {
                foreach (var item in discardDeck)
                {
                    drawDeck.Add(item);
                }
                // 重新洗牌
                ShuffleDeck();
            }
            // 抽取一张卡牌
            CardDataSO cardData = drawDeck[0];
            drawDeck.RemoveAt(0);

            // 更新UI显示抽牌堆的剩余卡牌数量
            drawCountEvent.RaiseEvent(drawDeck.Count, this);

            // 获取卡牌对象并初始化
            var card = cardManager.GetCardObject(cardData).GetComponent<Card>();
            card.Init(cardData);//初始化卡牌
            card.transform.position = deckPosition;//设置卡牌抽出的位置（牌堆）

            // 将卡牌添加到手牌列表
            handCardObjectList.Add(card);

            // 每次抽牌都设置一次布局位置，带延迟动画效果
            var delay = i * 0.2f;
            SetCardLayout(delay);
        }
        
    }

    /// <summary>
    /// 设置卡牌在屏幕上的布局位置
    /// </summary>
    /// <param name="delay">卡牌布局的延迟</param>
    public void SetCardLayout(float delay)
    {
        for (int i = 0; i < handCardObjectList.Count; i++)
        {
            // 获取当前卡牌和布局信息
            Card currentCard = handCardObjectList[i];
            CardTransform cardTransform = cardLayoutManager.GetCardTransform(i, handCardObjectList.Count);
            //currentCard.transform.SetLocalPositionAndRotation(cardTransform.pos, cardTransform.rotation);//设置卡牌的位置和旋转角度(无动画)

            //每次卡牌布局重新布局都更新卡牌的状态（如是否可以打出等）
            currentCard.UpdataCardState();

            //设置卡牌正在动画中
            currentCard.isAnimating = true;

            // 使用DOTween设置卡牌的动画（缩放、移动、旋转）
            currentCard.transform.DOScale(Vector3.one, 0.2f).SetDelay(delay).onComplete = () =>
            {
                currentCard.transform.DOMove(cardTransform.pos, 0.5f).onComplete = () => currentCard.isAnimating = false;
                currentCard.transform.DORotateQuaternion(cardTransform.rotation, 0.5f);
            };
            
            //设置卡牌的排序（层序）
            currentCard.GetComponent<SortingGroup>().sortingOrder = i;
            // 更新卡牌的具体位置和旋转角度
            currentCard.UpdataPositionRotation(cardTransform.pos, cardTransform.rotation);
        }
    }

    /// <summary>
    /// 洗牌功能，随机打乱抽牌堆顺序
    /// </summary>
    private void ShuffleDeck()
    {
        discardDeck.Clear();//每次洗牌清空弃牌堆
        //更新UI显示牌堆数量
        drawCountEvent.RaiseEvent(drawDeck.Count, this);
        discardCountEvent.RaiseEvent(discardDeck.Count, this);
        //交换顺序
        for (int i = 0; i < drawDeck.Count; i++)
        {
            CardDataSO temp = drawDeck[i];
            int randomIndex = Random.Range(i, drawDeck.Count);
            drawDeck[i] = drawDeck[randomIndex];
            drawDeck[randomIndex] = temp;
        }
    }

    /// <summary>
    /// 回收卡牌逻辑，将卡牌加入弃牌堆，并移除手牌列表
    /// </summary>
    /// <param name="obj">被丢弃的卡牌对象</param>
    public void DiscardCard(object obj)
    {
        Card card = obj as Card;
        // 将卡牌加入弃牌堆
        discardDeck.Add(card.cardData);
        // 从手牌中移除卡牌
        handCardObjectList.Remove(card);

        // 将卡牌返回给卡牌对象池
        cardManager.ReturnCardObject(card.gameObject);

        // 更新UI显示弃牌堆数量
        discardCountEvent.RaiseEvent(discardDeck.Count, this);

        // 更新手牌布局
        SetCardLayout(0);
    }

    /// <summary>
    /// 玩家回合结束时，将所有手牌加入弃牌堆并清空手牌
    /// </summary>
    public void OnPlayerTurnEnd()
    {
        // 将所有手牌加入弃牌堆
        for (int i = 0; i < handCardObjectList.Count; i++)
        {
            discardDeck.Add(handCardObjectList[i].cardData);
            // 将卡牌返回给卡牌对象池
            cardManager.ReturnCardObject(handCardObjectList[i].gameObject);
        }
        // 清空手牌列表
        handCardObjectList.Clear();
        // 更新UI显示弃牌堆数量
        discardCountEvent.RaiseEvent(discardDeck.Count, this);
    }

    public void ReleaseAllCards(object obj)
    {
        foreach (var card in handCardObjectList)
        {
            cardManager.ReturnCardObject(card.gameObject);
        }

        handCardObjectList.Clear();
        InitializeDeck();
    }
}
