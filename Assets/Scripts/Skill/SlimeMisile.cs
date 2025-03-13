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

        if (enemys.Count > 0)
        {
            int j = 0;

            UnitBase target = enemys[j];
            var totalDamage = 0f;

            for (int i = 0; i < 12; i++)
            {
                if (target.dead)
                {
                    if (j < enemys.Count - 1)
                        j++;
                    else
                        j = 0;

                    target = enemys[j];
                }

                if (PoolManager.Instance.projectilePool.GetPoolObject().TryGetComponent<Projectile>(out var proj))
                {
                    var damage = owner.CurrentUnitData.attackPower * skillData.damageRatio;
                    totalDamage += damage;
                    proj.Init(owner, target, 1.5f, damage);
                    proj.transform.position = owner.transform.position;

                    if (totalDamage > target.CurrentUnitData.hp)
                    {
                        if (j < enemys.Count - 1)
                            j++;
                        else
                            j = 0;

                        target = enemys[j];
                        totalDamage = 0;
                    }
                }
                if (PoolManager.Instance.projectilePool.GetPoolObject().TryGetComponent<Projectile>(out proj))
                {
                    var damage = owner.CurrentUnitData.attackPower * skillData.damageRatio;
                    totalDamage += damage;

                    proj.Init(owner, target, 1.5f, damage);
                    proj.transform.position = owner.transform.position;

                    if (totalDamage > target.CurrentUnitData.hp)
                    {
                        if (j < enemys.Count - 1)
                            j++;
                        else
                            j = 0;

                        target = enemys[j];
                        totalDamage = 0;
                    }
                }
                yield return YieldInstructionCache.WaitForSeconds(0.05f);
                j++;
            }
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
