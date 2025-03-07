using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum FairReason
{
    ShortMoney,
    ShortJual,
    FullUnit,
}

public class UnitSpawnManager : MonoBehaviour
{
    private List<UnitBase> spawnedUnits = new List<UnitBase>();
    private int maxCount = 26;
    private float spawnCost = 20;
    public float SpawnCost { get => spawnCost; set { spawnCost = value; OnChangeSpawnCost?.Invoke(spawnCost); } }


    public static event Action<int, int> OnSpawnUnit;
    public static event Action<float> OnChangeSpawnCost;
    public static event Action<FairReason> OnSpawnFail;
    public static event Action<int, UnitSlot> OnUnitSlotCreated;

    private Dictionary<int, Path> mapAPath = new Dictionary<int, Path>();
    private Dictionary<int, Path> mapBPath = new Dictionary<int, Path>();

    private Dictionary<int, UnitSlot> unitMapA = new Dictionary<int, UnitSlot>();
    private Dictionary<int, UnitSlot> unitMapB = new Dictionary<int, UnitSlot>();


    public void Init()
    {
        var aMap = GameManager.Instance.mapManager.UnitMapA();
        var bMap = GameManager.Instance.mapManager.UnitMapB();

        for (int i = 0; i < aMap.Count; i++)
        {
            mapAPath.Add(i, aMap[i]);
            var unitSlot = PoolManager.Instance.unitSlotPool.GetPoolObject().GetComponent<UnitSlot>();

            unitSlot.transform.position = mapAPath[i].position;
            unitMapA.Add(i, unitSlot);
            OnUnitSlotCreated?.Invoke(i, unitSlot);
        }
        for (int i = 0; i < bMap.Count; i++)
        {
            mapBPath.Add(i, bMap[i]);
        }

        SpawnCost = 20;
        OnSpawnUnit?.Invoke(spawnedUnits.Count, maxCount);
    }

    public void OnClickSpawnBtn()
    {
        if (GameManager.Instance.walletManager.Gold < spawnCost)
        {
            OnSpawnFail?.Invoke(FairReason.ShortMoney);
            return;
        }

        var unit = Spawn(GameManager.Instance.mapManager.UnitMapA(), mapAPath[0]);
        for(int i = 0; i < unitMapA.Count; i++)
        {
            UnitSlot unitSlot;
            unitMapA.TryGetValue(i, out unitSlot);
            if (unitSlot.IsFull()) continue;

            if (unitSlot.TryIn(unit))
            {
                GameManager.Instance.walletManager.Gold -= SpawnCost;
                SpawnCost += 2;
                return;
            }

            //for (int j = 0; j < unitSlot.Length; j++)
            //{
            //    if (unitSlot[j] == null)
            //    {
            //        unitMapA[i][j] = Spawn(GameManager.Instance.mapManager.UnitMapA(), mapAPath[i]);

            //        GameManager.Instance.walletManager.Gold -= SpawnCost;
            //        SpawnCost += 2;
            //        return;
            //    }
            //}
        }

        OnSpawnFail?.Invoke(FairReason.FullUnit);
    }

    public UnitBase Spawn(List<Path> guide, Path point)
    {
        var unit = PoolManager.Instance.unitPool.GetPoolObject();
        var dataManager = DataManager.Instance;
        
        var unitData = dataManager.GetUnitData(100 + Random.Range(0, 2));


        Debug.Log(unitData.name);
        var anims = unit.GetComponentsInChildren<Animator>();
        for (int i = 0; i < anims.Length; i++)
        {
            anims[i].runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(unitData.animatorSrc + anims[i].name);
        }

        if (unit.TryGetComponent<UnitBase>(out var uni))
        {
            uni.InitUnit(guide, point , unitData);
            spawnedUnits.Add(uni);
            OnSpawnUnit?.Invoke(spawnedUnits.Count, maxCount);
            return uni;
        }

        return null;
    }

}
