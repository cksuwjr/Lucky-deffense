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
        var aproachtime = Vector3.Distance(curPos, nextPos);
        var arriveTime = 0f;
        //Debug.Log("도달시간:" + aproachtime);
        if (CurrentUnitData.moveSpeed == 0)
            CurrentUnitData.moveSpeed = 10;
        var animators = GetComponentsInChildren<Animator>();
        for (int j = 0; j < animators.Length; j++)
            animators[j].SetBool("Move", true);

        while (betweenRatio < 1)
        {
            arriveTime += Time.deltaTime * CurrentUnitData.moveSpeed;
            betweenRatio = Mathf.Lerp(0, 1, arriveTime / aproachtime);
            Move(curPos, nextPos, betweenRatio);
            if(uiVisible)
                SetUIVisible(uiVisible);

            for(int i = 0; i < units.Count; i++)
            {
                if (curPos.x < nextPos.x) units[i].SetDirection(Direction.Right);
                if (curPos.x > nextPos.x) units[i].SetDirection(Direction.Left);
            }

            yield return null;
        }
        for (int j = 0; j < animators.Length; j++)
            animators[j].SetBool("Move", false);

        //Debug.Log("도착");
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
                SetUIVisible(uiVisible);
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

        if (slotTrans == three)
            OutLine(true);
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
        OutLine(tf);

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

    public override void OutLine(bool tf)
    {
        if (units.Count < 3)
        {
            for (int i = 0; i < units.Count; i++)
                units[i].OutLine(tf);
        }
        else
        {
            for (int i = 0; i < units.Count; i++)
                units[i].OutLine(true);
        }
    }
}
