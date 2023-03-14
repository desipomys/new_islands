using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public class EntityDamageEventArg:BaseEventArg
{
    public ValueChangeParm<float> heal;
    public EventCenter source,target;
    public BaseTool tool;
    public Damage dam;
    public EntityDamageEventArg(){}
    public EntityDamageEventArg(ValueChangeParm<float> h,EventCenter s,EventCenter t,BaseTool tool,Damage d)
    {
        heal=h;
        source=s;
        target=t;
        this.tool=tool;
        dam=d;
    }

    public void Set(ValueChangeParm<float> h,EventCenter s,EventCenter t,BaseTool tool,Damage d)
    {
        heal=h;
        source=s;
        target=t;
        this.tool=tool;
        dam=d;
    }
}