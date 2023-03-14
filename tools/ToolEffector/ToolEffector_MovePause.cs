using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolEffector_MovePause : ToolEffector
{
    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

   
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        for (int i = 0; i < target.Length; i++)
        {
            FP temp = valueSource.Get(caster, target[i], buff, parms);
            caster.SendEvent<float>(nameof(PlayerEventName.setMovePauseInTime), temp.Convert<float>());
        }
       
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
