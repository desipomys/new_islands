using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolEffector_ToolDataAdd_int : Effecttor_base
{
    public string keys;
    public int value;

    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

   
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        BaseTool bt = ((ToolEngine)buff).baseTool;
        bt.GetData(keys).data = bt.GetData<int>(keys) + value;
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
public class ToolEffector_ToolDataAdd_float : Effecttor_base
{
    public string keys;
    public float value;

    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

  
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        BaseTool bt = ((ToolEngine)buff).baseTool;
        bt.GetData(keys).data = bt.GetData<float>(keys) + value;
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
