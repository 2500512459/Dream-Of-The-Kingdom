using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Card : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    [Header("���")]
    public SpriteRenderer cardSprite;
    public TextMeshPro costText, descriptionText, typeText, cardName;

    public CardDataSO cardData;

    [Header("��괥��ǰ��ԭʼ����")]
    public Vector3 originalPosition;   //ԭʼλ��
    public Quaternion originalRotation;//ԭʼ�Ƕ�
    public int originalLayerOrder;     //ԭʼ�㼶

    public bool isAnimating;//�Ƿ��ڶ�����

    public Player player;
    [Header("�㲥�¼�")]
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
            CardType.Attack => "����",
            CardType.Defense => "����",
            CardType.Abilities => "����",
            _ => throw new System.NotImplementedException(),
        };
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    //���¿��Ƶ�λ�ú���ת
    public void UpdataPositionRotation(Vector3 Position, Quaternion Rotation)
    {
        originalPosition = Position;
        originalRotation = Rotation;
        originalLayerOrder = GetComponent<SortingGroup>().sortingOrder;
    }
    //������뿨��
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            //transform.position = originalPosition + Vector3.up;
            Vector3 newPosition = transform.position;
            newPosition.y = -3.5f;//Ĭ�ϸ߶�Ϊ-3.5f��֤���Ƹ߶�һ��
            transform.position = newPosition;
            transform.rotation = Quaternion.identity;
            GetComponent<SortingGroup>().sortingOrder = 20;
        }
    }
    //����뿪����
    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        if (!isAnimating)
            RestCardTransform();
    }
    //�ָ����Ƶ�λ�ú���ת
    public void RestCardTransform()
    {
        transform.SetLocalPositionAndRotation(originalPosition, originalRotation);
        GetComponent<SortingGroup>().sortingOrder = originalLayerOrder;
    }

    //ִ�п���
    public void ExecuteCardEffect(CharacterBase from, CharacterBase target)
    {
        //TODO:���ٶ�Ӧ������֪ͨ���տ���
        discardCardEvent.RaiseEvent(this, this);
        //ִ�����ſ��Ƶ�����Ч��
        foreach (var effect in cardData.effects)
        {
            effect.Execute(from, target);
        }
    }
}
