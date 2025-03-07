using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSlot : UnitBase
{
    private List<UnitBase> units;
    public List<UnitBase> Units => units;
    private SlotUI slotUI;


    private Transform one;
    private Transform two;
    private Transform three;
    private Transform Myth;

    private float shootTime;

    public static event Action<UnitSlot, bool> OnClickUnitSlot;

    private SpriteRenderer attackRangeSpriteRenderer;
    private bool uiVisible = false;

    private void Awake()
    {
        units = new List<UnitBase>();
        one = transform.GetChild(0);
        two = transform.GetChild(1);
        three = transform.GetChild(2);
        Myth = transform.GetChild(3);

        transform.Find("AttackRange").TryGetComponent<AttackRange>(out attackRange);
        attackRange.TryGetComponent<SpriteRenderer>(out attackRangeSpriteRenderer);

        OnClickUnitSlot += UnitSlotClicked;
    }

    public void Init(SlotUI slotUI)
    {
        this.slotUI = slotUI;

        slotUI.Init(this);
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

        if (units.Count > 0)
            Sort();
        else
        {
            SetUIVisible(false);
        }
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
        if (units.Count == 0)
            return;
        else if(units.Count == 1)
            slotTrans = one;
        else if(units.Count == 2)
            slotTrans = two;
        else
            slotTrans = three;

        for(int i = 0; i < slotTrans.childCount; i++)
            units[i].transform.position = slotTrans.GetChild(i).position;
    }

    public bool IsFull()
    {
        return units.Count >= 3;
    }

    public override void Attack()
    {
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
        Clear();
    }

    public void SetUIVisible(bool tf)
    {
        uiVisible = tf;
        attackRangeSpriteRenderer.enabled = uiVisible;
        OnClickUnitSlot?.Invoke(this, uiVisible);
    }

    private void UnitSlotClicked(UnitSlot unitSlot, bool arg2)
    {
        if (units != unitSlot.Units)
        {
            //MyUIVisible(false);
            uiVisible = false;
            attackRangeSpriteRenderer.enabled = uiVisible;

        }
    }
}
