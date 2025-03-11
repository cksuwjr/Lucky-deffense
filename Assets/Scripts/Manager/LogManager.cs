using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogManager : SingletonDestroy<LogManager>
{
    private List<string> logs;

    public void Log(string log)
    {
        logs.Add(log);
    }
}
