using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 取buff内的数据
/// </summary>
public class BuffValueSource_Buff : ValueSource_base
{
    public int index;
    public FP[] Gets(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        return new FP[] { ((BaseBuff)self).BuffDatas[index] };
    }
    public FP Get(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        return ((BaseBuff)self).BuffDatas[index] ;
    }

    public void init()
    {
        
    }

    public ValueSource_base Copy()
    {
        throw new System.NotImplementedException();
    }

    public void FromObject(dynamic dy)
    {
        throw new System.NotImplementedException();
    }
}
