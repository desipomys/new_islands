using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolEffector_Subsubid : ToolEffector
{
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        int decrese = valueSource.Get(caster, target[0], buff, parms);
        BaseTool bt = ((ToolEngine)buff).baseTool;

        Item it = bt.toolItem;
        it.subid -= decrese;
        bt.ChgItem(it);
    }
}
