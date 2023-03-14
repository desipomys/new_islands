using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Newtonsoft.Json;
using System;

public enum BaseToolEventName
{
    OnEquip,
    UnEquip,
    OnUpdate,
    Reload,
    
    OnUse,
    /// <summary>
    /// 来自controller的每帧一次的key状态
    /// </summary>
    OnAnyKey,
    OnItemChg,
    ChgMode,
}
public enum BaseToolDataName
{
    

/// <summary>
/// 当前散率
/// </summary>
    spread,
    /// <summary>
    /// 连击次数
    /// </summary>
    attCount,
}

[CreateAssetMenu(menuName = "toolEditor/basetool")]
public class BaseTool :SerializedScriptableObject,IEquipable//tool也有evc
{
    [HideInInspector]
    [JsonIgnore]
    public EventCenter holder;//不序列化，反序列化玩家时初始化获取
    [HideInInspector]
    [JsonIgnore]
    public Hand hand;
    [HideInInspector]
    [JsonIgnore]
    public Item toolItem;
    //==================
    [LabelText("模型名称")]
    public string ModelName;
    [LabelText("工具类型")]
    public ToolType type;

    /// <summary>
    /// 存放散率、连击次数等临时数据
    /// </summary>
    [LabelText("临时数据")]
    [NonSerialized,OdinSerialize]
    [JsonIgnore]
    Dictionary<string,FP> tempData=new Dictionary<string, FP>(StringComparer.OrdinalIgnoreCase);

    [HideInInspector]
    [JsonProperty]
    public Dictionary<string,FP> TempData{get{return tempData;}set{tempData=value;}}

    [LabelText("执行引擎")]
    public Dictionary<BaseToolEventName,ToolEngine[]> engines=new Dictionary<BaseToolEventName, ToolEngine[]>();

    [HideInInspector]
    [JsonProperty]
    IEquipable[] extention;
    bool active;

    public static bool IsNullOrBareHand(BaseTool bt)
    {
        if (bt == null) return true;
        if (Item.IsNullOrEmpty(bt.toolItem)) return true;
        else if (EventCenter.WorldCenter.GetParm<Item,bool>(nameof(EventNames.IsBareHand),bt.toolItem))
        {
            return true;
        }
        else return false;
    }
    public virtual bool ComPare(Item i)
    {
        return toolItem.Compare(i, ItemCompareMode.idEqual);//ID相等即为同种tool
    }
    public FP GetData(BaseToolDataName s)
    {
        return GetData(nameof(s));
    }
     public T GetData<T>(BaseToolDataName s)
    {
       return GetData<T>(nameof(s));
    }
    public T GetData<T>(string s)
    {
        if (TempData.ContainsKey(s))
            return TempData[s].Convert<T>();
        else return default(T);
    }
    public FP GetData(string s)
    {
        if (TempData.ContainsKey(s))
            return TempData[s];
        else return null;
    }
     public void SetData(BaseToolDataName s,FP fp)
    {
        SetData(nameof(s),fp);
    }
    public void SetData(BaseToolDataName s,object fp)
    {
        SetData(nameof(s),fp);
    }
    public void SetData(string s,FP fp)
    {
         if(!tempData.ContainsKey(s))
        {
            tempData.Add(s,fp);
        }
        TempData[s] = fp;
    }
    /// <summary>
    /// 减少fp的创建次数
    /// </summary>
    /// <param name="s"></param>
    /// <param name="fp"></param>
    public void SetData(string s,object fp)
    {
        if(!tempData.ContainsKey(s))
        {
            tempData.Add(s,new FP(fp));
        }
        TempData[s].data = fp;
    }
    /// <summary>
    /// effector调用改变item
    /// </summary>
    /// <param name="it"></param>
    public void ChgItem(Item it)
    {
        hand.OnToolUpdateItem(it, this);
        //传给hand->bp更新itempage发送改变事件
    }
    public void listenEvent()
    {

    }

    //发送inner事件
    void SendInnerEvent(BaseToolEventName s, object[] parms)
    {
        if (engines.ContainsKey(s))
        {
            for (int j = 0; j < engines[s].Length; j++)
            {               
                engines[s][j].StartRun(parms);
            }
            
        }
        
    }
    /// <summary>
    /// 外部事件
    /// </summary>
    /// <param name="s"></param>
    /// <param name="parms"></param>
    void SendInnerEvent(string s, object[] parms)
    {

    }


    /// <summary>
    /// 刚从container中被取出生成到手上时,晚于onevcreg
    /// 刚读取存档进入游戏时调用
    /// </summary>
    /// <param name="holder"></param>
    public virtual void OnInit(EventCenter holder,Item item)
    {
        //throw new System.NotImplementedException();
        this.holder = holder;
        hand = holder.GetComponent<Hand>();
        toolItem=item;
        foreach (var eng in engines)
        {
            for (int i = 0; i < eng.Value.Length; i++)
            {
                eng.Value[i].OnInit(holder);
            }
            
        }
        //根据item携带的配件数据生成配件到tool的gameobj，并调用其onevc
        if(extention!=null)
        for (int i = 0; i < extention.Length; i++)
        {
            extention[i].OnInit(holder,item);
        }
        //gameObject.SetActive(false);
    }
    /// <summary>
    /// 当hand切换到本tool作为当前使用的tool时，晚于oninit
    /// 刚读取存档进入游戏时不会被调用
    /// </summary>
    /// <param name="holder"></param>
    public virtual void OnEquip(EventCenter holder)
    {
        active = true;
        this.holder = holder;
        hand = holder.GetComponent<Hand>();

        foreach (var item in engines)
        {
            for (int i = 0; i < item.Value.Length; i++)
            {
                item.Value[i].OnEquip(holder,this);
            }
                
        }
        if(extention!=null)
         for (int i = 0; i < extention.Length; i++)
        {
            extention[i].OnEquip(holder);
        }

        //gameObject.SetActive(true);
    }
    /// <summary>
    /// 当hand从当前tool切换走时，先于下一个tool的onequip,不应该在此操作自身对应的item
    /// </summary>
    /// <param name="holder"></param>
    public virtual void UnEquip(EventCenter holder)
    {
        active = false;
        this.holder = holder;
        foreach (var item in engines)
        {
            for (int i = 0; i < item.Value.Length; i++)
            {
                item.Value[i].UnEquip(holder,this);
            }

        }
        if (extention!=null)
        for (int i = 0; i < extention.Length; i++)
        {
            extention[i].UnEquip(holder);
        }
        //gameObject.SetActive(false);
    }
    public virtual void OnUse(EventCenter holder,KeyCodeStat stat)
    {
        SendInnerEvent(BaseToolEventName.OnUse, new object[]{holder,stat});
         if (extention!=null)
        foreach (var item in extention)
        {
            item.OnUse(holder,stat);
        }
    }
    public virtual void OnAnyKey(EventCenter holder,KeyCodeKind code,Dictionary<KeyCode, KeyCodeStat> stat)
    {
        SendInnerEvent(BaseToolEventName.OnAnyKey, new object[] { holder, code, stat });
        if (extention!=null)
        foreach (var item in extention)
        {
            item.OnAnyKey(holder,code,stat);
        }
    }
    public virtual void OnItemChange(EventCenter holder, Item item)//当对应item改变，工具active/unactive时都可能被调用
    {
        //Debug.Log("itemchg");
        this.toolItem = item;
        SendInnerEvent(BaseToolEventName.OnItemChg, new object[] { holder, item });
        if (extention!=null)
        foreach (var items in extention)
        {
            items.OnItemChange(holder,item);
        }
    }
    

   public void OnUpdate(EventCenter holder)
   {
        if (!active) return;
        if (engines != null)
            //foreach (var item in engines)
            //{
            //    for (int i = 0; i < item.Value.Length; i++)
            //    {
            //        item.Value[i].OnRun();
            //    }

            //}
            SendInnerEvent(BaseToolEventName.OnUpdate, null);
        if (extention!=null)
        for (int i = 0; i < extention.Length; i++)
        {
            extention[i].OnUpdate(holder);
        }
    }
    /// <summary>
    /// 当持有者扣血等事件发生时
    /// </summary>
    /// <param name="holder"></param>
    /// <param name="parms"></param>
    public void OnHolderEvent(EventCenter holder,PlayerEventName type,object[] parms)
    {
        SendInnerEvent(nameof(type),  parms );
        if (extention!=null)
        foreach (var items in extention)
        {
            items.OnHolderEvent(holder,type,parms);
        }
    }
    public void OnHolderEvent(EventCenter holder, PlayerEventName type, object parms)
    {
        SendInnerEvent(nameof(type), new object[] {parms });
        if (extention!=null)
        foreach (var items in extention)
        {
            items.OnHolderEvent(holder,type,new object[]{parms});
        }
    }

    /// <summary>
    /// 当世界事件发生
    /// </summary>
    /// <param name="holder"></param>
    /// <param name="type">改成可用的世界事件类型</param>
    /// <param name="parms"></param>
    public void OnWorldEvent(EventCenter holder,PlayerEventName type,object[] parms)
    {
        SendInnerEvent(nameof(type), parms);
         if (extention!=null)
        foreach (var items in extention)
        {
            items.OnWorldEvent(holder,type,new object[]{parms});
        }
    }
    /// <summary>
    /// 当UI事件发生，由controller发出
    /// </summary>
    /// <param name="holder"></param>
    /// <param name="type"></param>
    /// <param name="parms"></param>
    public void OnUIEvent(EventCenter holder,PlayerEventName type,object[] parms)
    {
        SendInnerEvent(nameof(type), new object[] { parms });
         if (extention!=null)
        foreach (var items in extention)
        {
            items.OnUIEvent(holder,type,new object[]{parms});
        }
    }

    /// <summary>
    /// 当hand丢弃当前tool时，晚于unequip
    /// </summary>
    /// <param name="holder"></param>
    public void OnDispose(EventCenter holder)//当被销毁时（收回池中，不销毁）
    {
        
    }


    public virtual bool IsEqual(Item i)
    {
        return toolItem.Compare(i);
    }
}
public enum ToolType
{
    none=0,
    knife=1,
    axe=knife<<1,
    pickaxe= knife << 2,
    hammer= knife << 3,
    bow= knife << 4,
    crossbow= knife << 5,
    pistol= knife << 6,
    rifle= knife << 7,
    auto= knife << 8,
    explosive= knife << 9,
    magicStick = knife << 10,
    fist = knife << 11,
    spred= knife << 12,//长矛
    shavel = knife << 13,//铲子


}
