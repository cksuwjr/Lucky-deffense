using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMisile : SkillBase
{
    public override bool Active()
    {
        StartCoroutine(Shoot());

        return true;
    }

    private IEnumerator Shoot()
    {
        var skillData = DataManager.Instance.GetSkillData(skillID);
        Collider2D[] cols = Physics2D.OverlapCircleAll(owner.CurrentPoint.position, skillData.range);

        List<UnitBase> enemys = new List<UnitBase>();

        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].CompareTag("Player")) continue;
            if (cols[i].TryGetComponent<UnitBase>(out var unit)) enemys.Add(unit);
        }

        int j = 0;
        for (int i = 0; i < skillData.count; i++)
        {
            if (enemys[j].dead) continue;
            if (PoolManager.Instance.projectilePool.GetPoolObject().TryGetComponent<Projectile>(out var proj))
            {
                proj.Init(owner, enemys[j++], 5, owner.CurrentUnitData.attackPower * skillData.damageRatio);

                proj.transform.position = owner.transform.position;
            }
            if (j >= enemys.Count)
                j = 0;

            yield return YieldInstructionCache.WaitForSeconds(0.05f);
        }
    }

}
