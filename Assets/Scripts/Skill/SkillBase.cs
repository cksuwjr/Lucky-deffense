using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour, IActiveSkill
{
    protected int skillID;
    public int SkillID { get => skillID; set => skillID = value; }

    public float MPCost { get; protected set; }

    public int Cooldown { get; protected set; }

    protected UnitBase owner;

    public void Init(UnitBase onwer)
    {
        this.owner = onwer;
        var skillData = DataManager.Instance.GetSkillData(skillID);

        MPCost = skillData.mpCost;
    }

    public abstract bool Active();

    public void SkillName() { }

    protected bool TryUseSkill()
    {
        if (owner == null) return false;

        var skillData = DataManager.Instance.GetSkillData(skillID);


        if (skillData.mpCost != 0)
        {
            if (owner.CurrentUnitData.mp < MPCost) return false;
        }
        else if (skillData.chance != 0)
        {
            return Random.Range(0f, 1f) < skillData.chance;
        }
        else
        {
            Debug.Log("¿À·ù");
        }

        return true;
    }
}
