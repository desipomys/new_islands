using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WareHouseToNPC_View : BaseUIView
{
    //[HideInInspector]
    public ItemScrollView WareHouseScrollView;//�ֿ�scrollviewģ��

    //public PlayerNPCCharDataShower charDataShower;
    //public RawImage NPCImage;
    //public ItemScrollView_big skins,equips,props;
    public ItemScrollView LockedBackpack;//����Ҫ����updateeventname
    public ItemScrollView backpack;//����Ҫ����updateeventname
    public WareHousePageButtonGroup buttonGroup;

    //public Transform scrollViewGroup;
    //public Button[] buttons;

    int currentWareHousePage=0;

    string path = "Prefabs/UI/inGame/unit/ItemScrollView";
    string showerPath = "Prefabs/UI/inGame/unit/ItemShow";
    public override void UIInit(UICenter center)
    {
        if (!active1) return;
        base.UIInit(center);


        
        EventCenter.WorldCenter.ListenEvent(nameof(EventNames.LoadSaveDone), OnLoadSaveDone);
    }
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        this.center.RegistFunc<IBackPack>(nameof(NPCDetailEventName.NPCBackpack), GetNPCBackpack);
    }
    IBackPack GetNPCBackpack()
    {
        return EventCenter.WorldCenter.GetParm<int, IBackPack>(nameof(Container_PlayerData_EventNames.NPCbackpack), ((WareHouseToNPC_UIController)controller).trueIndex);
    }
    public override void OnUIOpen(int posi = 0)
    {
        base.OnUIOpen(posi);
        
        BuildMVConnect(UIName, EventCenter.WorldCenter, center);
        OnButtonHit("WareHousePage", 0);
        Flush();
    }
    public override void OnUIClose()
    {
        base.OnUIClose();
        BreakMVConnect(UIName, EventCenter.WorldCenter, center);
    }

    void OnNPCDataChg(NPCDataChangeParm parm)
    {
        if(parm.trueIndex==((WareHouseToNPC_UIController)controller).trueIndex)
        {
            if (parm.pos.Contain(NPCDataChgPOS.bp))
            {
                backpack.OnBPChange(parm.bpchg);
                center.SendEvent<ItemPageChangeParm>(nameof(NPCDetailEventName.NPCDataChg), parm.bpchg);
            }
            if (parm.pos.Contain(NPCDataChgPOS.defaultItem)) LockedBackpack.OnBPChange(parm.bpchg);
           
        }
    }
   
    void OnLoadSaveDone()
    {
        
    }

    public override void BuildMVConnect(string viewname, EventCenter model,EventCenter target)
    {
        base.BuildMVConnect(viewname, model,target);
       

        //scrollView.gameObject.SetActive(true);
        //model.ListenEvent<ItemPageChangeParm>(nameof(Container_PlayerData_EventNames.WareHouseItemChg), OnItemChg);
        model.ListenEvent<NPCDataChangeParm>(nameof(Container_PlayerData_EventNames.NPCDataChg), OnNPCDataChg);
        //����model��npc�ı��¼�
        //

        controller.SetModel(model,target);
    }
    public override void BreakMVConnect(string viewname, EventCenter model,EventCenter target)
    {
        //base.BreakMVConnect(viewname, model);
        controller.SetModel(null,null);//ʹcontroller���ٷ����¼�
        //model.UnListenEvent<ItemPageChangeParm>(nameof(Container_PlayerData_EventNames.WareHouseItemChg), OnItemChg);//�Լ����ټ���model�ĸı��¼�
        model.UnListenEvent<NPCDataChangeParm>(nameof(Container_PlayerData_EventNames.NPCDataChg), OnNPCDataChg);
    }
    public override void OnButtonHit(string typ, int id)
    {
        base.OnButtonHit(typ, id);
        switch (typ)
        {
            case "WareHousePage":
                currentWareHousePage = id;
                WareHouseScrollView.SetPage(id);
                WareHouseScrollView.Flush();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ʵ��index,�ɱ��UI����
    /// </summary>
    /// <param name="index"></param>
    public void ShowNPCIndex(int trueindex,int UIindex)
    {
        ((WareHouseToNPC_UIController)controller).SetTrueIndex(trueindex, UIindex);
        Flush();
    }
    public override void Flush()
    {
        int trueindex = ((WareHouseToNPC_UIController)controller).trueIndex;
        NpcData nd;
        if (trueindex == -1)//��ʾ�����Ϣ
        {
            nd = null;
        }
        else
        {
            nd = EventCenter.WorldCenter.GetParm<int, NpcData>(nameof(Container_PlayerData_EventNames.GetNPCByIndex), trueindex);
        }
        ShowNPCData(nd);
    }
    void ShowNPCData(NpcData nd)
    {
        if (nd == null)
        {
            LockedBackpack.SetItems(null);
            backpack.SetItems(null);
            return;
        }
        LockedBackpack.SetItems(nd.defaultItems);
        backpack.SetItems(nd.bp);
    }

    public override void OnQuitStartScene()
    {
        base.OnQuitStartScene();
        BreakMVConnect(UIName, EventCenter.WorldCenter,null);
        EventCenter.WorldCenter.UnListenEvent(nameof(EventNames.LoadSaveDone), OnLoadSaveDone);
    }
    public override void OnArriveStartScene()
    {
        base.OnArriveStartScene();
        if (EventCenter.WorldCenter.GetParm<bool>("IsInSave"))
        {
            //BuildMVConnect(UIName, EventCenter.WorldCenter,null);
        }
    }
}
