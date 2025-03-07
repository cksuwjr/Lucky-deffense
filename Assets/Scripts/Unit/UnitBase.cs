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
    public event Action<UnitBase> OnDespawned;

    public bool movable = false;
    public bool dead = false;

    protected AttackRange attackRange;

    public virtual void InitUnit(List<Path> movePath, Path startPos, UnitData unitData)
    {
        if(CurrentUnitData == null) CurrentUnitData = new UnitData();

        CurrentUnitData.name = unitData.name;
        CurrentUnitData.id = unitData.id;
        

        CurrentUnitData.maxHP = unitData.maxHP;
        CurrentUnitData.hp = unitData.hp;
        CurrentUnitData.maxMP = unitData.maxMP;
        CurrentUnitData.mp = unitData.mp;
        CurrentUnitData.moveSpeed = unitData.moveSpeed;
        CurrentUnitData.money = unitData.money;
        CurrentUnitData.attackPower = unitData.attackPower;
        CurrentUnitData.attackSpeed = unitData.attackSpeed;
        CurrentUnitData.attackRange = unitData.attackRange;
        CurrentUnitData.attackType = unitData.attackType;
        CurrentUnitData.attackCount = unitData.attackCount;

        this.movePath = movePath;
        CurrentPoint = startPos;
        transform.position = new Vector3(startPos.position.x, startPos.position.y, 0);

        OriginUnitData = unitData;

        SetMovable(true);
        dead = false;

        OnSpawned?.Invoke(this);


        var attackRangeObject = transform.Find("AttackRange");
        if (!attackRangeObject) return;

        //if (attackRangeObject.TryGetComponent<AttackRange>(out var attackRange))
        //    attackRange.Init(CurrentUnitData.attackRange);

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
}
