using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    private Transform userAInformation;
    private Transform userBInformation;

    private Transform unitInformation;

    private Image unitImage1;
    private Image unitImage2;
    private Image unitImage3;
    private TextMeshProUGUI unitNameText;
    private TextMeshProUGUI unitTypeText;
    private TextMeshProUGUI unitAttackPowerText;
    private TextMeshProUGUI unitAttackSpeedText;
    private TextMeshProUGUI unitSkillNameText;
    private TextMeshProUGUI unitSkillDescriptionText;
    private Image unitSkillIcon;


    private TextMeshProUGUI waveText;
    private TextMeshProUGUI timeText;
    private TextMeshProUGUI stageDifficultyText;

    private Image monsterCountGage;
    private TextMeshProUGUI monsterCountText;

    private GameObject unitUpgradeMenu;
    private TextMeshProUGUI goldText2;
    private TextMeshProUGUI jualText2;

    private Transform unitUpgradeSlots;


    private GameObject unitSpawnMenu;
    private TextMeshProUGUI jualText3;
    private TextMeshProUGUI unitText2;

    private Transform unitSpawnSlots;





    private TextMeshProUGUI goldText;
    private TextMeshProUGUI jualText;
    private TextMeshProUGUI unitText;

    private TextMeshProUGUI unitSpawnCostText;


    private GameObject dataLoading;
    
    private GameObject waveAlert;
    private TextMeshProUGUI waveAlertText;


    private Transform logTransform;

    private Transform unitSlotTransform;
    private GameObject unitSlot;
    private Transform unitManageButtons;
    private Button unitSellButton;
    private Button unitInteractionButton;


    private UnitGroup nowUnitSlot;


    private void Awake()
    {
        GameObject.Find("UserAInformation").TryGetComponent<Transform>(out userAInformation);
        GameObject.Find("UserBInformation").TryGetComponent<Transform>(out userBInformation);

        GameObject.Find("UnitInformation").TryGetComponent<Transform>(out unitInformation);

        GameObject.Find("UnitImage1").TryGetComponent<Image>(out unitImage1);
        GameObject.Find("UnitImage2").TryGetComponent<Image>(out unitImage2);
        GameObject.Find("UnitImage3").TryGetComponent<Image>(out unitImage3);
        GameObject.Find("UnitNameText").TryGetComponent<TextMeshProUGUI>(out unitNameText);
        GameObject.Find("UnitType").TryGetComponent<TextMeshProUGUI>(out unitTypeText);

        GameObject.Find("AttackPowerText").TryGetComponent<TextMeshProUGUI>(out unitAttackPowerText);
        GameObject.Find("AttackSpeedText").TryGetComponent<TextMeshProUGUI>(out unitAttackSpeedText);
        GameObject.Find("SkillNameText").TryGetComponent<TextMeshProUGUI>(out unitSkillNameText);
        GameObject.Find("SkillDescriptionText").TryGetComponent<TextMeshProUGUI>(out unitSkillDescriptionText);
        GameObject.Find("SkillIcon").TryGetComponent<Image>(out unitSkillIcon);

        GameObject.Find("WaveText").TryGetComponent<TextMeshProUGUI>(out waveText);
        GameObject.Find("TimeText").TryGetComponent<TextMeshProUGUI>(out timeText);
        GameObject.Find("StageDifficultyText").TryGetComponent<TextMeshProUGUI>(out stageDifficultyText);

        GameObject.Find("MonsterCountGage").TryGetComponent<Image>(out monsterCountGage);
        GameObject.Find("MonsterCountText").TryGetComponent<TextMeshProUGUI>(out monsterCountText);

        unitUpgradeMenu = GameObject.Find("UnitUpgradeMenu");
        GameObject.Find("GoldText2").TryGetComponent<TextMeshProUGUI>(out goldText2);
        GameObject.Find("JualText2").TryGetComponent<TextMeshProUGUI>(out jualText2);

        GameObject.Find("UnitUpgradeSlots").TryGetComponent<Transform>(out unitUpgradeSlots);


        unitSpawnMenu = GameObject.Find("UnitSpawnMenu");
        GameObject.Find("JualText3").TryGetComponent<TextMeshProUGUI>(out jualText3);
        GameObject.Find("UnitText2").TryGetComponent<TextMeshProUGUI>(out unitText2);

        GameObject.Find("UnitSpawnSlots").TryGetComponent<Transform>(out unitSpawnSlots);




        GameObject.Find("JualText").TryGetComponent<TextMeshProUGUI>(out jualText);
        GameObject.Find("GoldText").TryGetComponent<TextMeshProUGUI>(out goldText);
        GameObject.Find("UnitCountText").TryGetComponent<TextMeshProUGUI>(out unitText);
        GameObject.Find("UnitSpawnCostText").TryGetComponent<TextMeshProUGUI>(out unitSpawnCostText);

        GameObject.Find("LogContent").TryGetComponent<Transform>(out logTransform);


        dataLoading = GameObject.Find("DataLoading");
        waveAlert = GameObject.Find("WaveAlert");
        waveAlertText = waveAlert.transform.GetComponentInChildren<TextMeshProUGUI>();


        GameObject.Find("Slots").TryGetComponent<Transform>(out unitSlotTransform);
        unitSlot = unitSlotTransform.GetChild(0).gameObject;
        GameObject.Find("UnitManagerButtons").TryGetComponent<Transform>(out unitManageButtons);
        unitManageButtons.GetChild(0).TryGetComponent<Button>(out unitSellButton);
        unitManageButtons.GetChild(1).TryGetComponent<Button>(out unitInteractionButton);


        UnitGroup.OnClickUnitGround += SetUnitInformation;

        DataManager.OnDataLoad += SetDataLoadPannel;
        MonsterSpawnManager.OnChangeWave += SetWaveAlert;
        UnitSpawnManager.OnSpawnUnit += SetUnitText;
        UnitSpawnManager.OnChangeSpawnCost += SetUnitCostText;
        UnitSpawnManager.OnSpawnFail += ApplyFailEffectCostText;

        UnitSpawnManager.OnUnitSlotCreated += LinkUnitSlotUI;

        UnitManager.OnUpgradeFail += ApplyFailEffectCostText;

        WalletManager.OnChangeGold += SetGoldText;
        WalletManager.OnChangeJual += SetJualText;
    }

    public void Init()
    {
        var dataManager = DataManager.Instance;

        SetUserInformation(userAInformation, Resources.Load<Sprite>("Sprite/characterIcon"), dataManager.GetUserData(0).name);
        SetUserInformation(userBInformation, Resources.Load<Sprite>("Sprite/characterIcon"), dataManager.GetUserData(1).name);

        SetGameInformation();

        for (int i = 0; i < unitUpgradeSlots.childCount; i++)
        {
            var id = (i + 1) * 100000;
            var slotData = dataManager.GetUnitUpgradeData(id);
            var slot = unitUpgradeSlots.GetChild(i);

            if (slot.TryGetComponent<UpgradeSlot>(out var upgradeSlot))
                upgradeSlot.Init((UnitType)(i+1), slotData.target, slotData.imageSrc, slotData.level, slotData.costType, slotData.nextCost);
        }

        for(int i = 0; i < unitSpawnSlots.childCount; i++)
        {
            var id = (i + 1) * 100000;
            var slotData = dataManager.GetUnitSpawnData(id);
            var slot = unitSpawnSlots.GetChild(i);

            if (slot.TryGetComponent<SpawnSlot>(out var spawnSlot))
                spawnSlot.Init(slotData.imageSrc, slotData.spawnRatio, slotData.costType, slotData.cost);
        }



        MonsterSpawnManager.OnChangeWave += SetWaveInformation;
        MonsterSpawnManager.OnChangeMonsterCount += SetMonsterCount;

        TimeManager.OnSecondChange += SetTimeInformation;
    }

    
    private void SetGameInformation()
    {
        stageDifficultyText.text = "∫∏≈Î";
    }

    private void SetUserInformation(Transform transform, Sprite image, string name)
    {
        if(transform.Find("Icon").GetChild(1).TryGetComponent<Image>(out Image character))
            character.sprite = image;
        if(transform.Find("NickNameLabel").GetChild(0).TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI nameText))
            nameText.text = name;
    }

    private void SetWaveInformation(WaveData waveData)
    {
        waveText.text = waveData.name;
    }

    private void SetTimeInformation(int time)
    {
        timeText.text = $"{time / 60:D2}:{time%60:D2}";
    }

    private void SetMonsterCount(int count, int maxCount)
    {
        monsterCountText.text = $"{count}/{maxCount}";
        monsterCountGage.fillAmount = (float)count / maxCount;
    }

    private void SetGoldText(float value)
    {
        int nowGold = int.Parse(goldText.text);
        //goldText2.text = $"{value:F0}";
        //goldText.text = $"{value:F0}";
        LeanTween.value(goldText.gameObject,(v)=> { goldText.text = $"{v:F0}"; },nowGold, value, 0.3f);
        LeanTween.value(goldText2.gameObject, (v) => { goldText2.text = $"{v:F0}"; }, nowGold, value, 0.3f);

        if (value < GameManager.Instance.unitSpawnManager.SpawnCost)
            unitSpawnCostText.color = Color.red;
        else
            unitSpawnCostText.color = Color.white;
    }

    private void SetUnitInformation(UnitGroup slot, bool open)
    {
        if (open)
        {
            nowUnitSlot = slot;

            var list = slot.GetUnits;
            var sprite = list[0].GetComponentInChildren<SpriteRenderer>().sprite;
            unitImage1.sprite = sprite;
            unitImage2.sprite = sprite;
            unitImage3.sprite = sprite;
            if(list.Count == 1)
            {
                unitImage1.enabled = true;
                unitImage2.enabled = false;
                unitImage3.enabled = false;
            }
            if(list.Count == 2)
            {
                unitImage1.enabled = false;
                unitImage2.enabled = true;
                unitImage3.enabled = true;
            }
            if (list.Count == 3)
            {
                unitImage1.enabled = true;
                unitImage2.enabled = true;
                unitImage3.enabled = true;
            }

            unitNameText.text = list[0].OriginUnitData.name;
            unitTypeText.text = list[0].OriginUnitData.attackType;
            unitAttackPowerText.text = 
                $"{list[0].CurrentUnitData.attackPower * (1 + GameManager.Instance.unitManager.NormalUnitUpgradeData.reinforceRatio):F0}" +
                $"({list[0].CurrentUnitData.attackPower:F0} + " +
                $"{list[0].CurrentUnitData.attackPower * GameManager.Instance.unitManager.NormalUnitUpgradeData.reinforceRatio:F0})";
            unitAttackSpeedText.text = $"{list[0].CurrentUnitData.attackSpeed:F1}";
            unitSkillDescriptionText.text = list[0].OriginUnitData.name;
            unitSkillIcon.sprite = sprite;

            unitInformation.localScale = Vector3.one;


            SetUnitManageButtons(slot, true);
        }
        else
        {
            nowUnitSlot = null;

            unitInformation.localScale = Vector3.zero;
            SetUnitManageButtons(slot, false);
        }
    }

    private void SetJualText(float value)
    {
        int nowJual = int.Parse(jualText.text);

        jualText.text = $"{value:F0}";
        jualText2.text = $"{value:F0}";
        jualText3.text = $"{value:F0}";

        LeanTween.value(jualText.gameObject, (v) => { jualText.text = $"{v:F0}"; }, nowJual, value, 0.3f);
        LeanTween.value(jualText2.gameObject, (v) => { jualText2.text = $"{v:F0}"; }, nowJual, value, 0.3f);
        LeanTween.value(jualText3.gameObject, (v) => { jualText3.text = $"{v:F0}"; }, nowJual, value, 0.3f);
    }

    private void SetUnitText(int count, int maxCount)
    {
        unitText.text = $"{count}/{maxCount}";
        unitText2.text = $"{count}/{maxCount}";
    }

    private void SetUnitCostText(float value)
    {
        unitSpawnCostText.text = $"{value:F0}";
        
    }

    private void LinkUnitSlotUI(int id, UnitGroup slot)
    {
        var mapManager = GameManager.Instance.mapManager;

        var slotTouch = Instantiate(unitSlot, unitSlotTransform);
        slotTouch.transform.position = mapManager.UnitMapA()[id].position + new Vector2(0, 25f / 240);
        slotTouch.SetActive(true);

        slot.Init(id, slotTouch.GetComponent<UnitSlot>());
    }


    private void ApplyFailEffectCostText(FairReason reason)
    {
        switch(reason)
        {
            case FairReason.ShortMoney:
                LeanTween.value(goldText.gameObject, (color) => goldText.color = color, Color.white, Color.red, 0.15f);
                LeanTween.value(goldText.gameObject, (color) => goldText.color = color, Color.red, Color.white, 0.15f).setDelay(0.15f);

                LeanTween.value(goldText2.gameObject, (color) => goldText2.color = color, Color.white, Color.red, 0.15f);
                LeanTween.value(goldText2.gameObject, (color) => goldText2.color = color, Color.red, Color.white, 0.15f).setDelay(0.15f);
                break;
            case FairReason.ShortJual:
                LeanTween.value(jualText.gameObject, (color) => jualText.color = color, Color.white, Color.red, 0.15f);
                LeanTween.value(jualText.gameObject, (color) => jualText.color = color, Color.red, Color.white, 0.15f).setDelay(0.15f);

                LeanTween.value(jualText2.gameObject, (color) => jualText2.color = color, Color.white, Color.red, 0.15f);
                LeanTween.value(jualText2.gameObject, (color) => jualText2.color = color, Color.red, Color.white, 0.15f).setDelay(0.15f);

                LeanTween.value(jualText2.gameObject, (color) => jualText3.color = color, Color.white, Color.red, 0.15f);
                LeanTween.value(jualText3.gameObject, (color) => jualText3.color = color, Color.red, Color.white, 0.15f).setDelay(0.15f);
                break;
            case FairReason.FullUnit:
                LeanTween.value(unitText.gameObject, (color) => unitText.color = color, Color.white, Color.red, 0.15f);
                LeanTween.value(unitText.gameObject, (color) => unitText.color = color, Color.red, Color.white, 0.15f).setDelay(0.15f);

                LeanTween.value(unitText2.gameObject, (color) => unitText2.color = color, Color.white, Color.red, 0.15f);
                LeanTween.value(unitText2.gameObject, (color) => unitText2.color = color, Color.red, Color.white, 0.15f).setDelay(0.15f);
                break;
        }
    }

    private void SetDataLoadPannel(bool tf)
    {
        if(tf)
            dataLoading.transform.localScale = Vector3.one;
        else
            dataLoading.transform.localScale = Vector3.zero;
    }

    private void SetWaveAlert(WaveData waveData)
    {
        waveAlertText.text = waveData.name;
        LeanTween.scaleY(waveAlert, 1, 0.1f);
        LeanTween.scale(waveAlertText.gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeInOutBounce).setDelay(0.1f);
        LeanTween.scaleY(waveAlert, 0, 0.1f).setDelay(1.1f);
        waveAlertText.transform.localScale = Vector3.zero;
    }

    public void UnitUpgradeOpenBtn()
    {
        var scale = unitUpgradeMenu.transform.localScale;

        if (scale == Vector3.zero)
            LeanTween.scale(unitUpgradeMenu, Vector3.one, 0.2f).setEase(LeanTweenType.easeInOutBounce);
        else
            LeanTween.scale(unitUpgradeMenu, Vector3.zero, 0.2f).setEase(LeanTweenType.easeInOutBounce);
    }

    public void UnitSpawnOpenBtn()
    {
        var scale = unitSpawnMenu.transform.localScale;

        if (scale == Vector3.zero)
            LeanTween.scale(unitSpawnMenu, Vector3.one, 0.2f).setEase(LeanTweenType.easeInOutBounce);
        else
            LeanTween.scale(unitSpawnMenu, Vector3.zero, 0.2f).setEase(LeanTweenType.easeInOutBounce);
    }

    //unitManageButtons);
    //unitSellButton);
    //unitInteractionButton);
    private void SetUnitManageButtons(UnitGroup slot, bool tf)
    {
        if (tf)
        {
            unitSellButton.onClick.RemoveAllListeners();
            unitInteractionButton.onClick.RemoveAllListeners();

            unitManageButtons.transform.position = slot.transform.position;
            unitManageButtons.transform.localScale = Vector3.one;

            unitInteractionButton.gameObject.SetActive(slot.IsFull());

            unitSellButton.onClick.AddListener(slot.Sell);
            unitInteractionButton.onClick.AddListener(slot.Interaction);
        }
        else
        {
            unitManageButtons.transform.localScale = Vector3.zero;
            //unitSellButton.onClick.RemoveListener(slot.Sell);
            //unitInteractionButton.onClick.RemoveListener(slot.Interaction);
        }

    }


    //private void Update()
    //{
    //    if (!nowUnitSlot) return;

    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        nowUnitSlot.SetUIVisible(false);
    //    }

    //}
}
