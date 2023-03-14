using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[System.Obsolete]
public class BaseBackPack_Big : MonoBehaviour, IEventRegister, IBackPack, IDataContainer
{
    protected EventCenter center;
    public ItemPageBig_Data[] itemPages;

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
        center.RegistFunc<int>("get_bpSize", () => { return itemPages.Length; });
        center.RegistFunc<int, Item[]>("Getbp_Items", GetItems);
        
        //center.ListenEvent<BaseUIController, BaseUIView>("buildMVConnect", BuildMVConnect);
        //center.ListenEvent<BaseUIController, BaseUIView>("breakMVConnect", BreakMVConnect);
    }

    public virtual void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }
    public void init() { }
    public virtual void ClearItems(int page=0)
    {
        if (page >= 0 && page < itemPages.Length)
        { itemPages[page].ClearItems(); center.SendEvent<int>("bpFlush", page); }
        else { Debug.Log(page + "超出itempage范围" + itemPages.Length); }
    }
    public virtual void SetSize(int h, int w, int page = 0)
    {
        List<ItemPageBig_Data> temp = new List<ItemPageBig_Data>();
        for (int i = 0; i < page; i++)
        {
            temp.Add(new ItemPageBig_Data(h, w));
        }
        itemPages = temp.ToArray();
    }
    public virtual void SetItems(Item[] items, int[,] placements, int page = 0)
    {
        Item[,] temp = new Item[placements.GetLength(0), placements.GetLength(1)];
        int index = 0;
        for (int i = 0; i < temp.GetLength(0); i++)
        {
            for (int j = 0; j < temp.GetLength(1); j++)
            {
                temp[i, j] = items[placements[i,j]];
            }
        }
        itemPages[page].SetItems(temp);
        center.SendEvent<int>("bpFlush", page);
    }
    public virtual Item[] GetItems(int page = 0)
    {
        Item[] its = new Item[itemPages[page].items.GetLength(0) * itemPages[page].items.GetLength(1)];
        for (int i = 0; i < itemPages[page].items.GetLength(0); i++)
        {
            for (int j = 0; j < itemPages[page].items.GetLength(1); j++)
            {
                its[i * itemPages[page].items.GetLength(1) + j] = itemPages[page].items[i, j];
            }
        }
        return its;
    }
    public int[,] GetPlacements(int page = 0)
    {
        return null;
    }

    #region  set
    public virtual bool SetItemAt(Item i, int x, int y, int page = 0)
    {
        int[] xy = itemPages[page].GetItemLeftUp(x, y);
        int index = 0;
        bool p = itemPages[page].SetItemAt(i, x, y);

        Debug.Log("setitemat" + xy[0] + ":" + xy[1]);
        SendItemEvent(itemPages[page].GetItemAt(x, y), xy[0], xy[1], index, page, ObjInPageChangeMode.set);
        return p;
    }
    public virtual bool DeleteItemAt(int x, int y, int page = 0)
    {
        int[] xy = itemPages[page].GetItemLeftUp(x, y);
        int index = 0;
        bool p = itemPages[page].DeleteItemAt(x, y);

        SendItemEvent(itemPages[page].GetItemAt(x, y), xy[0], xy[1], index, page, ObjInPageChangeMode.delete);

        return p;
    }

    public virtual int AddItem(Item item, int page = 0)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return 0; }
        int p = 0;
        Item temp = new Item(item);
        if (page == -1)
        {
            
            for (int i = 0; i < itemPages.Length; i++)
            {
                for (int j = 0; j < itemPages[i].width; j++)//len1
                {
                    for (int k = 0; k < itemPages[i].height; k++)//len0
                    {
                        if(itemPages[i].CanPlaceAt(item,j,k))
                        {
                            int remain= itemPages[i].AddItemAt(temp, j, k);
                            temp.num = remain;
                            SendItemEvent(itemPages[i].GetItemAt(j, k), j, k, j, page, ObjInPageChangeMode.add);
                            if (temp.num == 0) break;
                        }
                    }
                    if (temp.num == 0) break;
                }
                if (temp.num == 0) break;
            }
        }
        else
        {
            
            for (int j = 0; j < itemPages[page].width; j++)//len1
            {
                for (int k = 0; k < itemPages[page].height; k++)//len0
                {
                    if (itemPages[page].CanPlaceAt(item, j, k))
                    {
                        int remain = itemPages[page].AddItemAt(temp, j, k);
                        temp.num = remain;
                        SendItemEvent(itemPages[page].GetItemAt(j, k), j, k, j, page, ObjInPageChangeMode.add);
                        if (temp.num == 0) break;
                    }
                }
                if (temp.num == 0) break;
            }
        }
        return 0;
    }
    public virtual int AddItemAt(Item i, int x, int y, int page = 0)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return 0; }
        int[] xy = itemPages[page].GetItemLeftUp(x, y);
        int index = 0;
        int p = itemPages[page].AddItemAt(i, x, y);

        SendItemEvent(itemPages[page].GetItemAt(x, y), xy[0], xy[1], index, page, ObjInPageChangeMode.add);
        return p;
    }
    public virtual int AddItemNumAt(int i, int x, int y, int page = 0)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return 0; }
        int[] xy = itemPages[page].GetItemLeftUp(x, y);
        int index = 0;
        int p = itemPages[page].AddItemNumAt(i, x, y);
        SendItemEvent(itemPages[page].GetItemAt(x, y), xy[0], xy[1], index, page, ObjInPageChangeMode.add);
        return p;
    }
    public virtual int SubItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        /*if (itemPages == null) { Debug.Log("物品栏为空"); return 0; }
        if (Item.IsNullOrEmpty(i)) return 0;
        i = new Item(i);
        int remain = i.num;
        Debug.Log("itemPages"+ itemPages[page].width+":"+ itemPages[page].height);
        for (int w = 0; w < itemPages[page].width; w++)//wid,x len1
        {
            for (int h = 0; h < itemPages[page].height; h++)//h len0
            {
                if (Item.IsNullOrEmpty(itemPages[page].GetItemAt(w, h))) continue;
                if(i.Compare(itemPages[page].GetItemAt(w,h),mode))
                {
                    int oldnum = itemPages[page].GetItemAt(w, h).num;
                   int temp= itemPages[page].SubItemAt(i, w, h, mode);
                    remain = remain - (oldnum - temp);
                    i.num = remain;
                    SendItemEvent(itemPages[page].GetItemAt(w,h), w,h,w, page, ObjInPageChangeMode.sub);
                }
            }
        }*/
        return 0;
    }
    
    public virtual int SubItemAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return 0; }
        int[] xy = itemPages[page].GetItemLeftUp(x, y);
        int index = 0;
        int p = itemPages[page].SubItemAt(item, x, y);
        SendItemEvent(itemPages[page].GetItemAt(x, y), xy[0], xy[1], index, page, ObjInPageChangeMode.sub);

        return p;
    }
    public virtual int SetItemNumAt(int num, int x, int y, int page = 0)
    {
        int[] xy = itemPages[page].GetItemLeftUp(x, y);
        int index = 0;
        int p = itemPages[page].SetItemNumAt(num, x, y);
        SendItemEvent(itemPages[page].GetItemAt(x, y), xy[0], xy[1], index, page, ObjInPageChangeMode.set);
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
            for (int i = 0; i < itemPages.Length; i++)
            {
                count += itemPages[i].Contain(item,page, mode);
            }
            return count;
        }
        else
        {
            return itemPages[page].Contain(item, page, mode);
        }

    }
    public virtual int ContainAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return 0; }
        return itemPages[page].ContainAt(item, x, y, page, mode);
    }
    public virtual Item GetItemAt(int x, int y, int page = 0)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return null; }
        return itemPages[page].GetItemAt(x, y);
    }
    public virtual int CountItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return 0; }
        return itemPages[page].CountItem(i, page, mode);
    }
    public virtual bool CanPlaceAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return false; }
        if (Item.IsNullOrEmpty(item)) return true;
        return itemPages[page].CanPlaceAt(item, x, y, page, mode);
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


    public virtual void SendItemEvent(Item i, int x, int y, int index, int page, ObjInPageChangeMode mode)
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
        if (itemPages[page].items == null) itemPages[page].init();
        return itemPages[page].items;
    }

    public virtual void FromObject(object data)
    {
        JArray temp = ((JArray)(data)).ToObject<JArray>();
        itemPages = temp.ToObject<ItemPageBig_Data[]>();
        for (int i = 0; i < itemPages.Length; i++)
        {
            itemPages[i].init();
        }
    }
    public virtual object ToObject()
    {
        //string[] temp ;
        JArray st = new JArray();
        if (itemPages != null)
        {
            JArray itempageArr = new JArray();
            for (int i = 0; i < itemPages.Length; i++)
            {
                itempageArr.Add((JObject)itemPages[i].ToObject());
            }
            st = itempageArr;
        }
        return st;
    }

    
}
