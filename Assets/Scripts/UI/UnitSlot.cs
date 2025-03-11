using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private RectTransform rectTransform;
    private Vector3 originPos;
    private Button button;
    private Image image;

    public UnitGroup unitGrop;
    public static UnitSlot seletedSlotUI;

    private void Awake()
    {
        TryGetComponent<RectTransform>(out rectTransform);
        TryGetComponent<Button>(out button);
        TryGetComponent<Image>(out image);
        
        originPos = rectTransform.localPosition;
    }

    public void Init(UnitGroup unitsGround)
    {
        this.unitGrop = unitsGround;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(unitGrop.OnClickUnitSlotUI);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        seletedSlotUI = this;
        button.interactable = false;
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
         rectTransform.localPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        seletedSlotUI = null;
        rectTransform.localPosition = originPos;
        button.interactable = true;
        image.raycastTarget = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var anotherGround = seletedSlotUI.unitGrop;
        var myGround = unitGrop;

        var emptySpacePoint = myGround.CurrentPoint;

        myGround.Moves(anotherGround.CurrentPoint);
        anotherGround.Moves(emptySpacePoint);

        //unitGrop = anotherGround;
        //seletedSlotUI.unitGrop = myGround;
        Init(anotherGround);
        seletedSlotUI.Init(myGround);

    }
}
