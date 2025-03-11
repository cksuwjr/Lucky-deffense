using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongKick : SkillBase
{
    public override bool Active()
    {
        if (TryUseSkill())
        {
            var skillData = DataManager.Instance.GetSkillData(skillID);
            Collider2D[] cols = Physics2D.OverlapCircleAll(owner.CurrentPoint.position, skillData.range);

            List<UnitBase> enemys = new List<UnitBase>();

            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].CompareTag("Player")) continue;
                if (cols[i].TryGetComponent<UnitBase>(out var unit)) enemys.Add(unit);
            }

            int maxCount = skillData.count > enemys.Count ? enemys.Count : skillData.count;
            for(int i = 0; i < maxCount; i++)
                enemys[i].GetDamage(owner.CurrentUnitData.attackPower * skillData.damageRatio);


            Debug.Log("스킬사용");

            return true;
        }

        return false;
    }
}
