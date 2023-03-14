using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolValueSource_baseTool : ValueSource_base
{
    //public MovEngineCacheType type;
    public string[] Keys;
    public ValueSource_base Copy()
    {
        throw new System.NotImplementedException();
    }

    public void FromObject(dynamic dy)
    {
        throw new System.NotImplementedException();
    }

    public FP[] Gets(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        BaseTool bt = ((ToolEngine)self).baseTool;
        FP[] temp = new FP[Keys.Length];
        for (int i = 0; i < Keys.Length; i++)
        {
            temp[i] = bt.GetData(Keys[i]);
        }

        return temp;
    }
    public FP Get(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        BaseTool bt = ((ToolEngine)self).baseTool;
        return bt.GetData(Keys[0]);
    }

    public void init()
    {
        throw new System.NotImplementedException();
    }
}
