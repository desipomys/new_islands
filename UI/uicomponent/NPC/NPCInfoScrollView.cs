using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 只是npchead的scrollview
/// </summary>
public class NPCInfoScrollView : Base_UIComponent, IMVConnector,IItemScrollView
{
    public UI_ModelType modleType;
    /// <summary>
    /// 被选择的NPCindex改变的事件名
    /// </summary>
    public string SelectionChgEventname;

    public static int NPCHeadHeight=250;
    public static int NPCHeadWid = 200;
    public Scrollbar bar;
    public Transform content, slotGroup,boundGroup; 

    CompShowerManager boundManager;
    CompShowerManager npcshowerManager;

    public int defaultWid, defaultHig;
    Dictionary<int, int> showerCoord_Index = new Dictionary<int, int>();//根据shower index找dic<int,npc>的key
    Dictionary<int, int> Index_showerCoord = new Dictionary<int, int>();//根据dic<int,npc>的key找shower index

    string path2 = "Prefabs/UI/inGame/unit/Npc_Head";
    string path = "Prefabs/UI/inGame/unit/bound";

    [HideInInspector]
    GameObject bound, shower;

    bool fixViewSize;


    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);

        //showerGroup = transform.Find("showerGroup");
        //page = p;
        bound = Resources.Load<GameObject>(path);//
        //slotPool = new BaseObjectPool(slot,this.gameObject);
        shower = Resources.Load<GameObject>(path2);
        //showerPool = new BaseObjectPool(shower,this.gameObject);

        boundManager = new CompShowerManager();
        npcshowerManager = new CompShowerManager();
        boundManager.Init(bound.GetComponent<Base_Shower>(),(RectTransform)boundGroup,this);
        npcshowerManager.Init(shower.GetComponent<Base_Shower>(),(RectTransform)slotGroup, this);

    }

    public void SetViewSize(int width, int height)
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 temp = rt.sizeDelta;
        rt.sizeDelta = new Vector2(width * NPCHeadWid + (width - 1) * slotGroup.GetComponent<GridLayoutGroup>().spacing.x + 20
            , height * NPCHeadHeight + (height - 1) * slotGroup.GetComponent<GridLayoutGroup>().spacing.y+2);
        fixViewSize = true;
        Debug.Log(rt.sizeDelta.x + "x=");
    }
    void SetContentSize(int width, int height)//区域大小,width是数组长，height是数组宽
    {
        RectTransform rt = GetComponent<RectTransform>();
        RectTransform contentrt = content.GetComponent<RectTransform>();
        float x = width * NPCHeadWid + (width - 1) * slotGroup.GetComponent<GridLayoutGroup>().spacing.x + 20;
        float y = height * NPCHeadHeight + (height - 1) * slotGroup.GetComponent<GridLayoutGroup>().spacing.y+2;
        //Debug.Log("width: " + x + "height: " + y);
        //if(!fixViewSize)rt.sizeDelta = new Vector2(x, y);//sizedelta中，width是横向长度，height是竖直长度
        contentrt.sizeDelta = new Vector2(0, y);
       
    }
    void SetSlotSize(int width, int height)
    {//leng(0)是高,leng(1)是宽
        
        slotGroup.GetComponent<GridLayoutGroup>().constraintCount = width;//列数
        npcshowerManager.SetNum(width, height);
    }

    public void Flush(Dictionary<int,NpcData> allnpc)
    {
        Debug.Log("刷新");
        showerCoord_Index.Clear();
        Index_showerCoord.Clear();
        if(allnpc==null||allnpc.Count==0){
            SetViewSize(defaultWid, defaultHig);
            SetContentSize(defaultWid, defaultHig);
            npcshowerManager.SetNum(0);
        boundManager.RecycleAll();
        return;
        }
        int height=Mathf.CeilToInt(allnpc.Count*1.0f/defaultWid) ;
        SetViewSize(defaultWid,height);
        SetContentSize(defaultWid,height);
        SetSlotSize(defaultWid,height);

        int i=0;
        foreach (var item in allnpc)
        {
            NPCInfoShower ish = (NPCInfoShower)npcshowerManager.GetOrNew(i);
            ish.SetIndex(i, 0, 0);
            ish.Show(item.Value);
            showerCoord_Index.Add(i,item.Key);
            Index_showerCoord.Add(item.Key,i);
            i++;
        }
        if(i< defaultWid*height)
        {
            for (; i < defaultWid * height; i++)
            {
                NPCInfoShower ish = (NPCInfoShower)npcshowerManager.GetOrNew(i);
                ish.Show(null);
            }
        }

    }
    
    /// <summary>
    /// select中的值为npcDic的key,一般晚于flush
    /// </summary>
    /// <param name="select"></param>
    public void SelectionChg(int[] select)
    {
        if(gameObject.activeInHierarchy)
        StartCoroutine(boundPosWait(select));
    }
    IEnumerator boundPosWait(int[] select)
    {
        yield return null;
        Debug.Log("select变化");
        boundManager.RecycleAll();
        if (select == null || select.Length == 0) { yield break; }
        boundManager.SetNum(select.Length);

        for (int i = 0; i < select.Length; i++)
        {
            if (select[i] < 0) { boundManager.Get(i).gameObject.SetActive(false); continue; }
            boundManager.Get(i).gameObject.SetActive(true);
            boundManager.Get(i).SetIndex(i, 0, 0);
            boundManager.SetPos(npcshowerManager.GetRect(select_npcshowerIndex(select[i])), i);
        }
    }
    int select_npcshowerIndex(int select)
    {
        try
        {
            return Index_showerCoord[select];
        }
        catch {
            Debug.Log("select的uiindex不存在"+select);
        throw;
        }
    }

     public UI_ModelType GetModelType()
    {
        return modleType;
    }

    public void BuildMVConnect(string viewname, EventCenter model)
    {
        //throw new NotImplementedException();
        model.ListenEvent<Dictionary<int,NpcData>>(UpdateEventName, Flush);
        model.ListenEvent<int[]>(SelectionChgEventname, SelectionChg);
        controll.BuildMVConnect(viewname, model);
        Flush(model.GetParm<Dictionary<int,NpcData>>(nameof(Container_PlayerData_EventNames.GetAllNPCData)));
        SelectionChg(model.GetParm<int[]>(nameof(Container_PlayerData_EventNames.GetNPCSelectedIndexs)));
    }

    public void BreakMVConnect(string viewname, EventCenter model)
    {
        model.UnListenEvent<Dictionary<int, NpcData>>(UpdateEventName, Flush);
        model.UnListenEvent<int[]>(SelectionChgEventname, SelectionChg);
        controll.BreakMVConnect(viewname, model);
        //throw new NotImplementedException();
    }

    public void SlotOnkey(int x, int y, int key)
    {
        Debug.Log("点击" + x);
        fatherView.OnButtonHit("NPCs", showerCoord_Index[x]);
    }
}
