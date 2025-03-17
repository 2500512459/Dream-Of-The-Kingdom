using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景加载管理器，负责处理游戏场景的异步加载/卸载和房间切换逻辑
/// </summary>
public class SceneLoadManager : MonoBehaviour
{
    public FadePanel fadePanel;         // 淡入淡出面板
    // ---------- 场景资源引用 ----------
    private AssetReference currentScene;  // 当前加载的场景引用
    public AssetReference map;           // 地图场景资源引用
    public AssetReference menu;          // 菜单场景资源引用

    // ---------- 房间数据 ----------
    private Vector2Int currentRoomVector = Vector2Int.one * -1; // 当前房间坐标（初始值-1,-1）
    private Room currentRoom;             // 当前房间数据

    // ---------- 事件系统 ----------
    [Header("广播")]
    public ObjectEventSO afterRoomLoadedEvent; // 房间加载完成事件
    public ObjectEventSO updataRoomEvent;      // 房间数据更新事件

    private void Awake()
    {
        LoadMenu(); // 游戏启动时加载菜单场景
    }

    /// <summary>
    /// 处理房间加载事件
    /// </summary>
    /// <param name="data">包含Room类型数据的参数</param>
    public async void OnLoadRoomEvent(object data)
    {
        if (data is Room targetRoom)
        {
            currentRoom = targetRoom;
            currentRoomVector = new Vector2Int(targetRoom.column, targetRoom.line);
            currentScene = targetRoom.roomData.sceneToLoad;
        }

        await UnloadSceneTask();  // 先卸载当前场景
        await LoadSceneTask();     // 加载新场景

        // 广播房间加载完成事件
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
            fadePanel.FadeOut(0.2f);
            SceneManager.SetActiveScene(s.Result.Scene);
        }
    }

    /// <summary>
    /// 异步卸载当前活动场景
    /// </summary>
    private async Awaitable UnloadSceneTask()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.isLoaded)
        {
            fadePanel.FadeIn(0.4f);
            await Awaitable.WaitForSecondsAsync(0.45f);
            await Awaitable.FromAsyncOperation(SceneManager.UnloadSceneAsync(activeScene));
        }
    }

    /// <summary>
    /// 加载地图场景并更新房间坐标
    /// </summary>
    public async void LoadMap()
    {
        await UnloadSceneTask();

        // 如果存在有效房间坐标则广播更新
        if (currentRoomVector != Vector2Int.one * -1)
        {
            updataRoomEvent.RaiseEvent(currentRoomVector, this);
        }

        currentScene = map;
        await LoadSceneTask();
    }

    /// <summary>
    /// 加载菜单场景
    /// </summary>
    public async void LoadMenu()
    {
        if (currentScene != null)
        {
            await UnloadSceneTask();
        }
        currentScene = menu;
        await LoadSceneTask();
    }
}