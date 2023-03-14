using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 只用于读取新的开始界面用的默认engine和每个地图上的engine，
/// </summary>
public class Loader_MovEngine : BaseLoader
{
    string path = "SC/MOVSCRIPT/engine";
    static Dictionary<string, MovScriptEngine> engines=new Dictionary<string, MovScriptEngine>();

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);

        e.RegistFunc<string, MovScriptEngine>(nameof(EventNames.GetOriginMovEngineByName), GetMovEngineByName);
    }

    public override void OnLoaderInit(int prio)//先于oneventregist执行
    {
        if (prio != 0) return;
        engines.Clear();
        try
        {
            MovScriptEngine[] all = Resources.LoadAll<MovScriptEngine>(path);
            for (int i = 0; i < all.Length; i++)
            {
                if(string.IsNullOrEmpty(all[i].Name))
                engines.Add(all[i].Name, all[i]);
                else engines.Add(all[i].name, all[i]);
            }
            Debug.Log(engines.Count+"个引擎加载成功");
        }
        catch (System.Exception)
        {
            Debug.Log("movengine加载失败");
            //throw;
        }


        //data=Resources.Load<TextAsset>(mapEnginePath).text;
        //mapengines=JsonConvert.DeserializeObject<Dictionary<string,MovScriptEngine>>(data);
    }
    /// <summary>
    /// 返回的是原始版本
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public MovScriptEngine GetMovEngineByName(string name)
    {
        if(engines.ContainsKey(name))
        return engines[name];
       else return null;
        //因为获取engine是直接返回引用，因此开始运行后loader存的engine会被改变
        //但是每次启动游戏都会重新读取一次原始版本
    }public string[] GetAllMovEngineName()
    {
        return new List<string>(engines.Keys).ToArray();
    }
   
}
