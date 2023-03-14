using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Newtonsoft.Json;

/// <summary>
/// ʵ����ɴ�����
/// </summary>
public interface ValueSource_base
{
   // [JsonProperty]
    //string typ{get{return this.GetType().Name;} set{}}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="caster">ʩ�š�������</param>
    /// <param name="target">Ŀ��</param>
    /// <param name="self">���ڵ�������</param>
    /// <param name="parms">�¼�����</param>
    /// <returns></returns>
      FP[] Gets( EventCenter caster, EventCenter target, object self,object[] parms);
      FP Get( EventCenter caster, EventCenter target, object self,object[] parms);

     void init();
     ValueSource_base Copy();
     void FromObject(dynamic dy);
}

[Serializable]

/// <summary>
/// buffֵ��Դ�������Ǵ�excel��Ҳ������ֱ��ֵ��һ�����ɷ�����һ������buffeffector�����
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