using System;

[Flags]
public enum RoomType//选择房间类型
{
    MinorEnemy = 1,  //普通敌人
    EliteEnemy = 2,  //精英敌人
    Shop = 4,        //商店
    Treasure = 8,     //宝藏
    RestRoom = 16,   //休息室
    Boss = 32        //Boss
}

//选择房间状态
public enum RoomState
{
    Locked,    //锁定
    Visited,   //已访问
    Attainable //可到达
}

