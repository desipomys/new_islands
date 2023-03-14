using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseToucher : MonoBehaviour,IEventRegister
{
    protected EventCenter center;
    public virtual void AfterEventRegist()
    {
        
    }

    public virtual void OnEventRegist(EventCenter e)
    {
        center = e;
    }

    
}
