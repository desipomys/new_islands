using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuilder : MonoBehaviour,IEventRegister
{
    protected EventCenter center;
    public void AfterEventRegist()
    {
       
    }

    public void OnEventRegist(EventCenter e)
    {
        center = e;
    }
    public virtual int GetBuildLevel()
    {
        return center.GetParm<int>(nameof(PlayerEventName.skill_build));
    }
    //
    //Ҫ���ܽ��콨�������ʵ�巽��
    public virtual bool BuildBBlockAt(string typ,int pos)
    {

        return false;
    }
    public virtual bool BuildEBlockAt(string typ, int pos)
    {

        return false;
    }

}
