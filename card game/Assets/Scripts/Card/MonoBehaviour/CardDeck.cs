using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    public CardManager cardManager;

    public List<CardDataSO> drawDeck = new List<CardDataSO>();   // ���ƶ�
    public List<CardDataSO> discardDeck = new List<CardDataSO>();// ���ƶ�
    public List<Card> handCardObjectList = new List<Card>();     // �����б�

    //����
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

        //ϴ�ƣ��ı�����)
    }

    [ContextMenu("���Գ���")]
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
                //ϴ�ƣ��ı�����)
            }
            CardDataSO cardData = drawDeck[0];
            drawDeck.RemoveAt(0);
            //��ȡ���ƶ���
            var card = cardManager.GetCardObject(cardData).GetComponent<Card>();
            card.Init(cardData);//��ʼ������
            handCardObjectList.Add(card);//��ӵ������б�
        }
    }
}
