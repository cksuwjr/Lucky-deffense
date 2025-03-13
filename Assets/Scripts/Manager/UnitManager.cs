using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Normal = 1,
    Unique = 10,
    Hero = 2,
    Legend = 3,
    Myth = 30,

    Null = 4,
    Null2 = 40,
}

public class UnitManager : MonoBehaviour
{
    private int normalId;
    private int uniqueId;
    private int heroId;
    private int legendId;
    private int mythId;

    public UnitUpgradeData NormalUnitUpgradeData => DataManager.Instance.GetUnitUpgradeData(normalId);
    public UnitUpgradeData UniqueUnitUpgradeData => DataManager.Instance.GetUnitUpgradeData(uniqueId);
    public UnitUpgradeData HeroUnitUpgradeData => DataManager.Instance.GetUnitUpgradeData(heroId);
    public UnitUpgradeData LegendUnitUpgradeData => DataManager.Instance.GetUnitUpgradeData(legendId);
    public UnitUpgradeData MythUnitUpgradeData => DataManager.Instance.GetUnitUpgradeData(mythId);

    public static event Action<UnitType, UnitUpgradeData> OnUpgrade;
    public static event Action<FairReason> OnUpgradeFail;

    public void Init()
    {
        normalId = 100000;
        uniqueId = 100000;
        heroId = 200000;
        legendId = 300000;
        mythId = 300000;
    }

    public void UnitUpgrade(UnitType unitType)
    {
        switch (unitType)
        {
            case UnitType.Normal:
                Upgrade(unitType, ref normalId);
                break;
            case UnitType.Unique:
                Upgrade(unitType, ref uniqueId);
                break;
            case UnitType.Hero:
                Upgrade(unitType, ref heroId);
                break;
            case UnitType.Legend:
                Upgrade(unitType, ref legendId);
                break;
            case UnitType.Myth:
                Upgrade(unitType, ref mythId);
                break;
        }
    }

    private void Upgrade(UnitType type, ref int id)
    {
        var upgradeData = DataManager.Instance.GetUnitUpgradeData(id);
        if (upgradeData.level == "Max") return;

        if (Spend(upgradeData.costType, upgradeData.nextCost))
        {
            id++;
            OnUpgrade?.Invoke(type, DataManager.Instance.GetUnitUpgradeData(id));
            return;
        }
    }

    private bool Spend(string costType, float cost)
    {
        if (costType == "gold")
        {
            if (cost > GameManager.Instance.walletManager.Gold)
            {
                OnUpgradeFail?.Invoke(FairReason.ShortMoney);
                return false;
            }
            else
            {
                GameManager.Instance.walletManager.Gold -= cost;
                return true;
            }
        }
        if (costType == "jual")
        {
            if (cost > GameManager.Instance.walletManager.Jual)
            {
                OnUpgradeFail?.Invoke(FairReason.ShortJual);
                return false;
            }
            else
            {
                GameManager.Instance.walletManager.Jual -= cost;
                return true;
            }
        }

        return false;
    }
}
