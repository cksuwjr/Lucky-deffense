using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviour
{
    private Transform shopMenu;
    private Transform heroMenu;
    private Transform fightMenu;
    private Transform treasureMenu;
    private Transform groupMenu;

    private void Awake()
    {
        var menu = GameObject.Find("Menu");

        menu.transform.GetChild(0)?.TryGetComponent<Transform>(out shopMenu);
        menu.transform.GetChild(1)?.TryGetComponent<Transform>(out heroMenu);
        menu.transform.GetChild(2)?.TryGetComponent<Transform>(out fightMenu);
        menu.transform.GetChild(3)?.TryGetComponent<Transform>(out treasureMenu);
        menu.transform.GetChild(4)?.TryGetComponent<Transform>(out groupMenu);

        Button btn = null;

        GameObject.Find("ShopSlot")?.transform.GetChild(0).TryGetComponent<Button>(out btn);
        btn?.onClick.AddListener(OnClickShopMenuSlot);
        GameObject.Find("HeroSlot")?.transform.GetChild(0).TryGetComponent<Button>(out btn);
        btn?.onClick.AddListener(OnClickHeroMenuSlot);
        GameObject.Find("FightSlot")?.transform.GetChild(0).TryGetComponent<Button>(out btn);
        btn?.onClick.AddListener(OnClickFightMenuSlot);
        GameObject.Find("TreasureSlot")?.transform.GetChild(0).TryGetComponent<Button>(out btn);
        btn?.onClick.AddListener(OnClickTreasureMenuSlot);
        GameObject.Find("GuildSlot")?.transform.GetChild(0).TryGetComponent<Button>(out btn);
        btn?.onClick.AddListener(OnClickGuildMenuSlot);

        GameObject.Find("StartButton")?.TryGetComponent<Button>(out btn);
        btn?.onClick.AddListener(OnClickStartButton);
    }



    private void OnClickShopMenuSlot()
    {
        shopMenu.gameObject.SetActive(true);
        heroMenu.gameObject.SetActive(false);
        fightMenu.gameObject.SetActive(false);
        treasureMenu.gameObject.SetActive(false);
        groupMenu.gameObject.SetActive(false);
    }

    private void OnClickHeroMenuSlot()
    {
        shopMenu.gameObject.SetActive(false);
        heroMenu.gameObject.SetActive(true);
        fightMenu.gameObject.SetActive(false);
        treasureMenu.gameObject.SetActive(false);
        groupMenu.gameObject.SetActive(false);
    }

    private void OnClickFightMenuSlot()
    {
        shopMenu.gameObject.SetActive(false);
        heroMenu.gameObject.SetActive(false);
        fightMenu.gameObject.SetActive(true);
        treasureMenu.gameObject.SetActive(false);
        groupMenu.gameObject.SetActive(false);
    }

    private void OnClickTreasureMenuSlot()
    {
        shopMenu.gameObject.SetActive(false);
        heroMenu.gameObject.SetActive(false);
        fightMenu.gameObject.SetActive(false);
        treasureMenu.gameObject.SetActive(true);
        groupMenu.gameObject.SetActive(false);
    }

    private void OnClickGuildMenuSlot()
    {
        shopMenu.gameObject.SetActive(false);
        heroMenu.gameObject.SetActive(false);
        fightMenu.gameObject.SetActive(false);
        treasureMenu.gameObject.SetActive(false);
        groupMenu.gameObject.SetActive(true);
    }

    private void OnClickStartButton()
    {
        LoadingSceneManager.SetNextScene("GameScene");
        SceneManager.LoadScene("LoadingScene");
    }
}