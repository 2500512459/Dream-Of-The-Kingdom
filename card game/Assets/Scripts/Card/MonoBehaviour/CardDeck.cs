using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

public class CardDeck : MonoBehaviour
{
    public CardManager cardManager;
    public CardLayoutManager cardLayoutManager;
    public Vector3 deckPosition;// �ƶ�λ��
    public List<CardDataSO> drawDeck = new List<CardDataSO>();   // ���ƶ�
    public List<CardDataSO> discardDeck = new List<CardDataSO>();// ���ƶ�
    public List<Card> handCardObjectList = new List<Card>();     // �����б�

    //����
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
            card.transform.position = deckPosition;//���ÿ��Ƴ����λ�ã��ƶѣ�

            handCardObjectList.Add(card);//��ӵ������б�
            var delay = i * 0.2f;
            SetCardLayout(delay);//ÿ�γ��ƶ�����һ��λ��
        }
        
    }

    //���ÿ���λ��
    public void SetCardLayout(float delay)
    {
        for (int i = 0; i < handCardObjectList.Count; i++)
        {
            Card currentCard = handCardObjectList[i];
            CardTransform cardTransform = cardLayoutManager.GetCardTransform(i, handCardObjectList.Count);
            //currentCard.transform.SetLocalPositionAndRotation(cardTransform.pos, cardTransform.rotation);//���ÿ��Ƶ�λ�ú���ת�Ƕ�(�޶���)

            //ʹ��DOTween���ÿ��Ƶ����Ŷ����������ӳ�
            currentCard.transform.DOScale(Vector3.one, 0.2f).SetDelay(delay).onComplete = () =>
            {
                //ʹ��DOTween���ÿ��Ƶĳ��ƶ���
                currentCard.transform.DOMove(cardTransform.pos, 0.5f);
                currentCard.transform.DORotateQuaternion(cardTransform.rotation, 0.5f);
            };
            
            //���ÿ��Ƶ����򣨲���
            currentCard.GetComponent<SortingGroup>().sortingOrder = i;
        }
    }
}
