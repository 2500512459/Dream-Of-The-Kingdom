using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public GameObject arrowPrefab;  //箭头预设
    private GameObject currentArrow;//当前箭头

    private Card currentCard;
    private bool canMove;//判断是否可以移动
    private bool canExecute;//判断是否可以执行

    private CharacterBase targetCharacter;
    private void Awake()
    {
        currentCard = GetComponent<Card>();//获取当前拖动的卡牌
    }

    //开始拖动
    public void OnBeginDrag(PointerEventData eventData)
    {
        //判断卡牌类型
        switch (currentCard.cardData.cardType)
        {
            //如果是攻击卡牌,则创建箭头
            //如果为法术卡牌或者防御卡牌,则设置canMove为true
            case CardType.Attack:
                currentArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                break;
            case CardType.Abilities:
            case CardType.Defense:
                canMove = true;
                break;
        }
    }
    //正在拖动
    public void OnDrag(PointerEventData eventData)
    {
        if (canMove)
        {
            currentCard.isAnimating = true;//设置卡牌正在动画中,防止拖动卡牌时卡牌位置被改变
            Vector3 screenPos = new(Input.mousePosition.x, Input.mousePosition.y, 10);//获取鼠标位置,10为相机深度
            Vector3 wordPos = Camera.main.ScreenToWorldPoint(screenPos);//将屏幕坐标转换为世界坐标

            currentCard.transform.position = wordPos;
            canExecute = wordPos.y > 1f;
        }
        else
        {
            if (eventData.pointerEnter == null) return;

            //如果鼠标在Enemy上，则能执行
            if (eventData.pointerEnter.CompareTag("Enemy"))
            {
                canExecute = true;
                targetCharacter = eventData.pointerEnter.GetComponent<CharacterBase>();
                return;//直接返回，不需要再执行下面的代码
            }
            //因为鼠标一直在移动，所以如果鼠标不在Enemy上，则不能执行
            canExecute = false;
            targetCharacter = null;
        }
    }
    //结束拖动
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
            //回到初始位置
            currentCard.RestCardTransform();
            currentCard.isAnimating = false;
        }
    }

}
