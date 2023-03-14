using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class SerializerHelper
{
     static JsonSerializerSettings Setting;
    public static JsonSerializerSettings setting
    {
        get
        {
            if(Setting==null)
            {
                Setting=new JsonSerializerSettings();
                Setting.TypeNameHandling = TypeNameHandling.Auto;
                Setting.Formatting = Formatting.Indented;
                Setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            }
            return Setting;
        }
        set{}
    }
}

public class DataWarper 
{
    
   
    public string name;
    public string data;

    public DataWarper() { }
    public DataWarper(object d)
    {
        Load(d);
    }
   public void Load(object d)
   {
        try
        {
            name=d.GetType().Name;
           data= JsonConvert.SerializeObject(d, Formatting.Indented);
        }
        catch (Exception)
        {
            name = "";
            data = "";
        }
       
   }
   public object UnLoad()
   {
       return JsonConvert.DeserializeObject(data, Type.GetType(name));
   }
   public T UnLoad<T>()
   {
       return JsonConvert.DeserializeObject<T>(data);
   }
   public override string ToString()
   {
       return JsonConvert.SerializeObject(this, Formatting.Indented);
   }

    public static DataWarper FromString(string d)
    {
        return JsonConvert.DeserializeObject<DataWarper>(d);
    }
}
