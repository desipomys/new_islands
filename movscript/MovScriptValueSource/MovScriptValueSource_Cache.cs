using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovEngineCacheType
{
    AX,BX,CX,DX,ans
}

public class MovScriptValueSource_Cache : ValueSource_base
{
    public MovEngineCacheType type;
    public  FP[] Gets( EventCenter caster, EventCenter target, object self,object[] parms)
    {
       MovScriptEngine e=((MovScriptEngine)self);
      return new FP[]{e.GetCache(type)};
    }
    public  FP Get( EventCenter caster, EventCenter target, object self,object[] parms)
    {
        MovScriptEngine e=((MovScriptEngine)self);
      return e.GetCache(type);
    }
    public  void init()
    {

    }
    public  ValueSource_base Copy()
    {
        return new MovScriptValueSource_Cache();
    }
    public  void FromObject(dynamic dy)
    {

    }
}
/// <summary>
/// 给console用的设引擎寄存器值的值来源
/// </summary>
public class MovScriptValueSource_Cache_STR:ValueSource_base
{
    public  FP[] Gets( EventCenter caster, EventCenter target, object self,object[] parms)
    {
        MovEngineCacheType type=(MovEngineCacheType)parms[0];
       MovScriptEngine e=((MovScriptEngine)self);
      return new FP[]{e.GetCache(type)};
    }
    public  FP Get( EventCenter caster, EventCenter target, object self,object[] parms)
    {
        MovEngineCacheType type=(MovEngineCacheType)parms[0];
       MovScriptEngine e=((MovScriptEngine)self);
      return e.GetCache(type);
    }
    public  void init()
    {

    }
    public  ValueSource_base Copy()
    {
        return new MovScriptValueSource_Cache();
    }
    public  void FromObject(dynamic dy)
    {

    }
}
