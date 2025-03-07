using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YieldInstructionCache
{
    private static Dictionary<float, WaitForSeconds> waitForSeconds = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds WaitForSeconds(float time)
    {
        if(!waitForSeconds.TryGetValue(time, out WaitForSeconds result))
            waitForSeconds.Add(time, result = new WaitForSeconds(time));
        return result;
    }
}
