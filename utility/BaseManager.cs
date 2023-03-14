using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public class BaseManager : MonoBehaviour,IEventRegister
{
    protected EventCenter center;
    public virtual void OnEventRegist(EventCenter e)
    {
        center = e;
    }
    public virtual void AfterEventRegist()
    {

    }
}