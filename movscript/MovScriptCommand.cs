using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Dynamic;

/// <summary>
/// 一条剧本系统的命令，不是剧本引擎的节点,
/// </summary>
[CreateAssetMenu(menuName = "MovScript/command")]
[JsonObject(MemberSerialization.OptIn)]
public class MovScriptCommand : SerializedScriptableObject
{
    /// <summary>
    /// 简化后的命令名
    /// </summary>
    [JsonIgnore]
    public string SimpleName;
    //描述
    [JsonIgnore]
    public string descript;

    [ShowIf("SimpleName")]
    [JsonProperty]
    [DictionaryDrawerSettings]
    [LabelText("参数项与index对应")]
    public Dictionary<string, int> op_index;
    //str simplestr
    [SerializeReference]
    [JsonProperty]
    public TargetSelector selector;
    [SerializeReference]
    [JsonProperty]
    public TargetFilter filter;

    [NonSerialized, OdinSerialize]
    public List<Effecttor_base> effectors = new List<Effecttor_base>();
    /// <summary>
    /// 只为newton序列化使用，
    /// </summary>
    [HideInInspector]
    [JsonProperty]
    List<Effecttor_base> Effectors { get { return effectors; } set { effectors = value; } }

    //protected override void OnAfterDeserialize()
    //{
    //Debug.Log("序列化完成");
    //newtonEffectors = effectors;
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="target"></param>
    /// <param name="self">一般是movengine</param>
    /// <param name="parms"></param>
    public void Run(EventCenter caller, EventCenter target, object self, object[] parms)
    {
        EventCenter[] evcs;
        if (selector == null)
        {
            evcs = new EventCenter[] { target };
        }
        else
            evcs = selector.Select(caller, target, self, parms);
        if (filter != null)
            evcs = filter.Filter(caller, target, evcs, parms, self);

        foreach (var item in effectors)
        {
            item.Run(parms, caller, evcs, target, self);
        }
    }
    /// <summary>
    /// 只有在引擎中才调用
    /// </summary>
    /// <param name="node"></param>
    public void OnEngineInit(MovScriptNode node)
    {
        foreach (var item in effectors)
        {
            item.OnInit(node.hostEngine.hostCenter, node);
        }
    }
    /// <summary>
    /// 单独使用str命令模式调用时的初始化
    /// 如 msg "hello"
    /// </summary>
    /// <param name="holder"></param>
    /// <param name="self"></param>
    /// <param name="parms"></param>
    public void RunFromStr(EventCenter holder, object self, object[] parms)
    {
        foreach (var item in effectors)
        {
            item.OnInit(holder, self);
        }
        Run(holder, holder, self, parms);
    }

    public static void RunFromSimpleStr(string simplename, string[] parms, EventCenter holder, object self)
    {
        /*
        msg 'aaa'=null null msg{v='aaa'}
        summon skelon{h=10}=null null summon{e=skelon,p={h=10}}
        */
        //loader_movCommand读取str-command对
        //调用loader获取str对应的command对象
        //command对象如何使用参数？
        MovScriptCommand command = EventCenter.WorldCenter.GetParm<string, MovScriptCommand>(nameof(EventNames.GetMovCommandByStr), simplename);
        command.RunFromStr(holder, self, decodeParm(command,parms));

    }
    public static object[] decodeParm(MovScriptCommand cmd,string[] s)
    {
        List<object> ans = new List<object>();
        int objindex = 0;//当前指向ans中哪个index的后一个
        int stat = 0;
        //0=刚处理完上一条指令，下一条可能是参数、选项
        //1 =准备读参数
        for (int i = 0; i < s.Length; i++)
        {
            if(s[i].StartsWith("-")&&s[i].Length<5&&stat==0)
                //可能是参数或是-选项,-选项长度不能大于5
            {
                double p;
                if(double.TryParse(s[i],out p))//可以解析为负数int/float/long
                {
                    FP temp = new FP(p);
                    if (objindex >= ans.Count)
                        ans.Add(temp);
                    else ans[objindex] = temp;

                    objindex++;
                    stat = 0;
                }else//是-选项
                {
                    stat = 1;
                    int tindex = cmd.op_index[s[i].Substring(1)];
                    objindex = tindex;
                    if(tindex>ans.Count)
                    {
                        for (int j = 0; j < tindex-ans.Count; j++)
                        {
                            ans.Add(null);
                        }
                    }
                }
            }
            else //是参数
            {
                //判断类型str/int/long/float/vec3/dic{}
                double p;
                if (double.TryParse(s[i], out p))//int/long/float
                {
                    FP temp = new FP(p);
                    if (objindex >= ans.Count)
                        ans.Add(temp);
                    else ans[objindex] = temp;
                    
                }
                else if(s[i].StartsWith("("))//vec3
                {
                    if (objindex >= ans.Count)
                        ans.Add(JsonConvert.DeserializeObject<Vector3>(s[i]));
                    else ans[objindex] = JsonConvert.DeserializeObject<Vector3>(s[i]);
                }
                else if(s[i].StartsWith("{"))
                {
                    //dic{}类型
                    Dictionary<string,object> temp;
                    try
                    {
                        temp=JsonConvert.DeserializeObject<Dictionary<string,object>>(s[i]);
                        ans.Add(temp);
                    }
                    catch (System.Exception)
                    {
                        
                        
                    }
                    
                }
                else
                {
                    if (objindex >= ans.Count)
                        ans.Add(s[i]);
                    else ans[objindex] = s[i];
                }
                objindex++;
                stat = 0;
            }
        }
        return ans.ToArray();
    }

    public string ToSimpleStr()
    {
        return null;
    }
}
