using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private RectTransform rectTransform;

    //private UnitBase unitGroup;
    private UnitSlot unitSlot;

    private static SlotUI seletedSlotUI;

    private void Awake()
    {
        TryGetComponent<RectTransform>(out rectTransform);
    }

    public void Init(UnitSlot unitSlot)
    {
        //this.unitGroup = unitGroup;
        Debug.Log(unitSlot);
        this.unitSlot = unitSlot;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        seletedSlotUI = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
         rectTransform.localPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        seletedSlotUI = null;
        //rectTransform.localPosition = unitSlot.CurrentPoint.position;
    }

    public void OnDrop(PointerEventData eventData)
    {
        ChangeSlot();
    }

    private void ChangeSlot()
    {
        var c = unitSlot;

        Debug.Log("바꾸기전" + unitSlot.CurrentUnitData.id);

        unitSlot = seletedSlotUI.unitSlot;
        seletedSlotUI.unitSlot = c;

        Debug.Log("바꾸기후" + unitSlot.CurrentUnitData.id);
    }
}
