using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Sirenix.OdinInspector;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Runtime.Serialization;

public enum ValueSetMode
{
    directSet, SetViaBuff,AddValue
}

[System.Serializable]
public class BuffValueModify : BuffEffector_base
{
    [LabelText("设值方式")]
    [Tooltip("是通过buff设值还是直接设")]
    
    public ValueSetMode setMode;

    [LabelText("改值模式")]
    [Tooltip("是+*还是%模式")]
    
    public List<buffModifyMode> modes=new List<buffModifyMode>();

    [LabelText("值目标")]
    
    public List<BuffValueModifyTarget_Type> modifier=new List<BuffValueModifyTarget_Type>();
   // public BuffValueModifyTarget_base[] modifier;//传给buffcenter调用，不要自己调用
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        Modify(parms, caster, target,(BaseBuff)buff);
    }
    public virtual void Modify(object[] parms, EventCenter caster, EventCenter[] target,BaseBuff buff)
    {
        //if (modifier == null) Init();
        if (setMode == ValueSetMode.directSet)
        {
            for (int j = 0; j < target.Length; j++)
            {
                FP[] values=valueSource.Gets(caster,target[j],buff,parms);
                for (int i = 0; i < modifier.Count; i++)
                {
                    FP par = values[i];
                    //modifier[i].GetInstance().valueSetter(values[i], target[j]);
                    //Debug.Log("改后" + modifier[i].valueGetter(target[j]));
                     target[j].GetParm<BuffCenter>("buffCenter").ModifyOriginValue(buff.buffname,
                     modifier[i].GetInstance(),new BuffStaticValueModifyData(buff,par,modes[i]));
                }
            }
        }
        else if (setMode == ValueSetMode.SetViaBuff)
        {
            
            for (int j = 0; j < target.Length; j++)
            {
                FP[] values=valueSource.Gets(caster,target[j],buff,parms);
                for (int i = 0; i < modifier.Count; i++)
                {
                    
                    FP par = values[i];
                    //Debug.Log(target.Length+"a"+ modifier.Count+","+par.data.ToString()+" ");
                    //Debug.Log(modifier[i].GetInstance()==null?"null": modifier[i].GetInstance().GetType().Name);
                    target[j].GetParm<BuffCenter>("buffCenter").ModifyValue(buff.buffname,modifier[i].GetInstance(),new BuffStaticValueModifyData(buff,par,modes[i]));
                }
                
            }
        }
        if (setMode == ValueSetMode.AddValue)
        {
            for (int j = 0; j < target.Length; j++)
            {
                FP[] values=valueSource.Gets(caster,target[j],buff,parms);
                for (int i = 0; i < modifier.Count; i++)
                {
                   /* BuffValueModifyTarget_base vmt=modifier[i].GetInstance();
                    FP temp = vmt.valueGetter(target[j]);
                    temp.Add(values[i], vmt.valueType());
                    vmt.valueSetter(temp, target[j]);
                    Debug.Log("改后" + vmt.valueGetter(target[j]));*/
                    FP par = values[i];
                    target[j].GetParm<BuffCenter>("buffCenter").ModifyOriginValue(buff.buffname,
                     modifier[i].GetInstance(),new BuffStaticValueModifyData(buff,par,modes[i]));
                }
            }
        }

    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        //throw new System.NotImplementedException();
    }
    [OnDeserialized()]
    internal void AfterDeserialize(System.Runtime.Serialization.StreamingContext con)
    {Debug.Log(con.State);
    Debug.Log(con.Context);
    }

    /*public override void FromObject(dynamic dy)
    {
        setMode = (ValueSetMode)dy.setMode;
        //Debug.Log(dy.valueSource.GetType().Name);
        modes = ((JArray)dy.modes).ToObject<List<buffModifyMode>>();
        modifier= ((JArray)dy.modifier).ToObject<List<BuffValueModifyTarget_Type>>();
        valueSource.FromObject( ((JObject)dy.valueSource).ToObject<System.Dynamic.ExpandoObject>());
        /*Debug.Log("init" + modifyTarget_Bases[0]);
        Assembly assembly = this.GetType().Assembly;
        modifier = new BuffValueModifyTarget_base[modifyTarget_Bases.Count];
        for (int i = 0; i < modifyTarget_Bases.Count; i++)
        {
            modifier[i]= ((BuffValueModifyTarget_base)(assembly.CreateInstance(modifyTarget_Bases[i].ToString()))).GetInstance();
        }*/

    //}
    public override BuffEffector_base BuffCopy()
    {
        BuffValueModify temp = new BuffValueModify();
        temp.setMode = setMode;
        if(valueSource!=null)
            temp.valueSource = valueSource.Copy();
        temp.modes = new List<buffModifyMode>(modes);
        temp.modifier = new List<BuffValueModifyTarget_Type>(modifier);
        
        return temp;
    }
}
