using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelect_UIController : BaseUIController
{
    public override void SetModel(EventCenter source, EventCenter target)
    {
        base.SetModel(source, target);
        if (source != null || target != null)
        {
            view.center.ListenEvent<string>(nameof(UIMapViewEventName.GoToOrEnterIsland), GoToOrEnterIsland);
        }
        else
        {
            view.center.UnListenEvent<string>(nameof(UIMapViewEventName.GoToOrEnterIsland), GoToOrEnterIsland);
        }
    }
    public void GoToOrEnterIsland(string mapName)
    {

        //���������Ǵ�ֻ���ڵ����½
        string shipatMap = EventCenter.WorldCenter.GetParm<string>(nameof(Container_PlayerData_EventNames.GetMapIndex));
        MapPrefabsData targetMapData= EventCenter.WorldCenter.GetParm<string,MapPrefabsData>(nameof(EventNames.GetMapDataByName),mapName);
        if (mapName == shipatMap)
        {
            EventCenter.WorldCenter.SendEvent<Resource_Data>(nameof(Container_PlayerData_EventNames.UseResource), targetMapData.land_Resource);

            EventCenter.WorldCenter.SendEvent<string>(nameof(EventNames.JumpToScene), ConstantValue.ingameSceneName);
        }
        else
        {
            //��ֻ�ƶ���ָ��㣬�����ͼ����·�����۳���Դ
            //����ʱ��ֹmapselectview�ĵ��
            //Ȼ����ȫ��Э�̣��ô�ֻһ��һ����Ŀ����ƶ�
            //����Ŀ�ĵغ��������ֹ
            EventCenter.WorldCenter.SendEvent<string>(nameof(Container_PlayerData_EventNames.MovShipIndex), mapName);

            Resource_Data mgd = ContainerManager.GetContainer<Container_MapPrefabData>().GetRouteCost(shipatMap, mapName);
            EventCenter.WorldCenter.SendEvent<Resource_Data>(nameof(Container_PlayerData_EventNames.UseResource), mgd);
        }

    }
}
