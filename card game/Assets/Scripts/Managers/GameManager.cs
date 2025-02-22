using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("��ͼ����")]
    public MapLayoutSO mapLayout;


    /// <summary>
    /// ���·���ļ����¼�
    /// </summary>
    /// <param name="roomVector"></param>
    public void UpdateMapLayoutData(object value)
    {
        var roomVector = (Vector2Int)value;
        //�ҵ�����ķ���,��״̬��Ϊ�ѷ���
        var currentRoom = mapLayout.mapRoomDataList.Find(r => r.colum == roomVector.x && r.line == roomVector.y);
        currentRoom.roomState = RoomState.Visited;

        //�ҵ�����ķ���ͬһ�еķ���,��״̬��Ϊ����
        var sameColumnRooms = mapLayout.mapRoomDataList.FindAll(r => r.colum == currentRoom.colum);
        foreach (var room in sameColumnRooms)
        {
            if(room.line != roomVector.y)
                room.roomState = RoomState.Locked;
        }

        //�ҵ�����ķ�����һ�����ߵķ���,��״̬��Ϊ�ɷ���
        foreach (var link in currentRoom.linkTo)
        {
            var linkedRoom = mapLayout.mapRoomDataList.Find(r => r.colum == link.x && r.line == link.y);
            linkedRoom.roomState = RoomState.Attainable;
        }
        
    }
}
