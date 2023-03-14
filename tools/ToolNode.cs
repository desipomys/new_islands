using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
/// <summary>
/// 条件节点从此类继承,需
/// 
/// </summary>
public class ToolNode_base
{
    [HideInInspector]
    [JsonIgnore]
    public ToolEngine father;
    public Effecttor_base[] effects;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parms"></param>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="self"></param>
    /// <param name="buff"></param>
    /// <returns>0=无，1=暂停，-1=出错，2结束</returns>
    public virtual int Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        ToolEngine te = (ToolEngine)buff;
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].Run(parms, caster, target, self, buff);
        }
        te.Next();
        return 0;
    }
    public virtual void OnEquip(EventCenter holder, ToolEngine father)
    {
        //事件等待节点在这里监听事件
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].OnInit(father.baseTool.hand.center,father);
        }
    }
    public virtual void UnEquip(EventCenter holder,ToolEngine father)
    {
       
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].UnInit(father.baseTool.hand.center,father);
        }
    }
    public virtual void OnInit(EventCenter holder,ToolEngine father)
    {
        this.father=father;
        foreach (var item in effects)
        {
            item.OnInit(holder,father);
        }
    }

}

/// <summary>
/// 条件节点从此类继承,需
/// </summary>
public class ToolNode_EventListen: ToolNode_base
{
    public int usedIndex;

    public override int Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        ToolEngine f = (ToolEngine)buff;
        if (!(bool)(f.cache[usedIndex]))
        {
            //监听事件
            f.cache[usedIndex] = true;
        }
        return base.Run(parms, caster, target, self, buff);
    }

    
    public override void UnEquip(EventCenter holder,ToolEngine father)
    {
        //解除监听事件
        base.UnEquip(holder,father);
    }
}

public enum LogicMode
{
    and,or,xor,
}
public class ToolNode_IF:ToolNode_base
{
     public Effecttor_base[] falseEffect;
     public Precondition_base[] condition;
     /// <summary>
     /// and,or
     /// </summary>
     public LogicMode mode;

    public override int Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
         ToolEngine te = (ToolEngine)buff;
        bool ans=false;
        switch (mode)
        {
            case LogicMode.and:
            if(target!=null)
            for (int j = 0; j < target.Length; j++)
            {
                 for (int i = 0; i < condition.Length; i++)
                {
                    ans=ans&&condition[i].Judge(caster,target[j],buff,parms);
                }
            }
           else
           for (int i = 0; i < condition.Length; i++)
                {
                    ans=ans&&condition[i].Judge(caster,null,buff,parms);
                }
            break;

            case LogicMode.or:
            if(target!=null)
            for (int j = 0; j < target.Length; j++)
            {
                 for (int i = 0; i < condition.Length; i++)
                {
                    ans=ans||condition[i].Judge(caster,target[j],buff,parms);
                }
            }
           else
           for (int i = 0; i < condition.Length; i++)
                {
                    ans=ans||condition[i].Judge(caster,null,buff,parms);
                }
            
            break;

            default:
            break;
        }
       
       if(ans){te.Next();return 0;}
        else{
             for (int i = 0; i < falseEffect.Length; i++)
            {
                falseEffect[i].Run(parms, caster, target, self, buff);
            }
            te.Next();
        }
        return 0;
    }
}
