using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour,IEventRegister
{
    protected EventCenter center;
    void Start()
    {
        
    }

    public virtual void OnEventRegist(EventCenter e)
    {
        //base.OnEventRegist(e);
        center=e;
    }

    public virtual void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }
}
