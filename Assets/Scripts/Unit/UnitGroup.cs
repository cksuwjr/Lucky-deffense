using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitGroup : UnitBase
{
    public int id;
    private List<UnitBase> units;
    public List<UnitBase> GetUnits { get => units; set => units = value; }
    public UnitSlot unitSlot;

    private Transform one, two, three, Myth;
    private SpriteRenderer attackRangeSpriteRenderer;

    private float shootTime;
    private bool uiVisible = false;

    private float betweenRatio = 0f;

    public static event Action<UnitGroup, bool> OnClickUnitGround;


    private void Awake()
    {
        units = new List<UnitBase>();
        one = transform.GetChild(0);
        two = transform.GetChild(1);
        three = transform.GetChild(2);
        Myth = transform.GetChild(3);

        transform.Find("AttackRange").TryGetComponent<AttackRange>(out attackRange);
        attackRange.TryGetComponent<SpriteRenderer>(out attackRangeSpriteRenderer);

        OnClickUnitGround += UnitSlotClicked;
    }

    public void Init(int id, UnitSlot slotUI)
    {
        this.id = id;
        this.unitSlot = slotUI;
        slotUI.Init(this);

    }

    public void Moves(Path Point)
    {
        StartCoroutine("StartMove", Point);
    }

    private IEnumerator StartMove(Path Point)
    {
        movable = false;
        CurrentPoint = Point;

        var curPos = transform.position;
        var nextPos = CurrentPoint.position;

        if (CurrentUnitData.moveSpeed == 0)
            CurrentUnitData.moveSpeed = 10;

        while (betweenRatio <= 1)
        {
            betweenRatio += Time.deltaTime * CurrentUnitData.moveSpeed;

            Move(curPos, nextPos, betweenRatio);

            //if (curPos.x < nextPos.x) SetDirection(Direction.Left);
            //if (curPos.x > nextPos.x) SetDirection(Direction.Right);

            yield return null;
        }

        betweenRatio = 0f;
        movable = true;
    }

    public bool TryIn(UnitBase unit)
    {
        if (units.Count < 1)
        {
            units.Add(unit);
            unit.SetAttackRange(attackRange);
            CurrentUnitData = unit.CurrentUnitData;
            attackRange.Init(CurrentUnitData.attackRange);
            
            Sort();
            return true;
        }
        else
        {
            if (units[0].OriginUnitData.id == unit.OriginUnitData.id)
            {
                units.Add(unit);
                unit.SetAttackRange(attackRange);
                Sort();
                return true;
            }
            else
                return false;
        }
    }

    public void Sell()
    {
        if (units.Count < 1) return;

        var sellUnit = units[units.Count - 1];
        units.Remove(sellUnit);
        sellUnit.Die();

        if (units.Count > 0) Sort();
        else SetUIVisible(false);
    }

    private void Clear()
    {
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i])
                units[i].Die();
        }
        units.Clear();
    }

    private void Sort()
    {
        Transform slotTrans;
        if (units.Count == 0) return;
        else if(units.Count == 1) slotTrans = one;
        else if(units.Count == 2) slotTrans = two;
        else slotTrans = three;

        for (int i = 0; i < slotTrans.childCount; i++)
        {
            units[i].transform.parent = slotTrans.GetChild(i);
            units[i].transform.localPosition = Vector3.zero;
        }
    }

    public bool IsFull()
    {
        return units.Count >= 3;
    }

    public bool IsNull()
    {
        return units.Count == 0;
    }

    public int GetID()
    {
        return units[0].CurrentUnitData.id;
    }

    public override void Attack()
    {
        if(movable)
            StartCoroutine("AttackSequence");
    }

    private void Update()
    {
        if(units.Count < 1) return;

        if (shootTime < Time.time)
        {
            Attack();
            shootTime = Time.time + 1f / CurrentUnitData.attackSpeed;
        }
    }

    private IEnumerator AttackSequence()
    {
        if (attackRange.GetEnemys(CurrentUnitData.attackCount).Length > 0)
        {
            for (int i = 0; i < units.Count; i++)
            {
                units[i].Attack();
                units[i].SetDirection(units[0].Direction);
                yield return YieldInstructionCache.WaitForSeconds((1f / CurrentUnitData.attackSpeed)/3);
            }
        }
    }

    public void OnClickUnitSlotUI()
    {
        if (units.Count < 1) return;

        SetUIVisible(!uiVisible);
    }

    public void Interaction()
    {
        if (units.Count < 1) return;

        SetUIVisible(false);
        var nextSpawn = GetID() / 100;
        Clear();
        switch(nextSpawn)
        {
            case 1:
                GameManager.Instance.unitSpawnManager.UniqueSpawn(CurrentPoint.position);
                break;
            case 2:
                GameManager.Instance.unitSpawnManager.HeroSpawn(CurrentPoint.position);
                break;
            case 3:
                GameManager.Instance.unitSpawnManager.LegendSpawn(CurrentPoint.position);
                break;
        }

    }

    public void SetUIVisible(bool tf)
    {
        uiVisible = tf;
        attackRangeSpriteRenderer.enabled = uiVisible;
        OnClickUnitGround?.Invoke(this, uiVisible);
    }

    private void UnitSlotClicked(UnitGroup unitSlot, bool arg2)
    {
        if (units != unitSlot.GetUnits)
        {
            //MyUIVisible(false);
            uiVisible = false;
            attackRangeSpriteRenderer.enabled = uiVisible;
        }
    }

    
}
