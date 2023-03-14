using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ҫ���ܣ�����playerdata��selectednpc����
/// </summary>
public class NpcHome_UIController : BaseUIController
{
    public override void SetModel(EventCenter i,EventCenter target)
    {
        base.SetModel(i,target);
    }
    public void ChangeSelectedNpc(int uiindex,int targetTrueIndex)
    {
        //��container_playerdata��selectindex[uiindex]��ΪtargetTrueIndex
        model.SendEvent<int, int>(nameof(Container_PlayerData_EventNames.SetNPCSelectIndex), uiindex, targetTrueIndex);
    }
}
