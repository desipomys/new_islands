using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffector_EventSender : BuffEffector_base
{
    public int s;

    

    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
    public override BuffEffector_base BuffCopy()
    {
        return new BuffEffector_EventSender();
    }
}
