using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header(header:"地图配置表")]
    public MapConfigSO mapConfig;

    [Header(header:"地图布局")]
    public MapLayoutSO mapLayout;

    [Header(header:"预制体")]
    public Room roomPrefab;// 房间预制体
    public LineRenderer linePrefab;// 用于连接房间的连线预制体

    private float screenWidth;  // 屏幕宽度
    private float screenHeight;  // 屏幕高度
    private float columnWidth;  // 每列房间的宽度
    private Vector3 generatePoint;  // 用于生成房间的起始点
    public float border;  // 房间生成的边界距离

    private List<Room> rooms = new List<Room>();  // 存储所有生成的房间
    private List<LineRenderer> lines = new List<LineRenderer>();  // 存储所有房间之间的连线

    public List<RoomDataSO> roomDataList = new();// 存储房间数据
    private Dictionary<RoomType, RoomDataSO> roomDataDict = new();// 用于快速查找房间数据的字典

    // 在开始时计算屏幕宽高，并初始化相关设置
    private void Awake()
    {
        screenHeight = Camera.main.orthographicSize * 2;  // 获取屏幕高度（以正交相机为基础）
        screenWidth = screenHeight * Camera.main.aspect;  // 计算屏幕宽度（根据宽高比计算）
        columnWidth = screenWidth / mapConfig.roomBlueprints.Count;  // 计算每列的宽度

        // 将房间数据列表添加到字典中，方便后续查找
        foreach (var roomData in roomDataList)
        {
            roomDataDict.Add(roomData.roomType, roomData);
        }
    }

    // 启用时加载已有的地图布局，或者生成新的地图
    private void OnEnable()
    {
        if (mapLayout.mapRoomDataList.Count > 0)
        {
            LoadMap();  // 如果已有地图数据，加载地图
        }
        else
        {
            GenerateMap();  // 如果没有地图数据，生成新的地图
        }
    }

    // 生成新的地图
    public void GenerateMap()
    {
        // 创建一个用于存储前一列房间的列表
        List<Room> previousColumnRooms = new List<Room>();

        // 遍历所有列的房间布局
        for (int column = 0; column < mapConfig.roomBlueprints.Count; column++)
        {
            var blueprint = mapConfig.roomBlueprints[column];  // 获取当前列的布局
            var amount = Random.Range(blueprint.min, blueprint.max);  // 随机生成当前列的房间数量
            var startHeight = screenHeight / 2 - screenHeight / (amount + 1);  // 计算起始房间位置的高度

            generatePoint = new Vector3(-screenWidth / 2 + border + columnWidth * column, startHeight, 0);  // 计算当前列生成房间的起始点
            var newPosition = generatePoint;  // 初始化房间位置
            var roomGapY = screenHeight / (amount + 1);  // 计算房间之间的垂直间距

            // 创建当前列的房间列表
            List<Room> currentColumnRooms = new List<Room>();

            // 循环生成当前列的所有房间
            for (int i = 0; i < amount; i++)
            {
                // 如果是最后一列（Boss房），则固定房间位置
                if (column == mapConfig.roomBlueprints.Count - 1)
                {
                    newPosition.x = screenWidth / 2 - border * 2;
                }
                // 如果不是第一列或最后一列，则随机调整房间位置
                else if (column != 0)
                {
                    newPosition.x = generatePoint.x + Random.Range(-border / 2, border / 2);
                }
                newPosition.y = startHeight - i * roomGapY;  // 设置房间的垂直位置

                // 生成房间并设置位置
                var room = Instantiate(roomPrefab, newPosition, Quaternion.identity, transform);
                RoomType newType = GetRoomType(mapConfig.roomBlueprints[column].roomType);  // 获取房间类型
                // 设置第一列房间可达，其他房间为锁定状态
                if (column == 0)
                    room.roomState = RoomState.Attainable;
                else
                    room.roomState = RoomState.Locked;

                // 设置房间的列、行和数据
                room.SetupRoom(column, i, GetRoomData(newType));

                rooms.Add(room);  // 将房间添加到房间列表
                currentColumnRooms.Add(room);  // 将房间添加到当前列的房间列表
            }

            // 判断是否是第一列，如果不是，生成当前列与前一列房间之间的连线
            if (previousColumnRooms.Count > 0)
            {
                CreateLine(previousColumnRooms, currentColumnRooms);  // 创建连线
            }

            previousColumnRooms = currentColumnRooms;  // 更新前一列房间列表
        }

        // 保存生成的地图
        SaveMap();
    }

    // 创建两列房间之间的连线
    private void CreateLine(List<Room> Column1, List<Room> Column2)
    {
        HashSet<Room> connectedColumn2Rooms = new HashSet<Room>();  // 用于记录连接的第二列房间

        // 遍历第一列房间，随机连接到第二列的房间
        foreach (var room in Column1)
        {
            var targetRoom = ConnectToRandomRoom(room, Column2, false);  // 正向连接
            connectedColumn2Rooms.Add(targetRoom);  // 将连接的房间添加到集合中
        }

        // 遍历第二列房间，如果未被连接，则进行反向连接
        foreach (var room in Column2)
        {
            if (!connectedColumn2Rooms.Contains(room))
            {
                ConnectToRandomRoom(room, Column1, true);  // 反向连接
            }
        }
    }

    // 创建房间之间的连线
    private Room ConnectToRandomRoom(Room room, List<Room> column2, bool check)
    {
        Room targetRoom;

        // 随机选择一个房间进行连接
        targetRoom = column2[UnityEngine.Random.Range(0, column2.Count)];

        // 如果是反向连接，记录该房间的列和行
        if (check)
        {
            targetRoom.linkTo.Add(new(room.column, room.line));
        }
        else
        {
            room.linkTo.Add(new(targetRoom.column, targetRoom.line));
        }

        // 创建连线并设置起始和结束位置
        var line = Instantiate(linePrefab, transform);
        line.SetPosition(0, room.transform.position);
        line.SetPosition(1, targetRoom.transform.position);

        lines.Add(line);  // 将连线添加到列表中

        return targetRoom;
    }

    // 重新生成地图
    [ContextMenu(itemName: "ReGenerateRoom")]
    public void ReGenerateMap()
    {
        // 销毁当前的所有房间和连线
        foreach (var room in rooms)
        {
            Destroy(room.gameObject);
        }
        rooms.Clear();
        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }
        lines.Clear();
        GenerateMap();
    }

    // 获取房间数据
    private RoomDataSO GetRoomData(RoomType roomType)
    {
        return roomDataDict[roomType];// 从字典中获取对应房间类型的数据
    }

    // 随机获取地图配置表中房间类型
    private RoomType GetRoomType(RoomType flags)
    {
        string[] options = flags.ToString().Split(',');// 将房间类型转换为字符串数组

        string randomOption = options[Random.Range(0, options.Length)];// 随机获取一个房间类型

        RoomType roomType = (RoomType)System.Enum.Parse(typeof(RoomType), randomOption);// 转换为RoomType类型

        return roomType;
    }

    // 保存地图
    public void SaveMap()
    {
        mapLayout.mapRoomDataList = new();
        //添加所有房间数据到地图配置表
        for (int i = 0; i < rooms.Count; i++)
        {
            var mapRoomData = new MapRoomData()
            {
                posX = rooms[i].transform.position.x,
                posY = rooms[i].transform.position.y,
                colum = rooms[i].column,
                line = rooms[i].line,
                roomData = rooms[i].roomData,
                roomState = rooms[i].roomState,
                linkTo = rooms[i].linkTo,
            };
            mapLayout.mapRoomDataList.Add(mapRoomData);
        }
        //添加所有房间之间的连线数据到地图配置表
        mapLayout.linePositionList = new();
        for (int i = 0; i < lines.Count; i++)
        {
            var linePosition = new LinePosition()
            {
                startPos = new SerializeVector3(lines[i].GetPosition(0)),
                endPos = new SerializeVector3(lines[i].GetPosition(1))
            };
            mapLayout.linePositionList.Add(linePosition);
        }


    }

    // 加载地图
    public void LoadMap()
    {
        //读取房间数据生成房间
        for (int i = 0; i < mapLayout.mapRoomDataList.Count; i++)
        {
            var newPos = new Vector3(mapLayout.mapRoomDataList[i].posX, mapLayout.mapRoomDataList[i].posY, 0);
            var newRoom = Instantiate(roomPrefab, newPos, Quaternion.identity, transform);
            newRoom.roomState = mapLayout.mapRoomDataList[i].roomState;
            newRoom.SetupRoom(mapLayout.mapRoomDataList[i].colum, mapLayout.mapRoomDataList[i].line, mapLayout.mapRoomDataList[i].roomData);
            newRoom.linkTo = mapLayout.mapRoomDataList[i].linkTo;
            rooms.Add(newRoom);
        }
        //读取房间连线数据生成房间连线
        for (int i = 0; i < mapLayout.linePositionList.Count; i++)
        {
            var line = Instantiate(linePrefab, transform);
            line.SetPosition(0, mapLayout.linePositionList[i].startPos.ToVector3());
            line.SetPosition(1, mapLayout.linePositionList[i].endPos.ToVector3());
            lines.Add(line);
        }
    }
}
