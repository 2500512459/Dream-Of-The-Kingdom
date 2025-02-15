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
    /// �ڷ�������¼��м���
    /// </summary>
    /// <param name="value"></param>
    public async void OnLoadRoomEvent(object value)
    {
        if (value is RoomDataSO)
        {
            RoomDataSO currentRoom = (RoomDataSO)value;
            //Debug.Log("�����" + currentRoom.roomType);

            currentScene = currentRoom.sceneToLoad;
        }

        await UnloadSceneTask();
        //���ط���
        await LoadSceneTask();
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
