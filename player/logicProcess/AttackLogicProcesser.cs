using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLogicProcesser : MonoBehaviour, IEventRegister
{
    //接收工具发出的伤害单元、弹道表现单元，经过buff处理后再发出
    //工具上效果器发出自身数据到手，手再传给attacklogicprocessor，
    //再经过buff处理后返回手，手正式进行效果器的逻辑实现

    public void AfterEventRegist()
    {
        throw new System.NotImplementedException();
    }

    public void OnEventRegist(EventCenter e)
    {
        throw new System.NotImplementedException();
    }
}
