using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Interuptable Yield Class using conditional bool to check if Yield should be interupted. 
/// </summary>
public class InteruptableYieldInstruction : CustomYieldInstruction
{
    bool stopWait;

    public event Action<InteruptableYieldInstruction> OnKeepWaiting;

    public void Stop(bool conditional)
    {
        if (conditional == true)
        {
            stopWait = true;
        }
    }

    public override bool keepWaiting
    {
        get
        {
            if (stopWait == true)
            {
                return false;
            }
            if (OnKeepWaiting == null)
            {
                return true;
            }
            OnKeepWaiting(this);
            return stopWait == false;
        }
    }

}
