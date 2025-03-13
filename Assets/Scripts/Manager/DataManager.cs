using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DataManagerTask
{
    None,
    LoadTable,
    CheckLogin,
    LoadUserInformation,
}

public class DataManager : Singleton<DataManager>
{
    public bool isDataLoad = false;
    public DataManagerTask task = DataManagerTask.None;

    private DataTable data;
    private Dictionary<int, UserData> userData = new Dictionary<int, UserData>();
    private Dictionary<int, UnitData> monsterData = new Dictionary<int, UnitData>();
    private Dictionary<int, UnitData> unitData = new Dictionary<int, UnitData>();
    private Dictionary<int, WaveData> waveData = new Dictionary<int, WaveData>();
    private Dictionary<int, UnitUpgradeData> unitUpgradeData = new Dictionary<int, UnitUpgradeData>();
    private Dictionary<int, UnitSpawnData> unitSpawnData = new Dictionary<int, UnitSpawnData>();
    private Dictionary<int, UnitSpawnProbability> unitSpawnProbability = new Dictionary<int, UnitSpawnProbability>();
    private Dictionary<int, SkillData> skillData = new Dictionary<int, SkillData>();
    private Dictionary<int, ProjectileData> projectileData = new Dictionary<int, ProjectileData>();

    public static event Action<bool> OnDataLoad;

    #region _Load_

    private void LoadTable()
    {
        data = Resources.Load<DataTable>("Data/DataTable");

        task = DataManagerTask.LoadTable;

        for (int i = 0; i < data.UserData.Count; i++)
            userData.Add(data.UserData[i].id, data.UserData[i]);

        for (int i = 0; i < data.MonsterData.Count; i++)
            monsterData.Add(data.MonsterData[i].id, data.MonsterData[i]);

        for (int i = 0; i < data.UnitData.Count; i++)
            unitData.Add(data.UnitData[i].id, data.UnitData[i]);

        for (int i = 0; i < data.WaveData.Count; i++)
            waveData.Add(data.WaveData[i].id, data.WaveData[i]);

        for (int i = 0; i < data.UnitUpgradeData.Count; i++)
            unitUpgradeData.Add(data.UnitUpgradeData[i].id, data.UnitUpgradeData[i]);

        for (int i = 0; i < data.UnitSpawnData.Count; i++)
            unitSpawnData.Add(data.UnitSpawnData[i].id, data.UnitSpawnData[i]);

        for (int i = 0; i < data.UnitSpawnProbability.Count; i++)
            unitSpawnProbability.Add(data.UnitSpawnProbability[i].id, data.UnitSpawnProbability[i]);

        for (int i = 0; i < data.SkillData.Count; i++)
            skillData.Add(data.SkillData[i].id, data.SkillData[i]);

        for(int i = 0; i < data.ProjectileData.Count; i++)
            projectileData.Add(data.ProjectileData[i].unitID, data.ProjectileData[i]);
    }

    private void CheckLogin()
    {
        task = DataManagerTask.CheckLogin;
    }

    private void LoadUserInformation()
    {
        task = DataManagerTask.LoadUserInformation;

    }

    public void LoadData()
    {
        if (isDataLoad)
            OnDataLoad?.Invoke(false);
        else
            StartCoroutine("LoadDataCoroutine");
    }

    public void LoadData(float time)
    {
        if (isDataLoad)
            OnDataLoad?.Invoke(false);
        else
            StartCoroutine(LoadDataCoroutine(time));
    }

    private IEnumerator LoadDataCoroutine()
    {
        OnDataLoad?.Invoke(true);
        LoadTable();
        yield return null;
        CheckLogin();
        yield return null;
        LoadUserInformation();
        yield return null;
        isDataLoad = true;
        OnDataLoad?.Invoke(false);
    }

    private IEnumerator LoadDataCoroutine(float time)
    {
        OnDataLoad?.Invoke(true);
        LoadTable();
        yield return YieldInstructionCache.WaitForSeconds(time);
        CheckLogin();

        yield return YieldInstructionCache.WaitForSeconds(time);
        LoadUserInformation();

        yield return YieldInstructionCache.WaitForSeconds(time);
        isDataLoad = true;
        OnDataLoad?.Invoke(false);
    }

    #endregion

    public UserData GetUserData(int id)
    {
        UserData user;
        userData.TryGetValue(id, out user);
        return user;
    }

    public UnitData GetUnitData(int id)
    {
        UnitData unit;
        unitData.TryGetValue(id, out unit);
        return unit;
    }

    public UnitData GetMonsterData(int id)
    {
        UnitData monster;
        monsterData.TryGetValue(id, out monster);
        return monster;
    }

    public WaveData GetWaveData(int id)
    {
        WaveData waveDat;
        waveData.TryGetValue(id, out waveDat);
        return waveDat;
    }

    public int GetWaveCount()
    {
        return waveData.Count;
    }

    public UnitUpgradeData GetUnitUpgradeData(int id)
    {
        UnitUpgradeData uUData;

        unitUpgradeData.TryGetValue(id, out uUData);
        return uUData;
    }

    public UnitSpawnData GetUnitSpawnData(int id)
    {
        UnitSpawnData uUData;
        unitSpawnData.TryGetValue(id, out uUData);
        return uUData;
    }

    public UnitSpawnProbability GetUnitSpawnProbability(int id)
    {
        UnitSpawnProbability usProbabilty;
        unitSpawnProbability.TryGetValue(id, out usProbabilty);
        return usProbabilty;
    }

    public SkillData GetSkillData(int id)
    {
        SkillData sData;
        skillData.TryGetValue(id, out sData);
        return sData;
    }

    public ProjectileData GetProjectileData(int id)
    {
        ProjectileData pData;
        projectileData.TryGetValue(id, out pData);
        return pData;
    }
}
