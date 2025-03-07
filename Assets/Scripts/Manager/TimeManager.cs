using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonDestroy<TimeManager>
{
    private int timer;
    public static event Action<int> OnSecondChange;

    public void Init()
    {
        StartCoroutine("Timer");
    }

    private IEnumerator Timer()
    {
        OnSecondChange?.Invoke(timer);

        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(1f);
            OnSecondChange?.Invoke(++timer);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Time.timeScale = 1f;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            Time.timeScale = 2f;

        if (Input.GetKeyDown(KeyCode.Alpha3))
            Time.timeScale = 3f;

        if (Input.GetKeyDown(KeyCode.Alpha4))
            Time.timeScale = 4f;

        if (Input.GetKeyDown(KeyCode.Alpha5))
            Time.timeScale = 5f;

        if (Input.GetKeyDown(KeyCode.Alpha6))
            Time.timeScale = 6f;

        if (Input.GetKeyDown(KeyCode.Alpha7))
            Time.timeScale = 7f;

        if (Input.GetKeyDown(KeyCode.Alpha8))
            Time.timeScale = 8f;

        if (Input.GetKeyDown(KeyCode.Alpha9))
            Time.timeScale = 9f;

        if (Input.GetKeyDown(KeyCode.G))
            GameManager.Instance.walletManager.Gold += 1000;
    }
}
