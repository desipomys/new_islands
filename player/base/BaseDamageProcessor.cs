using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 逻辑组件，不携带数据
/// </summary>
public class BaseDamageProcessor : MonoBehaviour,IEventRegister
{
     EventCenter center;
    public void OnEventRegist(EventCenter e)
    {
        center=e;
        e.ListenEvent<Damage,EventCenter,BaseTool>(nameof(PlayerEventName.onDamage),OnDamage);//默认优先级0，buff减伤可以用大于0的优先级以提前接收damage并修改damage
 
    }
    public void AfterEventRegist()
    {

    }
    public void OnDamage(Damage d,EventCenter source,BaseTool tool)
    {//传入damage在运算过程中不许修改
       float f= DamageLogic.ApplyDamage(d,source,tool,center);
       //HealthChange(f,source,tool,d);//发送healchg事件
        if(d.IsContainBuff())
            DamageLogic.ApplyBuff((BaseBuff[])d.exd["buff"],source,center);
    }
    void OnDestory()
    {

    }
}
