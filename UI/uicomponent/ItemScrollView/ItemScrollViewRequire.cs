using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemScrollViewRequire : ItemScrollView_big
{
    /// <summary>
    /// len0=h,len1=w
    /// </summary>
    public Item[,] itemNeed;
    [Tooltip("target正在合成的craftdata改变事件")]
    public CraftViewEventName RequireUpdateEventName;//target正在合成的craftdata改变事件

    public override void UIInit(UICenter center, BaseUIView view)
    {

        base.UIInit(center, view);

        //showerGroup = transform.Find("showerGroup");
        path2 = "Prefabs/UI/inGame/unit/ItemShowOnNeed";

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
    public override void Flush()
    {
        IBackPack ib = controll.GetModel().GetParm<IBackPack>(GetDataSourceName);//刷新

        Item[,] a = ib.GetBigItems(1);
        //Debug.Log(a.GetLength(0)+a[0,0].ToString());
        Item[,] b = ib.GetBigItems(0);
        //Debug.Log(b.GetLength(0) + b[0, 0].ToString());
        SetNeedItems(ib.GetBigItems(1),ib.GetBigItems(page));

        bar.value = 1;
    }

    public override void OnBPChange(ItemPageChangeParm parm)
    {
        Debug.Log(parm);
        if (parm.page == 1)
        {
            //需求物改变，不会触发
        }
        else {
            int xy = XYHelper.ToIntXY(parm.x, parm.y);
            switch (parm.mode)
            {
                case ObjInPageChangeMode.delete:
                    SetItemAt(Item.Empty, parm.x, parm.y);
                    //UnShowItemAt(parm.x, parm.y);
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
        }//存货改变
    }
    public override void SetItemAt(Item i,  int w,int h)
    {
        //Debug.Log("setitemat调用"+w+":"+h+"need="+itemNeed.GetLength(0)+":"+itemNeed.GetLength(1));
        ItemShowerOnNeed temp = (ItemShowerOnNeed)itemshowerManager.GetOrNew(h,w);
        if (temp == null)//新增一个itemshower
        {
            temp = (ItemShowerOnNeed)itemshowerManager.GetOrNew(h, w);
        }
        if (Item.IsNullOrEmpty(i))
        {
            temp.ClearNum(itemNeed[h,w].num);
            return;
        }
        temp.SetNumWithFixSize(i,itemNeed[h,w].num);//只改变显示值
        temp.SetCenterPosi(slotsManager.GetRect(h,w));
    }
    public void OnCraftingUpdate(Item[] cd)
    {
        //需求物改变
        Debug.Log("需求物改变");
        if (cd == null || cd.Length <= 0)
        {
            SetContentSize(0, 0);
            SetSlotSize(0, 0);
            itemshowerManager.RecycleAll();
        }
        else
        {
            Item[,] temp = new Item[cd.Length / 4 + 1, 4];
            for (int i = 0; i < cd.Length; i++)
            {
                temp[i / 4, i % 4] = cd[i];
            }
            SetNeedItems(temp);
        }
    }
    public void SetNeedItems(Item[,] need,Item[,] items)
    {
        if (need == null) return;
        SetContentSize(need.GetLength(1), need.GetLength(0));
        SetSlotSize(need.GetLength(1), need.GetLength(0));
        itemshowerManager.RecycleAll();

        itemNeed = need;

        StartCoroutine(waitSlotLayout_Require(need, items));
    }
    public void SetNeedItems(Item[,] items)
    {//先扩格子大小，再设置item显示物，如果不够则新增，如果太多则设空item

        if (items == null) return;
        SetContentSize(items.GetLength(1), items.GetLength(0));
        SetSlotSize(items.GetLength(1), items.GetLength(0));
        itemshowerManager.RecycleAll();

        itemNeed = items;

        Item[,] temp = new Item[items.GetLength(0), items.GetLength(1)];
        StartCoroutine(waitSlotLayout_Require(items,temp));
    }
    IEnumerator waitSlotLayout_Require(Item[,] need,Item[,] items)
    {//len(0)=hig，len(1)=wid
        yield return null;
        Debug.Log("waitLayout");
        //itemshowerManager.SetNum(items.GetLength(1), items.GetLength(0));
        for (int i = 0; i < need.GetLength(1); i++)//w
        {
            for (int j = 0; j < need.GetLength(0); j++)//h
            {
                if (Item.IsNullOrEmpty(need[j, i])) continue;
                ItemShowerOnNeed ish = (ItemShowerOnNeed)itemshowerManager.GetOrNew(j, i);
                Debug.Log(ish == null ? "a" : "b");
                ish.ShowWithFixSize(need[j, i], items[j,i]);
                ish.SetCenterPosi(slotsManager.GetRect(j, i));
            }
        }

    }

    /// <summary>
    /// x=横坐标wid，y=纵坐标height
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Item GetNeedItem(int x,int y)
    {
        //Debug.Log(itemNeed.GetLength(0)+":"+itemNeed.GetLength(1)+":"+x+":"+y);
        return itemNeed[ y,x];
    }

    public override void BuildMVConnect(string viewname, EventCenter model)
    {
        Debug.Log(RequireUpdateEventName.ToString());
        base.BuildMVConnect(viewname, model);
        model.ListenEvent<Item[]>(RequireUpdateEventName.ToString(), OnCraftingUpdate);//监听target的原料格变化、事件
    }
    public override void BreakMVConnect(string viewname, EventCenter model)
    {
        base.BreakMVConnect(viewname, model);
        model.UnListenEvent<Item[]>(RequireUpdateEventName.ToString(), OnCraftingUpdate);//监听target的原料格变化、事件
    }
}
