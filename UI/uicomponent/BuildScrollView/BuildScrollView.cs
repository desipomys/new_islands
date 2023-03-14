using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildScrollView : Base_UIComponent,IMVConnector
{
    public static int BuildShowerSize = 192;

    public Scrollbar bar;
    GameObject shower;
    public RectTransform content, slotsgroup;
    public Button[] Tlevel;
    public Button[] types;
    

    /// <summary>
    /// key是shower显示的eblock或bblock ID，该字典每次SetBuildings的时候都会重建
    /// </summary>
    //Dictionary<int, long> showerIndex = new Dictionary<int, long>();

    CompShowerManager manager = new CompShowerManager();

    public int defaultWid, defaultHig;
   
    int areaWidth;

    public int selectLevel, selectType;
    EventCenter opener;

    string path = "Prefabs/UI/inGame/unit/BuildShow";

    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);
        if (shower == null)
            shower = Resources.Load<GameObject>(path);//
       areaWidth= slotsgroup.GetComponent<GridLayoutGroup>().constraintCount ;
        manager.Init(shower.GetComponent<BuildShower>(), slotsgroup, this);
    }
    public override void OnUIOpen(UICenter center, BaseUIView view)
    {
        base.OnUIOpen(center, view);

        selectLevel = 0;
        selectType = 0;
        flush(opener);
    }

    //public void Listen(Action<ItemSlotOnHItArg> a) { slotOnKey = a; }
    public void SetBuildings(BuildAble_Data[] models,bool[] unLocked)
    {
        ClearBuildings();
        
        manager.SetNum(models.Length);
        for (int i = 0; i < models.Length; i++)
        {
            BuildShower bs = (BuildShower)manager.Get(i);
            bs.Show(models[i], i,unLocked[i]);
        }
    }
    
    public void SetBuildings(BuildAble_Data[] buds)
    {
        ClearBuildings();
        manager.SetNum(buds.Length);
        for (int i = 0; i < buds.Length; i++)
        {
            BuildShower bs = (BuildShower)manager.Get(i);
            bs.Show(buds[i], i);
        }
    }
    void ClearBuildings()
    {
        manager.RecycleAll();
    }

    

    public void SetViewSize(int width, int height)
    {
       /* RectTransform rt = GetComponent<RectTransform>();
        Vector2 temp = rt.sizeDelta;
        rt.sizeDelta = new Vector2(width * BuildShowerSize + (width - 1) * slotsgroup.GetComponent<GridLayoutGroup>().spacing.x 
            , height * BuildShowerSize + (height - 1) * slotsgroup.GetComponent<GridLayoutGroup>().spacing.y+ 20);*/
        //fixViewSize = true;
    }
    void SetContentSize(int width, int height)//区域大小,width是数组长，height是数组宽
    {
        RectTransform rt = slotsgroup.GetComponent<RectTransform>();
        RectTransform contentrt = content.GetComponent<RectTransform>();
        float x = width * BuildShowerSize + (width - 1) * slotsgroup.GetComponent<GridLayoutGroup>().spacing.x ;
        float y = height * BuildShowerSize + (height - 1) * slotsgroup.GetComponent<GridLayoutGroup>().spacing.y;
        //Debug.Log("width: " + x + "height: " + y);
        //if (!fixViewSize)
            //rt.sizeDelta = new Vector2(x, rt.sizeDelta.y);//sizedelta中，width是横向长度，height是竖直长度
        contentrt.sizeDelta = new Vector2(contentrt.sizeDelta.x, y);
        
    }

    //buildshower被点击事件,0左1中2右
    public override void OnEvent(object parm,object parm1)
    {
        base.OnEvent(parm,parm1);
        fatherView.center.SendEvent<BuildAble_Data, int>(nameof(PlayerBuilderEventName.OnShowerHit),(BuildAble_Data)parm,(int)parm1);
    }

    void OnBuilderLevelChg(ValueChangeParm<short> p)
    {

    }
    void OnTechTreeChg(ResearchData rd)
    {

    }
    void OnCurrentMapprefabsChg(MapPrefabsData mpd)
    {

    }

    public void OnLevelHit(int p)
    {
        selectLevel = p;
        flush(opener);
    }
    public void OnTypeHit(int p)
    {
        selectType = p;
        flush(opener);
    }

    GetBuildAbleRequestArg arg = new GetBuildAbleRequestArg();
    void flush(EventCenter model)
    {
        int playerlevel = model.GetParm<short>(nameof(CharacterEventName.skill_build));
        MapPrefabsData mpd = EventCenter.WorldCenter.GetParm<MapPrefabsData>(nameof(EventNames.GetCurrentMapPrefabs));
        ResearchData rd = EventCenter.WorldCenter.GetParm<ResearchData>(nameof(EventNames.GetReserchData));

        //监听playerlevel、mpd、rd改变事件

        arg.map = mpd;
        arg.playerBuildLevel = playerlevel;
        arg.research = rd;
        arg.selectlevel = selectLevel;
        arg.selectType = selectType;

        BuildAble_Data[] bds = EventCenter.WorldCenter.GetParm<GetBuildAbleRequestArg, BuildAble_Data[]>(nameof(EventNames.GetBuildAbleData),arg);

        bool[] canBuild = new bool[bds.Length];
        for (int i = 0; i < bds.Length; i++)
        {
            //检查玩家是否有足够bds[i].mats的物品
            if (bds[i].mats != null && bds[i].mats.Length > 0)
            {
                IBackPack ib = model.GetParm<IBackPack>(nameof(PlayerEventName.backpack));
                bool enought = true;
                for (int j = 0; j < bds[i].mats.Length; j++)
                {
                    if (ib.Contain(bds[i].mats[j]) < bds[i].mats[j].num)
                    {
                        enought = false; break;
                    }
                }
                Debug.Log("足够吗？" + enought);
                canBuild[i] = enought;
            }
            else canBuild[i]= true;
        }

        if(bds!=null&&bds.Length>0)
        {
            SetBuildings(bds,canBuild);
           /* if(bds[0].isBBlock)
            {
                List<B_Block> bbs = new List<B_Block>();
                for (int i = 0; i < bds.Length; i++)
                {
                    bbs.Add(bds[i].bblockTyp);
                }
                SetBuildings(bbs.ToArray());
            }
            else
            {
                List<Entity_BlockModel> ebs = new List<Entity_BlockModel>();
                for (int i = 0; i < bds.Length; i++)
                {
                    ebs.Add(bds[i].eblockTyp);
                }
                SetBuildings(ebs.ToArray());
            }
            */
        }
        else
        {
            ClearBuildings();
        }
    }

    UI_ModelType IMVConnector.GetModelType()
    {
        return UI_ModelType.source;
    }

    void IMVConnector.BuildMVConnect(string viewname, EventCenter model)
    {
        opener = model;
        //获取玩家建筑等级、mapprefabs最大等级，研究情况、当前选择类别、等级
        flush(model);
        
    }

    void IMVConnector.BreakMVConnect(string viewname, EventCenter model)
    {
        
    }
}
