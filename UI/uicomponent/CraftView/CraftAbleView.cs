using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UICompCTRL_CraftAbleView))]
public class CraftAbleView : ItemScrollView_big
{
    Dictionary<long, int> pos2UUID = new Dictionary<long, int>();

    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);
        view.GetCenter.ListenEvent<ItemGroup>(nameof(CraftViewEventName.typeClick), OnTypeClick);
    }
    void OnTypeFlush(int uuid)
    {
        Craft_Data cd = EventCenter.WorldCenter.GetParm<int, Craft_Data>(nameof(EventNames.GetCraftDataByUUID), uuid);
        if (uuid == 0||Craft_Data.IsNullOrEmpty(cd)) return;
        ItemGroup ig= EventCenter.WorldCenter.GetParm<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), cd.product.id);

        int[] i = EventCenter.WorldCenter.GetParm<ItemGroup, int[]>(nameof(EventNames.GetCraftUUIDByGroup), ig);
        SetCrafts(i);
    }
    void OnTypeClick(ItemGroup t)
    {
        Debug.Log(t);
        int[] i = EventCenter.WorldCenter.GetParm<ItemGroup, int[]>(nameof(EventNames.GetCraftUUIDByGroup), t);
        SetCrafts(i);
        if (i != null && i.Length > 0)
        {
            Debug.Log(i[0]);
            controll.OnEvent(UIComp_EventName.itemSlotClick, i[0]);
        }
        else
        {
            controll.OnEvent(UIComp_EventName.Clear,null);
        }

    }
    /// <summary>
    /// len(0)=hig,len(1)=wid
    /// </summary>
    /// <param name="items"></param>
    public void SetCrafts(int[] crafts)
    {
        Debug.Log("获取到");
        if (crafts == null) { pos2UUID.Clear(); base.SetItems(new Item[0,0]); return; }
        int w = itemWidth;
        int h = crafts.Length / w + 1;
        Item[,] items = new Item[h,w];
        pos2UUID.Clear();
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                if (i * w + j < crafts.Length)
                {
                    Craft_Data cd = EventCenter.WorldCenter.GetParm<int, Craft_Data>(nameof(EventNames.GetCraftDataByUUID), crafts[i * w + j]);
                    items[i, j] = cd.product;
                    pos2UUID.Add(XYHelper.ToLongXY(i, j), crafts[i * w + j]);
                }else
                {
                    items[i, j] = null;
                }
            }
        }
        base.SetItems(items);

    }

    public override void SetItems(Item[,] items)
    {//先扩格子大小，再设置item显示物，如果不够则新增，如果太多则设空item

        SetContentSize(items.GetLength(1), items.GetLength(0));
        SetSlotSize(items.GetLength(1), items.GetLength(0));
        itemshowerManager.RecycleAll();

        Debug.Log("设置大小：" + items.GetLength(1) + "," + items.GetLength(0));
        StartCoroutine(waitCraftSlotLayout(items));
    }
    IEnumerator waitCraftSlotLayout(Item[,] items)
    {//len(0)=hig，len(1)=wid
        yield return null;

        //itemshowerManager.SetNum(items.GetLength(1), items.GetLength(0));
        for (int i = 0; i < items.GetLength(1); i++)//h
        {
            for (int j = 0; j < items.GetLength(0); j++)//w
            {
                if (Item.IsNullOrEmpty(items[j, i])) continue;
                ItemShower ish = (ItemShower)itemshowerManager.GetOrNew(j, i);
                ish.ShowWithFixSize(items[j, i]);
                ish.SetBG(false);
                ish.SetCenterPosi(slotsManager.GetRect(j, i));
            }
        }

    }

    public override void OnClick(int stat, int x, int y, int page)
    {   if(pos2UUID.ContainsKey(XYHelper.ToLongXY(x, y)))
        {
            controll.OnEvent(UIComp_EventName.itemSlotClick, pos2UUID[XYHelper.ToLongXY(x, y)]);
        }
        //base.OnClick(stat, pos2UUID[XYHelper.ToLongXY(x,y)], 0, page);
    }
    public override void BuildMVConnect(string viewname, EventCenter model)
    {
        //base.BuildMVConnect(viewname, model);
        controll.BuildMVConnect(viewname, model);
        int curretCrafting = model.GetParm<int>(nameof(CraftViewEventName.GetCurrentCrafting));
        
        OnTypeFlush(curretCrafting);
    }

}
