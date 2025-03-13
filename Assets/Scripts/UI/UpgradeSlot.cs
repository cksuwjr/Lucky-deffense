using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour
{
    private UnitType type;
    private TextMeshProUGUI typeText;
    private Image typeImage;
    private TextMeshProUGUI levelText;
    private Button upgradeButton;
    private Image upgradeImage;
    private TextMeshProUGUI costText;

    public static event Action<UpgradeSlot> onUpgrade;

    private void Awake()
    {
        assign();
    }

    public void Init(UnitType type, string typeT, string typeImageSrc, string level, string costType, float cost)
    {
        this.type = type;
        SetSlotUI(typeT, typeImageSrc, level, costType, cost);
        upgradeButton.onClick.AddListener(() => GameManager.Instance.unitManager.UnitUpgrade(type));
        upgradeButton.onClick.AddListener(() => GameManager.Instance.unitManager.UnitUpgrade((UnitType)(10 * (int)type)));
        UnitManager.OnUpgrade += SetSlot;
    }

    private void SetSlot(UnitType type, UnitUpgradeData data)
    {
        if (this.type == type)
            SetSlotUI(data.target, data.imageSrc, data.level, data.costType, data.nextCost);
    }

    private void SetSlotUI(string typeT, string typeImageSrc, string level, string costType, float cost)
    {
        typeText.text = typeT;
        typeImage.sprite = Resources.Load<Sprite>(typeImageSrc);
        levelText.text = $"Lv.{level}";
        upgradeImage.sprite = Resources.Load<Sprite>("Sprite/" + costType);
        if(level == "Max")
            costText.text = $"{level}";
        else
            costText.text = $"{cost:F0}";
    }

    private void assign()
    {
        if(!typeText) transform.GetChild(0).TryGetComponent<TextMeshProUGUI>(out typeText);
        if(!typeImage) transform.GetChild(1).TryGetComponent<Image>(out typeImage);
        if(!levelText) transform.GetChild(2).TryGetComponent<TextMeshProUGUI>(out levelText);
        if(!upgradeButton) transform.GetChild(3).TryGetComponent<Button>(out upgradeButton);
        if(!upgradeImage) upgradeButton.transform.GetChild(0).TryGetComponent<Image>(out upgradeImage);
        if(!costText) upgradeButton.transform.GetChild(1).TryGetComponent<TextMeshProUGUI>(out costText);
    }
}
