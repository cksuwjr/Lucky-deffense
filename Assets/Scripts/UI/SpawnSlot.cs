using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnSlot : MonoBehaviour
{
    private UnitType type;
    private Image typeImage;
    private TextMeshProUGUI ratioText;
    private Button upgradeButton;
    private Image upgradeImage;
    private TextMeshProUGUI costText;

    public static event Action<SpawnSlot> onSpawn;

    private void Awake()
    {
        transform.GetChild(0).GetChild(0).TryGetComponent<TextMeshProUGUI>(out ratioText);
        transform.GetChild(1).TryGetComponent<Image>(out typeImage);
        transform.GetChild(2).TryGetComponent<Button>(out upgradeButton);
        upgradeButton.transform.GetChild(0).TryGetComponent<Image>(out upgradeImage);
        upgradeButton.transform.GetChild(1).TryGetComponent<TextMeshProUGUI>(out costText);
    }

    public void Init(string typeImageSrc, float ratio, string costType, float cost)
    {
        SetSlotUI(typeImageSrc, ratio, costType, cost);

        upgradeButton.onClick.AddListener(() => GameManager.Instance.unitManager.UnitUpgrade(type));
        upgradeButton.onClick.AddListener(() => GameManager.Instance.unitManager.UnitUpgrade((UnitType)(10 * (int)type)));
        //UnitManager.OnUpgrade += SetSlot;
    }

    private void SetSlot(UnitType type, UnitSpawnData data)
    {
        if (this.type == type)
            SetSlotUI(data.imageSrc, data.spawnRatio, data.costType, data.cost);
    }

    private void SetSlotUI(string typeImageSrc, float ratio, string costType, float cost)
    {
        //typeText.text = typeT;
        ratioText.text = $"{ratio*100}%";
        typeImage.sprite = Resources.Load<Sprite>(typeImageSrc);
        upgradeImage.sprite = Resources.Load<Sprite>("Sprite/" + costType);
        costText.text = $"{cost:F0}";
    }
}
