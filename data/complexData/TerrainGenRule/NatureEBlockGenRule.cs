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
    /// ������ʲô����Ⱥϵ
    /// </summary>
    [JsonProperty]
    public T_BIOME bio;
    /// <summary>
    /// �����ĸ����ɽ׶Σ���ʯ�������ݣ�������5�׶�
    /// </summary>
    [JsonProperty]
    public int stage;
    [JsonProperty]
    public int eblockid;
    [JsonProperty]
    public float chance;
    /// <summary>
    /// normal:����ÿ��ֱ��ȡchance�����ɼ���
    /// perlin:����ÿ��ȡclamp(perlin-(1-chance),0,1)*1/(chance)
    /// </summary>
    [JsonProperty]
    public RandomMode mode;
}
