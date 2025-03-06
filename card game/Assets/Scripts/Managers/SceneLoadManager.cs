using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
public class SceneLoadManager : MonoBehaviour
{
    private AssetReference currentScene;
    public AssetReference map;

    private Vector2Int currentRoomVector;
    private Room currentRoom;

    [Header("广播")]
    public ObjectEventSO afterRoomLoadedEvent;
    public ObjectEventSO updataRoomEvent;


    private void Start()
    {
        currentRoomVector = Vector2Int.one * -1;
    }

    /// <summary>
    /// 在房间加载事件中监听
    /// </summary>
    /// <param name="data"></param>
    public async void OnLoadRoomEvent(object data)
    {
        if (data is Room)
        {
            currentRoom = data as Room;

            var currentData = currentRoom.roomData;
            currentRoomVector = new(currentRoom.column, currentRoom.line);

            currentScene = currentData.sceneToLoad;
        }
        await UnloadSceneTask();
        //加载房间
        await LoadSceneTask();

        afterRoomLoadedEvent.RaiseEvent(currentRoom, this);
    }
    /// <summary>
    /// 异步操作加载场景
    /// </summary>
    /// <returns></returns>
    private async Awaitable LoadSceneTask()
    {
        var s = currentScene.LoadSceneAsync(LoadSceneMode.Additive);
        await s.Task;

        if (s.Status == AsyncOperationStatus.Succeeded)
        {
            SceneManager.SetActiveScene(s.Result.Scene);
        }
    }

    private async Awaitable UnloadSceneTask()
    {
        await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        
    }

    /// <summary>
    /// 监听返回地图中的事件函数
    /// </summary>
    public async void LoadMap()
    {
        await UnloadSceneTask();

        if (currentRoomVector != Vector2Int.one * -1)
        {
            updataRoomEvent.RaiseEvent(currentRoomVector, this);
        }

        currentScene = map;

        await LoadSceneTask();
    }
}
