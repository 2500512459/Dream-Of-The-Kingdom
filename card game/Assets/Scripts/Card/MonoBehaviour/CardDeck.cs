using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    public CardManager cardManager;

    public List<CardDataSO> drawDeck = new List<CardDataSO>();   // 抽牌堆
    public List<CardDataSO> discardDeck = new List<CardDataSO>();// 弃牌堆
    public List<Card> handCardObjectList = new List<Card>();     // 手牌列表

    //测试
    private void Start()
    {
        InitializeDeck();
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
            handCardObjectList.Add(card);//添加到手牌列表
        }
    }
}
