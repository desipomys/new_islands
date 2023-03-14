using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
/*
 * 横width竖height，对应到itemslot数组就是[height,width]
 * slot的点击事件先传到此再给uicontroller
 * 
 * 功能：接收item数组进行显示，或接收item变化数据更新现有的item显示
 * 可固定宽度
 */

public class ItemScrollView_big : Base_UIComponent,IMVConnector
{

    public UI_ModelType modleType;

    public Scrollbar bar;
    public int page;
    public Transform content, slotsgroup,viewport,showerGroup;
    public int itemWidth = 5;

    /// <summary>
    /// 是否固定viewsize
    /// </summary>
    protected bool fixViewSize = false;

    protected CompShowerManager slotsManager;
    protected CompShowerManager itemshowerManager;
    //Dictionary<int, ItemShower> showerIndex = new Dictionary<int, ItemShower>();

    protected GameObject slot, shower,self;
    Action<ItemSlotOnHItArg> slotOnKey;
    ItemSlotOnHItArg slotonhitarg = new ItemSlotOnHItArg();
    protected string path = "Prefabs/UI/inGame/unit/ItemSlot_big";
    protected string path2 = "Prefabs/UI/inGame/unit/ItemShow";

    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);

        //showerGroup = transform.Find("showerGroup");

        slot = Resources.Load<GameObject>(path);//
        //slotPool = new BaseObjectPool(slot,this.gameObject);
        shower = Resources.Load<GameObject>(path2);
        self = gameObject;

        slotsManager = new CompShowerManager();
        itemshowerManager = new CompShowerManager();
        slotsManager.Init(slot.GetComponent<Base_Shower>(), (RectTransform)slotsgroup, this);
        itemshowerManager.Init(shower.GetComponent<Base_Shower>(), (RectTransform)showerGroup, this);

        bar.value = 1;

        RectTransform rt = GetComponent<RectTransform>();
        Vector2 temp = rt.sizeDelta;
        rt.sizeDelta = new Vector2(itemWidth * Loader_Item.ItemUIBigSize + (itemWidth - 1) * slotsgroup.GetComponent<GridLayoutGroup>().spacing.x + 20, temp.y);//初始化宽度
        //showerPool = new BaseObjectPool(shower,this.gameObject);
    }
    public override void SetPage(int pag)
    {
        base.SetPage(pag);
        page = pag;
    }

    public void Listen(Action<ItemSlotOnHItArg> a) { slotOnKey = a; }

    /// <summary>
    /// 设置可显示区大小,x是宽度（横向，不变），y是长度
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetViewSize(int width, int height)
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 temp = rt.sizeDelta;
        rt.sizeDelta = new Vector2(width * Loader_Item.ItemUIBigSize + (width - 1) * slotsgroup.GetComponent<GridLayoutGroup>().spacing.x, height * Loader_Item.ItemUIBigSize + (height - 1) * slotsgroup.GetComponent<GridLayoutGroup>().spacing.y);
        fixViewSize = true;
    }
    public virtual void Flush()
    {
        IBackPack ib = controll.GetModel().GetParm<IBackPack>(GetDataSourceName);//刷新
        SetItems(ib.GetBigItems(page));
        bar.value = 1;
    }
    public void LockSlotPos(bool locked)
    {

    }

    protected void clearShower()
    {
       /* List<ItemShower> t = new List<ItemShower>(showerIndex.Values);
        for (int i = 0; i < t.Count; i++)
        {
            t[i].Show(Item.Empty);
            cachedShowers.Push(t[i]);
            //Destroy(.gameObject);
        }
        showerIndex.Clear();*/
    }

    protected void SetContentSize(int width, int height)//区域大小,width是数组长，height是数组宽
    {
        width = Mathf.Clamp(width, 0, this.itemWidth);
        height = Mathf.Clamp(height, 0, height+1);
        RectTransform rt = GetComponent<RectTransform>();
        RectTransform contentrt = content.GetComponent<RectTransform>();
        float x = width * Loader_Item.ItemUIBigSize + (width ) * slotsgroup.GetComponent<GridLayoutGroup>().spacing.x + 10;
        float y = height * Loader_Item.ItemUIBigSize + (height  ) * slotsgroup.GetComponent<GridLayoutGroup>().spacing.y;
        //Debug.Log("width: " + x + "height: " + y);
        if (!fixViewSize) rt.sizeDelta = new Vector2(x, y);//sizedelta中，width是横向长度，height是竖直长度
        contentrt.sizeDelta = new Vector2(0, height * Loader_Item.ItemUIBigSize);
        slotsgroup.GetComponent<GridLayoutGroup>().constraintCount = width;//列数

        if(!self.GetComponent<ScrollRect>().horizontal&&!self.GetComponent<ScrollRect>().vertical)
        {
            ((RectTransform)viewport).sizeDelta = new Vector2(0, 0);
        }
    }

    protected void SetSlotSize(int width, int height)
    {
        slotsgroup.GetComponent<GridLayoutGroup>().constraintCount = width;//列数
        slotsManager.SetNum(width, height);
        

    }
    /// <summary>
    /// len(0)=hig,len(1)=wid
    /// </summary>
    /// <param name="items"></param>
    public virtual void SetItems(Item[,] items)
    {//先扩格子大小，再设置item显示物，如果不够则新增，如果太多则设空item

        SetContentSize(items.GetLength(1), items.GetLength(0));
        SetSlotSize(items.GetLength(1), items.GetLength(0));
        itemshowerManager.RecycleAll();

        Debug.Log("设置大小：" + items.GetLength(1) + "," + items.GetLength(0));
        StartCoroutine(waitSlotLayout(items));
    }
    public void SetItems(Item[] items,bool isHorizontal)
    {//未测试
        if (items == null || items.Length == 0) { SetItems(new Item[0,0]); return; }
        if (!isHorizontal)//纵向排版
        {
            Item[,] its = new Item[items.Length, 1];
            for (int i = 0; i < items.Length; i++)
            {
                its[i, 0] = items[i];
            }
            SetItems(its);
        }
       else
        {
            Item[,] its = new Item[ 1, items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                its[0,i] = items[i];
            }
            SetItems(its);
        }
    }

    /// <summary>
    /// x横坐标，y纵坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void UnShowItemAt(int x, int y)
    {
        ItemShower temp = (ItemShower)itemshowerManager.Get(y, x);
        temp.Show(Item.Empty);
    }

    IEnumerator waitSlotLayout(Item[,] items)
    {//len(0)=hig，len(1)=wid
        yield return null;

        //itemshowerManager.SetNum(items.GetLength(1), items.GetLength(0));
        for (int i = 0; i < items.GetLength(1); i++)//h
        {
            for (int j = 0; j < items.GetLength(0); j++)//w
            {
                if (Item.IsNullOrEmpty(items[j, i])) continue;
                ItemShower ish = (ItemShower)itemshowerManager.GetOrNew(j,i);
                ish.ShowWithFixSize(items[j, i]);
                ish.SetCenterPosi(slotsManager.GetRect(j,i));
            }
        }
        
    }
    public virtual void SetItemAt(Item i, int x, int y)
    {
        //x横坐标，y纵坐标
        ItemShower temp = (ItemShower)itemshowerManager.GetOrNew(y, x);
        if (temp == null)//新增一个itemshower
        {
            temp = (ItemShower)itemshowerManager.GetOrNew(y, x);
        }
        if (Item.IsNullOrEmpty(i))
        {
            temp.Show(Item.Empty);
            return;
        }
        temp.ShowWithFixSize(i);
        temp.SetCenterPosi(slotsManager.GetRect(y, x));
    }
    public override void OnClick(int stat, int x, int y, int page)//page无用
    {
        base.OnClick(stat, x, y, page);

        slotonhitarg.x = x;
        slotonhitarg.y = y;
        slotonhitarg.page = this.page;
        slotonhitarg.key = stat;
        Debug.Log(slotonhitarg.ToString());

        controll.OnEvent(UIComp_EventName.itemSlotClick, slotonhitarg);
    }

    public virtual void OnBPChange(ItemPageChangeParm parm)
    {
        if (parm.page != page) return;
        int xy = XYHelper.ToIntXY(parm.x, parm.y);
        switch (parm.mode)
        {
            case ObjInPageChangeMode.delete:
                UnShowItemAt(parm.x, parm.y);
                break;
            case ObjInPageChangeMode.add:
                if (Item.IsNullOrEmpty(parm.item)) { }
                else
                {
                    SetItemAt(parm.item, parm.x, parm.y);
                }
                break;
            case ObjInPageChangeMode.set:

                SetItemAt(parm.item, parm.x, parm.y);
                break;
            case ObjInPageChangeMode.sub:
                if (Item.IsNullOrEmpty(parm.item))
                {
                    SetItemAt(Item.Empty, parm.x, parm.y);
                }
                else
                {
                    SetItemAt(parm.item, parm.x, parm.y);
                }
                break;
        }
    }

    int itemArrayNotNullCount(Item[,] its)
    {
        int c = 0;
        for (int i = 0; i < its.GetLength(0); i++)
        {
            for (int j = 0; j < its.GetLength(1); j++)
            {
                if (!Item.IsNullOrEmpty(its[i, j]))
                { c++; }
            }
        }
        return c;
    }
    void adjustItemshowSize(int count)
    {
        
    }

    public UI_ModelType GetModelType()
    {
        return modleType;
    }

    public virtual void BuildMVConnect(string viewname, EventCenter model)
    {
        model.ListenEvent<ItemPageChangeParm>(UpdateEventName, OnBPChange);
        model.ListenEvent(GetDataSourceName, Flush);
        controll.BuildMVConnect(viewname, model);

        Flush();
   }

    public virtual void BreakMVConnect(string viewname, EventCenter model)
    {
        model.UnListenEvent<ItemPageChangeParm>(UpdateEventName, OnBPChange);
        model.UnListenEvent(GetDataSourceName, Flush);
        controll.BreakMVConnect(viewname, model);
    }
}
