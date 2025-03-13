using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletManager : MonoBehaviour
{
    private float gold;
    private float jual;

    public float Gold { get => gold; set { gold = value; OnChangeGold?.Invoke(gold); } }
    public float Jual { get => jual; set { jual = value; OnChangeJual?.Invoke(jual); } }

    public static event Action<float> OnChangeGold;
    public static event Action<float> OnChangeJual;

    public void Init()
    {
        Gold = 70;
        Jual = 20;
    }
}
