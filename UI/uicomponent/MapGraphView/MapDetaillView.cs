using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIMapViewEventName
{
    OnMapNodeClick, GoToOrEnterIsland
}
public class MapDetaillView : Base_UIComponent, IMVConnector
{
    public UI_ModelType modelType;
    public ResourceShower resShower;
    public MapInfoDetailShower mapInfoDetail;
    MapPrefabsData currentMap;
    public Button landButton;

    EventCenter evc;
    public override void OnViewEventRegist(EventCenter e)
    {
        base.OnViewEventRegist(e);
        evc = e;
        e.ListenEvent<MapPrefabsData>(nameof(UIMapViewEventName.OnMapNodeClick), OnShowMap);

        mapInfoDetail.ShowerInit(this);
    }
    public override void UIDeInit()
    {
        base.UIDeInit();
        evc.UnListenEvent<MapPrefabsData>(nameof(UIMapViewEventName.OnMapNodeClick), OnShowMap);
    }

    public void OnShowMap(MapPrefabsData mpd)
    {
        currentMap = mpd;
        if (mpd == null)
        {
            //隐藏自己
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            landButton.enabled = true;
            landButton.GetComponent<Image>().enabled = true;
            MapPrefabsData shipat = EventCenter.WorldCenter.GetParm<MapPrefabsData>(nameof(EventNames.GetShipAtMapData));

            mapInfoDetail.Show(mpd);
            //显示目的位置描述
            if (mpd.mapName == shipat.mapName) {
                resShower.Show(mpd.land_Resource);
                landButton.SetText("登岛");
            }
            else if (ContainerManager.GetContainer<Container_MapPrefabData>().IsNearBy(shipat, currentMap))
            {
                resShower.Show(ContainerManager.GetContainer<Container_MapPrefabData>().GetRouteCost(shipat, mpd));
                landButton.SetText("前往");
            }
            else
            {
                landButton.SetText("不可达");
                landButton.enabled = false;
                landButton.GetComponent<Image>().enabled = false;
                resShower.Show(null);
            }
            
            //显示登陆消耗资源，获取船只当前位置，如果目的位置等于当前，则显示登岛费用
            //否则 如果目的位置可直达，则显示移动费用
            //否则不显示登陆按钮和资源
        }
    }
    public void OnLandClick()
    {
        MapPrefabsData shipat = EventCenter.WorldCenter.GetParm<MapPrefabsData>(nameof(EventNames.GetShipAtMapData));
        if(shipat.mapName==currentMap.mapName)
        {
            //登陆
            fatherView.center.SendEvent<string>(nameof(UIMapViewEventName.GoToOrEnterIsland), currentMap.mapName);
        }
        else if(ContainerManager.GetContainer<Container_MapPrefabData>().IsNearBy(shipat,currentMap))
        {
            //移动
            fatherView.center.SendEvent<string>(nameof(UIMapViewEventName.GoToOrEnterIsland), currentMap.mapName);
        }
    }
    public void OnWareHouseClick()
    {

    }

    public void BreakMVConnect(string viewname, EventCenter model)
    {
        //throw new System.NotImplementedException();
    }

    public void BuildMVConnect(string viewname, EventCenter model)
    {
        //throw new System.NotImplementedException();
    }

    public UI_ModelType GetModelType()
    {
        //throw new System.NotImplementedException();
        return modelType;
    }
}
