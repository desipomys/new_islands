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
    
   
    //public modeɢ�ʱ仯ģ�ͣ�ֱ�Ӽӡ�lerp 
    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

    
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        
        ToolEngine bt = (ToolEngine)buff;
        float temp = bt.baseTool.GetData<float>(BaseToolDataName.spread);//��ǰɢ��
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
    
  
    //public modeɢ�ʱ仯ģ�ͣ�ֱ�Ӽӡ�lerp 
    public SpreadDownMode mode;
    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

    

    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        
        ToolEngine bt = (ToolEngine)buff;
        float temp = bt.baseTool.GetData<float>(nameof(BaseToolDataName.spread));//��ǰɢ��
        //float MinSpread = bt.baseTool.GetData<float>("MinSpread");
        FP[] fps=valueSource.Gets(caster, caster, buff, parms);
        float values = fps[0];//ɢ��ÿ���½�
        float MinSpread = fps[1];//��Сɢ��

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
