using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ToolEffector : Effecttor_base
{
    [LabelText("参数")]
    [SerializeReference]
    public ValueSource_base valueSource;//如果有多参数不同来源的需求请在子类自定义valueSource[]
    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="parms"></param>
   /// <param name="caster"></param>
   /// <param name="target"></param>
   /// <param name="self"></param>
   /// <param name="buff">toolengine</param>
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        //需要参数：位置、形状、大小
      
        //parm.pos=datas[0]
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
