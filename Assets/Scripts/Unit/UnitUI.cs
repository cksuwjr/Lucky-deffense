using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    private GameObject bar;
    private Image hpBar;

    private void Awake()
    {
        bar = transform.GetChild(0).gameObject;
        bar.transform.GetChild(0).TryGetComponent<Image>(out hpBar);
        var unit = GetComponentInParent<UnitBase>();
        if (unit == null) return;
        
        unit.OnHit += SetHpbar;
        unit.OnDespawned += SetVisibleFalse;

    }


    public void Init()
    {
        SetVisible(false);
    }

    public void SetVisible(bool tf)
    {
        bar.SetActive(tf);
    }

    private void SetVisibleFalse(UnitBase unit)
    {
        SetVisible(false);
    }

    private void SetHpbar(UnitBase unit)
    {
        hpBar.fillAmount = unit.CurrentUnitData.hp / unit.CurrentUnitData.maxHP;
        SetVisible(true);
    }
}
