using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class BaseBackPack : MonoBehaviour, IEventRegister, IBackPack,IDataContainer
{
    protected EventCenter center;
    public Dictionary<int,IBackPack> itemPages;

   protected ItemPageChangeParm itemChangeArg = new ItemPageChangeParm();

    public int GetDataCollectPrio => 0;

    void Start()
    {

    }

    public virtual void OnEventRegist(EventCenter e)
    {
        //base.OnEventRegist(e);
        center = e;
        center.RegistFunc<IBackPack>("backpack", () => { return this; });
        center.RegistFunc<int>("get_bpSize", () => { return itemPages.Count; });
        center.RegistFunc<int, int[,]>("Getbp_Placement", (int p) => { return itemPages[p].GetPlacements(); });
        center.RegistFunc<int, Item[]>("Getbp_Items", GetItems);
        
         //center.ListenEvent<BaseUIController, BaseUIView>("buildMVConnect", BuildMVConnect);
        //center.ListenEvent<BaseUIController, BaseUIView>("breakMVConnect", BreakMVConnect);
    }

    public virtual void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }

    public void init() { }

    public virtual void SetSize(int h, int w, int page = 0)
    {
        itemPages.Clear();
        for (int i = 0; i < page; i++)
        {
            itemPages.Add(i,new ItemPage_Data(h, w));
        }
    }
    public virtual void SetItems(Item[] items, int[,] placements, int page = 0)
    {
        itemPages[page].SetItems(items, placements,page);
        center.SendEvent<int>("bpFlush", page);
    }
    public virtual Item[] GetItems(int page = 0)
    {
        return itemPages[page].GetItems(page);
    }
    public int[,] GetPlacements(int page = 0)
    {
        return itemPages[page].GetPlacements(page);
    }

    #region  set
    public virtual void ClearItems(int page = 0)
    {
        throw new System.NotImplementedException();
    }
    public virtual bool SetItemAt(Item i, int x, int y, int page = 0)
    {
        int[] xy = itemPages[page].GetItemLeftUp(x, y);
        int index = x;//itemPages[page].GetIndex(x, y);
        bool p = itemPages[page].SetItemAt(i, x, y);

        //Debug.Log("setitemat"+xy[0] + ":" + xy[1]);
        SendItemEvent(itemPages[page].GetItemAt(x, y), xy[0], xy[1],index, page, ObjInPageChangeMode.set);
        return p;
    }
    public virtual bool DeleteItemAt(int x, int y, int page = 0)
    {
        int[] xy = itemPages[page].GetItemLeftUp(x, y);
        int index = x;//itemPages[page].GetIndex(x, y);
        bool p = itemPages[page].DeleteItemAt(x, y);

        SendItemEvent(itemPages[page].GetItemAt(x, y), xy[0], xy[1],index, page, ObjInPageChangeMode.delete);

        return p;
    }

    public virtual int AddItem(Item item, int page = 0)
    {
        int p = 0;
        for (int i = 0; i < itemPages.Count; i++)
        {
            if (itemPages[i].Contain(item) > 0)
            {

            }
        }
        return 0;
    }
    public virtual int AddItemAt(Item i, int x, int y, int page = 0)
    {
        int[] xy = itemPages[page].GetItemLeftUp(x, y);
        int index = x;//itemPages[page].GetIndex(x, y);
        int p = itemPages[page].AddItemAt(i, x, y);

        
        SendItemEvent(itemPages[page].GetItemAt(x, y), xy[0], xy[1],index, page, ObjInPageChangeMode.add);
        return p;
    }
    public virtual int AddItemNumAt(int i, int x, int y, int page = 0)
    {
        int[] xy = itemPages[page].GetItemLeftUp(x, y);
        int index = x;//itemPages[page].GetIndex(x, y);
        int p = itemPages[page].AddItemNumAt(i, x, y);
        SendItemEvent(itemPages[page].GetItemAt(x, y), xy[0], xy[1],index, page, ObjInPageChangeMode.add);
        return p;
    }
    public virtual int SubItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return 0;
    }
    public virtual int SubItemAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        int[] xy = itemPages[page].GetItemLeftUp(x, y);
        int index = x;//itemPages[page].GetIndex(x, y);
        int p = itemPages[page].SubItemAt(item, x, y);
        SendItemEvent(itemPages[page].GetItemAt(x, y), xy[0], xy[1], index,page, ObjInPageChangeMode.sub);

        return p;
    }
    public virtual int SetItemNumAt(int num, int x, int y, int page = 0)
    {
        int[] xy = itemPages[page].GetItemLeftUp(x, y);
        int index = x;//itemPages[page].GetIndex(x, y);
        int p = itemPages[page].SetItemNumAt(num, x, y);
        SendItemEvent(itemPages[page].GetItemAt(x, y), xy[0], xy[1],index, page, ObjInPageChangeMode.set);
        return p;
    }

    #endregion

    #region  get
    public virtual bool IsBigChest()
    {
        return false;
    }
    public virtual int Contain(Item item, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        int count = 0;
        if (page == int.MaxValue)//int.max查询所有itempage里含有的数量
        {
            for (int i = 0; i < itemPages.Count; i++)
            {
                count += itemPages[i].Contain(item,page,mode);
            }
            return count;
        }
        else
        {
            return itemPages[page].Contain(item,page,mode);
        }

    }
    public virtual int ContainAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return itemPages[page].ContainAt(item, x, y,page,mode);
    }
    public virtual Item GetItemAt(int x, int y, int page = 0)
    {
        return itemPages[page].GetItemAt(x, y);
    }
    public virtual int CountItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return itemPages[page].CountItem(i,page, mode);
    }
    public virtual bool CanPlaceAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (Item.IsNullOrEmpty(item)) return true;
        return itemPages[page].CanPlaceAt(item, x, y,page, mode);
    }
    public virtual bool CanPlaceIgnoreCurrent(Item it, int x, int y, int page = 0)
    {
        return itemPages[page].CanPlaceIgnoreCurrent(it, x, y);
    }
    #endregion
    

   
    public virtual int[] GetItemLeftUp(int x, int y, int page = 0)
    {
        return itemPages[page].GetItemLeftUp(x, y);
    }


    public virtual void SendItemEvent(Item i,int x,int y,int index,int page,ObjInPageChangeMode mode)
    {//x=hig纵坐标,y=wid横坐标
        itemChangeArg.item = i;
        itemChangeArg.x = x;
        itemChangeArg.y = y;
        itemChangeArg.index = index;
        itemChangeArg.page = page;
        itemChangeArg.mode = mode;
        //Debug.Log(itemChangeArg.ToString());
        center.SendEvent<ItemPageChangeParm>(nameof(PlayerEventName.bpChg), itemChangeArg);
    }

    public virtual Item[,] GetBigItems(int page = 0)
    {
       return itemPages[page].GetBigItems();
    }

    public virtual void FromObject(object data)
    {
        JObject temp = (JObject)data;
        itemPages=temp.ToObject<Dictionary<int,IBackPack>>();
    }
    public virtual object ToObject()
    {
        //string[] temp ;
        return JObject.FromObject(itemPages);
    }

    
}
