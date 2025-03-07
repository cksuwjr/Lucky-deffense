using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField] private PoolObject poolObject; 
    private Queue<PoolObject> poolObjects = new Queue<PoolObject>();
    private int poolCount = 5;

    public void Init()
    {
        Allocate();
    }

    private void Allocate()
    {
        for (int i = 0; i < poolCount; i++)
        {
            var poolObj = Instantiate(poolObject, transform);
            poolObj.gameObject.SetActive(false);
            poolObjects.Enqueue(poolObj);
        }
    }

    public GameObject GetPoolObject()
    {
        if (poolObjects.Count < 1)
            Allocate();
        var pObject = poolObjects.Dequeue();
        pObject.Init(this);

        return pObject.gameObject;
    }

    public void ReturnPoolObject(PoolObject returnObject)
    {
        returnObject.gameObject.SetActive(false);
        poolObjects.Enqueue(returnObject);
    }

}
