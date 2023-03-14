using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class MovScriptFilter: TargetFilter
{
    //public abstract EventCenter[] Filter(EventCenter caster, EventCenter target,EventCenter[] source, object[] parms,object buff);//object[]是事件到来的参数
    //public abstract void FromObject(dynamic dy);
    public override TargetFilter Copy()
    {
        return null;
    }
    public abstract MovScriptFilter MovCopy();
}

public class MovScriptFilter_health:MovScriptFilter
{

    public override EventCenter[] Filter(EventCenter caster, EventCenter target, EventCenter[] source, object[] parms, object buff)
    {
        List<EventCenter> ans=new List<EventCenter>();
        for (int i = 0; i < source.Length; i++)
        {
            
            ans.Add(source[i]);
        }
        return ans.ToArray();
    }
    public override void FromObject(dynamic d)
    {

    }
     public override MovScriptFilter MovCopy()
     {
        return new MovScriptFilter_health();
     }
}
