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
    public void UIInit(UICenter center, BaseUIView view)//���ڲ���container����Դ����
    {
        //��ȡmapgraph���map�����������ǰ��ֻλ�ã���ʾ�ڽ���
        //�ɽ��зֿ���ʾ�Ż�
       
    }

    #region �����¼�
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

        //���ɽڵ�
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

        //���ɽڵ������
        for (int i = 0; i < mpg.Length; i++)
        {
            for (int j = 0; j < mpg[i].edges.Length; j++)
            {
                RectTransform source = mapNodes[mpg[i].mapName].GetComponent<RectTransform>();
                RectTransform target = mapNodes[mpg[i].edges[j].target].GetComponent<RectTransform>();
                drawLine(source, target);
            }
        }

        //���ɴ�ֻ����λ��
        string shipatMap = EventCenter.WorldCenter.GetParm<string>(nameof(Container_PlayerData_EventNames.GetMapIndex));
        Debug.Log(shipatMap);
        if (string.IsNullOrEmpty(shipatMap)) { shipatMap = mpd[0].mapName; EventCenter.WorldCenter.SendEvent<string>(nameof(Container_PlayerData_EventNames.MovShipIndex),shipatMap); }
        shipPos.GetChild(0).position = mapNodes[shipatMap].GetComponent<RectTransform>().position;

        //mapInfoDetail.Listen(OnGoToHit);
        //mapInfoDetail.Hide();
    }
    public void OnNodeClick(MapPrefabsData mpd,RectTransform button)
    {
        //��ʾ������Ĺؿ���ϸ��
        Debug.Log(mpd.mapName + "�����");
        dragmap.OnMapNodeClick(button);//�ƶ��ڵ��Ŀ��ڵ�shower
        //mapInfoDetail.Show(mpd,button);

        //���������ڵ㲻��ֱ�ӵ�������ʾgo
        string shipatMap = EventCenter.WorldCenter.GetParm<string>(nameof(Container_PlayerData_EventNames.GetMapIndex));
        Debug.Log(shipatMap);
        if (mpd.mapName==shipatMap)
        {
           // mapInfoDetail.HideGo(false,"��½");
           
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
        //��ֻ�ƶ���ָ��㣬�����ͼ����·������maingame_view�ȸ�UI����
        //���������Ǵ�ֻ���ڵ����½���ø�UI�ж�
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
            foreach (var item in mapNodes)//ȡ��һ��mapnode
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
        //���������¼����ĵĴ�ֻλ�øı��¼�����ͼ�䶯�¼�
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
