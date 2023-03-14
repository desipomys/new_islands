//NPC存储区
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public enum NpcHomeEventName
{
    OnNPCMouseEnter,OnNPCMouseExit
}

public class NpcHome_View : BaseUIView
{
    public Text header;
    public NPCOverView npcoverView;
    public GameObject SelectedNPCGroup;
    public GameObject ChangeSelectedGroup;//换NPC的组
       public NPCfullInfoView selectingNPCPos;
    // playerSlot;
    public NpcHome_allnpcbg allNpcGroup;
    public PlayerNPCCharDataShower charDataShower;

    public Button back, yes;

    public NPCfullInfoView[] selectedNPCShowers;

    /// <summary>
    /// true=5人队伍总览，false=换人
    /// </summary>
    bool isSelectMode =false;
    int SelectIndex;
    int selectIndex { get { return SelectIndex; }
        set { SelectIndex = value;header.text = selectIndex.ToString(); } }
    int tempSelectIndex;//uiindex当前选择的NPC的UIindex,trueindex在allnpc列表中临时选择的npcindex
    //监听playerdata的npcdata改变事件，并响应
    public override void UIInit(UICenter center)
    {
        base.UIInit(center);

        center.ListenEvent<RectTransform, int, string>(nameof(NpcHomeEventName.OnNPCMouseEnter), OnNPCMouseEnter);
        center.ListenEvent<RectTransform, int, string>(nameof(NpcHomeEventName.OnNPCMouseExit), OnNPCMouseExit);

       
        //playerSlot.UIInit(this);
        EventCenter.WorldCenter.ListenEvent(nameof(EventNames.LoadSaveDone), OnLoadSaveDone);

        //npcoverView.Init();
        SwitchMode(true);
    }
    public override void UIDeInit()
    {
        base.UIDeInit();
        uiCenter.UnListenEvent<RectTransform, int, string>(nameof(NpcHomeEventName.OnNPCMouseEnter), OnNPCMouseEnter);
        uiCenter.UnListenEvent<RectTransform, int, string>(nameof(NpcHomeEventName.OnNPCMouseExit), OnNPCMouseExit);
        EventCenter.WorldCenter.UnListenEvent(nameof(EventNames.LoadSaveDone), OnLoadSaveDone);
        //npcoverView.UnInit();
    }

    public override void OnUIOpen(int posi = 0)
    {
        base.OnUIOpen(posi);
        selectingNPCPos.SetPage(-1);
        BuildMVConnect(UIName, EventCenter.WorldCenter,null);

    }
    public void  OnLoadSaveDone()
    {
        
    }
    public override void BuildMVConnect(string viewname, EventCenter model, EventCenter target)
    {
        /*
         监听container_playerdata的npcdata变化事件
         设置npchome_controller的model，使其可以设置container_playerdata的selectednpc变量和玩家自己的实体数据
         */
        //初始化allNpcGroup的scrollview，
        //初始化selectedNPCGroup的玩家信息和NPC信息,初始化npc编队
        base.BuildMVConnect(viewname, model, target);
        //flush(model, 0);
        
       
    }
    public override void BreakMVConnect(string viewname, EventCenter model,EventCenter target)
    {
        base.BreakMVConnect(viewname, model, target);
        //使controller不再发送事件
        //model.UnListenEvent<NPCDataChangeParm>(nameof(Container_PlayerData_EventNames.NPCDataChg), OnNPCDataChg);
    }
    
    public void onViewNPC(int UIindex)
    {
        if (UIindex == -1)
        {
            
        }
        else
        {
            
            Debug.Log("onview" + UIindex);
            int[] selectindexs = EventCenter.WorldCenter.GetParm<int[]>(nameof(Container_PlayerData_EventNames.GetNPCSelectedIndexs));
            int trueIndex = selectindexs[UIindex];
            if (trueIndex < 0) return;
            BaseUIView view = uiCenter.GetView("NPCDetail");
            ((NPCDetail_View)view).ShowNPCIndex(trueIndex, UIindex);
            uiCenter.SendEvent(nameof(UIEventNames.UnShowNPCDetail));//NPC信息浮窗
            uiCenter.ShowView("NPCDetail");
        }
    }
  
    public override void OnButtonHit(string typ, int id)
    {
        switch (typ)
        {
            case "select":
                Debug.Log("npc点击");
                SwitchMode(!isSelectMode);
                if(!isSelectMode)
                    selectingNPCPos.SetPage(id);
                selectIndex = id;
                break;
            case "NPCs"://来自npcScrollview的格子点击
                //id=实际点击的shower的index对应到dic<int,npcdata>的int
                Debug.Log("发送" + id);
                EventCenter.WorldCenter.SendEvent<int, int>(nameof(Container_PlayerData_EventNames.SetNPCSelectIndex), selectIndex, id);
                break;
            case "viewNPC"://来自NPC大图的view按钮
                onViewNPC(id);
                break;
            default:
                break;
        }

    }
    /// <summary>
    /// true=队伍总览，false=换人
    /// </summary>
    /// <param name="mode"></param>
    public void SwitchMode(bool mode)
    {
        isSelectMode = mode;
        if(mode)//队伍总览
        {
            ChangeSelectedGroup.SetActive(false);
            //allNpcGroup.gameObject.SetActive(false);
            //selectingNPCPos.gameObject.SetActive(false);
            yes.gameObject.SetActive(false);
            back.gameObject.SetActive(false);

            SetAllSelectedNPCVisable(true);
            //exit.gameObject.SetActive(true);
            uiCenter.SendEvent(nameof(UIEventNames.UnShowNPCDetail));
        }
        else//换人
        {
            ChangeSelectedGroup.SetActive(true);
            //allNpcGroup.gameObject.SetActive(true);
            //selectingNPCPos.gameObject.SetActive(true);

            yes.gameObject.SetActive(true);
            back.gameObject.SetActive(true);

            SetAllSelectedNPCVisable(false);
            //exit.gameObject.SetActive(false);

            tempSelectIndex = selectIndex;
            uiCenter.SendEvent(nameof(UIEventNames.UnShowNPCDetail));
        }
    }

    void SetAllSelectedNPCVisable(bool b)
    {//隐藏已选择的NPC，将选中的slot移动到目标slot
        SelectedNPCGroup.SetActive(b);
    }
   
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mode">0全部刷新，1只刷新选中区，2只刷新全部NPC列表</param>
    public void flush(EventCenter model,int mode)
    {

    }

    public override void OnButtonHit(int id)
    {
        return;
       if(id==1)//换NPC的确定按钮按下
        {
            //获取到当前选择的npc uiindex和实际index，由controller写入到playerdata
            ((NpcHome_UIController)controller).ChangeSelectedNpc(selectIndex, tempSelectIndex);
        }
        SwitchMode(true);
    }

   


    #region 鼠标悬停NPC上时显示信息框
    RectTransform showerRT;
    int showerIndex;
    float previustime;//上次鼠标enter npcinfoshower的时间
    bool flag=false;
 /// <summary>
    /// 当鼠标悬停于npcinfoshower上时
    /// </summary>
    /// <param name="shower"></param>
    /// <param name="uiindex"></param>
    /// <param name="typ"></param>
    public void OnNPCMouseEnter(RectTransform shower,int uiindex,string typ)
    {
        Debug.Log("enter");
        previustime = Time.time;
        showerRT = shower;
        showerIndex = uiindex;
        flag = true;

        NpcData nd = EventCenter.WorldCenter.GetParm<int, NpcData>(nameof(Container_PlayerData_EventNames.GetNPCByUIIndex), uiindex);
        uiCenter.SendEvent<RectTransform, NpcData>(nameof(UIEventNames.ShowNPCDetail),uiCenter.GetMouse().GetComponent<RectTransform>() ,nd);
    }
    public void OnNPCMouseExit(RectTransform shower, int uiindex, string typ)
    {
        Debug.Log("exit");
        charDataShower.gameObject.SetActive(false);
        flag = false;
        uiCenter.SendEvent(nameof(UIEventNames.UnShowNPCDetail));
    }
    void ShowBound()
    {
        charDataShower.gameObject.SetActive(true);
        charDataShower.GetComponent<RectTransform>().position = Input.mousePosition;
        if(showerIndex!=-1)
            charDataShower.ShowNpcData(EventCenter.WorldCenter.GetParm<int, NpcData>(nameof(Container_PlayerData_EventNames.GetNPCByUIIndex), showerIndex));
        else
        {
            //显示玩家数据
        }
        //bound.
        flag = false;
    }
    private void Update()
    {
        if(flag)
        {
            if(previustime<Time.time-0.5f)
            {
                ShowBound();
            }
        }
    }
    #endregion
}
