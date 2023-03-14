using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawner : MonoBehaviour, IEventRegister
{
    public float respawnWait;
    Ticker tick ;
    bool isdead=false;
    EventCenter center;

    /// <summary>
    /// 向worldcenter发送的伤害或死亡事件参数
    /// </summary>
    /// <returns></returns>
    EntityDamageEventArg respawnEvent=new EntityDamageEventArg();


    public void AfterEventRegist()
    {
        
    }

    public void OnEventRegist(EventCenter e)
    {
        center = e;
        tick = new Ticker(respawnWait);
        e.ListenEvent<float, EventCenter, BaseTool, Damage>(nameof(PlayerEventName.onDie), onDie);
    }
    void onDie(float f, EventCenter evc, BaseTool btd, Damage d)
    {
        tick.ReTick();
        isdead = true;
    }
    public void Update()
    {
        if(isdead)
        {
            if(tick.IsReady())
            {
                respawn();
                isdead = false;
            }
        }
    }
    void respawn()
    {
        Vector3 spawnpoint = EventCenter.WorldCenter.GetParm<Vector3>(nameof(EventNames.GetDefaultSpawnPoint));
        transform.position = spawnpoint;
        center.SendEvent(nameof(PlayerEventName.onRespawn));

        respawnEvent.Set(null,null,center,null,null);
        EventCenter.WorldCenter.SendEvent<EntityDamageEventArg>(nameof(PlayerEventName.onRespawn), respawnEvent);
    }
}
