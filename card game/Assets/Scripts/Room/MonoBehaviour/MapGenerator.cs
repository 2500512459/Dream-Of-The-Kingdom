using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header(header:"��ͼ���ñ�")]
    public MapConfigSO mapConfig;

    [Header(header:"��ͼ����")]
    public MapLayoutSO mapLayout;

    [Header(header:"Ԥ����")]
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

    // ���ɵ�ͼ
    public void GenerateMap()
    {
        //����ǰһ�з����б�
        List<Room> previousColumnRooms = new List<Room>();

        for (int column = 0; column < mapConfig.roomBlueprints.Count; column++)
        {
            var blueprint = mapConfig.roomBlueprints[column];

            var amount = Random.Range(blueprint.min, blueprint.max);

            var startHeight = screenHeight / 2 - screenHeight / (amount + 1);

            generatePoint = new Vector3(-screenWidth / 2 + border + columnWidth * column, startHeight, 0);

            var newPosition = generatePoint;

            var roomGapY = screenHeight / (amount + 1);

            //������ǰ�з����б�
            List<Room> currentColumnRooms = new List<Room>();

            //ѭ����ǰ�е����з����������ɷ���
            for (int i = 0; i < amount; i++)
            {
                //�������Boss������������ɷ����λ��
                if (column == mapConfig.roomBlueprints.Count - 1)
                {
                    newPosition.x = screenWidth / 2 - border * 2;
                }
                else if (column != 0)
                {
                    newPosition.x = generatePoint.x + Random.Range(-border / 2, border / 2);
                }
                newPosition.y = startHeight - i * roomGapY;

                //���ɷ���
                var room = Instantiate(roomPrefab, newPosition, Quaternion.identity, transform);
                RoomType newType = GetRoomType(mapConfig.roomBlueprints[column].roomType);
                //��ʼ���õ�һ�з�����Խ��룬������������
                if (column == 0)
                    room.roomState = RoomState.Attainable;
                else
                    room.roomState = RoomState.Locked;

                room.SetupRoom(column, i, GetRoomData(newType));

                rooms.Add(room);
                currentColumnRooms.Add(room);
            }
            
            //�ж��Ƿ��ǵ�һ�У�������ǣ�����������һ�з���֮�������
            if (previousColumnRooms.Count > 0)
            {
                //���������б��������
                CreateLine(previousColumnRooms, currentColumnRooms);
            }

            previousColumnRooms = currentColumnRooms;
        }

        //�����ͼ
        SaveMap();
    }
    //���������б��������
    private void CreateLine(List<Room> Column1, List<Room> Column2)
    {
        HashSet<Room> connectedColumn2Rooms = new();

        foreach (var room in Column1)
        {
            var targetRoom = ConnectToRandomRoom(room, Column2, false);//����
            connectedColumn2Rooms.Add(targetRoom);
        }
        foreach (var room in Column2)
        {
            if (!connectedColumn2Rooms.Contains(room))
            {
                ConnectToRandomRoom(room, Column1, true);//����
            }
        }
    }
    //��������֮�������
    private Room ConnectToRandomRoom(Room room, List<Room> column2, bool check)
    {
        Room targetRoom;

        targetRoom = column2[UnityEngine.Random.Range(0, column2.Count)];

        //������¼��һ�з����
        if (check)
        {
            targetRoom.linkTo.Add(new(room.column, room.line));
        }
        else 
        {
            room.linkTo.Add(new (targetRoom.column, targetRoom.line));
        }

        //��������֮�������
        var line = Instantiate(linePrefab, transform);
        line.SetPosition(0, room.transform.position);
        line.SetPosition(1, targetRoom.transform.position);

        lines.Add(line);

        return targetRoom;
    }

    // �������ɵ�ͼ
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

    // ��ȡ��������
    private RoomDataSO GetRoomData(RoomType roomType)
    {
        return roomDataDict[roomType];
    }
    // �����ȡ��ͼ���ñ��з�������
    private RoomType GetRoomType(RoomType flags)
    {
        string[] options = flags.ToString().Split(',');

        string randomOption = options[Random.Range(0, options.Length)];

        RoomType roomType = (RoomType)System.Enum.Parse(typeof(RoomType), randomOption);

        return roomType;
    }

    // �����ͼ
    public void SaveMap()
    {
        mapLayout.mapRoomDataList = new();
        //������з������ݵ���ͼ���ñ�
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
        //������з���֮����������ݵ���ͼ���ñ�
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

    //���ص�ͼ
    public void LoadMap()
    {
        //��ȡ�����������ɷ���
        for (int i = 0; i < mapLayout.mapRoomDataList.Count; i++)
        {
            var newPos = new Vector3(mapLayout.mapRoomDataList[i].posX, mapLayout.mapRoomDataList[i].posY, 0);
            var newRoom = Instantiate(roomPrefab, newPos, Quaternion.identity, transform);
            newRoom.roomState = mapLayout.mapRoomDataList[i].roomState;
            newRoom.SetupRoom(mapLayout.mapRoomDataList[i].colum, mapLayout.mapRoomDataList[i].line, mapLayout.mapRoomDataList[i].roomData);
            newRoom.linkTo = mapLayout.mapRoomDataList[i].linkTo;
            rooms.Add(newRoom);
        }
        //��ȡ���������������ɷ�������
        for (int i = 0; i < mapLayout.linePositionList.Count; i++)
        {
            var line = Instantiate(linePrefab, transform);
            line.SetPosition(0, mapLayout.linePositionList[i].startPos.ToVector3());
            line.SetPosition(1, mapLayout.linePositionList[i].endPos.ToVector3());
            lines.Add(line);
        }
    }
}
