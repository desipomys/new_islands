using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using System.Runtime.Serialization;
//using Newtonsoft.Json.Serialization;

/// <summary>
/// 通用的效果器,可以有状态
/// </summary>
//[JsonConverter(typeof(JsonConverter_EffecttorBase))]
public abstract class Effecttor_base
{
    //[另一种方案]可以存序列化数据
   // [JsonProperty]
   // [HideInInspector]
   // protected string typ{get{return this.GetType().Name;}set{}}

    //[LabelText("值来源")]
   // [Tooltip("有其他可配置参数时作废")]
    //[SerializeReference]
    //public ValueSource_base valueSource;//如果有多参数不同来源的需求请在子类自定义valueSource[]
    public abstract void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff);//object[]是事件到来的参数
    public abstract void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff);
   
    [System.Obsolete]
    public abstract Effecttor_base Copy();

    public virtual void OnInit(EventCenter self, object buff){}
    public virtual void UnInit(EventCenter self, object buff){}
}

/// <summary>
/// 
/// </summary>
public abstract class BuffEffector_base:Effecttor_base
{
    [LabelText("值来源")]
    [Tooltip("有其他可配置参数时作废")]
    [SerializeReference]
    public ValueSource_base valueSource;//如果有多参数不同来源的需求请在子类自定义valueSource[]
    public override Effecttor_base Copy(){return null;}
    public abstract BuffEffector_base BuffCopy();
     

}
