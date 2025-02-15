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

    /// <summary>
    /// 在房间加载事件中监听
    /// </summary>
    /// <param name="value"></param>
    public async void OnLoadRoomEvent(object value)
    {
        if (value is RoomDataSO)
        {
            RoomDataSO currentRoom = (RoomDataSO)value;
            //Debug.Log("点击了" + currentRoom.roomType);

            currentScene = currentRoom.sceneToLoad;
        }

        await UnloadSceneTask();
        //加载房间
        await LoadSceneTask();
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

        currentScene = map;

        await LoadSceneTask();
    }
}
