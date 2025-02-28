using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//需要继承几个有关拖拽的类
public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public GameObject arrowPrefab;  // 箭头预设
    private GameObject currentArrow;// 当前箭头

    private Card currentCard;// 当前拖动的卡牌
    private bool canMove;    // 判断是否可以移动
    private bool canExecute; // 判断是否可以执行

    private CharacterBase targetCharacter;// 目标角色（如果有的话）

    private void Awake()
    {
        currentCard = GetComponent<Card>();// 获取当前拖动的卡牌的 Card 组件
    }

    private void OnDisable()
    {
        // 重置卡牌的状态（例如，洗牌后重置）
        canMove = false;
        canExecute = false;
    }

    // 开始拖动时调用
    public void OnBeginDrag(PointerEventData eventData)
    {
        //判断是否可以打出
        if (!currentCard.isAvailable)
            return;

        // 根据卡牌类型处理不同的拖动行为
        switch (currentCard.cardData.cardType)
        {
            case CardType.Attack:
                // 如果是攻击卡，创建箭头
                currentArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                break;
            case CardType.Abilities:
            case CardType.Defense:
                // 如果是技能卡或防御卡，允许卡牌移动
                canMove = true;
                break;
        }
    }

    // 拖动过程中调用
    public void OnDrag(PointerEventData eventData)
    {
        //判断是否可以打出
        if (!currentCard.isAvailable)
            return;

        if (canMove)
        {
            //设置卡牌正在动画中,防止拖动卡牌时卡牌位置被改变
            currentCard.isAnimating = true;

            // 获取鼠标的位置并转换为世界坐标
            Vector3 screenPos = new(Input.mousePosition.x, Input.mousePosition.y, 10);//获取鼠标位置,10为相机深度
            Vector3 wordPos = Camera.main.ScreenToWorldPoint(screenPos);//将屏幕坐标转换为世界坐标

            // 将卡牌位置设置为鼠标位置（世界坐标）
            currentCard.transform.position = wordPos;

            // 如果卡牌在 y 轴上方，则可以执行（后续可以修改）
            canExecute = wordPos.y > 1f;
        }
        else
        {
            if (eventData.pointerEnter == null) return;

            // 如果鼠标指针在敌人上方，表示卡牌可以执行
            if (eventData.pointerEnter.CompareTag("Enemy"))
            {
                canExecute = true;
                targetCharacter = eventData.pointerEnter.GetComponent<CharacterBase>();
                return;// 提前返回，因为我们找到了有效的目标
            }

            // 因为鼠标一直在移动，所以如果鼠标不在敌人上，则不能执行
            canExecute = false;
            targetCharacter = null;
        }
    }
    // 拖动结束时调用
    public void OnEndDrag(PointerEventData eventData)
    {
        //判断是否可以打出
        if (!currentCard.isAvailable)
            return;

        // 如果存在箭头，销毁它（攻击卡箭头）
        if (currentArrow != null)
            Destroy(currentArrow);

        // 如果卡牌可以执行（目标存在且符合条件）
        if (canExecute)
        {
            // 执行卡牌的效果（使用者，目标）
            currentCard.ExecuteCardEffect(currentCard.player, targetCharacter);
        }
        else
        {
            // 如果无法执行，恢复卡牌到初始位置
            currentCard.RestCardTransform();
            currentCard.isAnimating = false;
        }
    }

}
