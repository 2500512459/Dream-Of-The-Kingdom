using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CardManager : MonoBehaviour
{
    public PoolTool poolTool;//卡牌对象池
    public List<CardDataSO> cardDataList; //所有的卡牌种类

    [Header("卡牌库")]
    public CardLibrarySO newGameCardLibrary;//新游戏卡牌库
    public CardLibrarySO currentCardLibrary;//当前卡牌库（随着游戏发展改变）

    private int previousIndex;
    private void Awake()
    {
        InitializeCardDataList();

        foreach (var cardData in newGameCardLibrary.cardLibraryList)
        {
            currentCardLibrary.cardLibraryList.Add(cardData);
        }
    }

    private void OnDisable()
    {
        currentCardLibrary.cardLibraryList.Clear();//游戏结束时清空当前卡牌库
    }

    #region 获取项目卡牌生成卡牌池
    /// <summary>
    /// 初始化从Addressables中获得项目所有卡牌数据
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

    /// <summary>
    /// 抽卡时调用的函数获得卡牌GameObject
    /// </summary>
    /// <param name="cardData"></param>
    /// <returns></returns>
    public GameObject GetCardObject(CardDataSO cardData)
    {
        var cardObject = poolTool.GetObjectFromPool();
        cardObject.transform.localScale = Vector3.zero;//设置卡牌刚抽出来时大小为0
        return cardObject;
    }
    public void ReturnCardObject(GameObject cardObject)
    {
        poolTool.ReturnObjectToPool(cardObject);
    }

    public CardDataSO GetNewCardData()
    {
        var randomIndex = 0;
        do
        {
            randomIndex = UnityEngine.Random.Range(0, cardDataList.Count);
        } while (previousIndex == randomIndex);

        previousIndex = randomIndex;
        return cardDataList[randomIndex];
    }

    /// <summary>
    /// 解锁/添加卡牌
    /// </summary>
    /// <param name="newCardData"></param>
    public void UnlockCard(CardDataSO newCardData)
    {
        var newCard = new CardLibraryEntry
        {
            cardData = newCardData,
            amount = 1
        };
        if (currentCardLibrary.cardLibraryList.Contains(newCard))
        {
            var target = currentCardLibrary.cardLibraryList.Find(t => t.cardData == newCardData);
            target.amount++;
        }
        else
        {
            currentCardLibrary.cardLibraryList.Add(newCard);
        }
    }
}
