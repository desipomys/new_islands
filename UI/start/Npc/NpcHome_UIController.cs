using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主要功能，设置playerdata的selectednpc数组
/// </summary>
public class NpcHome_UIController : BaseUIController
{
    public override void SetModel(EventCenter i,EventCenter target)
    {
        base.SetModel(i,target);
    }
    public void ChangeSelectedNpc(int uiindex,int targetTrueIndex)
    {
        //将container_playerdata的selectindex[uiindex]改为targetTrueIndex
        model.SendEvent<int, int>(nameof(Container_PlayerData_EventNames.SetNPCSelectIndex), uiindex, targetTrueIndex);
    }
}
