using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class BaseEBlockHealth : MonoBehaviour,IEventRegister,IDataContainer
{
    public float health, maxHealth;
    protected bool isDead =false;
    protected EventCenter center;
    public virtual void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }

    public virtual void OnEventRegist(EventCenter e)
    {
        e.ListenEvent<Damage, EventCenter, BaseTool>(nameof(PlayerEventName.onDamage), OnDamage);
        e.ListenEvent<ValueChangeParm<float>>("processGo", OnProcessGo);
        e.ListenEvent("StartBuildInited", StartBuildInited);
        center = e;
        //throw new System.NotImplementedException();
    }

    
    protected ValueChangeParm<float> hparm = new ValueChangeParm<float>();

    public virtual int GetDataCollectPrio => 0;

    // Start is called before the first frame update
    public virtual void OnDamage(Damage d, EventCenter source, BaseTool tool)
    {//传入damage在运算过程中不许修改
        ChgHealth(d.value);
        if(health<=0) center.SendEvent<float, EventCenter, BaseTool, Damage>(nameof(PlayerEventName.onDie), d.value, source, tool, d);
    }
    public virtual void ChgHealth(float f)
    {
        health = Mathf.Clamp(health + f, 0, maxHealth);

    }
    public virtual void StartBuildInited()
    {
        health = 0;
    }
    public virtual void OnProcessGo(ValueChangeParm<float> f)
    {
        ChgHealth((f.now - f.old) * maxHealth);
    }

    public virtual void FromObject(object data)
    {
        JArray j = (JArray)data;
        health = j[0].ToObject<float>();
        maxHealth = j[1].ToObject<float>();
    }

    public virtual object ToObject()
    {
        return new float[] { health, maxHealth };
    }
}
