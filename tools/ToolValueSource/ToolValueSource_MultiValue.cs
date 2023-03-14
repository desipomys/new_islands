using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 不能用
/// </summary>
public class ToolValueSource_MultiValue : ValueSource_base
{
    public ValueSource_base[] values;

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
        FP[] ans = new FP[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            ans[i] = values[i].Get(caster, target, self, parms);
        }
        return ans;
    }
    public FP Get(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        return values[0].Get(caster, target, self, parms);
    }

    public void init()
    {
        throw new System.NotImplementedException();
    }
}

/// <summary>
/// 不能用
/// </summary>
public class ToolValueSource_HandModify : ValueSource_base
{
    public string[] name;//hand中修正值的名称

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
        
        return null;
    }
     public FP Get(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        
        return null;
    }

    public void init()
    {
        throw new System.NotImplementedException();
    }
}