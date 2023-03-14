using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 可以有内部数据，但必须是简单类型的
/// </summary>
public abstract class Precondition_base
{
    [LabelText("值来源")]
    
    [SerializeReference]
    public ValueSource_base valueSource;
    public abstract bool Judge(EventCenter caster, EventCenter target, object self, object[] parms);
    
    public abstract void FromObject(dynamic dy);
    public abstract Precondition_base Copy();

    
}

public abstract class buffPrecondition_base :Precondition_base
{
    /*[LabelText("值来源")]
    
    [SerializeReference]
    public new BuffValueSource_base valueSource;*/
    public override Precondition_base Copy(){return null;}
    public abstract buffPrecondition_base BuffCopy();
    public virtual void OnBuffInit(EventCenter holder,BaseBuff bf){}
}

/// <summary>
/// target血量大于caster
/// </summary>
public class buffPrecondition_health : buffPrecondition_base
{
    public int a;

    public override buffPrecondition_base BuffCopy()
    {
        buffPrecondition_health temp = new buffPrecondition_health();
        temp.a = a;
        return temp;
    }

    public override void FromObject(dynamic dy)
    {
        a = dy.a;
    }

    public override bool Judge(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        return true;
    }
}