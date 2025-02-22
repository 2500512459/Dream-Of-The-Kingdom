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

    //测试（开局抽取3张卡牌）
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
        ShuffleDeck();
    }

    [ContextMenu("测试抽牌")]
    public void TestDrawCard()
    {
        DrawCard(1);
    }

    /// <summary>
    /// 抽牌逻辑，事件函数
    /// </summary>
    /// <param name="amount"></param>
    public void DrawCard(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            //先从抽牌堆中取出一张卡牌再判断抽牌堆是否已经为空
            CardDataSO cardData = drawDeck[0];
            drawDeck.RemoveAt(0);

            if (drawDeck.Count == 0)
            {
                //将弃牌堆中的牌加入到抽牌堆中
                foreach (var item in discardDeck)
                {
                    drawDeck.Add(item);
                }
                //洗牌（改变牌序)
                ShuffleDeck();
            }

            //获取卡牌对象
            var card = cardManager.GetCardObject(cardData).GetComponent<Card>();
            card.Init(cardData);//初始化卡牌
            card.transform.position = deckPosition;//设置卡牌抽出的位置（牌堆）

            handCardObjectList.Add(card);//添加到手牌列表
            var delay = i * 0.2f;
            SetCardLayout(delay);//每次抽牌都设置一次位置
        }
        
    }

    /// <summary>
    /// 设置卡牌布局
    /// </summary>
    /// <param name="delay"></param>
    public void SetCardLayout(float delay)
    {
        for (int i = 0; i < handCardObjectList.Count; i++)
        {
            Card currentCard = handCardObjectList[i];
            CardTransform cardTransform = cardLayoutManager.GetCardTransform(i, handCardObjectList.Count);
            //currentCard.transform.SetLocalPositionAndRotation(cardTransform.pos, cardTransform.rotation);//设置卡牌的位置和旋转角度(无动画)

            currentCard.isAnimating = true;//设置卡牌正在动画中

            //使用DOTween设置卡牌的缩放动画并设置延迟
            currentCard.transform.DOScale(Vector3.one, 0.2f).SetDelay(delay).onComplete = () =>
            {
                //使用DOTween设置卡牌的抽牌动画
                currentCard.transform.DOMove(cardTransform.pos, 0.5f).onComplete = () => currentCard.isAnimating = false;
                currentCard.transform.DORotateQuaternion(cardTransform.rotation, 0.5f);
            };
            
            //设置卡牌的排序（层序）
            currentCard.GetComponent<SortingGroup>().sortingOrder = i;
            currentCard.UpdataPositionRotation(cardTransform.pos, cardTransform.rotation);
        }
    }

    /// <summary>
    /// 洗牌
    /// </summary>
    private void ShuffleDeck()
    {
        discardDeck.Clear();//每次洗牌清空弃牌堆
        //TODO:更新UI显示牌堆数量

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
    /// 回收卡牌逻辑，事件函数
    /// 弃牌堆中添加卡牌，并移除手牌列表
    /// </summary>
    /// <param name="obj"></param>
    public void DiscardCard(object obj)
    {
        Card card = obj as Card;
        discardDeck.Add(card.cardData);//将卡牌加入到弃牌堆中
        handCardObjectList.Remove(card);//从手牌列表中移除卡牌
        Debug.Log("弃牌堆中添加了卡牌：" + card.cardData.cardName);
        cardManager.ReturnCardObject(card.gameObject);//将卡牌加入到卡牌对象池中

        SetCardLayout(0);
    }
}
