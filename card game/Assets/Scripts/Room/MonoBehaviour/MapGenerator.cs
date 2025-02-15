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
    public Room roomPrefab;
    public LineRenderer linePrefab;

    private float screenWidth;
    private float screenHeight;
    private float columnWidth;
    private Vector3 generatePoint;
    public float border;

    private List<Room> rooms = new List<Room>();
    private List<LineRenderer> lines = new List<LineRenderer>();

    public List<RoomDataSO> roomDataList = new();
    private Dictionary<RoomType, RoomDataSO> roomDataDict = new();
    private void Awake()
    {
        screenHeight = Camera.main.orthographicSize * 2;
        screenWidth = screenHeight * Camera.main.aspect;
        columnWidth = screenWidth / mapConfig.roomBlueprints.Count;

        foreach (var roomData in roomDataList)
        {
            roomDataDict.Add(roomData.roomType, roomData);
        }
    }
    //private void Start()
    //{
    //    GenerateMap();
    //}

    private void OnEnable()
    {
        if (mapLayout.mapRoomDataList.Count > 0)
        {
            LoadMap();
        }
        else
        {
            GenerateMap();
        }
    }

    // 生成地图
    public void GenerateMap()
    {
        //创建前一列房间列表
        List<Room> previousColumnRooms = new List<Room>();

        for (int column = 0; column < mapConfig.roomBlueprints.Count; column++)
        {
            var blueprint = mapConfig.roomBlueprints[column];

            var amount = Random.Range(blueprint.min, blueprint.max);

            var startHeight = screenHeight / 2 - screenHeight / (amount + 1);

            generatePoint = new Vector3(-screenWidth / 2 + border + columnWidth * column, startHeight, 0);

            var newPosition = generatePoint;

            var roomGapY = screenHeight / (amount + 1);

            //创建当前列房间列表
            List<Room> currentColumnRooms = new List<Room>();

            //循环当前列的所有房间数量生成房间
            for (int i = 0; i < amount; i++)
            {
                //如果不是Boss房，则随机生成房间的位置
                if (column == mapConfig.roomBlueprints.Count - 1)
                {
                    newPosition.x = screenWidth / 2 - border * 2;
                }
                else if (column != 0)
                {
                    newPosition.x = generatePoint.x + Random.Range(-border / 2, border / 2);
                }
                newPosition.y = startHeight - i * roomGapY;

                //生成房间
                var room = Instantiate(roomPrefab, newPosition, Quaternion.identity, transform);
                RoomType newType = GetRoomType(mapConfig.roomBlueprints[column].roomType);
                //起始设置第一列房间可以进入，其它房间锁上
                if (column == 0)
                    room.roomState = RoomState.Attainable;
                else
                    room.roomState = RoomState.Locked;

                room.SetupRoom(column, i, GetRoomData(newType));

                rooms.Add(room);
                currentColumnRooms.Add(room);
            }
            
            //判断是否是第一列，如果不是，则生成与上一列房间之间的连线
            if (previousColumnRooms.Count > 0)
            {
                //创建二个列表房间的连线
                CreateLine(previousColumnRooms, currentColumnRooms);
            }

            previousColumnRooms = currentColumnRooms;
        }

        //保存地图
        SaveMap();
    }
    //创建二个列表房间的连线
    private void CreateLine(List<Room> Column1, List<Room> Column2)
    {
        HashSet<Room> connectedColumn2Rooms = new();

        foreach (var room in Column1)
        {
            var targetRoom = ConnectToRandomRoom(room, Column2, false);//正向
            connectedColumn2Rooms.Add(targetRoom);
        }
        foreach (var room in Column2)
        {
            if (!connectedColumn2Rooms.Contains(room))
            {
                ConnectToRandomRoom(room, Column1, true);//反向
            }
        }
    }
    //创建房间之间的连线
    private Room ConnectToRandomRoom(Room room, List<Room> column2, bool check)
    {
        Room targetRoom;

        targetRoom = column2[UnityEngine.Random.Range(0, column2.Count)];

        //用来记录下一列房间号
        if (check)
        {
            targetRoom.linkTo.Add(new(room.column, room.line));
        }
        else 
        {
            room.linkTo.Add(new (targetRoom.column, targetRoom.line));
        }

        //创建房间之间的连线
        var line = Instantiate(linePrefab, transform);
        line.SetPosition(0, room.transform.position);
        line.SetPosition(1, targetRoom.transform.position);

        lines.Add(line);

        return targetRoom;
    }

    // 重新生成地图
    [ContextMenu(itemName: "ReGenerateRoom")]
    public void ReGenerateMap()
    {
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
        return roomDataDict[roomType];
    }
    // 随机获取地图配置表中房间类型
    private RoomType GetRoomType(RoomType flags)
    {
        string[] options = flags.ToString().Split(',');

        string randomOption = options[Random.Range(0, options.Length)];

        RoomType roomType = (RoomType)System.Enum.Parse(typeof(RoomType), randomOption);

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

    //加载地图
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
