using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;


/// <summary>
/// tool的前摇效果器，不能和其他效果器一起放一个toolnode节点上
/// </summary>
public class ToolEffector_Wait : ToolEffector
{
    //[JsonIgnore]

   public float runedTime = 0;//{ get { return tool.GetData("runedTime"); }set { } }
                              //[NonSerialized]
                              //[JsonIgnore]
    public bool HadBreak = false;

    //[NonSerialized]
    //[JsonIgnore]
    public string BreakEventName;
    //[NonSerialized]
    //[JsonIgnore]
    public int BreakLevel;

    public Effecttor_base Breaked;

    bool inited = false;

    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

   
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)//buff=toolengine
    {
        ToolEngine te = (ToolEngine)buff;
        if (!inited) { Init(parms, caster, target, self, buff); }
        //if (tool == null) tool = (BaseTool)buff;
        //toolEffector的target不会是数组
        runedTime += Time.deltaTime;
       if(runedTime> valueSource.Get(caster, target[0], buff, parms).Convert<float>()&&!HadBreak)
        {//未被打断的情况下时间到达
         //TimeEnd.Run(parms,caster,target,self, buff);
         //直接跳toolengine下一节点的效果器
            UnInit(parms, caster, target, self, buff);
            te.Next();
        }
        else if(HadBreak)
        {//时间还没到的时候被打断了
            //走打断逻辑(一般另起一个新引擎)
            UnInit(parms, caster, target, self, buff);
            te.Stop();
            Breaked.Run(parms, caster, target, self, buff);
        }
        
    }

    void Init(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        inited = true;
    }
    void UnInit(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        inited = false;
    }
    //监听到basetool上
    public void OnBreak(string str, int level)
    {
        if(str==this.BreakEventName)
        {
            if (level >= BreakLevel) HadBreak = true;
        }
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
