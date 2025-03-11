using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    private GameObject bar;
    private Image hpmpBar;

    private void Awake()
    {
        bar = transform.GetChild(0).gameObject;
        bar.transform.GetChild(0).TryGetComponent<Image>(out hpmpBar);
        var unit = GetComponentInParent<UnitBase>();
        if (unit == null) return;
        
        unit.OnHit += SetHpMpbar;
        unit.OnManaUp += SetHpMpbar;

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

    private void SetHpMpbar(UnitBase unit)
    {
        SetVisible(true);

        if (unit.CurrentUnitData.maxHP == 0f)
            hpmpBar.fillAmount = unit.CurrentUnitData.mp / unit.CurrentUnitData.maxMP;
        else if (unit.CurrentUnitData.mp == 0f)
            hpmpBar.fillAmount = unit.CurrentUnitData.hp / unit.CurrentUnitData.maxHP;
        else
            SetVisible(false);
    }
}
