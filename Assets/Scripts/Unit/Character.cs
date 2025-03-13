using System;
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

public enum CharGrade
{
    Normal,
    Unique,
    Hero,
    Legend,
    Myth,
}


public class Character : UnitBase
{
    //private float shootTime;
    private Animator[] animators;
    private AttackType attackType;
    private CharGrade charGrade;

    bool usingSkill = false;

    private void Awake()
    {
        //transform.Find("AttackRange").TryGetComponent<AttackRange>(out  attackRange);
        animators = transform.GetComponentsInChildren<Animator>();
        spriteRenderer = transform.GetComponentsInChildren<SpriteRenderer>();
    }

    public override void InitUnit(List<Path> movePath, Path startPos, UnitData unitData)
    {
        base.InitUnit(movePath, startPos, unitData);
        if (unitData.attackType == "melee") attackType = AttackType.Melee;
        if (unitData.attackType == "long") attackType = AttackType.Long;
        if (unitData.attackType == "none") attackType = AttackType.None;

        if (unitData.id / 100 == 1) charGrade = CharGrade.Normal;
        if (unitData.id / 100 == 2) charGrade = CharGrade.Unique;
        if (unitData.id / 100 == 3) charGrade = CharGrade.Hero;
        if (unitData.id / 100 == 4) charGrade = CharGrade.Legend;
        if (unitData.id / 100 == 5) charGrade = CharGrade.Myth;
    }


    public override void Attack()
    {
        for(int i = 0; i < skills.Count; i++)
        {
            if (skills[i] == null) continue;
            if (skills[i].Active())
            {
                for (int j = 0; j < animators.Length; j++)
                {
                    animators[j].speed = CurrentUnitData.attackSpeed;
                    animators[j].SetTrigger("Skill" + (i+1));
                }
                StartCoroutine("Wait");
                //Debug.Log("스킬이 발동되어 돌아갑니다");
                return;
            }
        }

        if (usingSkill) return;

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator Wait()
    {
        usingSkill = true;
        yield return YieldInstructionCache.WaitForSeconds(1f);
        usingSkill = false;
    }


    private IEnumerator AttackCoroutine()
    {
        var enemys = attackRange.GetEnemys(CurrentUnitData.attackCount);

        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].speed = CurrentUnitData.attackSpeed;
            animators[i].SetTrigger("Attack");
        }
        yield return YieldInstructionCache.WaitForSeconds((1f / CurrentUnitData.attackSpeed) / 2);

        if (enemys.Length > 0)
        {

            var unitManager = GameManager.Instance.unitManager;
            var damageAddRatio = 0f;
            switch (charGrade)
            {
                case CharGrade.Normal:
                    damageAddRatio = unitManager.NormalUnitUpgradeData.reinforceRatio; break;
                case CharGrade.Unique:
                    damageAddRatio = unitManager.UniqueUnitUpgradeData.reinforceRatio; break;
                case CharGrade.Hero:
                    damageAddRatio = unitManager.HeroUnitUpgradeData.reinforceRatio; break;
                case CharGrade.Legend:
                    damageAddRatio = unitManager.LegendUnitUpgradeData.reinforceRatio; break;
                case CharGrade.Myth:
                    damageAddRatio = unitManager.MythUnitUpgradeData.reinforceRatio; break;
            }

            switch (attackType)
            {
                case AttackType.Melee:
                    for (int i = enemys.Length - 1; i >= 0; i--)
                    {
                        if (transform.position.x < enemys[i].transform.position.x)
                            SetDirection(Direction.Right);
                        else
                            SetDirection(Direction.Left);

                        enemys[i].GetDamage(CurrentUnitData.attackPower * (1 + damageAddRatio));
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
                            proj.Init(this, enemys[i], CurrentUnitData.attackSpeed / 2f , CurrentUnitData.attackPower * (1 + damageAddRatio));
                            
                            proj.transform.position = transform.position;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        if (manaSkill != null)
        {
            CurrentUnitData.maxMP = manaSkill.MPCost;

            if (CurrentUnitData.mp < CurrentUnitData.maxMP)
            {
                HealMana(3);
                //Debug.Log("마나회복");
            }
            else
            {
                //Debug.Log("스킬 사용");
                if (manaSkill.Active())
                {
                    CurrentUnitData.mp = 0;
                    HealMana(0);

                    for (int j = 0; j < animators.Length; j++)
                    {
                        animators[j].speed = CurrentUnitData.attackSpeed;
                        animators[j].SetTrigger("Skill" + 4);
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        for(int i = 0; i < animators.Length; i++)
        {
            animators[i].runtimeAnimatorController = null;
        }
        var sps = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sps.Length; i++)
        {
            sps[i].sprite = null;
        }
        OutLine(false);
    }

    
    private void Update()
    {
        if (!movable) return;
    }

    public override void Die()
    {
        base.Die();
        Invoke("DieAfter", 0f);
        GameManager.Instance.walletManager.Gold += CurrentUnitData.money;
    }
}
