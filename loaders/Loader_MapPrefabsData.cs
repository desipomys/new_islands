//读取预制的ingame地图数据并生成该地图engine到contianer
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class Loader_MapPrefabsData : BaseLoader
{
    
    static List<MapPrefabsData> mapdata=new List<MapPrefabsData>();
    static List<MapGraphData> graphData = new List<MapGraphData>();

    Dictionary<string, MapPrefabsData> mapdataDic = new Dictionary<string, MapPrefabsData>();
    Dictionary<string, MapGraphData> graphdataDic = new Dictionary<string, MapGraphData>();

    #region Init
    public override void OnEventRegist(EventCenter e)
    {
       center=e;
        //e.RegistFunc<string,Texture>("StrtoTexture",StrToTexture);
        //Debug.Log("textureloaderinit");
       
    }

    string path= "SC/MAPPREFAB";
    string gpath = "SC/MAPGRAPH";

    public override void OnLoaderInit(int prio)
    {
        if(prio!=0)return;
        //刚打开游戏时是从scripableObject加载，在进入存档后从存档的txt中加载
        LoadPrefabs();
        LoadGraph();
    }
    void LoadPrefabs()
    {
        try
        {
            MapPrefabsData[] t = Resources.LoadAll<MapPrefabsData>(path);
            Debug.Log(t.Length);
            HashSet<string> mapnames = new HashSet<string>();
            foreach (var item in t)
            {
                MapPrefabsData mpd = new MapPrefabsData( item);
                
                if (!string.IsNullOrEmpty(mpd.mapName) && !mapnames.Contains(mpd.mapName))
                {
                    mapdata.Add(mpd);
                    mapnames.Add(mpd.mapName);
                    mapdataDic.Add(mpd.mapName, mpd);
                }
            }
            //string t=Resources.Load<TextAsset>(path).text;
           /*  MapPrefabsData[] mapdatas=JsonConvert.DeserializeObject<MapPrefabsData[]>(t);
            HashSet<string> mapnames = new HashSet<string>();
            for (int i = 0; i < mapdatas.Length; i++)
            {
                if (!string.IsNullOrEmpty(mapdatas[i].mapName)&&!mapnames.Contains(mapdatas[i].mapName))
                {
                    mapdata.Add(mapdatas[i]);
                    mapnames.Add(mapdatas[i].mapName);
                    mapdataDic.Add(mapdatas[i].mapName, mapdatas[i]);
                }
            }*/
            Debug.Log("mapprefabs loader 加载完成，有" + mapdata.Count+"个不重复的map，原"+t.Length+"个");
        }
        catch (System.Exception ex)
        {

            Debug.LogError("mapprefabs loader 预设加载失败"+ex.Message);
            //Debug.LogError()
        }
    }
    void LoadGraph()//加载用于开始界面的地图关卡连接关系
    {
        try
        {
            MapGraphData[] t = Resources.LoadAll<MapGraphData>(gpath);
            HashSet<string> mapnames = new HashSet<string>();
            foreach (var item in t)
            {
                MapGraphData mpd = item;
                
                if (!string.IsNullOrEmpty(mpd.mapName) && !mapnames.Contains(mpd.mapName))
                {
                    graphData.Add(mpd);
                    mapnames.Add(mpd.mapName);
                    graphdataDic.Add(mpd.mapName, mpd);
                }
            }

            /*MapGraphData[] mapGraphdatas = JsonConvert.DeserializeObject<MapGraphData[]>(t);
            
            for (int i = 0; i < mapGraphdatas.Length; i++)
            {
                if (!string.IsNullOrEmpty(mapGraphdatas[i].mapName) && !mapnames.Contains(mapGraphdatas[i].mapName))
                {
                    graphData.Add(mapGraphdatas[i]);
                    mapnames.Add(mapGraphdatas[i].mapName);
                    graphdataDic.Add(mapGraphdatas[i].mapName, mapGraphdatas[i]);
                }
            }*/
            Debug.Log("mapgraph loader 图完成，" + mapdata.Count + "个map共有"+graphData.Count+"个不重复连接数据，原"+t.Length+"个");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("mapgraph loader 图加载失败" + ex.Message);
        }
    }

    #endregion

   
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
   
}

public enum MapPrefabsDataType
{
    MaxTech,//int
    MovEngine,//str,以,分割
}

