using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WareHouseToNPC_UIController : BaseUIController
{
    public int trueIndex, UIIndex;
    //�ڴ˹��˴�-1~-4�⼸�������page,-1skin,-2props,-3equip,-4npcbp
    //�ڴ����Ʋ��ɷ��÷�skin��Ʒ��skin�ۡ����ɷ��÷�װ����Ʒ��װ���ۡ����ɷ��÷������Ʒ������ۡ�����NPCԤ������Ʒ
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
    /// �н�lockbackpack��backpack��controller����
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
