using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ToolEffector_CastDamage : ToolEffector
{
    
    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

   
    AOEParm parm=new AOEParm();
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        ToolEngine eng=(ToolEngine) buff;
        BaseTool tool=eng.baseTool;
        Hand hand=tool.hand;
        Item toolItem=tool.toolItem;
        Vector3 firePos=hand.GetHoldingToolPosi(ToolPosition.silencePos);
        Vector3 targetPos=EventCenter.WorldCenter.GetParm<Vector3>(nameof(EventNames.GetMouseLookAt));

        Dictionary<string, FP> toolexcel = (Dictionary<string, FP>)(valueSource.Get(caster, target[0], buff, new object[] { toolItem }).data);

        float damage = 0;
        float damAdd = 0;
        float damPercent = 0;
        float crit = 0;
        float CritAdd = 0;

        string effectType=toolexcel["特效预制名"];
        DamageType dt = toolexcel["伤害类型"].Convert<DamageType>();
        ToolType tt=toolexcel["工具类型"].Convert<ToolType>(); 
        float size = toolexcel["形状大小"];
        Shape3D shape= toolexcel["形状"].Convert<Shape3D>(); ;


        Dictionary<string, object> exd = new Dictionary<string, object>();
        Dictionary<ItemContent, object> toolExd = toolItem.exd;
        if (toolExd != null)
            foreach (var item in toolExd)
            {
                exd.Add(item.Key.ToString(), item.Value);
            }
        FP temp = hand.GetBuffedValue(HandDataName.damBuff);
        if (temp != null)
        {
            exd.Add(nameof(HandDataName.damBuff), temp);
        }

        parm.dam.value = damage;
        parm.dam.AdditionV = damAdd;
        parm.dam.AdditionPercent = damPercent;
        parm.dam.CritPercent = crit;
        parm.dam.CritAdd = CritAdd;

        parm.dam.type = dt;
        parm.shape = shape;
        parm.time = 0;
        parm.shapeArgs = new float[] { size };
        
        parm.tool = tool;
        parm.parms = exd;
        parm.pos = firePos;
        parm.targetPos = targetPos;

        //parm.pos=datas[0]
        EventCenter.WorldCenter.SendEvent<AOEParm>(nameof(EventNames.CastDamage), parm);
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
