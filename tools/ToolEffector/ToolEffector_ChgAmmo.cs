using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolEffector_ChgAmmo : ToolEffector
{
    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

   

    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        FP temp = valueSource.Gets(caster, caster, buff, parms)[0];
        ToolEngine bt=(ToolEngine)buff;
        Item ite = bt.baseTool.toolItem;
        ite.subid += temp.Convert<int>();
        bt.baseTool.ChgItem(ite);
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
