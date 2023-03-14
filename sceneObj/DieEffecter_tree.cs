using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieEffecter_tree : MonoBehaviour,IEventRegister
{
    EventCenter evc;
    public void AfterEventRegist()
    {

    }

    public void OnEventRegist(EventCenter e)
    {
        e.ListenEvent<float, EventCenter, BaseTool, Damage>(nameof(PlayerEventName.onDie), onDie);
        evc = e;
    }
    void onDie(float f, EventCenter ev, BaseTool bt, Damage d)
    {
        //在container_eblock移除自己并播放树倒特效

        EventCenter.WorldCenter.SendEvent<long>(nameof(EventNames.RemoveEBlock), evc.UUID);
    }
}
