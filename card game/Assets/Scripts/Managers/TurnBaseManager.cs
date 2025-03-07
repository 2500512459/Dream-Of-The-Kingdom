using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TurnBaseManager : MonoBehaviour
{
    public GameObject playerObj;

    private bool isPlayerTurn = false;
    private bool isEnemyTurn = false;
    public bool battleEnd = true;

    private float timeCounter;
    public float enemyTurnDuration;
    public float playerTurnDuration;

    [Header("广播")]
    public ObjectEventSO playerTurnBegin;
    public ObjectEventSO enemyTurnBegin;
    public ObjectEventSO enemyTurnEnd;
    private void Update()
    {
        if (battleEnd)
        {
            return;
        }

        if (isEnemyTurn)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= enemyTurnDuration)
            {
                timeCounter = 0;
                //敌人回合结束
                EnemyTurnEnd();
                //玩家回合开始
                isPlayerTurn = true;
            }
        }

        if (isPlayerTurn)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= playerTurnDuration)
            {
                timeCounter = 0;
                //玩家回合开始
                PlayerTurnBegin();
                isPlayerTurn = false;
            }
        }
    }

    public void GameStart()
    {
	//游戏开始
        isPlayerTurn = true;
        isEnemyTurn = false;
        battleEnd = false;
        timeCounter = 0;
    }

    public void PlayerTurnBegin()
    {
        playerTurnBegin.RaiseEvent(null, this);
    }
    public void EnemyTurnBegin()
    {
        isEnemyTurn = true;
        enemyTurnBegin.RaiseEvent(null, this);
    }
    public void EnemyTurnEnd()
    {
        isEnemyTurn = false;
        enemyTurnEnd.RaiseEvent(null, this);
    }


    /// <summary>
    /// 注册时间函数 after room load
    /// </summary>
    /// <param name="data"></param>
    public void OnRoomLoadEvent(object data)
    {
        Room room = data as Room;
        switch (room.roomData.roomType)
        {
            case RoomType.MinorEnemy:
            case RoomType.EliteEnemy:
            case RoomType.Boss:
                playerObj.SetActive(true);
                GameStart();
                break;
            case RoomType.Shop:
            case RoomType.Treasure:
                playerObj.SetActive(false);
                break;
            case RoomType.RestRoom:
                playerObj.SetActive(true);
                break;
        }
    }
}
