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
    /// �������ĸ��Ƽ�,0=��
    /// </summary>
    [JsonProperty]
    public int relyOnTech;
    [JsonProperty]
    public string img;
    [JsonProperty]
    public bool isBBlock;
    /// <summary>
    /// ֻ�õ�typ�ֶ�
    /// </summary>
    [JsonProperty]
    public Entity_BlockModel eblockTyp;
    [JsonProperty]
    public B_Block bblockTyp;
    [JsonProperty]
    [Tooltip("�����������")]
    public Item[] mats;
}