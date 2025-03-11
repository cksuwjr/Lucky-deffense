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
            if (cols[i].TryGetComponent<UnitBase>(out var unit))
            {
                if(!unit.dead)
                    enemys.Add(unit);
            }
        }

        int j = enemys.Count - 1;
        var target = enemys[j];

        for (int i = 0; i < skillData.count; i++)
        {
            
            if (target.dead)
            {
                j--;
                if (j < 1)
                    j = enemys.Count - 1;
                target = enemys[j--];
            }

            if (PoolManager.Instance.projectilePool.GetPoolObject().TryGetComponent<Projectile>(out var proj))
            {
                proj.Init(owner, target, 5, owner.CurrentUnitData.attackPower * skillData.damageRatio);
                proj.transform.position = owner.transform.position;
            }
            j--;
            
            target = enemys[j];
            yield return YieldInstructionCache.WaitForSeconds(0.05f);
        }


        //int j = enemys.Count - 1;
        //var target = enemys[j];
        //for (int i = 0; i < skillData.count; i++)
        //{
        //    if (target.dead) continue;

        //    if (PoolManager.Instance.projectilePool.GetPoolObject().TryGetComponent<Projectile>(out var proj))
        //    {
        //        proj.Init(owner, target, 5, owner.CurrentUnitData.attackPower * skillData.damageRatio);
        //        proj.transform.position = owner.transform.position;
        //    }
        //    j--;
        //    if (j < 1)
        //        j = enemys.Count - 1;

        //    target = enemys[j];
        //    yield return YieldInstructionCache.WaitForSeconds(0.05f);
        //}
    }

}
