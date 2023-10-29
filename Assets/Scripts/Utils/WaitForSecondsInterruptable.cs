using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// Same use as WaitForSeconds but with ability for bool to interupt
/// </summary>
public class WaitForSecondsInterruptable : InteruptableYieldInstruction
{
    public WaitForSecondsInterruptable(float seconds)
    {
        var startTime = Time.time;
        OnKeepWaiting += i => i.Stop(Time.time - startTime >= seconds);
    }
}
