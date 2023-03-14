using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WareHouseToNPC_UIController : BaseUIController
{
    public int trueIndex, UIIndex;
    //在此过滤处-1~-4这几个特殊的page,-1skin,-2props,-3equip,-4npcbp
    //在此限制不可放置非skin物品到skin槽、不可放置非装备物品到装备槽、不可放置非配件物品到配件槽、锁定NPC预带的物品
    Container_PlayerData cacheModel;

    public override void UIInit()
    {
        base.UIInit();
        cacheModel = ContainerManager.GetContainer<Container_PlayerData>();
        
    }
    public override EventCenter GetModel()
    {
        return this.view.center;
    }
    public override void SetModel(EventCenter source, EventCenter target)
    {
        base.SetModel(source, target);
        
    }
    /// <summary>
    /// 承接lockbackpack和backpack的controller功能
    /// </summary>
    /// <param name="state"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="area"></param>
    public override void OnSlotClick(int state, int x, int y, int area)
    {
        //Debug.Log(area);
        if (area > 0)
        {
            base.OnSlotClick(bp, state, x, y, area);
        }


    }
  
    public void SetTrueIndex(int trueindex, int uiindex)
    {
        trueIndex = trueindex;
        UIIndex = uiindex;
        bp = EventCenter.WorldCenter.GetParm<int, IBackPack>(nameof(Container_PlayerData_EventNames.NPCbackpack), trueIndex);
    }
}
