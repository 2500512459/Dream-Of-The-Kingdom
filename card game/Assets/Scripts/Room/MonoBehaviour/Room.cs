using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int column;  // 当前房间所在的列
    public int line;    // 当前房间所在的行
    private SpriteRenderer spriteRenderer;  // 用于显示房间图标的精灵渲染器

    public RoomDataSO roomData;  // 当前房间的数据
    public RoomState roomState;  // 当前房间的状态（如锁定、可达、已访问等）
    public List<Vector2Int> linkTo;  // 记录当前房间与其他房间的连接（例如，连线）

    [Header(header:"广播")]
    public ObjectEventSO loadRoomEvent;  // 房间加载事件，当点击房间时触发

    // 在Awake中初始化SpriteRenderer
    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();  // 获取房间子物体中的SpriteRenderer组件
    }

    // 当鼠标点击房间时触发
    private void OnMouseDown()
    {
        // 如果房间状态是可达的，则触发加载房间事件
        if(roomState == RoomState.Attainable)
            loadRoomEvent.RaiseEvent(this, this);  // 调用广播事件，传递当前房间对象
    }

    // 设置房间的列、行和数据，并更新房间的显示
    public void SetupRoom(int column, int line, RoomDataSO roomData)
    {
        this.column = column;  // 设置列
        this.line = line;  // 设置行
        this.roomData = roomData;  // 设置房间的数据（图标等）

        spriteRenderer.sprite = roomData.roomIcon;  // 设置房间的图标

        // 根据房间的状态来设置房间的颜色
        spriteRenderer.color = roomState switch
        {
            RoomState.Locked => new Color(0.5f, 0.5f, 0.5f, 1.0f),  // 锁定状态时，颜色为灰色
            RoomState.Visited => new Color(0.5f, 0.8f, 0.5f, 0.5f),  // 已访问状态时，颜色为半透明绿色
            RoomState.Attainable => Color.white,  // 可达状态时，颜色为白色
            _ => throw new System.NotImplementedException(),  // 如果房间状态未定义，则抛出异常
        };
    }
}
