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

    //���ԣ����ֳ�ȡ3�ſ��ƣ�
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
        ShuffleDeck();
    }

    [ContextMenu("���Գ���")]
    public void TestDrawCard()
    {
        DrawCard(1);
    }

    /// <summary>
    /// �����߼����¼�����
    /// </summary>
    /// <param name="amount"></param>
    public void DrawCard(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            //�ȴӳ��ƶ���ȡ��һ�ſ������жϳ��ƶ��Ƿ��Ѿ�Ϊ��
            CardDataSO cardData = drawDeck[0];
            drawDeck.RemoveAt(0);

            if (drawDeck.Count == 0)
            {
                //�����ƶ��е��Ƽ��뵽���ƶ���
                foreach (var item in discardDeck)
                {
                    drawDeck.Add(item);
                }
                //ϴ�ƣ��ı�����)
                ShuffleDeck();
            }

            //��ȡ���ƶ���
            var card = cardManager.GetCardObject(cardData).GetComponent<Card>();
            card.Init(cardData);//��ʼ������
            card.transform.position = deckPosition;//���ÿ��Ƴ����λ�ã��ƶѣ�

            handCardObjectList.Add(card);//��ӵ������б�
            var delay = i * 0.2f;
            SetCardLayout(delay);//ÿ�γ��ƶ�����һ��λ��
        }
        
    }

    /// <summary>
    /// ���ÿ��Ʋ���
    /// </summary>
    /// <param name="delay"></param>
    public void SetCardLayout(float delay)
    {
        for (int i = 0; i < handCardObjectList.Count; i++)
        {
            Card currentCard = handCardObjectList[i];
            CardTransform cardTransform = cardLayoutManager.GetCardTransform(i, handCardObjectList.Count);
            //currentCard.transform.SetLocalPositionAndRotation(cardTransform.pos, cardTransform.rotation);//���ÿ��Ƶ�λ�ú���ת�Ƕ�(�޶���)

            currentCard.isAnimating = true;//���ÿ������ڶ�����

            //ʹ��DOTween���ÿ��Ƶ����Ŷ����������ӳ�
            currentCard.transform.DOScale(Vector3.one, 0.2f).SetDelay(delay).onComplete = () =>
            {
                //ʹ��DOTween���ÿ��Ƶĳ��ƶ���
                currentCard.transform.DOMove(cardTransform.pos, 0.5f).onComplete = () => currentCard.isAnimating = false;
                currentCard.transform.DORotateQuaternion(cardTransform.rotation, 0.5f);
            };
            
            //���ÿ��Ƶ����򣨲���
            currentCard.GetComponent<SortingGroup>().sortingOrder = i;
            currentCard.UpdataPositionRotation(cardTransform.pos, cardTransform.rotation);
        }
    }

    /// <summary>
    /// ϴ��
    /// </summary>
    private void ShuffleDeck()
    {
        discardDeck.Clear();//ÿ��ϴ��������ƶ�
        //TODO:����UI��ʾ�ƶ�����

        //����˳��
        for (int i = 0; i < drawDeck.Count; i++)
        {
            CardDataSO temp = drawDeck[i];
            int randomIndex = Random.Range(i, drawDeck.Count);
            drawDeck[i] = drawDeck[randomIndex];
            drawDeck[randomIndex] = temp;
        }
    }

    /// <summary>
    /// ���տ����߼����¼�����
    /// ���ƶ�����ӿ��ƣ����Ƴ������б�
    /// </summary>
    /// <param name="obj"></param>
    public void DiscardCard(object obj)
    {
        Card card = obj as Card;
        discardDeck.Add(card.cardData);//�����Ƽ��뵽���ƶ���
        handCardObjectList.Remove(card);//�������б����Ƴ�����
        Debug.Log("���ƶ�������˿��ƣ�" + card.cardData.cardName);
        cardManager.ReturnCardObject(card.gameObject);//�����Ƽ��뵽���ƶ������

        SetCardLayout(0);
    }
}
