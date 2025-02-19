using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLayoutManager : MonoBehaviour
{
    public bool isHorizontal;//�Ƿ�ˮƽ����
    public float maxWidth = 7f;//�����
    public float cardSpacing = 2f;//��ƬĬ�ϼ��
    public Vector3 centerPoint;//�����������ĵ�
    [SerializeField]private List<Vector3> cardPositions = new List<Vector3>();//��Ƭλ��
    private List<Quaternion> cardRotations = new List<Quaternion>();//��Ƭ��ת

    public CardTransform GetCardTransform(int index, int totalCards)
    {
        CalculatePosition(totalCards, isHorizontal);//��ȡָ�������Ŀ�Ƭλ��
        return new CardTransform(cardPositions[index], cardRotations[index]);//����ָ����ŵĿ���λ�ú���ת
    }
    private void CalculatePosition(int number0fCards, bool horizontal)
    {
        cardPositions.Clear();//ÿ�μ���λ��ʱ�Ƚ���Ƭλ���б����
        cardRotations.Clear();//ÿ�μ���λ��ʱ�Ƚ���Ƭ��ת�б����
        if (horizontal)
        {
            float currentWidth = cardSpacing * (number0fCards - 1);//��ǰ��Ƭ�ܿ��
            float totalWidth = Mathf.Min(currentWidth, maxWidth);  //�����

            float currentSpacing = totalWidth > 0 ? totalWidth / (number0fCards - 1) : 0;//��Ƭ���

            for (int i = 0; i < number0fCards; i++)
            {
                //��ȡ��ǰ��Ƭ��x����
                float xPos = 0 - totalWidth / 2 + currentSpacing * i;

                var pos = new Vector3(xPos, centerPoint.y, 0);
                cardPositions.Add(pos);

                var rotation = Quaternion.identity;//����ת
                cardRotations.Add(rotation);
            }
        }
    }
}
