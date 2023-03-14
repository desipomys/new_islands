using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
/*
 * 横width竖height，对应到itemslot数组就是[height,width]
 * slot的点击事件先传到此再给uicontroller
 * 
 */
 public interface IItemScrollView
{
    void SlotOnkey(int x, int y, int key);
}

public class ItemScrollView : Base_UIComponent,IMVConnector
{
    public Scrollbar bar;
    public UI_ModelType modleType;

    CompShowerManager slotsManager;
    CompShowerManager itemshowerManager;

    /// <summary>
    /// key=x,y
    /// </summary>
    Dictionary<long, int> showerCoord_Index = new Dictionary<long, int>();//根据x,y坐标找shower index

    public int defaultWid, defaultHig;
    public int page;
    public Transform content, slotsgroup, showerGroup;

    bool fixViewSize = false;

   [HideInInspector]
    GameObject slot, shower;
    Action<ItemSlotOnHItArg> slotOnKey;
    ItemSlotOnHItArg slotonhitarg = new ItemSlotOnHItArg();
    string path = "Prefabs/UI/inGame/unit/ItemSlot_Small";
    string path2 = "Prefabs/UI/inGame/unit/ItemShow";

    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);

        //showerGroup = transform.Find("showerGroup");
        //page = p;
        slot = Resources.Load<GameObject>(path);//
        //slotPool = new BaseObjectPool(slot,this.gameObject);
        shower = Resources.Load<GameObject>(path2);
        //showerPool = new BaseObjectPool(shower,this.gameObject);

        slotsManager = new CompShowerManager();
        itemshowerManager = new CompShowerManager();
        slotsManager.Init(slot.GetComponent<Base_Shower>(),(RectTransform)slotsgroup,this);
        itemshowerManager.Init(shower.GetComponent<Base_Shower>(),(RectTransform)showerGroup, this);

        bar.value = 1;
        if(defaultHig!=0&&defaultWid!=0)
        {
            SetViewSize(defaultWid, defaultHig);
        }
    }
    public override void SetPage(int pag)
    {
        base.SetPage(pag);
        page = pag;
        Debug.Log(page);
    }

    public void Listen(Action<ItemSlotOnHItArg> a) { slotOnKey = a; }


    public void SetViewSize(int width, int height)
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 temp = rt.sizeDelta;
        rt.sizeDelta = new Vector2(width * Loader_Item.ItemUISize + (width - 1) * slotsgroup.GetComponent<GridLayoutGroup>().spacing.x + 20
            , height * Loader_Item.ItemUISize + (height - 1) * slotsgroup.GetComponent<GridLayoutGroup>().spacing.y+2);
        fixViewSize = true;
    }
    void SetContentSize(int width, int height)//区域大小,width是数组长，height是数组宽
    {
        RectTransform rt = GetComponent<RectTransform>();
        RectTransform contentrt = content.GetComponent<RectTransform>();
        float x = width * Loader_Item.ItemUISize + (width - 1) * slotsgroup.GetComponent<GridLayoutGroup>().spacing.x + 20;
        float y = height * Loader_Item.ItemUISize + (height - 1) * slotsgroup.GetComponent<GridLayoutGroup>().spacing.y+2;
        //Debug.Log("width: " + x + "height: " + y);
        if(!fixViewSize)rt.sizeDelta = new Vector2(x, y);//sizedelta中，width是横向长度，height是竖直长度
        contentrt.sizeDelta = new Vector2(0, y);
       
    }
    void SetSlotSize(int width, int height)
    {//leng(0)是高,leng(1)是宽
        
        slotsgroup.GetComponent<GridLayoutGroup>().constraintCount = width;//列数
        slotsManager.SetNum(width, height);
    }
    void clearShower()
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


    public void Flush()
    {
        IBackPack ib = controll.GetModel().GetParm<IBackPack>(GetDataSourceName);//刷新
        SetItems(ib.GetItems(page), ib.GetPlacements(page));
        bar.value = 1;
    }

    public void SetItems(ItemPage_Data data)
    {
        if (data == null) { Debug.LogError("data=null"); }
        SetItems(data.items, data.placement);
    }
    public void SetItems(Item[] items, int[,] placement)
    {//leng(0)是高,leng(1)是宽
        SetItems(items, placement, null);
    }
    public void SetItems(Item[] items, int[,] placement,bool[] locks)
    {
        if (placement == null) { Debug.LogError("placement=null"); }
        SetContentSize(placement.GetLength(1), placement.GetLength(0));
        SetSlotSize(placement.GetLength(1), placement.GetLength(0));
        //clearShower();
        if (items == null) itemshowerManager.SetNum(0);
        else
        {
            itemshowerManager.SetNum(items.Length);
        }
        showerCoord_Index.Clear();

        HashSet<int> showedItemIndex = new HashSet<int>();
        
        for (int i = 0; i < placement.GetLength(0); i++)//height
        {
            for (int j = 0; j < placement.GetLength(1); j++)//width
            {
                if (placement[i, j] != -1)
                {
                    if (showedItemIndex.Contains(placement[i, j])) continue;
                    ItemShower temp;
                    try
                    {
                        temp = (ItemShower)itemshowerManager.Get(placement[i, j]);

                    }
                    catch (Exception)
                    {
                        string a="";
                        for (int k = 0; k < placement.GetLength(0); k++)
                        {
                            for (int p = 0; p < placement.GetLength(1); p++)
                            {
                                a+=placement[k, p]+" " ;
                            }
                            a += "\n";
                        }
                        Debug.Log(a);
                        throw;
                    }
                    
                    if (locks != null && locks.Length > placement[i, j] && locks[placement[i, j]])
                    { temp.Show(items[placement[i, j]], true); }
                    else { temp.Show(items[placement[i, j]]); }
                    //temp.SetPosi(slotsManager.GetRect(i,j));
                    if(gameObject.activeInHierarchy)
                        StartCoroutine(synpos(temp, slotsManager.GetRect(i, j)));

                    showerCoord_Index.Add(XYHelper.ToLongXY(j,i), placement[i, j]);
                    showedItemIndex.Add(placement[i, j]);
                    
                    //Debug.Log("set at" + i + " " + j);
                }
                else { }
            }
        }
        
    }
    IEnumerator synpos(ItemShower its,RectTransform rt)
    {
        yield return new WaitForSeconds(0.1f);
        its.SetPosi(rt);
    }

    public void SetItemAt(Item item, int x, int y,int index)//第x行第Y列
    {
        SetItemAt(item, x, y,index, false);
    }
    public void SetItemAt(Item item, int x, int y,int index,bool islock)
    {//x横坐标，y纵坐标
        //根据横纵坐标找到已有的shower重新设置item
        //如果没有则新增shower设置item
        
        long ind = XYHelper.ToLongXY(x, y);
        if (showerCoord_Index.ContainsKey(ind))//此格有记录到shower index的映射
        {
            ItemShower temp = (ItemShower)itemshowerManager.Get(showerCoord_Index[ind]);
            if (temp == null)//此格没有shower
            {
                if (Item.IsNullOrEmpty(item))//没有，且现在设空
                {
                    return;
                }
                temp = (ItemShower)itemshowerManager.GetOrNew(showerCoord_Index[ind]);
                temp.Show(item, islock);
                temp.SetPosi(slotsManager.GetRect(y, x));
            }
            else
            {
                if (Item.IsNullOrEmpty(item))//已有，但现在设空
                {
                    itemshowerManager.Recycle(null,showerCoord_Index[ind]);
                    showerCoord_Index.Remove(ind);
                    return;
                }
               
                temp.Show(item, islock);
                temp.SetPosi(slotsManager.GetRect(y, x));
            }
           
        }else
        {
            if (Item.IsNullOrEmpty(item))//现在设空
            {
            }
            else
            {
                int temindex;
                ItemShower temp = (ItemShower)itemshowerManager.Create(out temindex);
                showerCoord_Index.Add(ind, temindex);
                temp.Show(item, islock);
                temp.SetPosi(slotsManager.GetRect(y, x));
            }
        }
    }
    /// <summary>
    /// x横坐标，y纵坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void UnShowItemAt(int x,int y)
    {
        if (showerCoord_Index.ContainsKey(XYHelper.ToLongXY(x, y)))
        {
            itemshowerManager.Recycle(null, showerCoord_Index[XYHelper.ToLongXY(x, y)]);
            showerCoord_Index.Remove(XYHelper.ToLongXY(x, y));
        }
        
    }

    IEnumerator setPosWait(ItemShower i, Transform pos)
    {
        yield return null;
        i.SetPosi((RectTransform)pos);
    }

    public void FlushPos()
    {
        return;//暂时
        StartCoroutine(flushPos());
    }
    IEnumerator flushPos()
    {
        yield return null;
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void OnBPChange(ItemPageChangeParm parm)
    {
        //Debug.Log(parm.ToString());
        if (parm.page != page) return;
        //int xy = XYHelper.ToIntXY(parm.x, parm.y);
        switch (parm.mode)
        {
            case ObjInPageChangeMode.delete:
                SetItemAt(Item.Empty, parm.x, parm.y, parm.index);
                //UnShowItemAt(parm.x, parm.y);
                break;
            case ObjInPageChangeMode.add:
                if (Item.IsNullOrEmpty(parm.item)) break;
                SetItemAt(parm.item, parm.x, parm.y,parm.index);
               
                break;
            case ObjInPageChangeMode.set:
                //if (Item.IsNullOrEmpty(parm.item)) { UnShowItemAt(parm.x,parm.y);  break;}
                SetItemAt(parm.item, parm.x, parm.y, parm.index);
                break;
            case ObjInPageChangeMode.sub:

                if (Item.IsNullOrEmpty(parm.item)) { UnShowItemAt(parm.x, parm.y); break; }
                SetItemAt(parm.item, parm.x, parm.y, parm.index);
                break;
        }

    }

    public override void OnClick(int stat, int x, int y, int page)//page无用
    {
        base.OnClick(stat, x, y, this.page);

        slotonhitarg.x = x;
        slotonhitarg.y = y;
        slotonhitarg.key = stat;
        slotonhitarg.page = this.page;

        controll.OnEvent(UIComp_EventName.itemSlotClick, slotonhitarg);
    }
    private void OnEnable()
    {
        FlushShowerPosi();
    }
    void FlushShowerPosi()
    {
        
    }
    

    public UI_ModelType GetModelType()
    {
        return modleType;
    }

    public void BuildMVConnect(string viewname, EventCenter model)
    {
        if (string.IsNullOrEmpty(GetDataSourceName) && string.IsNullOrEmpty(UpdateEventName)) { Debug.Log("不进行MVConnect"); return; }
        //throw new NotImplementedException();
        model.ListenEvent<ItemPageChangeParm>(UpdateEventName, OnBPChange);
        model.ListenEvent(GetDataSourceName, Flush);
        controll.BuildMVConnect(viewname, model);

        IBackPack ib = model.GetParm<IBackPack>(GetDataSourceName);//刷新
        
        SetItems(ib.GetItems(), ib.GetPlacements());
        //Debug.Log("hhhh");
    }

    public void BreakMVConnect(string viewname, EventCenter model)
    {
        if (string.IsNullOrEmpty(GetDataSourceName) && string.IsNullOrEmpty(UpdateEventName)) { Debug.Log("不进行MVConnect"); return; }
        model.UnListenEvent<ItemPageChangeParm>(UpdateEventName, OnBPChange);
        model.UnListenEvent(GetDataSourceName, Flush);
        controll.BreakMVConnect(viewname, model);
        //throw new NotImplementedException();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            string s = "",v="";
            foreach (var item in showerCoord_Index)
            {
                s += item.Key.GetX()+":"+item.Key.GetY() + ",";
                v += item.Value + ",";
            }
            Debug.Log(s);
            Debug.Log(v);
        }
    }
    
    public int GetNotNullCount(Item[] it)
    {
        int ans = 0;
        for (int i = 0; i < it.Length; i++)
        {
            if(!Item.IsNullOrEmpty( it[i]))
            {
                ans++;
            }
        }
        return ans;
    }
}