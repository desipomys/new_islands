using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Newtonsoft.Json;

/// <summary>
/// 实现类可带变量
/// </summary>
public interface ValueSource_base
{
   // [JsonProperty]
    //string typ{get{return this.GetType().Name;} set{}}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="caster">施放、调用者</param>
    /// <param name="target">目标</param>
    /// <param name="self">所在的数据类</param>
    /// <param name="parms">事件参数</param>
    /// <returns></returns>
      FP[] Gets( EventCenter caster, EventCenter target, object self,object[] parms);
      FP Get( EventCenter caster, EventCenter target, object self,object[] parms);

     void init();
     ValueSource_base Copy();
     void FromObject(dynamic dy);
}

[Serializable]

/// <summary>
/// buff值来源，可能是从excel来也可能是直接值，一个即可服务于一个诸如buffeffector的组件
/// </summary>
public abstract class BuffValueSource_base :ValueSource_base
{
    //public BuffValueSourceKeys[] keys;
    
    public abstract FP[] Gets( EventCenter caster, EventCenter target, object self,object[] parms);
    public abstract FP Get( EventCenter caster, EventCenter target, object self,object[] parms);
 
    public abstract void init();
    public ValueSource_base Copy(){return null;}
    public abstract BuffValueSource_base BuffCopy();
    public abstract void FromObject(dynamic dy);
}
public enum BuffValueSourceKeys
{
    VM_1,VM_2,VM_3,//ValueModify

}