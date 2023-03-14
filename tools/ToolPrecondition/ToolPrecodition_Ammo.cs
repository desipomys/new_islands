using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPrecodition_Ammo : Precondition_base
{
    public override Precondition_base Copy()
    {
        throw new System.NotImplementedException();
    }

    public override void FromObject(dynamic dy)
    {
        throw new System.NotImplementedException();
    }
    //self=basetool
    public override bool Judge(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        BaseTool bt = (BaseTool)self;
        if (bt.toolItem.subid > 0) return true;
        return false;
    }
}
