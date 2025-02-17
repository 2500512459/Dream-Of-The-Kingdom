using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolTool : MonoBehaviour
{
    public GameObject objPrefab;
    private ObjectPool<GameObject> pool;

    private void Start()
    {
        //��ʼ�������
        pool = new ObjectPool<GameObject>( 
            createFunc: () => Instantiate(objPrefab, transform),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 30
        );
        PreFillPool(10);
    }

    //Ԥ���������
    private void PreFillPool(int count)
    {
        var preFillArray = new GameObject[count];
        //�ȴӶ���ػ�ȡ����
        for (int i = 0; i < count; i++)
        {
            preFillArray[i] = pool.Get();
        }
        //�ͷŶ���
        foreach (var obj in preFillArray)
        {
            pool.Release(obj);
        }
    }
    public GameObject GetObjectFromPool()
    {
        return pool.Get();
    }
    public void ReturnObjectToPool(GameObject obj)
    {
        pool.Release(obj);
    }
}
