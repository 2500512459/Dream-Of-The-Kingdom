using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CardManager : MonoBehaviour
{
    public PoolTool poolTool;//���ƶ����
    public List<CardDataSO> cardDataList; //��Ϸ�����еĿ�������

    [Header("���ƿ�")]
    public CardLibrarySO newGameCardLibrary;//����Ϸ���ƿ�
    public CardLibrarySO currentCardLibrary;//��ǰ���ƿ⣨������Ϸ��չ�ı䣩

    private void Awake()
    {
        InitializeCardDataList();

        foreach (var cardData in newGameCardLibrary.cardLibraryList)
        {
            currentCardLibrary.cardLibraryList.Add(cardData);
        }
    }
    #region ��ȡ��Ŀ�������ɿ��Ƴ�
    /// <summary>
    /// ��ʼ�������Ŀ���п�������
    /// </summary>
    private void InitializeCardDataList()
    {
        Addressables.LoadAssetsAsync<CardDataSO>("CardData", null).Completed += OnCardDataLoad;
    }
    /// <summary>
    /// �ص�����
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
            Debug.LogError("CardDataSO����ʧ��");
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
