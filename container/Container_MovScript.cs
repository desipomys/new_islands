using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
//只存储实例不去文件中读取
public class Container_MovScript : BaseContainer
{   
    /// <summary>
    /// 存档中在运行的引擎，都是并行运行的
    /// </summary>
    public Dictionary<string,MovScriptEngine> allengines=new Dictionary<string, MovScriptEngine>();

    

    public override void OnLoadSave(SaveData sd)
    {
        base.OnLoadSave(sd);
        //如果存档里有engine文件则读取，否则从loader的satrt中获取
        if (FileSaver.CheckSaveEngineLoaded(sd.savePath))
        {
            //反序列化
            string s = FileSaver.GetSaveEngineData(sd.savePath);
            allengines = JsonConvert.DeserializeObject<Dictionary<string, MovScriptEngine>>(s,JsonSetting.serializerSettings);
            initAllEng();
        }
        else
        {
            MovScriptEngine temp = center.GetParm<string, MovScriptEngine>(nameof(EventNames.GetOriginMovEngineByName), ConstantValue.startSceneName);
            if (temp != null)
            {
                allengines.Add(ConstantValue.startSceneName, temp);
            }
            initAllEng();
        }
    }
    void initAllEng()
    {
        foreach (var item in allengines)
        {
            item.Value.hostCenter=center;
            item.Value.OnInit(null);
            
        }
    }
    public override void UnLoadSave()
    {
        base.UnLoadSave();
        allengines.Clear();
    }
    public override void Save(string path)
    {
        base.Save(path);
        //整个字典存到存档的moveng文件这
        FileSaver.SaveMovEngineData(path, JsonConvert.SerializeObject(allengines, JsonSetting.serializerSettings));

        if(center.GetParm<bool>(nameof(EventNames.IsInGame)))
        {
            MapPrefabsData mpd=center.GetParm<MapPrefabsData>(nameof(EventNames.GetCurrentMapPrefabs));
            List<string> engs;
            if(mpd.data==null) return;
            if (!mpd.data.ContainsKey(MapPrefabsDataType.MovEngine))return;

            engs=new List<string>(mpd.data[MapPrefabsDataType.MovEngine].Convert<string>().Split(','));
            Dictionary<string,MovScriptEngine> ans=new Dictionary<string, MovScriptEngine>();
            foreach (var item in allengines)
            {
                if(engs.Contains(item.Key))
                {
                    ans.Add(item.Key,item.Value);
                }
            }
           
            FileSaver.SetCurrentMapEngineData(path, JsonConvert.SerializeObject(ans, JsonSetting.serializerSettings));
  
        }
    }
    public override void OnLoadGame(MapPrefabsData data, int index)
    {
        if (index != 3) return;
        base.OnLoadGame(data, index);
        //将属于此map的engine存到GenedMap中
        if(data.generated)
        {
            SaveData sd = center.GetParm<SaveData>(nameof(EventNames.GetCurrentSaveData));
            string s = FileSaver.GetGenedMapEngine(sd.savePath,data.mapName);
            if(!string.IsNullOrEmpty(s))
            {
                Dictionary<string,MovScriptEngine> temp = JsonConvert.DeserializeObject<Dictionary<string,MovScriptEngine>>(s, JsonSetting.serializerSettings);

                foreach (var item in temp)
                {
                    allengines.Add(item.Key,item.Value);
                }
                
            }
        }else
        { 
            List<string> engs;
            if (data.data==null) return; 
            if (!data.data.ContainsKey(MapPrefabsDataType.MovEngine))return;
            try
            {
                engs=new List<string>(data.data[MapPrefabsDataType.MovEngine].Convert<string>().Split(','));
                foreach (var item in engs)
                {
                    allengines.Add(data.mapName, LoaderManager.GetLoader<Loader_MovEngine>().GetMovEngineByName(item));
                }
            }
            catch (System.Exception)
            {
                
            }
           
            
        }
    }
    public override void UnLoadGame(int ind)
    {
        if (ind != 0) return;
        base.UnLoadGame(ind);
        MapPrefabsData mpd = ContainerManager.GetContainer<Container_MapPrefabData>().currentMap;
        if (mpd == null) { Debug.LogError("当前MPD为空");return; }

        List<string> engs;
        if(!mpd.data.ContainsKey(MapPrefabsDataType.MovEngine))return;

        engs=new List<string>(mpd.data[MapPrefabsDataType.MovEngine].Convert<string>().Split(','));
        
        foreach (var item in allengines)
        {
            if(engs.Contains(item.Key))
            {
                item.Value.OnLeave();
            }
        }

        foreach (var item in engs)
        {
            allengines.Remove(item);
        }
        
        //不触发结算，结算要调用movcommand才触发
    }

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        //MovScriptCommandResolver.Init();
        e.RegistFunc<string,MovScriptEngine>(nameof(EventNames.GetMovEngineByName) ,GetEngine);
        e.RegistFunc<string[]>(nameof(EventNames.GetAllMovEngineName),GetAllEngineName);
    }

    
   public override void OnUpdate()
    {

        foreach (var item in allengines.Values)
        {
            item.OnUpdate();
        }
        
    }

    public T GetEngineData<T>(string path,string enginename,out bool succ)
    {
        MovScriptEngine engine;
        if(allengines.TryGetValue(enginename,out engine))
        {
            succ=true;
            return engine.GetData(path).Convert<T>();
        }
        succ=false;
        return default(T);
    }
    public void SetEngineData<T>(string path,T d,string enginename)
    {
        MovScriptEngine engine;
        if(allengines.TryGetValue(enginename,out engine))
        {
            engine.SetData(path,d);
        }
    }
    public void AddEngine(string enginename, string data)
    {
        allengines.Add(enginename, MovScriptEngine.DeSerailize(data));
    }
    public void AddEngine(string enginename, MovScriptEngine data)
    {
        allengines.Add(enginename, data);
    }
    public void DeleteEngine(string enginename)
    {
        MovScriptEngine engine;
        if (allengines.TryGetValue(enginename, out engine))
        {
            engine.OnDelete();
        }
    }
    public MovScriptEngine GetEngine(string name)
    {
        MovScriptEngine engine;
        if(allengines.TryGetValue(name,out engine))
        {
            return engine;
        }
        return engine;
    }
    public string[] GetAllEngineName()
    {
        List<string> names=new List<string>();
        names.AddRange(allengines.Keys);
        return names.ToArray();
    }


    public override string ToString()
    { 
        return JsonConvert.SerializeObject(allengines,JsonSetting.serializerSettings);
    }
    public void FromString(string data)
    {
        allengines = JsonConvert.DeserializeObject<Dictionary<string, MovScriptEngine>>(data,JsonSetting.serializerSettings);
    }
}