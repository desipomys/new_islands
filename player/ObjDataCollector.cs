using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class ObjDataCollector : MonoBehaviour,IEventRegister
{
    public void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }

    public void OnEventRegist(EventCenter e)
    {
        e.RegistFunc<string>(nameof(EntityBlockEventNames.saveStr), ToString);
        e.RegistFunc<object>(nameof(PlayerEventName.save), ToObject);
        e.ListenEvent<object>(nameof(PlayerEventName.load), FromObject);
        e.ListenEvent<string>(nameof(EntityBlockEventNames.loadStr), FromString);
    }
    public override string ToString()
    {
        return ((JObject)ToObject()).ToString();
    }
    public object ToObject()
    {
        //返回dic的序列化数据，dic key为组件名，value为调用组件tostring的值
        Dictionary<string,object> datadic=new Dictionary<string,object>();
        IDataContainer[] alldata=GetComponents<IDataContainer>();
        for (int i = 0; i < alldata.Length; i++)
        {
            datadic.Add(alldata[i].GetType().Name,alldata[i].ToObject());
        }
        return JObject.FromObject(datadic);
    }
    public void FromObject(object dat)
    {
        if (dat == null) return;
        //序列化为dic,用此字典调用所有idatacontainer组件的fromstring
        //组件自取key对应的value
        //string s = (string)(dat);
         Dictionary<string,object> datadic=((JObject)dat).ToObject<Dictionary<string,object>>();
        List<IDataContainer> alldata=new List<IDataContainer>(GetComponents<IDataContainer>());//prefabs上现有的组件
        sort(alldata, false);

        for (int i = 0; i < alldata.Count; i++)
        {
            Debug.Log(alldata[i].GetType().Name+":"+alldata[i].GetDataCollectPrio);
            if(datadic.ContainsKey(alldata[i].GetType().Name))
            {
                alldata[i].FromObject(datadic[alldata[i].GetType().Name]);
                datadic.Remove(alldata[i].GetType().Name);
            }
        }
        foreach(var i in datadic)//数据中有，但prefab中没有的要addcomponent
        {
           IDataContainer idc= gameObject.AddComponent(Type.GetType(i.Key)) as IDataContainer;
            idc.FromObject(i.Value);
        }
    }
    public void FromString(string data)
    {
        if(string.IsNullOrEmpty(data))return;
        JObject j = JObject.Parse(data);
        FromObject(j);
    }

    void sort(List<IDataContainer> idc, bool bigfrist)
    {
        for (int i = 0; i < idc.Count; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if((idc[i].GetDataCollectPrio<idc[j].GetDataCollectPrio)==bigfrist)
                {
                    IDataContainer sw = idc[i];
                    idc[i] = idc[j];
                    idc[j] = sw;
                }
            }
        }
    }
}
