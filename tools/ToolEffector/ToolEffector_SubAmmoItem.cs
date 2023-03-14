using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolEffector_SubAmmoItem : Effecttor_base
{
    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

   

    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        ToolEngine te = (ToolEngine)buff;
        //throw new System.NotImplementedException();
        //根据当前subid、max弹夹确定扣多少弹药item
        //根据弹药轮转数组和当前弹药index确定扣哪些弹药
        //返回成功扣除的数量，添加到subid

        te.Next();
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
