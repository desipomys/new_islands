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
            //�����Լ�
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            landButton.enabled = true;
            landButton.GetComponent<Image>().enabled = true;
            MapPrefabsData shipat = EventCenter.WorldCenter.GetParm<MapPrefabsData>(nameof(EventNames.GetShipAtMapData));

            mapInfoDetail.Show(mpd);
            //��ʾĿ��λ������
            if (mpd.mapName == shipat.mapName) {
                resShower.Show(mpd.land_Resource);
                landButton.SetText("�ǵ�");
            }
            else if (ContainerManager.GetContainer<Container_MapPrefabData>().IsNearBy(shipat, currentMap))
            {
                resShower.Show(ContainerManager.GetContainer<Container_MapPrefabData>().GetRouteCost(shipat, mpd));
                landButton.SetText("ǰ��");
            }
            else
            {
                landButton.SetText("���ɴ�");
                landButton.enabled = false;
                landButton.GetComponent<Image>().enabled = false;
                resShower.Show(null);
            }
            
            //��ʾ��½������Դ����ȡ��ֻ��ǰλ�ã����Ŀ��λ�õ��ڵ�ǰ������ʾ�ǵ�����
            //���� ���Ŀ��λ�ÿ�ֱ�����ʾ�ƶ�����
            //������ʾ��½��ť����Դ
        }
    }
    public void OnLandClick()
    {
        MapPrefabsData shipat = EventCenter.WorldCenter.GetParm<MapPrefabsData>(nameof(EventNames.GetShipAtMapData));
        if(shipat.mapName==currentMap.mapName)
        {
            //��½
            fatherView.center.SendEvent<string>(nameof(UIMapViewEventName.GoToOrEnterIsland), currentMap.mapName);
        }
        else if(ContainerManager.GetContainer<Container_MapPrefabData>().IsNearBy(shipat,currentMap))
        {
            //�ƶ�
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
