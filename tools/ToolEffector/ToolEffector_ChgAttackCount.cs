using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 改变连击次数
/// </summary>
public class ToolEffector_ChgAttackCount : ToolEffector
{
    public int num;
    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

    

    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        ToolEngine bt = (ToolEngine)buff;
        int temp = bt.baseTool.GetData<int>(BaseToolDataName.attCount);
        int values = valueSource.Get(caster, caster, buff, parms).Convert<int>();
        bt.baseTool.SetData(BaseToolDataName.attCount,new FP(temp+values));

    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
