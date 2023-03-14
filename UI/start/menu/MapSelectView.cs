using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[Obsolete]
public class MapSelectView : Base_UIComponent,IUIInitReciver,IUIOpenReciver
{
    public MapInfoDetailShower mapInfoDetail;
    GameObject mapNode;
    MapSelectView_DragMap dragmap;

    Dictionary<string, MapNodeShower> mapNodes=new Dictionary<string, MapNodeShower>();

    Action<string> onLeaveIsLand, onLandAtIsland,onGoTo;
    Action<string, string> onArriveIsland, OnTravel;

    string path = "Prefabs/UI/Start/unit/MapNode";
    public void UIInit(UICenter center, BaseUIView view)//先于部分container的资源加载
    {
        //获取mapgraph里的map连线情况、当前船只位置，显示于界面
        //可进行分块显示优化
       
    }

    #region 监听事件
    /*public void  ListenLeaveIsland(Action<string> a)
    {
        onLeaveIsLand = a;
    }
    public void ListenArriveIsland(Action<string, string> a)
    {
        onArriveIsland = a;
    }
    public void ListenOnTravel(Action<string,string> a)
    {
        OnTravel = a;
    }*/
    public void ListenLandAtIsland(Action<string> a)
    {
        onLandAtIsland = a;
    }
    public void ListenOnGoTo(Action<string> a)
    {
        onGoTo = a;
    }

    #endregion
    void clear()
    {
        mapNodes.Clear();
    }
    void load(UICenter center, BaseUIView view)
    {
        mapNode = Resources.Load<GameObject>(path);
        dragmap = GetComponent<MapSelectView_DragMap>();
        MapPrefabsData[] mpd = EventCenter.WorldCenter.GetParm<MapPrefabsData[]>(nameof(EventNames.GetMapprefabDatas));
        MapGraphData[] mpg = EventCenter.WorldCenter.GetParm<MapGraphData[]>(nameof(EventNames.GetMapGraphDatas));
        RectTransform islandGroup = GetComponent<MapSelectView_DragMap>().islandGroup;
        RectTransform shipPos = GetComponent<MapSelectView_DragMap>().shippos;

        //生成节点
        for (int i = 0; i < mpd.Length; i++)
        {
            MapGraphData mapg=null;
            for (int j = 0; j < mpg.Length; j++)
            {
                if(mpg[j].mapName==mpd[i].mapName)
                {
                    mapg = mpg[j];
                    break;
                }
            }
            if (mapg != null)
            {
                GameObject temp = GameMainManager.CreateGameObject(mapNode, islandGroup.position, islandGroup.rotation, islandGroup);

                MapNodeShower mns = temp.GetComponent<MapNodeShower>();
                mns.ShowerInit(  this);
                mns.Show(mpd[i], mapg);
                mapNodes.Add(mpd[i].mapName,mns);
            }
        }

        //生成节点间连线
        for (int i = 0; i < mpg.Length; i++)
        {
            for (int j = 0; j < mpg[i].edges.Length; j++)
            {
                RectTransform source = mapNodes[mpg[i].mapName].GetComponent<RectTransform>();
                RectTransform target = mapNodes[mpg[i].edges[j].target].GetComponent<RectTransform>();
                drawLine(source, target);
            }
        }

        //生成船只所在位置
        string shipatMap = EventCenter.WorldCenter.GetParm<string>(nameof(Container_PlayerData_EventNames.GetMapIndex));
        Debug.Log(shipatMap);
        if (string.IsNullOrEmpty(shipatMap)) { shipatMap = mpd[0].mapName; EventCenter.WorldCenter.SendEvent<string>(nameof(Container_PlayerData_EventNames.MovShipIndex),shipatMap); }
        shipPos.GetChild(0).position = mapNodes[shipatMap].GetComponent<RectTransform>().position;

        //mapInfoDetail.Listen(OnGoToHit);
        //mapInfoDetail.Hide();
    }
    public void OnNodeClick(MapPrefabsData mpd,RectTransform button)
    {
        //显示被点击的关卡的细节
        Debug.Log(mpd.mapName + "被点击");
        dragmap.OnMapNodeClick(button);//移动节点框到目标节点shower
        //mapInfoDetail.Show(mpd,button);

        //如果被点击节点不可直接到达则不显示go
        string shipatMap = EventCenter.WorldCenter.GetParm<string>(nameof(Container_PlayerData_EventNames.GetMapIndex));
        Debug.Log(shipatMap);
        if (mpd.mapName==shipatMap)
        {
           // mapInfoDetail.HideGo(false,"登陆");
           
        }
        else
        {
            if (ContainerManager.GetContainer<Container_MapPrefabData>().IsNearBy(shipatMap,mpd.mapName))
            {
                //mapInfoDetail.HideGo(false);
            }
            else { //mapInfoDetail.HideGo(true);
            }
        }
       
    }
    public void OnGoToHit(string mapName)
    {
        //船只移动到指向点，需根据图计算路径，让maingame_view等父UI计算
        //如果点击的是船只所在点则登陆，让父UI判断
        onGoTo(mapName);
    }

    public void OnUIOpen(UICenter center, BaseUIView view)
    {
        Debug.Log("mapselectview init");
        return;
        load(center, view);

        SaveData sd = EventCenter.WorldCenter.GetParm<SaveData>(nameof(EventNames.GetCurrentSaveData));
        if (!string.IsNullOrEmpty(sd.shipAt))
        {
            dragmap.OnUIOpen(center, view, mapNodes[sd.shipAt]);
        }
        else
        {
            foreach (var item in mapNodes)//取第一个mapnode
            {
                dragmap.OnUIOpen(center, view, item.Value);
                break;
            }
            
        }

       
    }

    public void BreakMVConnect(string viewname, EventCenter model)
    {
        //
        //model.UnListenEvent<string>(nameof(Container_PlayerData_EventNames.OnMapIndexChg), OnShipMove);
        clear();
    }
    public void BuildMVConnect(string viewname, EventCenter model)
    {
        //监听世界事件中心的船只位置改变事件、地图变动事件
        return;
        //model.ListenEvent<string>(nameof(Container_PlayerData_EventNames.OnMapIndexChg), OnShipMove);
    }

    void drawLine(RectTransform source,RectTransform target)
    {

    }
    void OnShipMove(string mapname)
    {
        RectTransform shipPos = GetComponent<MapSelectView_DragMap>().shippos;
        shipPos.GetChild(0).position = mapNodes[mapname].GetComponent<RectTransform>().position;
        MapPrefabsData mpd=EventCenter.WorldCenter.GetParm<string,MapPrefabsData>(nameof(EventNames.GetMapDataByName),mapname);
        OnNodeClick(mpd, mapNodes[mapname].GetComponent<RectTransform>());
    }
}
