using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class Container_MapPrefabData : BaseContainer
{
    static List<MapPrefabsData> mapdata = new List<MapPrefabsData>();
    static List<MapGraphData> graphData = new List<MapGraphData>();

    Dictionary<string, MapPrefabsData> mapdataDic = new Dictionary<string, MapPrefabsData>();//属于本存档的mapprefabs
    Dictionary<string, MapGraphData> graphdataDic = new Dictionary<string, MapGraphData>();//属于本存档的mapgraph

    public MapPrefabsData currentMap;//存在map文件夹的proper.txt里
    //[Obsolete]
    string selectedMapIndex="";

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.RegistFunc<MapPrefabsData[]>(nameof(EventNames.GetMapprefabDatas), GetAllMapPrefabsData);
        e.RegistFunc<MapGraphData[]>(nameof(EventNames.GetMapGraphDatas), GetAllMapGraphData);
        e.RegistFunc<string, MapGraphData>(nameof(EventNames.GetGraphDataByName), GetGraphDataByName);
        e.RegistFunc<string, MapPrefabsData>(nameof(EventNames.GetMapDataByName), GetMapDataByName);
        e.RegistFunc<MapPrefabsData, MapPrefabsData, bool>(nameof(EventNames.IsMapNearBy), IsNearBy);
        //e.RegistFunc<MapPrefabsData>(nameof(EventNames.GetCurrentMapPrefabs),()=>{return currentMap;});
        e.ListenEvent<string>(nameof(EventNames.StartGameFromMap),StartGame);
    }

    public override void OnArriveInGameScene()
    {
        base.OnArriveInGameScene();
        selectedMapIndex = ContainerManager.GetContainer<Container_PlayerData>().GetMapIndex();
        bool isIngame = ContainerManager.GetContainer<Container_SaveData>().IsInGame();
        Debug.Log("进入地图：" + selectedMapIndex);

        if (!isIngame&&selectedMapIndex!="test")//新地图
        {
            Debug.Log("新建地图");
            //从loadermap中找到index对应的mapprefabs
            currentMap = GetMapDataByName(selectedMapIndex);
            selectedMapIndex = "";
        }
        else//没有选择map，代表map已经在文件中
        {
            Debug.Log("读取现有map");
            string mapprefab = FileSaver.GetCurrentMapPrefabsData(center.GetParm<string>(nameof(EventNames.ThisSavePath)));
            //如果读取失败，则代表文件损毁或是测试直接从ingame进入，此时直接生成new的mapprefabsdata
            currentMap = MapPrefabsData.FromString(mapprefab);
            if (currentMap == null) currentMap = new MapPrefabsData("test");
        }
    }
    

    public override void OnLoadSave(SaveData save)
    {
        base.OnLoadSave(save);
        //如果从存档文件夹下没有找到mapprefabs
        //就从loader找，并复制到map文件夹下
        string s = FileSaver.GetAllMapPrefabsData(save.savePath);
        if(string.IsNullOrEmpty(s))
        {
            Loader_MapPrefabsData mp = LoaderManager.GetLoader<Loader_MapPrefabsData>();
            s = JsonConvert.SerializeObject( mp.GetAllMapPrefabsData(),JsonSetting.serializerSettings);

        }
        mapdata =new List<MapPrefabsData>( JsonConvert.DeserializeObject<MapPrefabsData[]>(s,JsonSetting.serializerSettings));
        for (int i = 0; i < mapdata.Count; i++)
        {
            mapdataDic.Add(mapdata[i].mapName, mapdata[i]);
        }

        s = FileSaver.GetAllMapGraphData(save.savePath);
        if (string.IsNullOrEmpty(s))
        {
            Loader_MapPrefabsData mp = LoaderManager.GetLoader<Loader_MapPrefabsData>();
            s = JsonConvert.SerializeObject(mp.GetAllMapGraphData(), JsonSetting.serializerSettings);

        }
        graphData = new List<MapGraphData>(JsonConvert.DeserializeObject<MapGraphData[]>(s, JsonSetting.serializerSettings));
        for (int i = 0; i < graphData.Count; i++)
        {
            graphdataDic.Add(graphData[i].mapName, graphData[i]);
        }
    }
    public override void OnLoadGame(MapPrefabsData data,int index)
    {
        if (index != 0) return;
        base.OnLoadGame(data,index);
        if (data == null)//正常不会走这
        {
            //currentMap = JsonUtility.FromJson<MapPrefabsData>(FileSaver.GetMapPrefabsData(center.GetParm<string>(nameof(EventNames.ThisSavePath))));
        }
        else
        {
            currentMap = data;

            //建立map文件夹
            string savepath = center.GetParm<string>(nameof(EventNames.ThisSavePath));
            FileSaver.CreateMapFolder(savepath);
            FileSaver.SaveMapPrefabsData(savepath, currentMap.ToString());
        }

        center.RegistFunc<long>(nameof(EventNames.GetMaxUUID), GetMaxUUID);
        center.RegistFunc<long>(nameof(EventNames.GetMapSize), GetMapSize);
        center.RegistFunc<MapPrefabsData>(nameof(EventNames.GetCurrentMapPrefabs), GetCurrentMapPrefabs);
    }
    public override void UnLoadGame(int ind)
    {
        if (ind != 2) return;
        base.UnLoadGame(ind);
        center.UnRegistFunc<long>(nameof(EventNames.GetMaxUUID), GetMaxUUID);
        center.UnRegistFunc<long>(nameof(EventNames.GetMapSize), GetMapSize);

        string savepath = center.GetParm<string>(nameof(EventNames.ThisSavePath));
        //bool avaliable = false;
        FileSaver.DeleteMapFolder(savepath);
        /*if (center.GetParm<bool>(nameof(EventNames.IsInGame), out avaliable))
        {
            if (avaliable)
                
        }*/
        //currentMap = null;
    }
    public override void Save(string path)
    {
        base.Save(path);
        Debug.Log("mapprefab保存开始");
        string savepath = center.GetParm<string>(nameof(EventNames.ThisSavePath));
        bool avaliable = false;
        if (center.GetParm<bool>(nameof(EventNames.IsInGame), out avaliable))
        {
            if(avaliable)
                FileSaver.SaveMapPrefabsData(savepath, currentMap.ToString());
        }
        FileSaver.SetAllMapPrefabsData(savepath, JsonConvert.SerializeObject(mapdata, JsonSetting.serializerSettings));
        FileSaver.SetAllMapGraphData(savepath, JsonConvert.SerializeObject(graphData, JsonSetting.serializerSettings));
    }
    public override void UnLoadSave()
    {
        base.UnLoadSave();
        
    }
    [Obsolete]
    public void SetSelected(string index)//当点选map按钮时
    {
        selectedMapIndex = index;
       
    }

    #region getsetMapprefabs



    #region 外部访问
    public void StartGame(string name)
    {
        bool avaliable = false;
        if (center.GetParm<bool>(nameof(EventNames.IsInGame), out avaliable))
        {
            if(avaliable){return;}
        }
        if(mapdataDic.ContainsKey(name))
        {
            currentMap=DataExtension.DeepCopy(mapdataDic[name]); 
            EventCenter.WorldCenter.SendEvent<string>(nameof(EventNames.JumpToScene),ConstantValue.ingameSceneName);
        }
    }


    public MapPrefabsData[] GetRoute(MapPrefabsData source, MapPrefabsData target)
    {//返回起点到终点需经过的路径
        return null;
    }
    public Resource_Data GetRouteCost(string source, string target)
    {
        return GetRouteCost(mapdataDic[source], mapdataDic[target]);
    }
    public Resource_Data GetRouteCost(MapPrefabsData source, MapPrefabsData target)
    {//返回起点到终点需消耗的物资
        if (!IsNearBy(source, target)) { Debug.Log("不相领"); return null; }
        //找到起点所有邻接点
        //找到邻接点中目的点，返回消耗
        //
        MapGraphData mg = null;
        for (int i = 0; i < graphData.Count; i++)
        {
            if (graphData[i].mapName == source.mapName) { mg = graphData[i]; break; }
        }
        if (mg == null) { Debug.Log("没有" + source.mapName + "这个graph节点"); return null; }
        for (int i = 0; i < mg.edges.Length; i++)
        {
            if (mg.edges[i].target == target.mapName)
            {
                Debug.Log("没有" + target.mapName + "这个目标节点");
                return mg.edges[i].moveCost;
            }
        }
        return null;
    }
    public DateTime GetRouteTimeCost(MapPrefabsData source, MapPrefabsData target)
    {//返回起点到终点需消耗的时间
        return DateTime.Now;
    }
    public bool IsNearBy(MapPrefabsData source, MapPrefabsData target)
    {
        bool b = false;
        MapGraphData mg = null;
        for (int i = 0; i < graphData.Count; i++)
        {
            if (graphData[i].mapName == source.mapName) { mg = graphData[i]; break; }
        }
        if (mg == null) return b;
        for (int i = 0; i < mg.edges.Length; i++)
        {
            if (mg.edges[i].target == target.mapName) return true;
        }
        return b;
    }
    public bool IsNearBy(string source, string target)
    {
        bool b = false;
        MapGraphData mg = null;
        for (int i = 0; i < graphData.Count; i++)
        {
            if (graphData[i].mapName == source) { mg = graphData[i]; break; }
        }
        if (mg == null) { Debug.Log(source + "=null"); return b; }
        for (int i = 0; i < mg.edges.Length; i++)
        {
            if (mg.edges[i].target == target) { return true; }
        }
        Debug.Log(target + "=null");
        return b;
    }
    public MapGraphData GetGraphDataByName(string name)
    {
        return graphdataDic[name];
    }
    public MapPrefabsData GetMapDataByName(string name)
    {
        try
        {
            return new MapPrefabsData(mapdataDic[name]);
        }
        catch (System.Exception)
        {
            Debug.Log("获取测试地图" + name);
            return new MapPrefabsData();
        }

    }

    public MapPrefabsData GetMapPrefabsDataByIndex(int index)
    {
        try
        {
            return new MapPrefabsData(mapdata[Mathf.Clamp(index, 0, mapdata.Count - 1)]);
        }
        catch (System.Exception)
        {
            Debug.LogError(index + "在mapdata中不存在,mapdata的size=" + mapdata.Count);
            throw;
        }

    }
    public MapPrefabsData[] GetAllMapPrefabsData()
    {
        try
        {
            return mapdata.ToArray();
        }
        catch (System.Exception)
        {

            throw;
        }

    }
    public MapGraphData[] GetAllMapGraphData()
    {
        try
        {
            return graphData.ToArray();
        }
        catch (System.Exception)
        {

            throw;
        }

    }

    #endregion

    long GetMaxUUID()
    {
        return currentMap.GetMaxUUID();
    }
    long GetMapSize()
    {
        return XYHelper.ToLongXY(currentMap.config.width, currentMap.config.height);
    }
    MapPrefabsData GetCurrentMapPrefabs()
    {
        return currentMap;
    }

    #endregion
}
