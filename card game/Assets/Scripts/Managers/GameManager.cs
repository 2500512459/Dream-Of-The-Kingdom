using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("地图布局")]
    public MapLayoutSO mapLayout;

    public List<Enemy> aliveEnemyList;

    [Header("事件广播")]
    public ObjectEventSO gameWinEvent;
    public ObjectEventSO gameOverEvent;

    /// <summary>
    /// 更新房间的监听事件,加载地图
    /// </summary>
    /// <param name="roomVector"></param>
    public void UpdateMapLayoutData(object value)
    {
        var roomVector = (Vector2Int)value;
        //找到点击的房间,将状态改为已访问
        var currentRoom = mapLayout.mapRoomDataList.Find(r => r.colum == roomVector.x && r.line == roomVector.y);
        currentRoom.roomState = RoomState.Visited;

        //找到点击的房间同一列的房间,将状态改为锁定
        var sameColumnRooms = mapLayout.mapRoomDataList.FindAll(r => r.colum == currentRoom.colum);
        foreach (var room in sameColumnRooms)
        {
            if(room.line != roomVector.y)
                room.roomState = RoomState.Locked;
        }

        //找到点击的房间下一列连线的房间,将状态改为可访问
        foreach (var link in currentRoom.linkTo)
        {
            var linkedRoom = mapLayout.mapRoomDataList.Find(r => r.colum == link.x && r.line == link.y);
            linkedRoom.roomState = RoomState.Attainable;
        }

        aliveEnemyList.Clear();//清空敌人角色列表
    }

    /// <summary>
    /// 加载房间后，更新敌人角色列表
    /// </summary>
    /// <param name="obj"></param>
    public void OnRoomLoadedEvent(object obj)
    {
        //第一个参数是是否包含没有激活的对象，第二个参数是是否按名称排序
        var enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            aliveEnemyList.Add(enemy);
        }
    }

    public void OnCharacterDeadEvent(object character)
    {
        if (character is Player)
        {
            //发出失败的通知
            StartCoroutine(EventDelayAction(gameOverEvent));
        }

        if (character is Enemy)
        {
            aliveEnemyList.Remove(character as Enemy);
            if (aliveEnemyList.Count == 0)
            {
                //发出胜利的通知
                StartCoroutine(EventDelayAction(gameWinEvent));
            }
        }
    }

    IEnumerator EventDelayAction(ObjectEventSO eventSO)
    {
        yield return new WaitForSeconds(1.0f);
        eventSO.RaiseEvent(null, this);
    }
}
