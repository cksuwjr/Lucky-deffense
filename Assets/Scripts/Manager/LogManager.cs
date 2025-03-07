using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    private List<string> logs;

    public void Log(string log)
    {
        logs.Add(log);
    }
}
