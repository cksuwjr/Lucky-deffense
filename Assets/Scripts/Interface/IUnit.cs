using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    public UnitData OriginUnitData { get; }
    public UnitData CurrentUnitData { get; }

    //public string Name { get; }
    //public float HP { get; }
    //public float MP { get; }
    //public float MaxHP { get;}
    //public float MaxMP { get;}
    //public float AttackPower { get; }
    //public float AttackSpeed { get; }
    public Path CurrentPoint { get; }
}
