using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum AttackType
{
    None,
    Melee,
    Long,
}

public class Character : UnitBase
{
    //private float shootTime;
    private Animator[] animators;
    private AttackType attackType;

    private void Awake()
    {
        //transform.Find("AttackRange").TryGetComponent<AttackRange>(out  attackRange);
        animators = transform.GetComponentsInChildren<Animator>();
    }

    public override void InitUnit(List<Path> movePath, Path startPos, UnitData unitData)
    {
        base.InitUnit(movePath, startPos, unitData);
        if(unitData.attackType == "melee")
            attackType = AttackType.Melee;
        if (unitData.attackType == "long")
            attackType = AttackType.Long;
        if (unitData.attackType == "none")
            attackType = AttackType.None;
    }


    public override void Attack()
    {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        var enemys = attackRange.GetEnemys(CurrentUnitData.attackCount);

        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].SetTrigger("Attack");
            animators[i].speed = CurrentUnitData.attackSpeed;
        }

        yield return YieldInstructionCache.WaitForSeconds((1f / CurrentUnitData.attackSpeed) / 2);

        if (enemys.Length > 0)
        {

            var unitManager = GameManager.Instance.unitManager;
            //Debug.Log("АјАн");

            switch (attackType)
            {
                case AttackType.Melee:
                    for (int i = enemys.Length - 1; i >= 0; i--)
                    {
                        if (transform.position.x < enemys[i].transform.position.x)
                            SetDirection(Direction.Right);
                        else
                            SetDirection(Direction.Left);

                        enemys[i].GetDamage(CurrentUnitData.attackPower * (1 + unitManager.NormalUnitUpgradeData.reinforceRatio));
                    }
                    break;
                case AttackType.Long:
                    for (int i = enemys.Length - 1; i >= 0; i--)
                    {
                        if (transform.position.x < enemys[i].transform.position.x)
                            SetDirection(Direction.Right);
                        else
                            SetDirection(Direction.Left);

                        if (PoolManager.Instance.projectilePool.GetPoolObject().TryGetComponent<Projectile>(out var proj))
                        {
                            proj.Init(this, enemys[i], 5, CurrentUnitData.attackPower * (1 + unitManager.NormalUnitUpgradeData.reinforceRatio));
                            proj.transform.position = transform.position;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }



    private void Update()
    {
        if (!movable) return;

        //if (shootTime < Time.time)
        //{
        //    Attack();
        //    shootTime = Time.time + 1f / CurrentUnitData.attackSpeed;
        //}
    }

    public override void Die()
    {
        base.Die();
        Invoke("DieAfter", 0f);
        GameManager.Instance.walletManager.Gold += CurrentUnitData.money;
    }
}
