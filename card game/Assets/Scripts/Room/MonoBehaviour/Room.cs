using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int column;
    public int line;
    private SpriteRenderer spriteRenderer;

    public RoomDataSO roomData;
    public RoomState roomState;

    [Header(header:"¹ã²¥")]
    public ObjectEventSO loadRoomEvent;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void Start()
    {
        SetupRoom(0, 0, roomData);
    }
    private void OnMouseDown()
    {
        Debug.Log(roomData.roomType);
        loadRoomEvent.RaiseEvent(roomData, this);
    }

    public void SetupRoom(int column, int line, RoomDataSO roomData)
    {
        this.column = column;
        this.line = line;
        this.roomData = roomData;
        spriteRenderer.sprite = roomData.roomIcon;
    }
}
