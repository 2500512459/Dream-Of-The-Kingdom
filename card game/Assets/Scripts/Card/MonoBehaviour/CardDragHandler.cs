using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public GameObject arrowPrefab;  //��ͷԤ��
    private GameObject currentArrow;//��ǰ��ͷ

    private Card currentCard;
    private bool canMove;//�ж��Ƿ�����ƶ�
    private bool canExecute;//�ж��Ƿ����ִ��

    private CharacterBase targetCharacter;
    private void Awake()
    {
        currentCard = GetComponent<Card>();//��ȡ��ǰ�϶��Ŀ���
    }

    //��ʼ�϶�
    public void OnBeginDrag(PointerEventData eventData)
    {
        //�жϿ�������
        switch (currentCard.cardData.cardType)
        {
            //����ǹ�������,�򴴽���ͷ
            //���Ϊ�������ƻ��߷�������,������canMoveΪtrue
            case CardType.Attack:
                currentArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                break;
            case CardType.Abilities:
            case CardType.Defense:
                canMove = true;
                break;
        }
    }
    //�����϶�
    public void OnDrag(PointerEventData eventData)
    {
        if (canMove)
        {
            currentCard.isAnimating = true;//���ÿ������ڶ�����,��ֹ�϶�����ʱ����λ�ñ��ı�
            Vector3 screenPos = new(Input.mousePosition.x, Input.mousePosition.y, 10);//��ȡ���λ��,10Ϊ������
            Vector3 wordPos = Camera.main.ScreenToWorldPoint(screenPos);//����Ļ����ת��Ϊ��������

            currentCard.transform.position = wordPos;
            canExecute = wordPos.y > 1f;
        }
        else
        {
            if (eventData.pointerEnter == null) return;

            //��������Enemy�ϣ�����ִ��
            if (eventData.pointerEnter.CompareTag("Enemy"))
            {
                canExecute = true;
                targetCharacter = eventData.pointerEnter.GetComponent<CharacterBase>();
                return;//ֱ�ӷ��أ�����Ҫ��ִ������Ĵ���
            }
            //��Ϊ���һֱ���ƶ������������겻��Enemy�ϣ�����ִ��
            canExecute = false;
            targetCharacter = null;
        }
    }
    //�����϶�
    public void OnEndDrag(PointerEventData eventData)
    {
        if(currentArrow != null)
            Destroy(currentArrow);

        if (canExecute)
        {
            currentCard.ExecuteCardEffect(currentCard.player, targetCharacter);
        }
        else
        {
            //�ص���ʼλ��
            currentCard.RestCardTransform();
            currentCard.isAnimating = false;
        }
    }

}
