using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovScriptEffector_Cache : MovScriptEffector
{
    public MovEngineCacheType type;
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter attacher, object self)//object[]是事件到来的参数
    {
        MovScriptEngine engine=((MovScriptEngine)self);
        FP TEMP=valueSource==null?new FP(parms[0]): valueSource.Get(caster,target[0],self,parms);
        engine.SetCache(type,TEMP);
    
    }
    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {

    }
    
    public override Effecttor_base Copy(){return new MovScriptEffector_Cache();}
}


