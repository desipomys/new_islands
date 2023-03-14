using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EyeLookFilter//注视的实体的过滤类型
{
    friend,enemy,netrual,
    player,
}
public class BaseEyes : MonoBehaviour,IEventRegister
{
    protected EventCenter center;
    public EyeLookFilter filter;
    public bool active;
    public virtual void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }

    public virtual EventCenter GetLookAt(EyeLookFilter filter)
    {
        return null;
    }
    public virtual EventCenter[] GetNotified(EyeLookFilter filter)
    {
        return null;
    }

    public virtual void OnEventRegist(EventCenter e)
    {
        center=e;
        //Debug.Log("s");
        //throw new System.NotImplementedException();
    }
}