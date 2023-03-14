using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Sirenix.OdinInspector;


/// <summary>
/// node需catch运行报错，并发送riaserror
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public abstract class MovScriptNode :SerializedScriptableObject,IMovScriptNode
{
    [HideInInspector]
    [JsonIgnore]
    public MovScriptEngine hostEngine;
    [JsonProperty]
    public string Name;
    [JsonProperty]
    public string tag;
    /// <summary>
    /// 决定节点是否接收onupdate
    /// </summary>
    [JsonProperty]
    public bool isActive=true;
    [JsonProperty]
    public int runCount;
    
    /// <summary>
    /// 前摇
    /// </summary>
    [JsonProperty]
    public float frontWait;
     /// <summary>
    /// 前摇已等待时间
    /// </summary>
    [JsonProperty]
    [HideInInspector]
    public float FrontTimed=0;

    public virtual void OnEnter(MovScriptEngine eng)
    {
        hostEngine=eng;
    }
   /// <summary>
   /// 由上级引擎调用
   /// </summary>
    public virtual void OnLeave()
    {

    }
    
    public virtual void OnPause()
    {
      isActive=false;
    }
    
    public virtual void OnResume()
    {
      isActive=true;
    }
    
    public abstract int OnUpdate();
    public virtual void OnSkip()
    {
        hostEngine.Next();
    }
    public virtual void OnDelete()
    {

    }
    public virtual void RaiseError(string msg)
    {
        if(hostEngine!=null){hostEngine.RaiseError("\n 于节点"+hostEngine.currentIndex+":"+msg);}
        else 
        {//可定义记录保存位置
            Debug.Log("错误发生于\""+Name+"\"的节点"+"："+msg);
        }
    }
    public virtual void OnInit(MovScriptEngine father)
    {
        hostEngine=father;
    }
}
