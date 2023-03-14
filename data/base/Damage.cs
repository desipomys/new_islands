using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public enum DamageType
{
    None,Cut,Bullet,Explo,Hammer,
    
    fire,ice,electro,radio,
    posion,corruption,heal
    //necroman死灵术没有伤害技能
    //元素抵抗及反应
    //fire<ice,electro<ion,radio>ion,fire>ion,ice>radio,elecctro>ice
}
//爆炸衰减公式 -(x-1.1)^(3)+0.1
[Serializable]
public class Damage :SerializedScriptableObject
{
    public float value;
    public DamageType type;
    /// <summary>
    /// 额外伤害加成
    /// </summary>
    public float AdditionV;
     /// <summary>
    /// 额外伤害加成%
    /// </summary>
    public float AdditionPercent;
    /// <summary>
    /// 暴击率
    /// </summary>
    public float CritPercent;
    /// <summary>
    /// 暴击加成%
    /// </summary>
    public float CritAdd;

    public Dictionary<string, object> exd;
    public bool IsContainBuff()
    {
        if (exd == null) return false;
        if (exd.ContainsKey("buff")) return true;
       else return false;
    }

    public void AddBuff(BaseBuff buff)
    {
        
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this,typeof(Damage), JsonSetting.serializerSettings);
    }
    public static Damage FromString(string data)
    {
        return (Damage)JsonConvert.DeserializeObject(data,typeof(Damage),JsonSetting.serializerSettings);
    }
}
