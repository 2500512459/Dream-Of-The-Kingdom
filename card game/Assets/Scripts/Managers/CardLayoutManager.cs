using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLayoutManager : MonoBehaviour
{
    public bool isHorizontal;//是否水平排列
    public float maxWidth = 7f;//最大宽度
    public float cardSpacing = 2f;//卡片默认间距
    [Header("弧形参数")]
    public float angleBetweenCards = 7f;//卡片之间的角度
    public float radius = 17f;//卡片弧形半径
    public Vector3 centerPoint;//卡牌区域中心点
    [SerializeField]private List<Vector3> cardPositions = new List<Vector3>();//卡片位置
    private List<Quaternion> cardRotations = new List<Quaternion>();//卡片旋转


    private void Awake()
    {
        centerPoint = isHorizontal? Vector3.up * -4.5f : Vector3.up * -21.5f;
    }

    public CardTransform GetCardTransform(int index, int totalCards)
    {
        CalculatePosition(totalCards, isHorizontal);//获取指定数量的卡片位置
        return new CardTransform(cardPositions[index], cardRotations[index]);//返回指定序号的卡牌位置和旋转
    }
    private void CalculatePosition(int number0fCards, bool horizontal)
    {
        cardPositions.Clear();//每次计算位置时先将卡片位置列表清空
        cardRotations.Clear();//每次计算位置时先将卡片旋转列表清空
        if (horizontal)
        {
            float currentWidth = cardSpacing * (number0fCards - 1);//当前卡片总宽度
            float totalWidth = Mathf.Min(currentWidth, maxWidth);  //最大宽度

            float currentSpacing = totalWidth > 0 ? totalWidth / (number0fCards - 1) : 0;//卡片间距

            for (int i = 0; i < number0fCards; i++)
            {
                //获取当前卡片的x坐标
                float xPos = 0 - totalWidth / 2 + currentSpacing * i;

                var pos = new Vector3(xPos, centerPoint.y, 0);
                
                var rotation = Quaternion.identity;//不旋转

                cardPositions.Add(pos);
                cardRotations.Add(rotation);
            }
        }
        else
        {   //未完成角度随卡牌数量变化
            float cardAngle = (number0fCards - 1) * angleBetweenCards / 2;
            for (int i = 0; i < number0fCards; i++)
            {
                var pos = FanCardPosition(cardAngle - i * angleBetweenCards);
                
                var rotation = Quaternion.Euler(0, 0, cardAngle - i * angleBetweenCards);//绕Z轴旋转
            
                cardPositions.Add(pos);
                cardRotations.Add(rotation);
            }
        }
    }
    //
    private Vector3 FanCardPosition(float angle)
    {
        return new Vector3(
            centerPoint.x - Mathf.Sin(Mathf.Deg2Rad * angle) * radius,
            centerPoint.y + Mathf.Cos(Mathf.Deg2Rad * angle) * radius,
            0
        );
    }
}
