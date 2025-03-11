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
    public static event Action<int, UnitGroup> OnUnitSlotCreated;

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
            var unitGroup = PoolManager.Instance.unitSlotPool.GetPoolObject().GetComponent<UnitGroup>();

            unitGroup.InitUnit(aMap, aMap[i], null);
            OnUnitSlotCreated?.Invoke(i, unitGroup);
            unitMapA.Add(i, unitGroup.unitSlot);
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
        if (!CheckSpawnCondition()) return;

        if (GameManager.Instance.walletManager.Gold < spawnCost)
        {
            OnSpawnFail?.Invoke(FairReason.ShortMoney);
            return;
        }

        var unit = SpawnUnit(DataManager.Instance.GetUnitData(100 + Random.Range(0, 3)));
        PlaceUnit(unit, new Vector3(0, -3.4f), () => {
            GameManager.Instance.walletManager.Gold -= SpawnCost;
            SpawnCost += 2;
        });
    }

    public void OnClickLuckyUniqueSpawnBtn()
    {
        if (!CheckSpawnCondition()) return;

        if (GameManager.Instance.walletManager.Jual < 1)
        {
            OnSpawnFail?.Invoke(FairReason.ShortJual);
            return;
        }

        var unit = SpawnUnit(DataManager.Instance.GetUnitData(200 + Random.Range(0, 2)));
        PlaceUnit(unit, new Vector3(0, -3.4f), () => { GameManager.Instance.walletManager.Jual -= 1; });
    }

    public void OnClickLuckyHeroSpawnBtn()
    {
        if (!CheckSpawnCondition()) return;

        if (GameManager.Instance.walletManager.Jual < 1)
        {
            OnSpawnFail?.Invoke(FairReason.ShortJual);
            return;
        }

        var unit = SpawnUnit(DataManager.Instance.GetUnitData(300 + Random.Range(0, 1)));
        PlaceUnit(unit, new Vector3(0, -3.4f), () => { GameManager.Instance.walletManager.Jual -= 1; });
    }

    public void OnClickLuckyLegendSpawnBtn()
    {
        if (!CheckSpawnCondition()) return;

        if (GameManager.Instance.walletManager.Jual < 2)
        {
            OnSpawnFail?.Invoke(FairReason.ShortJual);
            return;
        }
        Debug.Log("아직 구현되지않았습니다 + return됨");
        return;

        var unit = SpawnUnit(DataManager.Instance.GetUnitData(400 + Random.Range(0, 1)));
        PlaceUnit(unit, new Vector3(0, -3.4f), () => { GameManager.Instance.walletManager.Jual -= 2; });
    }

    public void UniqueSpawn(Vector3 startPos)
    {
        var unit = SpawnUnit(DataManager.Instance.GetUnitData(200 + Random.Range(0, 2)));
        PlaceUnit(unit, startPos, () => { });
        if (unit == null)
        {
            GameManager.Instance.walletManager.Jual += 1;
            Debug.Log("유닛이 가득차 보석+1로 대체됩니다.");
        }
    }

    public void HeroSpawn(Vector3 startPos)
    {
        var unit = SpawnUnit(DataManager.Instance.GetUnitData(300 + Random.Range(0, 1)));
        PlaceUnit(unit, startPos, () => { });
        if (unit == null)
        {
            GameManager.Instance.walletManager.Jual += 2;
            Debug.Log("유닛이 가득차 보석+2로 대체됩니다.");
        }
    }

    public void LegendSpawn(Vector3 startPos)
    {
        var unit = SpawnUnit(DataManager.Instance.GetUnitData(400 + Random.Range(0, 1)));
        PlaceUnit(unit, startPos, () => { });
        if (unit == null)
        {
            GameManager.Instance.walletManager.Jual += 4;
            Debug.Log("유닛이 가득차 보석+4로 대체됩니다.");
        }
    }

    private bool CheckSpawnCondition()
    {
        if (spawnedUnits.Count >= maxCount)
        {
            OnSpawnFail?.Invoke(FairReason.FullUnit);
            return false;
        }

        return true;
    }

    private UnitBase SpawnUnit(UnitData unitData)
    {
        var guide = GameManager.Instance.mapManager.UnitMapA();
        var point = mapAPath[0];

        var unit = PoolManager.Instance.unitPool.GetPoolObject();
        var dataManager = DataManager.Instance;

        //Debug.Log(unitData.name);
        var spriteRenderer = unit.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < spriteRenderer.Length; i++)
            spriteRenderer[i].sprite = null;


        var anims = unit.GetComponentsInChildren<Animator>();
        for (int i = 0; i < anims.Length; i++)
            anims[i].runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(unitData.animatorSrc + anims[i].name);

        if (unit.TryGetComponent<UnitBase>(out var uni))
        {
            uni.InitUnit(guide, point, unitData);
            spawnedUnits.Add(uni);
            uni.OnDespawned += UnitDecrease;
            OnSpawnUnit?.Invoke(spawnedUnits.Count, maxCount);
            return uni;
        }
        return null;
    }

    private void PlaceUnit(UnitBase unit, Vector3 effectStartPos, Action consume)
    {
        // null 인거 모두 패스후 아이디 같으면 채워넣기
        for (int i = 0; i < unitMapA.Count; i++)
        {
            UnitSlot unitSlot;
            unitMapA.TryGetValue(i, out unitSlot);
            if (unitSlot.unitGrop.IsNull()) continue;
            if (unitSlot.unitGrop.IsFull()) continue;
            if (unitSlot.unitGrop.GetID() == unit.CurrentUnitData.id)
            {
                if (unitSlot.unitGrop.TryIn(unit))
                {
                    PrintSpawnEffect(effectStartPos, unitSlot.unitGrop.CurrentPoint.position);

                    consume();
                    return;
                }
            }
        }

        // null 인곳에 채워넣기
        for (int i = 0; i < unitMapA.Count; i++)
        {
            UnitSlot unitSlot;
            unitMapA.TryGetValue(i, out unitSlot);
            if (unitSlot.unitGrop.IsFull()) continue;

            if (unitSlot.unitGrop.TryIn(unit))
            {
                PrintSpawnEffect(effectStartPos, unitSlot.unitGrop.CurrentPoint.position);

                consume();
                //GameManager.Instance.walletManager.Gold -= SpawnCost;
                //SpawnCost += 2;
                return;
            }
        }
        unit.Die();
        OnSpawnFail?.Invoke(FairReason.FullUnit);
    }

    private void UnitDecrease(UnitBase unit)
    {
        spawnedUnits.Remove(unit);
        OnSpawnUnit?.Invoke(spawnedUnits.Count, maxCount);
        unit.OnDespawned -= UnitDecrease;
    }

    private void PrintSpawnEffect(Vector3 start, Vector3 end)
    {
        if (PoolManager.Instance.effectPool.GetPoolObject().TryGetComponent<SpawnEffect>(out var effect))
            effect.Init(start, end);
    }
}
