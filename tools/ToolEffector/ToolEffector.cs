using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ToolEffector : Effecttor_base
{
    [LabelText("����")]
    [SerializeReference]
    public ValueSource_base valueSource;//����ж������ͬ��Դ���������������Զ���valueSource[]
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
        //��Ҫ������λ�á���״����С
      
        //parm.pos=datas[0]
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
