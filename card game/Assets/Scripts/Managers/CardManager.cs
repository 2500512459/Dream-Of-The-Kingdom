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

    private void OnDisable()
    {
        currentCardLibrary.cardLibraryList.Clear();//��Ϸ����ʱ��յ�ǰ���ƿ�
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

    /// <summary>
    /// �鿨ʱ���õĺ�����ÿ���GameObject
    /// </summary>
    /// <param name="cardData"></param>
    /// <returns></returns>
    public GameObject GetCardObject(CardDataSO cardData)
    {
        var cardObject = poolTool.GetObjectFromPool();
        cardObject.transform.localScale = Vector3.zero;//���ÿ��Ƹճ����ʱ��СΪ0
        return cardObject;
    }
    public void ReturnCardObject(GameObject cardObject)
    {
        poolTool.ReturnObjectToPool(cardObject);
    }
}
