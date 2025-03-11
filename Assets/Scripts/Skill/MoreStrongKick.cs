using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreStrongKick : SkillBase
{
    public override bool Active()
    {
        if (TryUseSkill())
        {
            var skillData = DataManager.Instance.GetSkillData(skillID);
            Collider2D[] cols = Physics2D.OverlapCircleAll(owner.CurrentPoint.position, skillData.range);

            for(int i = 0; i < cols.Length; i++)
            {
                if (cols[i].CompareTag("Player")) continue;

                if(cols[i].TryGetComponent<UnitBase>(out var unit))
                {
                    unit.GetDamage(owner.CurrentUnitData.attackPower * skillData.damageRatio);
                }
            }
            Debug.Log("스킬사용");

            return true;
        }
        Debug.Log("스킬사용실패");
        return false;
    }
}
