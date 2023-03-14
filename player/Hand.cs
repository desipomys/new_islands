using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public enum ToolPosition
{
    /// <summary>
/// 消音器位
/// </summary>
    silencePos,
    /// <summary>
/// 瞄准镜位
/// </summary>
    scopePos,
    /// <summary>
/// 把手位
/// </summary>
    handlePos,
    /// <summary>
/// 弹夹位
/// </summary>
    magPos,
    /// <summary>
/// 枪托位
/// </summary>
    buttomPos,
  
}
public enum HandDataName
{
    
    /// <summary>
    /// 伤害加成+
    /// </summary>
    damAdd,
    /// <summary>
    /// 伤害加成%
    /// </summary>
    damPercent,
    /// <summary>
    /// 暴击率
    /// </summary>
    crit,
    /// <summary>
    /// 暴击加成
    /// </summary>
    CritAdd,
     /// <summary>
    /// 子弹预设
    /// </summary>
    bulletPrefab,
     /// <summary>
    /// 伤害类型
    /// </summary>
    damType,
    /// <summary>
    /// 散率形状
    /// </summary>
    bulletSpreadShape,
     /// <summary>
    /// 伤害的buff附加
    /// </summary>
    damBuff,

     /// <summary>
    /// 散率增加的减少值
    /// </summary>
    spreadDec,
     /// <summary>
    /// 散率增加的减少百分比
    /// </summary>
    spreadPercent,

     /// <summary>
    /// 弹夹容量提升+
    /// </summary>
    magExt,
    /// <summary>
    /// 弹夹容量提升%
    /// </summary>
    magExtPercent,
}

/// <summary>
/// 玩家、AI等任意可持有工具的实体的持有工具功能基类
/// </summary>
public class Hand : MonoBehaviour,IEventRegister,IDataContainer
{

   public EventCenter center;
   public BaseTool[] Tools;
    public GameObject[] ToolObjs;
/// <summary>
/// 支持buff修改进而影响tool的值
/// </summary>
/// <typeparam name="string"></typeparam>
/// <typeparam name="FP"></typeparam>
/// <returns></returns>
    public Dictionary<string, FP> buffedValue=
    new Dictionary<string, FP>();
   protected Vector3 mainHandPos;//上一帧的主手位置
   protected bool active=true;
    protected PauseHandler handler = new PauseHandler();
    public int currentToolIndex=0;

    public string bareHandName;
    
    
    protected Dictionary<ToolPosition,Transform> ToolPoscache=new Dictionary<ToolPosition, Transform>();

    public int GetDataCollectPrio => 1;

    public virtual void OnEventRegist(EventCenter evc)
    {
        center=evc;
        center.ListenEvent<KeyCodeKind,Dictionary<KeyCode, KeyCodeStat>>(nameof(PlayerEventName.onKey),OnKey);
        center.ListenEvent<bool>(nameof(PlayerEventName.setHandActive),(bool p)=>{ handler.DoPause(!p); });
        center.ListenEvent<Item,BaseTool>(nameof(PlayerEventName.onToolUpdateItem),OnToolUpdateItem);
        center.ListenEvent<ItemPageChangeParm>(nameof(PlayerEventName.bpChg), OnHandItemPageChg);
        center.ListenEvent<string>(nameof(PlayerEventName.onAnimationEvent), OnAnim);
    }
    public virtual void AfterEventRegist()
    {

    }
    
    protected virtual void OnKey(KeyCodeKind keys, Dictionary<KeyCode, KeyCodeStat> stats)
    {

    }
    public virtual void OnToolSendFloatToUI(string name,float value)
    {

    }
/// <summary>
/// 当tool自己更新了其对应的item后
/// </summary>
/// <param name="item"></param>
/// <param name="updateTool"></param>
    public virtual void OnToolUpdateItem(Item item,BaseTool updateTool)
    {

    }

    public virtual void OnHandItemPageChg(ItemPageChangeParm parm)
    {

    }
    public virtual void OnAnim(string parm)
    {

    }
    public virtual Transform getMainHand() { return null; }
    public virtual BaseTool GetNowHolding() {
        return Tools[currentToolIndex];
    }
    public virtual Vector3 GetHoldingToolPosi(ToolPosition pos,int index=-1)
    {
        if(ToolPoscache.ContainsKey(pos))return ToolPoscache[pos].position;
        Transform t=ToolObjs[currentToolIndex].transform.Find(nameof(pos));
        ToolPoscache.Add(pos,t);
        return t.position;
    }

     public virtual FP GetBuffedValue(HandDataName name)
    {
        return GetBuffedValue(nameof(name));
    }
    public virtual void SetBuffedValue(HandDataName name,FP val)
    {
       SetBuffedValue(nameof(name),val);
    }
    public virtual FP GetBuffedValue(string name)
    {
        if(!buffedValue.ContainsKey(name))return null;
        return buffedValue[name];
    }
    public virtual void SetBuffedValue(string name,FP val)
    {
        if(buffedValue.ContainsKey(name))
        buffedValue[name]=val;
        else buffedValue.Add(name,val);
    }


    public virtual void FromObject(object str)
    {
        currentToolIndex=int.Parse(str.ToString());
       

    }
    public virtual object ToObject()
    {
        return currentToolIndex;
    }
}
