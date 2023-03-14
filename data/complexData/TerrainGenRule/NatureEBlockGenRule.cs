using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public enum RandomMode
{
    normal,perlin
}

[CreateAssetMenu(menuName = "dataEditor/NatureEBlockGenRule")]
[JsonObject(MemberSerialization.OptIn)]
public class NatureEBlockGenRule : ScriptableObject
{
    /// <summary>
    /// 生成于什么生物群系
    /// </summary>
    [JsonProperty]
    public T_BIOME bio;
    /// <summary>
    /// 属于哪个生成阶段：矿，石，树，草，人造物5阶段
    /// </summary>
    [JsonProperty]
    public int stage;
    [JsonProperty]
    public int eblockid;
    [JsonProperty]
    public float chance;
    /// <summary>
    /// normal:对于每格直接取chance做生成几率
    /// perlin:对于每格取clamp(perlin-(1-chance),0,1)*1/(chance)
    /// </summary>
    [JsonProperty]
    public RandomMode mode;
}
