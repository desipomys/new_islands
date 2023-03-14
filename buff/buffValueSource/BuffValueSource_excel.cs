using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffValueSource_excel : BuffValueSource_base
{
    [SerializeField]
    public int[] indexs;

    FP[] cacheData;

    public override BuffValueSource_base BuffCopy()
    {
        BuffValueSource_excel temp = new BuffValueSource_excel();
        temp.indexs = (int[])indexs.Clone();
        return temp; 
    }

    public override void FromObject(dynamic dy)
    {
        indexs = dy.indexs;
    }

    public override FP[] Gets(EventCenter caster, EventCenter target, object self,object[] parms)
    {
        if(cacheData==null)
        {
            List<FP> ans = new List<FP>();
        FP[] TEMP = EventCenter.WorldCenter.GetParm<BaseBuff, FP[]>("GetBuffExcelData", (BaseBuff)self);
        for (int i = 0; i < indexs.Length; i++)
        {
            ans.Add(TEMP[indexs[i]]);
        }
        cacheData=ans.ToArray();
        }
        return cacheData;
    }

     public override FP Get(EventCenter caster, EventCenter target, object self,object[] parms)
    {
         if(cacheData==null)
        {
        FP[] TEMP = EventCenter.WorldCenter.GetParm<BaseBuff, FP[]>("GetBuffExcelData", (BaseBuff)self);
        cacheData=new FP[]{TEMP[indexs[0]]};
        }
        return cacheData[0];
    }
    public override void init()
    {
        
    }
}
