using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(menuName = "dataEditor/BuildAble")]
[JsonObject(MemberSerialization.OptIn)]
public class BuildAble_Data : ScriptableObject
{
    [JsonProperty]
    public int level;
    [JsonProperty]
    public int type;
    /// <summary>
    /// 依赖于哪个科技,0=无
    /// </summary>
    [JsonProperty]
    public int relyOnTech;
    [JsonProperty]
    public string img;
    [JsonProperty]
    public bool isBBlock;
    /// <summary>
    /// 只用到typ字段
    /// </summary>
    [JsonProperty]
    public Entity_BlockModel eblockTyp;
    [JsonProperty]
    public B_Block bblockTyp;
    [JsonProperty]
    [Tooltip("建筑所需材料")]
    public Item[] mats;
}