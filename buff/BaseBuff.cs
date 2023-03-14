using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public enum BuffStackType
{
    cantStack, replace,
    stackAddLevel,
    stackAddTime,
    stackAddTag,
    stackMaxLevel, stackMinLevel,
    stackMaxTime, stackMinTime,

}
public enum BuffEventPoint
{
    OnInit, OnAttach, OnUpdate, OnTimeEnd, OnDestory, OnLevelChg,
    BeforeBuffAdd, AfterBuffAdd, BeforeBuffDestory, AfterBuffDestory,


}
[CreateAssetMenu(menuName = "dataEditor/buff/buff")]
[JsonObject(MemberSerialization.OptIn)]
public class BaseBuff : SerializedScriptableObject, IComparable<BaseBuff>
{//buff根据配置的scobj在loadsave初始化阶段组装完成，
    [LabelText("buff预制名称")]
    [JsonProperty]
    public string prefabName;//依据哪个prefabs制成
    [JsonProperty]
    public string buffname;//索引
    [JsonProperty]
    public int level;
    [JsonProperty]
    public string tag;
    [JsonProperty]
    public float keepTime;
    [JsonProperty]
    public float updateStep;
    [JsonProperty]
    public float RunedTime;
    [JsonProperty]
    public BuffStackType stackAble;//true则buffcenter会将level叠加到同名buff上，时间增加keeptime
    [JsonProperty]
    public long sourceUUID;
    [JsonProperty]
    public FP[] BuffDatas;

    //public Dictionary<string,object> exd;
    //buff只能在临时变量处持有不可序列化对象

        //只负责在inspector显示物件
    //[DictionaryDrawerSettings()]
    //[ShowInInspector]
    [NonSerialized,OdinSerialize]
    public List<BuffLogicSeg> logics=new List<BuffLogicSeg>();


    /// <summary>
    /// 运行时实际存储
    /// </summary>
    Dictionary<BuffEventPoint, List<BuffLogicSeg>> LOGICS=new Dictionary<BuffEventPoint, List<BuffLogicSeg>>();

    //只负责json的序列化、反序列化，运行时使用的
    [HideInInspector]
    [JsonProperty]
    public Dictionary<BuffEventPoint, List<BuffLogicSeg>> datas
    {
        get { if ((LOGICS==null&& logics!=null) ||(LOGICS .Count==0 && logics.Count >0)) init(); return LOGICS; }
        set { LOGICS = value; }
    }
    /// <summary>
    /// 同步logics,datas
    /// </summary>
    void init()
    {
        LOGICS.Clear();
        for (int i = 0; i < logics.Count; i++)
        {
            AddLogicSeg(logics[i], logics[i].eventPoint);
        }
    }

    public BaseBuff() { }
    public BaseBuff(string name) { buffname = name; }

    public void AddLogicSeg(BuffLogicSeg log, BuffEventPoint point)
    {
        if (LOGICS == null) LOGICS = new Dictionary<BuffEventPoint, List<BuffLogicSeg>>();
        if (!LOGICS.ContainsKey(point)) { LOGICS.Add(point, new List<BuffLogicSeg>()); }
        LOGICS[point].Add(log);
    }

    #region 外部事件方法
    public void OnAttach(EventCenter source, EventCenter target)
    {
        if (datas.ContainsKey(BuffEventPoint.OnAttach))
        {
            foreach (var item in datas[BuffEventPoint.OnAttach])
            {
                item.Run(source, target, this, null);
            }

        }
    }
    public void OnUpdate(EventCenter target)
    {
        if (RunedTime > keepTime)
        { target.GetComponent<BuffCenter>().RemoveBuff(target, this); return; }
        int updatedTime = (int)(RunedTime / updateStep);
        RunedTime += Time.deltaTime;

        int newUpdatedTime = (int)(RunedTime / updateStep);
        if (newUpdatedTime - updatedTime > 0)
        {
            for (int i = 0; i < newUpdatedTime - updatedTime; i++)
            {
                if (datas.ContainsKey(BuffEventPoint.OnUpdate))
                {
                    for (int j = 0; j < datas[BuffEventPoint.OnUpdate].Count; j++)
                    {
                        datas[BuffEventPoint.OnUpdate][j].Run(null, target, this, null);
                    }

                }

            }
        }
        if (RunedTime > keepTime)
        { target.GetComponent<BuffCenter>().RemoveBuff(target, this); return; }
    }
    public void OnTimeEnd(EventCenter target)
    {

    }
    public void OnDestory(EventCenter source, EventCenter target)
    {

    }
    public void OnStackLevel(EventCenter source, EventCenter target, BaseBuff buff)
    {
        if (datas.ContainsKey(BuffEventPoint.OnLevelChg))
        {
            for (int j = 0; j < datas[BuffEventPoint.OnLevelChg].Count; j++)
            {
                datas[BuffEventPoint.OnLevelChg][j].Run(source, target, this, null);
            }

        }
    }

    public void OnHandChg(EventCenter target)//需要tool,index作为参数
    {

    }
    public void OnEquipChg(EventCenter target)//需要装备,index作为参数
    {

    }
    public void OnPartChg(EventCenter target)//需要配件,index作为参数
    {

    }
    public void OnSkinChg(EventCenter target)//需要皮肤,index作为参数
    {

    }
    public void OnHit(EventCenter target)
    {

    }
    public void OnHealthChg(EventCenter target, float newHealth, float oldHealth)
    {

    }


    public bool BeforeBuffAdd(EventCenter source, EventCenter target, BaseBuff buffAdded)
    {
        return true;
    }
    public void AfterBuffadd(EventCenter source, EventCenter target, BaseBuff buffAdded)
    {

    }
    public bool BeforeBuffDestory(EventCenter source, EventCenter target, BaseBuff buffAdded)
    {
        return true;
    }
    public void AfterBuffDestory(EventCenter source, EventCenter target, BaseBuff buffAdded)
    {

    }

    public void OnBuffInit(EventCenter holder,BuffCenter buffCenter)
    {
        foreach (var item in LOGICS)
        {
            foreach (var logs in item.Value)
            {
                logs.OnBuffInit(holder,this);
            }
        }
    }
    #endregion


    public int CompareTo(BaseBuff b)
    {
        int x = b.buffname.CompareTo(buffname);
        int y = b.level.CompareTo(level);
        if (x == 0) return y;
        return x;
    }
    public bool IsEqual(BaseBuff bf)
    {
        return CompareTo(bf) == 0;
    }

    public BaseBuff Copy()
    {
        return BaseBuff.FromString(ToString());
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
    }
    public static BaseBuff FromString(string s)
    {
        BaseBuff bfbf= JsonConvert.DeserializeObject<BaseBuff>(s, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return bfbf;
    }
}

/*
 *对应
 * [
触发事件

[生效条件]*n
目标选择器
目标筛选器


效果器+特效(与效果器共用目标)
]
 *
 * 
 */
public class BuffLogicSeg
{
    public BuffEventPoint eventPoint;
    //public BuffValueSource_base valueSource;

    //子组件
    public List<buffPrecondition_base> preconditions;
    public BuffTargetSelector targetSelector;
    public BuffTargetFilter targetFilter;

    public List<BuffEffector_base> effectors;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="caster">buff发送者</param>
    /// <param name="target">buff挂载者</param>
    /// <param name="self">buff自己</param>
    public void Run(EventCenter caster, EventCenter target, BaseBuff self, object[] parms)
    {

        //FPParms parm = null;//valueSource.Get(caster, target, self);

        bool succ = true;
        if (preconditions != null)
            foreach (var item in preconditions)
            {
                if (!item.Judge(caster, target, self, parms)) { succ = false; break; }
            }
        if (succ)
        {
            EventCenter[] evcs;
            if (targetSelector == null)
            {
                evcs = new EventCenter[] { target };
            }
            else
                evcs = targetSelector.Select(caster, target, self, parms);
            if (targetFilter != null)
                evcs = targetFilter.Filter(caster, target, evcs, parms, self);

            foreach (var item in effectors)
            {
                item.Run(parms, caster, evcs, target, self);
            }
        }
    }

    public void OnBuffInit(EventCenter holder,BaseBuff bf)
    {
        foreach (var item in preconditions)
        {
            item.OnBuffInit(holder,bf);
        }
        targetSelector.OnBuffInit(holder,bf);
        targetFilter.OnBuffInit(holder,bf);
        foreach (var item in effectors)
        {
            item.OnInit(holder,bf);
        }
    }

    /// <summary>
    /// 并不能从空logseg转为具体类
    /// </summary>
    /// <param name="dy">纯数据，不包括子组件类型</param>
    [System.Obsolete]
    public void FromObject(dynamic dy)
    {
        /*eventPoint = (BuffEventPoint)dy.eventPoint;
        if (preconditions != null)
            for (int i = 0; i < preconditions.Count; i++)
            {
                Debug.Log(dy.preconditions.GetType().Name);
                preconditions[i].FromObject(dy.preconditions[i]);
            }
        if (targetSelector != null)
            targetSelector.FromObject(dy.targetSelector);
        if (targetFilter != null)
            targetFilter.FromObject(dy.targetFilter);
        if (effectors != null)
            for (int i = 0; i < effectors.Count; i++)
            {
                //Debug.Log(dy.effectors.GetType().Name);
                dynamic temp = dy.effectors[i].ToObject<ExpandoObject>();
                effectors[i].FromObject(temp);
            }*/
    }
}

public class BaseBuffConverter : CustomCreationConverter<BaseBuff>
{
    public override BaseBuff Create(Type objectType)
    {
        Debug.Log(objectType.Name);
        BaseBuff bf = EventCenter.WorldCenter.GetParm<string, BaseBuff>("GetBuffByName", "");
        return bf;
    }
}
