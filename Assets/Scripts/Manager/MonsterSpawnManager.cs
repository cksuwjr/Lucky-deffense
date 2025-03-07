using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnManager : MonoBehaviour
{
    private int waveID = 10000;
    private WaveData waveData;

    private List<UnitBase> spawnedMonsters = new List<UnitBase>();


    public static event Action<WaveData> OnChangeWave;
    public static event Action<int, int> OnChangeMonsterCount;

    private int maxCount = 100;
    private int wave = 0;

    public void Init()
    {
        StartSpawn();
    }

    public UnitBase Spawn(List<Path> guide)
    {
        var monster = PoolManager.Instance.monsterPool.GetPoolObject();
        var dataManager = DataManager.Instance;
        var unitData = dataManager.GetMonsterData(dataManager.GetWaveData(waveID).monsterID);

        var anim = monster.GetComponentInChildren<Animator>();
        anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(unitData.animatorSrc);

        if (monster.TryGetComponent<UnitBase>(out var unit))
        {
            unit.OnSpawned += CountCheckPlus;
            unit.OnDespawned += CountCheckMinus;

            unit.InitUnit(guide, guide[0], unitData);
            unit.transform.localScale = Vector3.one;
            return unit;
        }

        return null;
    }

    public void StartSpawn()
    {
        StartCoroutine("Spawning");
    }

    public void StopSpawn()
    {
        StopCoroutine("Spawning");
    }

    private IEnumerator Spawning()
    {
        var mapManager = GameManager.Instance.mapManager;
        var dataManager = DataManager.Instance;
        float term = 0f;

        yield return YieldInstructionCache.WaitForSeconds(2f);

        for(; wave < dataManager.GetWaveCount(); wave++)
        {
            waveID = 10000 + wave;
            waveData = dataManager.GetWaveData(waveID);
            GameManager.Instance.walletManager.Gold += waveData.clearMoney;
            if (PoolManager.Instance.toastGoldPool.GetPoolObject().TryGetComponent<ToastObject>(out ToastObject toastObject))
            {
                toastObject.transform.position = new Vector3(-0.58f, -2.64f); // jual = 0.14f
                toastObject.Init(waveData.clearMoney, "+", ToastType.Bubble);
            }

            OnChangeWave?.Invoke(waveData);
            term = waveData.termSecond;
            if (waveID % 10 == 9)
            {
                for(int j = 0; j < waveData.monsterCount; j++)
                {
                    var boss = Spawn(mapManager.GuideMapA());
                    boss.transform.localScale = new Vector3(2, 2, 2);

                    yield return YieldInstructionCache.WaitForSeconds(term);
                    boss = Spawn(mapManager.GuideMapB());
                    boss.transform.localScale = new Vector3(2, 2, 2);

                    yield return YieldInstructionCache.WaitForSeconds(term);
                }
                for(int j = 0; j < spawnedMonsters.Count; j++)
                    spawnedMonsters[j].OnDespawned += CheckAllDie;
                StopSpawn(); // boss stage
                break;
            }
            else
            {
                for (int j = 0; j < waveData.monsterCount; j++)
                {
                    Spawn(mapManager.GuideMapA());
                    yield return YieldInstructionCache.WaitForSeconds(term);
                    Spawn(mapManager.GuideMapB());
                    yield return YieldInstructionCache.WaitForSeconds(term);
                }
                yield return YieldInstructionCache.WaitForSeconds(5);
            }
        }
    }


    private void CountCheckPlus(UnitBase unit)
    {
        spawnedMonsters.Add(unit);
        OnChangeMonsterCount?.Invoke(spawnedMonsters.Count, maxCount);
    }

    private void CountCheckMinus(UnitBase unit)
    {
        unit.OnSpawned -= CountCheckPlus;
        unit.OnDespawned -= CountCheckMinus;

        spawnedMonsters.Remove(unit);
        OnChangeMonsterCount?.Invoke(spawnedMonsters.Count, maxCount);
    }

    private void CheckAllDie(UnitBase unit)
    {
        unit.OnDespawned -= CheckAllDie;

        if (spawnedMonsters.Count < 1)
        {
            wave++;
            StartSpawn();
        }
    }
}
