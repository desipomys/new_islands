using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BuffValueSource_direct : BuffValueSource_base
{
    [SerializeReference]
    public FP data;

    public override BuffValueSource_base BuffCopy()
    {
        BuffValueSource_direct temp = new BuffValueSource_direct();
        temp.data = DataExtension.DeepCopy(data);
        return temp;
    }

    public override FP[] Gets(EventCenter caster, EventCenter target, object self,object[] parms)
    {
        return new FP[] { new FP(this.data) };
    }    
    public override FP Get(EventCenter caster, EventCenter target, object self,object[] parms)
    {
        return this.data ;
    }
    public override void init()
    {
        
    }
    public override void FromObject(dynamic dy)
    {
        //Debug.Log(dy.data.GetType().Name);
        //data可能是int,str,str[],item,item[],dic<str,item>等
        //dy.data可能是int64,double,str,jarray,jobject
        data = dy.data;
    }

}
[Serializable]
public class BuffValueSource_directArr : BuffValueSource_base
{
    public FP[] data;
    public override FP[] Gets(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        return  this.data ;
    }
    public override FP Get(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        return  this.data[0] ;
    }

    public override void init()
    {

    }
    public override BuffValueSource_base BuffCopy()
    {
        BuffValueSource_directArr temp = new BuffValueSource_directArr();
        if (data != null)
        {
            temp.data = new FP[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                temp.data[i] = new FP(DataExtension.DeepCopy(data[i].data));
            }
        }
        return temp;
    }
    public override void FromObject(dynamic dy)
    {

    }
}