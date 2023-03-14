using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum SpreadDownMode
{
    sub,lerp
}

public class ToolEffector_AddSpread : ToolEffector
{
    
   
    //public mode散率变化模型：直接加、lerp 
    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

    
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        
        ToolEngine bt = (ToolEngine)buff;
        float temp = bt.baseTool.GetData<float>(BaseToolDataName.spread);//当前散率
        //float MinSpread = bt.baseTool.GetData<float>("MinSpread");
        FP fps=valueSource.Get(caster, caster, buff, parms);
        temp+=fps;
        bt.baseTool.SetData(BaseToolDataName.spread, temp);

    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}

public class ToolEffector_DownSpread : ToolEffector
{
    
  
    //public mode散率变化模型：直接加、lerp 
    public SpreadDownMode mode;
    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

    

    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        
        ToolEngine bt = (ToolEngine)buff;
        float temp = bt.baseTool.GetData<float>(nameof(BaseToolDataName.spread));//当前散率
        //float MinSpread = bt.baseTool.GetData<float>("MinSpread");
        FP[] fps=valueSource.Gets(caster, caster, buff, parms);
        float values = fps[0];//散率每秒下降
        float MinSpread = fps[1];//最小散率

        switch (mode)
        {
            case SpreadDownMode.sub:
                bt.baseTool.SetData(nameof(BaseToolDataName.spread), new FP(temp - values));
                break;
            case SpreadDownMode.lerp:
                bt.baseTool.SetData(nameof(BaseToolDataName.spread), new FP(Mathf.Lerp(temp, MinSpread,values*Time.deltaTime)));
                break;
            default:
                break;
        }
        
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
