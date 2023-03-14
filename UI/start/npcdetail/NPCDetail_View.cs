using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCDetailEventName
{
    NPCBackpack,
    /// <summary>
    ///实际是npc背包物品改变 
    /// </summary>
        NPCDataChg
}

public class NPCDetail_View : BaseUIView
{
    public NPCfullInfoView npcslot;
    public PlayerNPCCharDataShower playerNPCCharDataShower;
    public ItemScrollView LockedBackpack;//不需要配置updateeventname
    public ItemScrollView backpack;//不需要配置updateeventname

    public override void UIInit(UICenter center)
    {
        base.UIInit(center);
        
    }
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        this.center.RegistFunc<IBackPack>(nameof(NPCDetailEventName.NPCBackpack), GetNPCBackpack);
    }
    public override void OnUIClose()
    {
        base.OnUIClose();
        BreakMVConnect(UIName, EventCenter.WorldCenter, center);
    }
    public override void OnUIOpen(int posi = 0)
    {
        base.OnUIOpen(posi);
        BuildMVConnect(UIName, EventCenter.WorldCenter, center);
        Flush();
    }
    public override void BuildMVConnect(string viewname, EventCenter modelSource, EventCenter modelTarget)
    {
        base.BuildMVConnect(viewname, modelSource, modelTarget);

    }
    public override void BreakMVConnect(string viewname, EventCenter model, EventCenter target)
    {
        base.BreakMVConnect(viewname, model, target);
    }
    public void GoToWareHouse()
    {
        BaseUIView view = uiCenter.GetView("WareHouseToNPC");
        ((WareHouseToNPC_View)view).ShowNPCIndex(((NPCDetail_UIController)controller).trueIndex, ((NPCDetail_UIController)controller).UIIndex);
        uiCenter.ShowView("WareHouseToNPC");
    }

    public void ShowNPCIndex(int trueIndex,int UIindex)
    {
        ((NPCDetail_UIController)controller).SetNPCIndex(trueIndex, UIindex);
        
        EventCenter.WorldCenter.ListenEvent<NPCDataChangeParm>(nameof(Container_PlayerData_EventNames.NPCDataChg), OnNPCBPChg);
        
    }
    public override void Flush()
    {
        NpcData nd = EventCenter.WorldCenter.GetParm<int, NpcData>(nameof(Container_PlayerData_EventNames.GetNPCByIndex), ((NPCDetail_UIController)controller).trueIndex);
        npcslot.SetPage(((NPCDetail_UIController)controller).UIIndex);
        ShowNPCData(nd);
        if (nd != null)
        {
            LockedBackpack.SetItems(nd.defaultItems);
            backpack.SetItems(nd.bp);
        }
        else
        {
            LockedBackpack.SetItems(null);
            backpack.SetItems(null);
        }
    }
    void ShowNPCData(NpcData nd)
    {
        if (nd == null)
        {
            
            playerNPCCharDataShower.ShowNpcData(null);
            return;
        }
       
        playerNPCCharDataShower.ShowNpcData(nd);
        
    }
    [System.Obsolete]
    void SendToSingleSlot()
    {
        SingleSlot[] allslots = GetComponentsInChildren<SingleSlot>();
        foreach (var item in allslots)
        {
            item.BreakMVConnect(UIName, EventCenter.WorldCenter);
            item.BuildMVConnect(UIName, EventCenter.WorldCenter);
        }
    }
    void OnNPCBPChg(NPCDataChangeParm parm)
    {
        ShowNPCData(parm.data);

        if(parm.bpchg.page==0)
        {
            LockedBackpack.OnBPChange(parm.bpchg);
        }
        else if (parm.bpchg.page == 1)
            backpack.OnBPChange(parm.bpchg);
        center.SendEvent<ItemPageChangeParm>(nameof(NPCDetailEventName.NPCDataChg), parm.bpchg);
    }
    IBackPack GetNPCBackpack()
    {
        return EventCenter.WorldCenter.GetParm<int, IBackPack>(nameof(Container_PlayerData_EventNames.NPCbackpack), ((NPCDetail_UIController)controller).trueIndex);
    }
}
