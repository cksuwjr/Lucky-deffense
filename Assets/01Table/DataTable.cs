using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class DataTable : ScriptableObject
{
	public List<UserData> UserData; // Replace 'EntityType' to an actual type that is serializable.
	public List<UnitData> UnitData; // Replace 'EntityType' to an actual type that is serializable.
	public List<UnitData> MonsterData; // Replace 'EntityType' to an actual type that is serializable.
	public List<WaveData> WaveData; // Replace 'EntityType' to an actual type that is serializable.
	public List<UnitUpgradeData> UnitUpgradeData;
	public List<UnitSpawnProbability> UnitSpawnProbability;
}
