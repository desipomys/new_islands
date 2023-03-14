using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public class LoaderManager : BaseManager
{
    static Dictionary<string,BaseLoader> loaders=new Dictionary<string,BaseLoader>();
    public static T GetLoader<T>()where T:BaseLoader
    {
        BaseLoader bc;
        loaders.TryGetValue(typeof(T).Name,out bc);
        return (T)bc;
    }

    public override void OnEventRegist(EventCenter e)
    {//e是worldcenter
        Debug.Log("生命周期事件：loaderinit");
        base.OnEventRegist(e);
        findClass();
        initLoaders(e);
        //e.ListenEvent<EventCenter>("LoaderInit", initLoaders);//由lifecycle调用
    }
    public override void AfterEventRegist()
    {
        base.AfterEventRegist();
        foreach (var item in loaders)
        {
            item.Value.AfterEventRegist();
        }
    }

    void findClass()//反射查找每一个继承BaseLoader的类实例化后加入字典
    {
        if(loaders.Count>0)return;
        Assembly assembly = this.GetType().Assembly;
        Type[] types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.IsSubclassOf(typeof(BaseLoader)) && !type.IsAbstract)
                {
                   loaders.Add(type.Name, (BaseLoader)assembly.CreateInstance(type.Name));
                }
        }
        
    }
   void initLoaders(EventCenter e)
   {
        HashSet<BaseLoader> eventRegisted = new HashSet<BaseLoader>();
       foreach (KeyValuePair<string,BaseLoader> item in loaders)
       {
           item.Value.OnLoaderInit(0);
            if (!eventRegisted.Contains(item.Value))
            {
                item.Value.OnEventRegist(e);
                eventRegisted.Add(item.Value);
            }
       }
        foreach (KeyValuePair<string, BaseLoader> item in loaders)
        {
            item.Value.OnLoaderInit(1);
            if (!eventRegisted.Contains(item.Value))
            {
                item.Value.OnEventRegist(e);
                eventRegisted.Add(item.Value);
            }
        }
        foreach (KeyValuePair<string, BaseLoader> item in loaders)
        {
            item.Value.OnLoaderInit(2);
            if (!eventRegisted.Contains(item.Value))
            {
                item.Value.OnEventRegist(e);
                eventRegisted.Add(item.Value);
            }
        }
        foreach (KeyValuePair<string, BaseLoader> item in loaders)
        {
            item.Value.OnLoaderInit(3);
            if (!eventRegisted.Contains(item.Value))
            {
                item.Value.OnEventRegist(e);
                eventRegisted.Add(item.Value);
            }
        }
        Debug.Log("loader资源加载完成");
       
        Debug.Log("loader 事件注册完成");
    }
}