using System;

[Flags]
public enum RoomType//ѡ�񷿼�����
{
    MinorEnemy = 1,  //��ͨ����
    EliteEnemy = 2,  //��Ӣ����
    Shop = 4,        //�̵�
    Treasure = 8,     //����
    RestRoom = 16,   //��Ϣ��
    Boss = 32        //Boss
}

//ѡ�񷿼�״̬
public enum RoomState
{
    Locked,    //����
    Visited,   //�ѷ���
    Attainable //�ɵ���
}

