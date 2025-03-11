using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private DataManager dataManager;

    public MapManager mapManager;
    private PoolManager poolManager;
    private MonsterSpawnManager monsterSpawnManager;
    private UIManager uiManager;
    private TimeManager timeManager;
    public UnitSpawnManager unitSpawnManager;
    public UnitManager unitManager;
    public WalletManager walletManager;
    private SoundManager soundManager;
    private SkillManager skillManager;

    protected override void DoAwake()
    {
        GameObject.Find("DataManager")?.TryGetComponent<DataManager>(out dataManager);
        GameObject.Find("MapManager")?.TryGetComponent<MapManager>(out mapManager);
        GameObject.Find("PoolManager")?.TryGetComponent<PoolManager>(out poolManager);
        GameObject.Find("MonsterSpawnManager")?.TryGetComponent<MonsterSpawnManager>(out monsterSpawnManager);
        GameObject.Find("UIManager")?.TryGetComponent<UIManager>(out uiManager);
        GameObject.Find("TimeManager")?.TryGetComponent<TimeManager>(out timeManager);
        GameObject.Find("UnitManager")?.TryGetComponent<UnitManager>(out unitManager);
        GameObject.Find("UnitSpawnManager")?.TryGetComponent<UnitSpawnManager>(out unitSpawnManager);
        GameObject.Find("WalletManager")?.TryGetComponent<WalletManager>(out walletManager);
        GameObject.Find("SoundManager")?.TryGetComponent<SoundManager>(out soundManager);
        GameObject.Find("SkillManager")?.TryGetComponent<SkillManager>(out skillManager);

        InitManagers();
    }

    private void InitManagers()
    {
        StartCoroutine("Init");
    }

    private IEnumerator Init()
    {
        dataManager.LoadData();

        var task = dataManager.task;
        while (!dataManager.isDataLoad)
        {
            if (task != dataManager.task)
                task = dataManager.task;
            yield return null;
        }
        mapManager.Init();
        unitManager.Init();
        poolManager.Init();
        uiManager.Init();
        StartGame();

    }


    public void StartGame()
    {
        soundManager.Init();
        walletManager.Init();
        timeManager.Init();

        skillManager.Init();
        monsterSpawnManager.Init();
        unitSpawnManager.Init();
    }
}
