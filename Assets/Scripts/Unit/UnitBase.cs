using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Left = -1,
    Right = 1
}

[System.Serializable]
public abstract class UnitBase : PoolObject, IUnit, IMove
{
    private Transform body;

    public UnitData OriginUnitData { get; protected set; }
    public UnitData CurrentUnitData { get; protected set; }
    public Path CurrentPoint { get; protected set; }

    public Direction Direction { get; protected set; }
    
    protected List<Path> movePath;

    public event Action<UnitBase> OnSpawned;
    public event Action<UnitBase> OnHit;
    public event Action<UnitBase> OnManaUp;
    public event Action<UnitBase> OnDespawned;

    public bool movable = false;
    public bool dead = false;

    protected AttackRange attackRange;
    protected List<IActiveSkill> skills = new List<IActiveSkill>();
    protected IActiveSkill manaSkill;

    protected SpriteRenderer[] spriteRenderer;

    public virtual void InitUnit(List<Path> movePath, Path startPos, UnitData unitData)
    {
        if(CurrentUnitData == null) CurrentUnitData = new UnitData();
        if (unitData is not null)
        {
            CurrentUnitData.name = unitData.name;
            CurrentUnitData.id = unitData.id;


            CurrentUnitData.maxHP = unitData.maxHP;
            CurrentUnitData.hp = unitData.hp;
            CurrentUnitData.maxMP = unitData.maxMP;
            CurrentUnitData.mp = unitData.mp;
            CurrentUnitData.moveSpeed = unitData.moveSpeed;
            CurrentUnitData.money = unitData.money;
            CurrentUnitData.jual = unitData.jual;
            CurrentUnitData.attackPower = unitData.attackPower;
            CurrentUnitData.attackSpeed = unitData.attackSpeed;
            CurrentUnitData.attackRange = unitData.attackRange;
            CurrentUnitData.attackType = unitData.attackType;
            CurrentUnitData.attackCount = unitData.attackCount;

            CurrentUnitData.skillID1 = unitData.skillID1;
            CurrentUnitData.skillID2 = unitData.skillID2;
            CurrentUnitData.skillID3 = unitData.skillID3;
            CurrentUnitData.manaSkill = unitData.manaSkill;

            var skillManager = SkillManager.Instance;

            skills.Clear();
            SkillBase skill;
            int skillID;

            skillID = CurrentUnitData.skillID1;
            if (skillID != 0)
            {
                skill = skillManager.GetSkill(skillID);
                skills.Add(skill);
                skill.Init(this);
            }
            skillID = CurrentUnitData.skillID2;
            if (skillID != 0)
            {
                skill = skillManager.GetSkill(skillID);
                skills.Add(skill);
                skill.Init(this);
            }
            skillID = CurrentUnitData.skillID3;
            if (skillID != 0)
            {
                skill = skillManager.GetSkill(skillID);
                skills.Add(skill);
                skill.Init(this);
            }
            skillID = CurrentUnitData.manaSkill;
            if (skillID != 0)
            {
                skill = skillManager.GetSkill(skillID);
                skill.Init(this);
                manaSkill = skill;
            }
        }
        this.movePath = movePath;
        CurrentPoint = startPos;
        transform.position = new Vector3(startPos.position.x, startPos.position.y, 0);

        OriginUnitData = unitData;

        SetMovable(true);
        dead = false;

        OnSpawned?.Invoke(this);

        var unitUI = GetComponentInChildren<UnitUI>();
        if (unitUI) unitUI.Init();

        var attackRangeObject = transform.Find("AttackRange");
        if (!attackRangeObject) return;

    }

    public void Move(Vector3 positionA, Vector3 positionB, float t) 
    {
        transform.position = Vector2.Lerp(positionA, positionB, t);
    }

    public void SetMovable(bool tf) 
    {
        movable = tf;
    }

    public virtual void SetDirection(Direction newDirection)
    {
        if(body == null)
            transform.GetChild(0).TryGetComponent<Transform>(out body);

        if (newDirection != 0)
            body.localScale = new Vector3(Mathf.Abs(body.localScale.x) * (int)newDirection, body.localScale.y, body.localScale.z);
    }

    public abstract void Attack();

    public void SetAttackRange(AttackRange attackRange)
    {
        this.attackRange = attackRange;
    }

    public virtual void GetDamage(float value)
    {
        if (dead) return;

        CurrentUnitData.hp -= value;
        OnHit?.Invoke(this);

        if (PoolManager.Instance.damagePool.GetPoolObject().TryGetComponent<ToastObject>(out ToastObject damageObject))
        {
            damageObject.transform.position = transform.position + Vector3.up * 0.2f;
            damageObject.Init(value);
        }

        if (CurrentUnitData.hp < 0)
            Die();
    }

    public virtual void HealMana(float value)
    {
        if (CurrentUnitData.maxMP <= 0) return;

        if (CurrentUnitData.mp >= CurrentUnitData.maxMP) return;

        CurrentUnitData.mp += 10;
        OnManaUp?.Invoke(this);
        
    }

    public virtual void Die()
    {
        dead = true;
        SetMovable(false);
        OnDespawned?.Invoke(this);
        GameManager.Instance.walletManager.Gold += CurrentUnitData.money;
        if (PoolManager.Instance.toastGoldPool.GetPoolObject().TryGetComponent<ToastObject>(out ToastObject toastObject))
        {
            toastObject.transform.position = new Vector3(-0.58f, -2.64f); // jual = 0.14f
            toastObject.Init(CurrentUnitData.money, "+", ToastType.Bubble);
        }
    }

    protected virtual void DieAfter()
    {
        ReturnToPool();
    }

    public virtual void OutLine(bool tf)
    {
        if (spriteRenderer == null) return;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();

        for (int i = 0; i < spriteRenderer.Length; i++)
        {
            spriteRenderer[i].GetPropertyBlock(mpb);
            mpb.SetFloat("_Outline", tf ? 1f : 0);
            mpb.SetColor("_OutlineColor", Color.white);
            mpb.SetFloat("_OutlineSize", 10);
            spriteRenderer[i].SetPropertyBlock(mpb);
        }
    }

}
