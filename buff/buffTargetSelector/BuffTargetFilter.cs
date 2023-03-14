using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum compareMode
{
    Greater,Smaller,Equal,
    NotGreater,NotSmaller,NotEqual
}

public abstract class TargetFilter
{
     [LabelText("值来源")]
    [SerializeReference]
    [LabelWidth(45)]
    public ValueSource_base valueSource;

    public abstract EventCenter[] Filter(EventCenter caster, EventCenter target,EventCenter[] source, object[] parms,object buff);//object[]是事件到来的参数
    public abstract void FromObject(dynamic dy);
    public abstract TargetFilter Copy();
}

public abstract class BuffTargetFilter:TargetFilter
{
    [LabelText("值来源")]
    [SerializeReference]
    [LabelWidth(45)]
    public new BuffValueSource_base valueSource;
    //public abstract EventCenter[] Filter(EventCenter caster, EventCenter target,EventCenter[] source, object[] parms,object buff);//object[]是事件到来的参数
    //public abstract void FromObject(dynamic dy);
    public virtual void OnBuffInit(EventCenter holder,BaseBuff bf)
    {
        
    }
    public override TargetFilter Copy()
    {
        return null;
    }
    public abstract BuffTargetFilter BuffCopy();
}
