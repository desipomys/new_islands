using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLogicProcesser : MonoBehaviour, IEventRegister
{
    //受击伤害处理流程，先经过buff处理再交给charEntity
    //接收ondamage事件
    public void AfterEventRegist()
    {
        throw new System.NotImplementedException();
    }

    public void OnEventRegist(EventCenter e)
    {
        throw new System.NotImplementedException();
    }


}
