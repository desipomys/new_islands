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

        //如果点击的是船只所在点则登陆
        string shipatMap = EventCenter.WorldCenter.GetParm<string>(nameof(Container_PlayerData_EventNames.GetMapIndex));
        MapPrefabsData targetMapData= EventCenter.WorldCenter.GetParm<string,MapPrefabsData>(nameof(EventNames.GetMapDataByName),mapName);
        if (mapName == shipatMap)
        {
            EventCenter.WorldCenter.SendEvent<Resource_Data>(nameof(Container_PlayerData_EventNames.UseResource), targetMapData.land_Resource);

            EventCenter.WorldCenter.SendEvent<string>(nameof(EventNames.JumpToScene), ConstantValue.ingameSceneName);
        }
        else
        {
            //船只移动到指向点，需根据图计算路径，扣除资源
            //先暂时禁止mapselectview的点击
            //然后开启全局协程，让船只一步一步向目标点移动
            //到达目的地后解除点击禁止
            EventCenter.WorldCenter.SendEvent<string>(nameof(Container_PlayerData_EventNames.MovShipIndex), mapName);

            Resource_Data mgd = ContainerManager.GetContainer<Container_MapPrefabData>().GetRouteCost(shipatMap, mapName);
            EventCenter.WorldCenter.SendEvent<Resource_Data>(nameof(Container_PlayerData_EventNames.UseResource), mgd);
        }

    }
}
