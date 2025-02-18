using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CardManager : MonoBehaviour
{
    public PoolTool poolTool;//卡牌对象池
    public List<CardDataSO> cardDataList; //游戏中所有的卡牌数据

    [Header("卡牌库")]
    public CardLibrarySO newGameCardLibrary;//新游戏卡牌库
    public CardLibrarySO currentCardLibrary;//当前卡牌库（随着游戏发展改变）

    private void Awake()
    {
        InitializeCardDataList();

        foreach (var cardData in newGameCardLibrary.cardLibraryList)
        {
            currentCardLibrary.cardLibraryList.Add(cardData);
        }
    }
    #region 获取项目卡牌生成卡牌池
    /// <summary>
    /// 初始化获得项目所有卡牌数据
    /// </summary>
    private void InitializeCardDataList()
    {
        Addressables.LoadAssetsAsync<CardDataSO>("CardData", null).Completed += OnCardDataLoad;
    }
    /// <summary>
    /// 回调函数
    /// </summary>
    /// <param name="handle"></param>
    private void OnCardDataLoad(AsyncOperationHandle<IList<CardDataSO>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            cardDataList = new List<CardDataSO>(handle.Result);
        }
        else
        {
            Debug.LogError("CardDataSO加载失败");
        }

    }
    #endregion

    public GameObject GetCardObject(CardDataSO cardData)
    {
        return poolTool.GetObjectFromPool();
    }
    public void ReturnCardObject(GameObject cardObject)
    {
        poolTool.ReturnObjectToPool(cardObject);
    }
}
