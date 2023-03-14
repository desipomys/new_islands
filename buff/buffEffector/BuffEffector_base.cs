using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using System.Runtime.Serialization;
//using Newtonsoft.Json.Serialization;

/// <summary>
/// ͨ�õ�Ч����,������״̬
/// </summary>
//[JsonConverter(typeof(JsonConverter_EffecttorBase))]
public abstract class Effecttor_base
{
    //[��һ�ַ���]���Դ����л�����
   // [JsonProperty]
   // [HideInInspector]
   // protected string typ{get{return this.GetType().Name;}set{}}

    //[LabelText("ֵ��Դ")]
   // [Tooltip("�����������ò���ʱ����")]
    //[SerializeReference]
    //public ValueSource_base valueSource;//����ж������ͬ��Դ���������������Զ���valueSource[]
    public abstract void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff);//object[]���¼������Ĳ���
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
    [LabelText("ֵ��Դ")]
    [Tooltip("�����������ò���ʱ����")]
    [SerializeReference]
    public ValueSource_base valueSource;//����ж������ͬ��Դ���������������Զ���valueSource[]
    public override Effecttor_base Copy(){return null;}
    public abstract BuffEffector_base BuffCopy();
     

}
