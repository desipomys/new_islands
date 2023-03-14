using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;

[CreateAssetMenu(menuName = "toolEditor/toolengine")]
public class ToolEngine :SerializedScriptableObject, IEquipableEngine
{
    [HideInInspector]
    [JsonIgnore]
    public BaseTool baseTool;

    /// <summary>
    /// 不保存
    /// </summary>
    [JsonIgnore]
    public int currentIndex;
    [JsonIgnore]
    public bool runing=false;
    //
    /// <summary>
    /// 代表内部事件发生时对引擎传入的参数，需序列化,不需要odin显示,可改为obj
    /// </summary>
    [HideInInspector]
    [JsonIgnore]
    public object[] parms=new FP[10];

    /// <summary>
    /// 不会被保存
    /// </summary>
    [HideInInspector]
    [JsonIgnore]
    public object[] cache=new object[4];

    public List<ToolNode_base> nodes=new List<ToolNode_base>();

   

   public void OnRun()
   {
        if (runing&& nodes != null && nodes.Count > currentIndex&&currentIndex>=0)
        {
           int p= nodes[currentIndex].Run(parms, baseTool.holder, null, baseTool.holder, this);
            if(p==1)//运行到暂停
            {
                runing = false;
                //currentIndex = 0;
            }else if(p==0)//继续运行下一节点
            {
                OnRun();
            }
            else if(p==2)//重置
            {
                runing = false;
                currentIndex = 0;
            }
            else
            {
                //报错
            }
        }
        else if(currentIndex>= nodes.Count)
        {
            runing = false;
            currentIndex = 0;
        }
        
   }
    public void OnEquip(EventCenter holder,IEquipable father)
    {
        baseTool = (BaseTool)father;
        foreach (var item in nodes)
        {
            item.OnEquip(holder,this);
        }
    }
    public void UnEquip(EventCenter holder,IEquipable father)
    {
        runing = false;
    }

    public void Next()
    {
        //currentIndex = 0;
        currentIndex += 1;
    }
    public void Stop()
    {
        runing = false;
    }
   
    public void OnInit(EventCenter holder)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].OnInit(holder,this);
        } 
    }
    /// <summary>
    /// 重复调用不影响当前运行节点
    /// </summary>
    /// <param name="par"></param>
    public void StartRun(object[] par)
    {
        parms = new object[par.Length];
        for (int i = 0; i < parms.Length; i++)
        {
            parms[i] = par[i];//指向
        }
        runing = true;
        OnRun();
    }
    
}
