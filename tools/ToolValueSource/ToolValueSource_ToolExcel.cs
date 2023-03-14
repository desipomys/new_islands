using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExcelType
{
    Ammo,Tool
}

public class ToolValueSource_ToolExcel : ValueSource_base
{
    public string[] indexs;
    FP[] temp;
    public ValueSource_base Copy()
    {
        throw new System.NotImplementedException();
    }

    public void FromObject(dynamic dy)
    {
        throw new System.NotImplementedException();
    }

    public FP[] Gets(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        
        if (temp == null)
        {
            Dictionary<string,FP> fps =null;
            fps=EventCenter.WorldCenter.GetParm<BaseTool, Dictionary<string, FP>>(nameof(EventNames.GetToolExcelData), ((ToolEngine)self).baseTool);
             
            FP[] newfp = new FP[indexs.Length];
            for (int i = 0; i < indexs.Length; i++)
            {
                newfp[i] = fps[indexs[i]];
            }
            temp = newfp;
        }
        return temp;
    }
    public FP Get(EventCenter caster, EventCenter target, object self, object[] parms)
    {
            Dictionary<string, FP> fps = EventCenter.WorldCenter.GetParm<BaseTool, Dictionary< string,FP > > (nameof(EventNames.GetToolExcelData), ((ToolEngine)self).baseTool);

        return new FP(fps);
    }

    public void init()
    {
        throw new System.NotImplementedException();
    }
}
