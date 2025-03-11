using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActiveSkill
{
    public bool Active();
    public void SkillName();
    public float MPCost { get; }
    public int Cooldown { get; }
}
