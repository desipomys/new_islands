using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuffSender : BuffEffector_base
{
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
        BuffSender temp = new BuffSender();
       
        temp.valueSource = valueSource;
      
        return temp;
    }

}
