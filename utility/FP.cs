using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Reflection;


[Serializable]
/// <summary>
/// 通用数据类型
/// </summary>

[JsonObject(MemberSerialization.OptOut)]
public class FP : ISerializationCallbackReceiver,ValueSource_base
{
   

    public IEnumerable<string> TypeList()
    {

        return new []
        {
          typeof(System.Int32).Name,
          typeof(float).Name,
          typeof(long).Name,
            typeof(string).Name,
            typeof(Item).Name,
            typeof(System.Int32[]).Name,
            typeof(System.Single[]).Name,
            typeof(Vector3).Name,
            typeof(BaseBuff).Name,
        };
    }
    void onChg() { Debug.Log(dataType);
        switch (dataType)
        {
            case "Int32":
                data = 0;
                break;
            case "Int64":
                data = (long)0;
                break;
            case "Int32[]":
                data = new int[] { 0 };
                break;
            case "String":
                data = "";
                break;
            case "Single":
                data = 0.0f;
                break;
            case "Float[]":
                data = new float[] { 0};
                break;
            case "Item":
                data = new Item();
                break;
            case "Vector3":
                data = new Vector3();
                break;
            case "BaseBuff":
                data = new BaseBuff();
                break;
            default:
                data = 0;
                break;
        }

    }
   

    //懒加载模式，当从str反序列化回来后
    //obj可能变为jobj,decimal,str,jarray
    //只有当有人要用这个fp时，obj才会根据来取的类型再次反射变为具体类型

    [OnValueChanged("onChg")]
    [ValueDropdown("TypeList")]
    //[SerializeReference]
    [ShowInInspector]
    [NonSerialized,OdinSerialize]
    public string dataType;

    //[SerializeReference]  
    [ShowInInspector]
    [JsonIgnore]
   
    public object data;

    [JsonProperty]
    [HideInInspector]
    public object Data { get {  return data; }  set {data = value; synObjAndType(); } }
    bool isTyped = false;
    public FP() { }
    public FP(object p) { data = p; }
    public FP(FP p) { Data = DataExtension.DeepCopy( p.data); }

    /// <summary>
    /// 将obj的data转化为具体类对象
    /// </summary>
    /// <param name="v"></param>
    void Check(Type v)
    {
        if(isTyped)
        {
            return;
        }
        else
        {
            Type t = data.GetType();
            switch (t.Name)
            {
                case "Int64":
                    
                case "Float":
                    
                case "String":
                    isTyped = true;
                    break;
                case "JObject":
                    data = ((JObject)data).ToObject(v);
                    isTyped = true;
                    break;
                case "JArray":
                    if (v.IsArray)
                    {
                        data = ((JArray)data).ToObject(v);
                        isTyped = true;
                    }
                    else throw new Exception("尝试取数组类型为非数组类型");
                    break;
                default:
                    break;
            }
            synObjAndType();
        }
    }
    
    /// <summary>
    /// 只支持值类型add
    /// </summary>
    /// <param name="p"></param>
    /// <param name="t"></param>
    public void Add(FP p,Type t)
    {
        switch (t.Name)
        {
            case "Int32":
                data = Convert<int>() + p.Convert<int>();
                break;
            case "Single":
                Debug.Log(p.Convert<float>()+"+"+ Convert<float>()+"="+data);
                data = Convert<float>() + p.Convert<float>();
                Debug.Log(Convert<float>());
                break;
            case "Double":
                data = Convert<double>() + p.Convert<double>();
                break;
            case "Int64":
                data = Convert<long>() + p.Convert<long>();
                break;
            default:
                break;
        }
        
    }

        public T Convert<T>()
        {
            Check(typeof(T));
            return (T)System.Convert.ChangeType(data,typeof(T));
        }

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        synObjAndType();
    }
    void synObjAndType()
    {
        if (data == null) { dataType = ""; return; }
        dataType = data.GetType().Name;
        Debug.Log(dataType);
    }

    public FP[] Gets(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        return new FP[] { this };
    }
     public FP Get(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        return this ;
    }

    public void init()
    {
        
    }

    public ValueSource_base Copy()
    {
        return new FP(this);
    }
    public override string ToString()
    {
        return data.ToString();
    }

    public void FromObject(dynamic dy)
    {
       
    }

    /// <summary>
    /// 自定义隐式类型转换
    /// </summary>
    /// <param name="T("></param>
    public static implicit operator int (FP s){  return s.Convert<int>();}
    public static implicit operator int[](FP s) { return s.Convert<int[]>(); }
    public static implicit operator FP(int s) { FP TEMP = new FP(); TEMP.Data = s; return TEMP; }
    public static implicit operator FP(int[] s) { FP TEMP = new FP(); TEMP.Data = s; return TEMP; }

    public static implicit operator float (FP s){  return s.Convert<float>();}
    public static implicit operator float[](FP s) { return s.Convert<float[]>(); }
    public static implicit operator FP(float s) { FP TEMP = new FP();TEMP.Data = s; return TEMP; }
    public static implicit operator FP(float[] s) { FP TEMP = new FP(); TEMP.Data = s; return TEMP; }

    public static implicit operator string(FP s) { return s.Convert<string>(); }
    public static implicit operator string[] (FP s) { return s.Convert<string[]>(); }
    public static implicit operator FP(string s) { FP TEMP = new FP(); TEMP.Data = s; return TEMP; }
    public static implicit operator FP(string[] s) { FP TEMP = new FP(); TEMP.Data = s; return TEMP; }

    public static implicit operator long (FP s){  return s.Convert<long>();}
    public static implicit operator long[](FP s) { return s.Convert<long[]>(); }
    public static implicit operator FP(long s) { FP TEMP = new FP(); TEMP.Data = s; return TEMP; }
    public static implicit operator FP(long[] s) { FP TEMP = new FP(); TEMP.Data = s; return TEMP; }

    public static implicit operator Item(FP s) { s.Check(typeof(Item));return (Item)s.data; }
    public static implicit operator FP(Item s) { FP TEMP = new FP(); TEMP.Data = s; return TEMP; }
    public static implicit operator Item[](FP s) { s.Check(typeof(Item[])); return (Item[])s.data; }
    public static implicit operator FP(Item[] s) { FP TEMP = new FP(); TEMP.Data = s; return TEMP; }
    //public static implicit operator DamageType (FP s){  return (DamageType)Enum.Parse(typeof(DamageType),s.data); }
    //public static implicit operator Item (FP s){  return JsonConvert.DeserializeObject<Item>(s.data);}
    
}
