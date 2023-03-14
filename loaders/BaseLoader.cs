
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// loader可以自己去读取存取文件
/// </summary>
public class BaseLoader : IEventRegister
{
   protected EventCenter center;

    public void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }

    public virtual void OnEventRegist(EventCenter e)
    {
       center=e;
       e.ListenEvent<int>("loaderInit",OnLoaderInit);
    }
    /// <summary>
    /// 先于oneventreg
    /// </summary>
    /// <param name="prio"></param>
    public virtual void OnLoaderInit(int prio)//loader要按顺序加载,先于oneventreg,且先于摄像机和UI的init
    {

    }

}
