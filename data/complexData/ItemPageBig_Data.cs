using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

[Serializable]
public class ItemPageBig_Data:IBackPack
{
    int index;
    public bool isBigChest = false;
    public int width, height;
    /// <summary>
    /// len 0=height,len 1=width
    /// </summary>
    public Item[,] items;
    /// len(0)=height,len(1)=wid,x=横坐标
    public ItemPageBig_Data() { }
    public ItemPageBig_Data(int h, int w) { items = new Item[ h,w]; width = w; height = h; }//行是height，列是width
    public ItemPageBig_Data(int h, int w,bool isbig) { items = new Item[h, w]; width = w; height = h; isBigChest = isbig; }//行是height，列是width

    public void init()
    {
        if(items==null||items.GetLength(0)!=height||items.GetLength(1)!=width)
        {
            items = new Item[height, width];
        }
    }

    public void SetItems(Item[,] items)
    {
        this.items = items;

        width = items.GetLength(1);
        height = items.GetLength(0);
    }
    public void SetItems(Item[] its,int width)
    {
        this.width = width;
        height = (its.Length / width)+((its.Length % width)==0?0:1);
        Debug.Log(its.Length+":"+width + ":" + height);
        init();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (i * width + j >= its.Length)
                { items[i, j] = Item.Empty; }
                else { items[i, j] = its[i * width + j]; }
            }
        }

    }
    public void SetItems(Item[] items, int[,] placements,int page=0)
    {
        width = placements.GetLength(1);
        height = placements.GetLength(0);
        Item[,] its = new Item[width, height];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if(placements[j,i]!=-1)
                    its[j, i] = items[placements[j,i]];
            }
        }
        this.items = its;
    }

    public int Contain(Item item,int page=0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        int contain = 0;
        for (int j = 0; j < items.GetLength(0); j++)//height
        {
            for (int i = 0; i < items.GetLength(1); i++)
            {
                if (items[j, i].Compare(item, mode)) contain += items[j, i].num;
            }
        }
        return contain;
    }
    #region get
    public bool IsBigChest()
    {
        return isBigChest;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x">行号</param>
    /// <param name="y">列号</param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public int ContainAt(Item item, int x, int y,int page=0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (y >= height || y < 0) return -1;
        if (x >= width || x < 0) return -1;
        if (items[y, x].Compare(item, mode)) return items[y, x].num;

        return -1;
    }

    public bool CanPlaceIgnoreCurrent(Item it, int x, int y,int page=0)
    {
        if (it.IsNumOverFlow(isBigChest) > 0) return false;
        if (y >= height || y < 0) return false;
        if (x>= width || x < 0) return false;
        return true;
    }

    public Item[] GetItems(int page=0)
    {
        List<Item> ans=new List<Item>();
        for (int i = 0; i < items.GetLength(0); i++)
        {
           for (int j = 0; j < items.GetLength(1); j++)
           {
                ans.Add(items[i,j]);
           }
        }
        return ans.ToArray();//先hig再wid
    }
    public Item[,] GetBigItems(int page = 0)
     {
        return items;
     }
     /// <summary>
     /// 有用，返回wid,hig
     /// </summary>
     /// <param name="page"></param>
     /// <returns></returns>
    public int[,] GetPlacements(int page=0)
     {
        
        return new int[height,width];
     }
    /// <summary>
    /// x=len1,y=len0
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Item GetItemAt(int x, int y,int page=0)
    {
        try
        {
            if (Item.IsNullOrEmpty(items[y, x])) return Item.Empty;
            return items[ y,x];
        }
        catch (System.Exception)
        {
            Debug.Log("获取item失败at" + x + "," + y+":"+items.GetLength(0)+":"+items.GetLength(1));
            return Item.Empty;
            //throw;
        }

    }
    public int CountItem(Item item,int page=0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (Item.IsNullOrEmpty(item)) return 0;
        int count = 0;
        for (int i = 0; i < items.GetLength(0); i++)//height
        {
            for (int j = 0; j < items.GetLength(1); j++)//width
            {
                if (Item.IsNullOrEmpty(items[i, j])) continue;
                if (items[i, j].Compare(item, mode))
                {
                    count += items[i, j].num;
                }
            }
        }
        return count;
    }
    /// <summary>
    /// 只有空和相等才可放置
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public bool CanPlaceAt(Item item, int x, int y,int page=0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {

        if (y >= height || y < 0) return false;
        if (x >= width || x < 0) return false;
        if (Item.IsNullOrEmpty(GetItemAt(x,y))) return true;
        if (GetItemAt(x, y).Compare(item, mode)) return true;
        return false;
    }
    public int[] GetItemLeftUp(int x, int y,int page=0)
    {
        return new int[] { x, y };

    }
    #endregion

    #region set
    public void SetSize(int x, int y, int page=0)
    {

    }
    public bool SetItemAt(Item item, int x, int y,int page=0)
    {
        if (Item.IsNullOrEmpty(item)) { Debug.Log("空item"); return false; }
        try
        {
            if (y >= height || y < 0) return false;
            if (x >= width ||x < 0) return false;
            items[y,x] = item;
            return true;
        }
        catch (System.Exception)
        {
            Debug.Log("无法在此设置item " + x + "," + y);
            return false;
        }

    }
    public bool DeleteItemAt(int x, int y,int page=0)
    {
        try
        {
            if (y >= height || y < 0) return false;
            if (x >= width || x < 0) return false;
            items[y, x] = Item.Empty;
            return true;
        }
        catch (System.Exception)
        {
            Debug.Log("无法在此删除item " + x + "," + y);
            return false;
        }
    }
    /// <summary>
    /// 无用
    /// </summary>
    /// <param name="i"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public int AddItem(Item i, int page = 0)
    {
        return 0;
    }
    /// <summary>
    /// 无用
    /// </summary>
    /// <param name="i"></param>
    /// <param name="page"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
public int SubItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
{
    return 0;
}
    /// <summary>
    /// 返回加完剩下多少item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int AddItemAt(Item item, int x, int y,int page=0)
    {
        if (y >= height || y < 0) return -1;
        if (x >= width || x < 0) return -1;

        if (Item.IsNullOrEmpty(item)) { Debug.Log("空item"); return 0; }
        try
        {
            if (item.Compare(GetItemAt(x,y)))
            {
                return GetItemAt(x, y).SafeAdd(item.num,item.level);
            }
            else if (Item.IsNullOrEmpty(GetItemAt(x, y)))
            {
                items[y,x] = new Item(item);
                int max = items[y, x].GetMaxNum();
                items[y, x].num = Mathf.Clamp(items[y, x].num,0,max);
                return item.num-items[y, x].num;
            }
            else return -1;//添加位置上的item与要加item不一样
        }
        catch (System.Exception)
        {
            Debug.Log("无法在此additem " + x + "," + y);
            throw;
        }
    }

    public int AddItemNumAt(int i,int x,int y,int page=0)
    {
        if (y >= height || y < 0) return -1;
        if (x >= width || x < 0) return -1;
        try
        {
            if (!Item.IsNullOrEmpty(GetItemAt(x, y)))
            {
                return GetItemAt(x, y).SafeAdd(i,-1);
            }
            else return -1;//添加位置上的item与要加item不一样
        }
        catch (System.Exception)
        {
            Debug.Log("无法在此additem " + x + "," + y);
            throw;
        }
    }
    /// <summary>
    /// 如果不够减，则不做任何处理
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public int TrySubItemAt(Item item, int x, int y, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (y >= height || y < 0) return -1;
        if (x >= width || x < 0) return -1;
        if (Item.IsNullOrEmpty(item)) { Debug.Log("空item"); return 0; }
        try
        {
            if (item.Compare(GetItemAt(x, y)))
            {
                if (item.num > GetItemAt(x, y).num)
                {
                    int res = GetItemAt(x, y).num - item.num;
                    //GetItemAt(x, y).num = 0;

                    return res;
                }
                else { GetItemAt(x, y).num -= item.num; return GetItemAt(x, y).num; }
                //return GetItemAt(x, y).SafeSub(item.num);
            }
            else if (Item.IsNullOrEmpty(GetItemAt(x, y)))
            {
                return 0;
            }
            else return -1;//添加位置上的item与要加item不一样
        }
        catch (System.Exception)
        {
            Debug.Log("无法在此subitem " + x + "," + y);
            throw;
        }
    }

    /// <summary>
    /// 返回现有格内减剩下多少,不够减的话返回差多少个
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public int SubItemAt(Item item, int x, int y,int page=0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (y >= height || y < 0) return -1;
        if (x >= width || x < 0) return -1;
        if (Item.IsNullOrEmpty(item)) { Debug.Log("空item"); return 0; }
        try
        {
            if (item.Compare(GetItemAt(x, y)))
            {
                if (item.num > GetItemAt(x, y).num)
                {
                    int res = GetItemAt(x, y).num - item.num;
                    GetItemAt(x, y).num = 0;
                    
                    return res;
                }
                else { GetItemAt(x, y).num -= item.num; return GetItemAt(x, y).num; }
                //return GetItemAt(x, y).SafeSub(item.num);
            }
            else if (Item.IsNullOrEmpty(GetItemAt(x, y)))
            {
                return 0;
            }
            else return -1;//添加位置上的item与要加item不一样
        }
        catch (System.Exception)
        {
            Debug.Log("无法在此subitem " + x + "," + y);
            throw;
        }
    }
    public int SetItemNumAt(int num, int x, int y,int page=0)
    {
        if (y >= height || y < 0) return -1;
        if (x >= width || x < 0) return -1;
        try
        {
            GetItemAt(x, y).num = num;
            return 0;
        }
        catch (System.Exception)
        {
            Debug.Log("无法在此设值 " + x + "," + y);
            throw;
        }
    }

    #endregion

    public void ClearItems(int page=0)
    {
        for (int i = 0; i < items.GetLength(0); i++)
        {
            for (int j = 0; j < items.GetLength(1); j++)
            {
                items[i, j] = Item.Empty;
            }
        }
    }
    public object ToObject()
    {
        return JObject.FromObject(this);
    }
    public static ItemPage_Data FromObject(object obj)
    {
        ItemPage_Data i = ((JObject)obj).ToObject<ItemPage_Data>();
        i.init();
        return i;
    }
    /// <summary>
    /// 不可用
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        //List<string> datas = new List<string>();
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
