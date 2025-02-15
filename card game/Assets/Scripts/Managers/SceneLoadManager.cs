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

    [Header("�㲥")]
    public ObjectEventSO afterRoomLoadedEvent;
    /// <summary>
    /// �ڷ�������¼��м���
    /// </summary>
    /// <param name="data"></param>
    public async void OnLoadRoomEvent(object data)
    {
        if (data is Room)
        {
            Room currentRoom = data as Room;

            var currentData = currentRoom.roomData;
            currentRoomVector = new(currentRoom.column, currentRoom.line);

            currentScene = currentData.sceneToLoad;
        }
        await UnloadSceneTask();
        //���ط���
        await LoadSceneTask();

        afterRoomLoadedEvent.RaiseEvent(currentRoomVector, this);
    }
    /// <summary>
    /// �첽�������س���
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
    /// �������ص�ͼ�е��¼�����
    /// </summary>
    public async void LoadMap()
    {
        await UnloadSceneTask();

        currentScene = map;

        await LoadSceneTask();
    }
}
