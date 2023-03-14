using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapGraphView : Base_UIComponent,IMVConnector,IPointerClickHandler
{
    public string GetShipPosName;
    public UI_ModelType modelType;

    Dictionary<string, long> mapNodesIndexs = new Dictionary<string, long>();
    CompShowerManager mapNodesManager = new CompShowerManager();
    CompShowerManager ShipManager = new CompShowerManager();
    CompShowerManager lineManager = new CompShowerManager();

    public RectTransform mapbg;
    public RectTransform nodes,shipNode;
    public RectTransform selectMask;
    public MapNodeShower nodePrefabs;
    public Base_Shower shipPrefabs,linePrefabs;
    public RectTransform lineNode;
    int shipindex;

    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);
        mapNodesManager.Init(nodePrefabs, nodes, this);
        ShipManager.Init(shipPrefabs, shipNode, this);
        ShipManager.Create(out shipindex);
        lineManager.Init(linePrefabs, lineNode, this);
    }

    void RecycleMapShower()
    {
        mapNodesManager.RecycleAll();
        mapNodesIndexs.Clear();
    }
    void RecycleLineShower()
    {
        lineManager.RecycleAll();

    }
    RectTransform GetNodeRect(string name)
    {
        if (!mapNodesIndexs.ContainsKey(name)) return null;
        return mapNodesManager.GetRect(mapNodesIndexs[name]);
    }

    Vector2 GetMapSize(MapGraphData[] mgds)
    {
        Vector2 v = new Vector2();
        float minx=float.MaxValue, miny= float.MaxValue, maxx=0, maxy=0;
        for (int i = 0; i < mgds.Length; i++)
        {
            minx = (minx > mgds[i].x ? mgds[i].x : minx);
            miny = (miny > mgds[i].y ? mgds[i].y : miny);
            maxx = (maxx < mgds[i].x ? mgds[i].x : maxx);
            maxy = (maxy < mgds[i].y ? mgds[i].y : maxy);
        }
        v.x =  maxx ;
        v.y = maxy ;
        return v;
    }
    
    public void FlushMap(MapPrefabsData[] mpds, MapGraphData[] mgds)
    {
        if (mpds == null || mgds == null) return;
        if (mpds.Length == 0 || mgds.Length == 0) return;
        //根据list确定整个地图的大小，设置背景大小
        //在相应位置生成地图节点
        //生成节点间连线
        RecycleMapShower();

        Vector2 v2 = GetMapSize(mgds);//临时写死地图大小
        mapbg.sizeDelta = new Vector2(1920,1080);

        for (int i = 0; i < mpds.Length; i++)
        {
            MapGraphData mapg = null;
            for (int j = 0; j < mgds.Length; j++)
            {
                if (mpds[i].mapName == mgds[j].mapName) { mapg = mgds[j];break; }
            }
            if (mapg != null)
            {
                int ind;
                GameObject temp = mapNodesManager.Create(out ind).gameObject;
                    //GameMainManager.CreateGameObject(nodePrefabs, nodes.position
                   // +new Vector3(mapg.x,mapg.y,0), nodes.rotation, nodes);

                MapNodeShower mns = temp.GetComponent<MapNodeShower>();
                mns.ShowerInit(this);
                mns.Show(mpds[i], mapg);
                mapNodesIndexs.Add(mpds[i].mapName, ind);
            }
        }
        OnNodeClick(mpds[0], GetNodeRect(mpds[0].mapName));//临时措施
    }
    public void FlushShip(PlayerData pd)
    {

    }
    public void OnShipMove(ShipPosChgParm parm)
    {
        RectTransform rt = GetNodeRect(parm.toMapName);
        if (rt == null) return;
        ShipManager.Get(shipindex).GetComponent<RectTransform>().position = rt.position;
    }

    public void OnNodeClick(MapPrefabsData mpd,RectTransform rt)
    {
        if (rt != null)
        { selectMask.position = rt.position; }
        else
        {
            selectMask.position = new Vector3(10000, 10000, 0);
        }
        fatherView.GetComponent<EventCenter>().SendEvent<MapPrefabsData>(nameof(UIMapViewEventName.OnMapNodeClick),mpd);
    }

    public void BuildMVConnect(string viewname, EventCenter model)
    {
        model.ListenEvent<MapPrefabsData[], MapGraphData[]>(GetDataSourceName, FlushMap);
        model.ListenEvent<ShipPosChgParm>(GetShipPosName, OnShipMove);

        controll.BuildMVConnect(viewname, model);

        FlushMap(ContainerManager.GetContainer<Container_MapPrefabData>().GetAllMapPrefabsData(),
            ContainerManager.GetContainer<Container_MapPrefabData>().GetAllMapGraphData());
        string shipatMap = EventCenter.WorldCenter.GetParm<string>(nameof(Container_PlayerData_EventNames.GetMapIndex));
        OnShipMove(new ShipPosChgParm("", shipatMap));
    }

    public void BreakMVConnect(string viewname, EventCenter model)
    {
        model.UnListenEvent<MapPrefabsData[], MapGraphData[]>(GetDataSourceName, FlushMap);
        model.UnListenEvent<ShipPosChgParm>(GetShipPosName, OnShipMove);

        controll.BreakMVConnect(viewname, model);
    }

    public UI_ModelType GetModelType()
    {
        return modelType;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnNodeClick(null, null);

    }
}
