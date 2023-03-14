using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 方块，NPC,玩家共同的背包父类
/// </summary>
[Obsolete]
public class BackPack : MonoBehaviour,IEventRegister,IDataContainer,IBackPack
{
   protected Dictionary<int,IBackPack> items;
    protected EventCenter center;

    protected ItemPageChangeParm parm=new ItemPageChangeParm();

    public virtual int GetDataCollectPrio => 0;

    public void AfterEventRegist()
    {
        
    }
    public void OnEventRegist(EventCenter e)
    {
        center = e;
        e.RegistFunc<IBackPack>("backpack", () => { return this; });
    }
    public void init() { }
    void SendItemEvent(Item i,int x,int y, int page,ObjInPageChangeMode mode)
    {
        parm.item = i;
        parm.x = x;
        parm.y = y;
        parm.page = page;
        parm.mode = mode;
        center.SendEvent<ItemPageChangeParm>("backpackChg",parm);
    }




    #region backpack
    public void ClearItems(int page = 0)
    {
        throw new NotImplementedException();
    }
    public void SetSize(int h, int w, int page = 0)
    {
        items.Clear();
        //List<ItemPage_Data> temp = new List<ItemPage_Data>();
        for (int i = 0; i < page; i++)
        {
            items.Add(i,new ItemPage_Data(h, w));
        }
        
    }
    public void SetItems(Item[] items, int[,] placements, int page = 0)
    {
        this.items[page].SetItems(items, placements);
    }
    #region  set


    public virtual bool SetItemAt(Item i, int x, int y, int page = 0)
    {
        int[] xy = items[page].GetItemLeftUp(x, y);
        bool p = items[page].SetItemAt(i, x, y);
        SendItemEvent(items[page].GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.set);
        return p;
    }
    public virtual bool DeleteItemAt(int x, int y, int page = 0)
    {
        int[] xy = items[page].GetItemLeftUp(x, y);
        bool p = items[page].DeleteItemAt(x, y);
        SendItemEvent(items[page].GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.delete);
        return p;
    }

    public virtual int AddItem(Item item, int page = 0)
    {
        int p = item.num;
        
        return p;
    }
    public virtual int AddItemAt(Item i, int x, int y, int page = 0)
    {
        int[] xy = items[page].GetItemLeftUp(x, y);
        int p = items[page].AddItemAt(i, x, y);
        SendItemEvent(items[page].GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.add);

        return p;
    }
    public virtual int SubItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return 0;
    }
    public virtual int SubItemAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        int[] xy = items[page].GetItemLeftUp(x, y);
        int p = items[page].SubItemAt(item, x, y);
        SendItemEvent(items[page].GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.sub);

        return p;
    }
    public virtual int SetItemNumAt(int num, int x, int y, int page = 0)
    {
        int[] xy = items[page].GetItemLeftUp(x, y);
        int p = items[page].SetItemNumAt(num, x, y);
        SendItemEvent(items[page].GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.set);

        return p;
    }
    public int AddItemNumAt(int i, int x, int y, int page = 0)
    {
        int[] xy = items[page].GetItemLeftUp(x, y);
        int p = items[page].AddItemNumAt(i, x, y);

        SendItemEvent(items[page].GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.add);
        return p;
    }
    #endregion

    #region  get
    public virtual int Contain(Item item, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        int count = 0;
        if (page == -1)
        {
            foreach (var ite in items)
            {
                count += items[ite.Key].Contain(item,page, mode);
            }
            
            return count;
        }
        else
        {
            count += items[page].Contain(item,page, mode);
            return count;
        }
    }
    public virtual int ContainAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return items[page].ContainAt(item, x, y);
    }
    public virtual Item GetItemAt(int x, int y, int page = 0)
    {
        return items[page].GetItemAt(x, y);
    }
    public virtual int CountItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return items[page].CountItem(i,page, mode);
    }
    public virtual bool CanPlaceAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return items[page].CanPlaceAt(item, x, y, page,mode);
    }
    public int[] GetItemLeftUp(int x, int y, int page = 0)
    {
        //throw new System.NotImplementedException();
        return items[page].GetItemLeftUp(x, y);
    }
    public virtual bool CanPlaceIgnoreCurrent(Item it, int x, int y, int page = 0)
    {
        return items[page].CanPlaceIgnoreCurrent(it, x, y);
    }
    public Item[] GetItems(int page = 0)
    {
        return items[page].GetItems(page);
    }
    public int[,] GetPlacements(int page = 0)
    {
        return items[page].GetPlacements(page);
    }
    public Item[,] GetBigItems(int page = 0)
    {
        throw new System.NotImplementedException();
    }
    public bool IsBigChest()
    {
        throw new NotImplementedException();
    }
    #endregion
    #endregion

    


    #region serialize
    public void FromObject(object data)
    {

    }
    public object ToObject()
    {
        return null;
    }

   


    #endregion
}
