using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UserData
{
    public int id;
    public string name;
}


[System.Serializable]
public class UnitData
{
    public int id;
    public string name;
    public float hp;
    public float mp;
    public float maxHP;
    public float maxMP;
    public float moveSpeed;
    public float money;
    public string animatorSrc;
    public float attackPower;
    public float attackSpeed;
    public float attackRange;
    public string attackType;
    public int attackCount;
}

[System.Serializable]
public class WaveData
{
    public int id;
    public string name;
    public int monsterID;
    public int monsterCount;
    public float clearMoney;
    public float termSecond;
}

[System.Serializable]
public class UnitUpgradeData
{
    public int id;
    public string level;
    public string costType;
    public float nextCost;
    public string target;
    public float reinforceRatio;
    public string imageSrc;
}

[System.Serializable]
public class UnitSpawnProbability
{
    public int id;
    public float normal;
    public float unique;
    public float hero;
    public float legend;
    public float myth;
}
