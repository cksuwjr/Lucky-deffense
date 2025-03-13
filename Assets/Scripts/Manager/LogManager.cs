using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogManager : SingletonDestroy<LogManager>
{
    private List<string> logs = new List<string>();

    public event Action<string> OnLogCreate;

    public void Log(string log)
    {
        logs.Add(log);
        OnLogCreate?.Invoke(log);
    }
}
