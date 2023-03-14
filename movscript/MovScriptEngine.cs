using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Newtonsoft.Json;
using System;
/// <summary>
/// 剧本引擎数据,一个update只能执行一个节点
/// </summary>
[CreateAssetMenu(menuName = "MovScript/engine/engine")]
public class MovScriptEngine :MovScriptNode
{
    [HideInInspector]
    [JsonIgnore]
    public EventCenter hostCenter;
    [JsonProperty]
    public int currentIndex;
    [DictionaryDrawerSettings()]
    [ShowInInspector]
    [JsonProperty]
    public Dictionary<string,FP> datas=new Dictionary<string, FP>();
    [SerializeReference]
    [JsonProperty]
    public List<MovScriptNode> nodes=new List<MovScriptNode>();
    /// <summary>
    /// index的别名，用来辅助实现类似汇编中jmp loopend的效果
    /// </summary>
    /// <typeparam name="string">别名</typeparam>
    /// <typeparam name="int">索引</typeparam>
    /// <returns></returns>    
    [DictionaryDrawerSettings()]
    [ShowInInspector]
    [JsonProperty]
    public Dictionary<string,int> aliasIndex=new Dictionary<string, int>();

    /// <summary>
    /// “寄存器AX,BX,CX,DX”，会被序列化保存
    /// </summary>
    [JsonProperty]
    [NonSerialized,OdinSerialize]
    public FP AX,BX,CX,DX;//可用于单个节点内各command间的公共数据区，可跨节点传数据
    /// <summary>
    /// 保存cmp指令比较结果，1>,0=,-1<
    /// </summary>
    [JsonProperty]
    public int ans;
    /// <summary>
    /// 下一节点的index
    /// </summary>
    [JsonProperty]
    public int PC;
    
    
    public void SetCache(MovEngineCacheType type,FP value)
    {
         switch (type)
       {
        case MovEngineCacheType.AX:
         AX=value;
        break;
        case MovEngineCacheType.BX:
         BX=value;
        break;
        case MovEngineCacheType.CX:
         CX=value;
        break;
        case MovEngineCacheType.DX:
         DX=value;
        break;
        default:
        AX=value;
        break;
       }
    }
    public FP GetCache(MovEngineCacheType type)
    {
        switch (type)
       {
        case MovEngineCacheType.AX:
        return AX;
        break;
        case MovEngineCacheType.BX:
        return BX;
        break;
        case MovEngineCacheType.CX:
        return CX;
        break;
        case MovEngineCacheType.DX:
        return DX;
        break;
        case MovEngineCacheType.ans:
        return ans;
        default:
        return AX;
       }
    }

    public FP GetData(string key)
    {
        return datas[key];
    }
    public void SetData(string key,object data)
    {
        datas[key].Data = data;
    }

   
    /// <summary>
    /// 只移动指针不执行
    /// </summary>
    /// <param name="index"></param>
    public void Next(int index=-1)
    {
        nodes[currentIndex].OnLeave();
        if(index==-1)
        {
            currentIndex=PC;
            PC++;
            if(PC>nodes.Count)//运行结束
            {
                if (hostEngine != null) hostEngine.Next();
                else isActive = false;
            }
        }
        else if(index>=0&&index<nodes.Count)
        {
            PC=index;
            currentIndex=PC;
        }
        else
        {
            RaiseError("跳转到不可达index:"+index);
        }
        //currentIndex=index;
    }
    public void Jump(string alias)
    {
        if(aliasIndex.ContainsKey(alias))
        {
            Next(aliasIndex[alias]);
        }else{RaiseError("引擎未定义\""+alias+"\"对应的index");}
    }

    #region 当事件发生时
    public override void OnEnter(MovScriptEngine eng)
    {
         try
        {
            hostEngine = eng;
            if(nodes==null||nodes.Count==0)
            {
                runCount++;
                if(hostEngine!=null) hostEngine.Next();
                return;
            }

            currentIndex=0;
            PC=1;
            nodes[currentIndex].OnEnter(this);
            runCount++;
        }
        catch (System.Exception e)
        {
            RaiseError(e.Message);
            
        }    
    }
    public override void OnLeave()
    {

    }
    /// <summary>
    /// 无论是否在运行，都会接收update
    /// </summary>
    /// <returns></returns>
    public override int OnUpdate()
    {
        if (!isActive) return 0;
        try
        {
            if (nodes[currentIndex].isActive)
                return nodes[currentIndex].OnUpdate();
            else return 1;
        }
        catch (System.Exception e)
        {
            RaiseError(e.Message);
            return -1;
        }    
        
    }
    public override void OnSkip()
    {
        try
        {
             nodes[currentIndex].OnSkip();
        }
        catch (System.Exception e)
        {
            RaiseError(e.Message);
            
        }    
       
    }
    public override void OnPause()
    {
        try
        {
           nodes[currentIndex].OnPause();
        }
        catch (System.Exception e)
        {
            RaiseError(e.Message);
            
        }    

        
    }
    public override void OnResume()
    {
        try
        {
           nodes[currentIndex].OnResume();
        }
        catch (System.Exception e)
        {
            RaiseError(e.Message);
            
        }    
        
    }
    public override void OnInit(MovScriptEngine eng)
    {
        base.OnInit(eng);
        foreach (var item in nodes)
        {
            item.OnInit(this);
        }
    }
    #endregion

    public static MovScriptEngine DeSerailize(string s)
    {
        return JsonConvert.DeserializeObject<MovScriptEngine>(s);
    }
}
