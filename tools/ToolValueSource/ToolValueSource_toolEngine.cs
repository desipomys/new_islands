using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolValueSource_toolEngine : ValueSource_base
{
    public MovEngineCacheType type;
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
        switch (type)
        {
            case MovEngineCacheType.AX:
                return new FP[] { new FP(((ToolEngine)self).cache[0]) };
                break;
            case MovEngineCacheType.BX:
                return new FP[] { new FP(((ToolEngine)self).cache[1]) };
                break;
            case MovEngineCacheType.CX:
                return new FP[] { new FP(((ToolEngine)self).cache[2]) };
                break;
            case MovEngineCacheType.DX:
                return new FP[] { new FP(((ToolEngine)self).cache[3]) };
                break;
            case MovEngineCacheType.ans:
                return new FP[] { new FP(((ToolEngine)self).cache[0]) };
                break;
            default:
                return null;
                break;
        }
    }
    public FP Get(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        switch (type)
        {
            case MovEngineCacheType.AX:
                return new FP(((ToolEngine)self).cache[0]) ;
                break;
            case MovEngineCacheType.BX:
                return new FP(((ToolEngine)self).cache[1]) ;
                break;
            case MovEngineCacheType.CX:
                return new FP(((ToolEngine)self).cache[2]) ;
                break;
            case MovEngineCacheType.DX:
                return new FP(((ToolEngine)self).cache[3]) ;
                break;
            case MovEngineCacheType.ans:
                return new FP(((ToolEngine)self).cache[0]) ;
                break;
            default:
                return null;
                break;
        }
    }

    public void init()
    {
        throw new System.NotImplementedException();
    }
}
