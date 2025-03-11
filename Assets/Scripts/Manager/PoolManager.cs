using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonDestroy<PoolManager>
{
    public Pool monsterPool;
    public Pool unitPool;
    public Pool projectilePool;
    public Pool damagePool;
    public Pool toastGoldPool;
    public Pool toastJualPool;
    public Pool soundPool;
    public Pool unitSlotPool;
    public Pool effectPool;

    public void Init()
    {
        transform.GetChild(0).TryGetComponent<Pool>(out monsterPool);
        transform.GetChild(1).TryGetComponent<Pool>(out unitPool);
        transform.GetChild(2).TryGetComponent<Pool>(out projectilePool);
        transform.GetChild(3).TryGetComponent<Pool>(out damagePool);
        transform.GetChild(4).TryGetComponent<Pool>(out toastGoldPool);
        transform.GetChild(5).TryGetComponent<Pool>(out toastJualPool);
        transform.GetChild(6).TryGetComponent<Pool>(out soundPool);
        transform.GetChild(7).TryGetComponent<Pool>(out unitSlotPool);
        transform.GetChild(8).TryGetComponent<Pool>(out effectPool);

        foreach (var pool in transform.GetComponentsInChildren<Pool>())
            pool.Init();
    }
}
