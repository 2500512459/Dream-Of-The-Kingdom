using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLayoutManager : MonoBehaviour
{
    public bool isHorizontal;//是否水平排列
    public float maxWidth = 7f;//最大宽度
    public float cardSpacing = 2f;//卡片默认间距
    public Vector3 centerPoint;//卡牌区域中心点
    [SerializeField]private List<Vector3> cardPositions = new List<Vector3>();//卡片位置
    private List<Quaternion> cardRotations = new List<Quaternion>();//卡片旋转

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
                cardPositions.Add(pos);

                var rotation = Quaternion.identity;//不旋转
                cardRotations.Add(rotation);
            }
        }
    }
}
