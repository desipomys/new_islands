using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MovScriptEffector : Effecttor_base
{
    [LabelText("值来源")]
    [Tooltip("有其他可配置参数时作废")]
    [SerializeReference]
    public ValueSource_base valueSource;//如果有多参数不同来源的需求请在子类自定义valueSource[]
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        
    }
    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        
    }
    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }
    
}
