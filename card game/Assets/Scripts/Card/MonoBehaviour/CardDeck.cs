using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

public class CardDeck : MonoBehaviour
{
    public CardManager cardManager;
    public CardLayoutManager cardLayoutManager;
    public Vector3 deckPosition;// 牌堆位置
    public List<CardDataSO> drawDeck = new List<CardDataSO>();   // 抽牌堆
    public List<CardDataSO> discardDeck = new List<CardDataSO>();// 弃牌堆
    public List<Card> handCardObjectList = new List<Card>();     // 手牌列表

    //测试
    private void Start()
    {
        InitializeDeck();
        DrawCard(3);
    }

    public void InitializeDeck()
    {
        drawDeck.Clear();
        foreach (var entry in cardManager.currentCardLibrary.cardLibraryList)
        {
            for (int i = 0; i < entry.amount; i++)
            {
                drawDeck.Add(entry.cardData);
            }
        }

        //洗牌（改变牌序)
    }

    [ContextMenu("测试抽牌")]
    public void TestDrawCard()
    {
        DrawCard(1);
    }

    public void DrawCard(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (drawDeck.Count == 0)
            {
                //洗牌（改变牌序)
            }
            CardDataSO cardData = drawDeck[0];
            drawDeck.RemoveAt(0);
            //获取卡牌对象
            var card = cardManager.GetCardObject(cardData).GetComponent<Card>();
            card.Init(cardData);//初始化卡牌
            card.transform.position = deckPosition;//设置卡牌抽出的位置（牌堆）

            handCardObjectList.Add(card);//添加到手牌列表
            var delay = i * 0.2f;
            SetCardLayout(delay);//每次抽牌都设置一次位置
        }
        
    }

    //设置卡牌位置
    public void SetCardLayout(float delay)
    {
        for (int i = 0; i < handCardObjectList.Count; i++)
        {
            Card currentCard = handCardObjectList[i];
            CardTransform cardTransform = cardLayoutManager.GetCardTransform(i, handCardObjectList.Count);
            //currentCard.transform.SetLocalPositionAndRotation(cardTransform.pos, cardTransform.rotation);//设置卡牌的位置和旋转角度(无动画)

            //使用DOTween设置卡牌的缩放动画并设置延迟
            currentCard.transform.DOScale(Vector3.one, 0.2f).SetDelay(delay).onComplete = () =>
            {
                //使用DOTween设置卡牌的抽牌动画
                currentCard.transform.DOMove(cardTransform.pos, 0.5f);
                currentCard.transform.DORotateQuaternion(cardTransform.rotation, 0.5f);
            };
            
            //设置卡牌的排序（层序）
            currentCard.GetComponent<SortingGroup>().sortingOrder = i;
        }
    }
}
