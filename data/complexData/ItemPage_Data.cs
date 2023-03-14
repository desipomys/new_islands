using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

/// <summary>
/// add,sub,delete,set
/// </summary>
public enum ObjInPageChangeMode
{
    add, sub, delete, set
}
public delegate void OnItemPageChange(Item i, int x, int y, int page, ItemCompareMode mode = ItemCompareMode.excludeNum);

[Serializable]
public class ItemPage_Data:IBackPack
{
    int index;

    public int width, height;
    public Item[] items=new Item[1];
    public bool isBigChest=false;
    /// <summary>
    /// len(0)=height,len(1)=wid
    /// </summary>
    public int[,] placement=new int[1,1];

    public ItemPage_Data() { }
    public ItemPage_Data( int h,int w)
    {
        placement = new int[h, w];
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                placement[i, j] = -1;
            }
        }
        width = w;
        height = h;
    }//行是height，列是width
    public ItemPage_Data(int h,int w,bool isbig)
    {
        placement = new int[h, w];
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                placement[i, j] = -1;
            }
        }
        width = w;
        height = h;
        isBigChest=isbig;
    }
    public void init()
    {
        if(placement == null||placement.GetLength(0)!=height||placement.GetLength(1)!=width)
        {
            placement = new int[height, width];
            for (int i = 0; i < placement.GetLength(0); i++)
            {
                for (int j = 0; j < placement.GetLength(1); j++)
                {
                    placement[i, j] = -1;
                }
            }
        }
    }

    int findEmptyItemAt()
    {
        if (items == null) { items = new Item[] { new Item() }; return 0; }
        for (int i = 0; i < items.Length; i++)
        {
            if (Item.IsNullOrEmpty(items[i])) return i;
        }
        return -1;//没有空的了
    }

    

    public void SetItems(Item[] items, int[,] placements)
    {
        this.items = items;
        this.placement = placements;

        width = placements.GetLength(1);
        height = placements.GetLength(0);

        for (int i = 0; i < height; i++)//将指向空item的index设为-1
        {
            for (int j = 0; j < width; j++)
            {
                if (placement[i, j] != -1 && Item.IsNullOrEmpty(items[placement[i, j]]))
                {
                    placement[i, j] = -1;
                }
            }
        }
    }

    public int Contain(Item item,int page=0, ItemCompareMode mode = ItemCompareMode.absEqual)
    {
        if (Item.IsNullOrEmpty(item)) return 0;
        if (items==null) return 0;
        int contain = 0;
        for (int j = 0; j < items.Length; j++)
        {
            if (item.Compare(items[j], mode)) contain += items[j].num;
        }
        return contain;
    }
    #region get
    public bool IsBigChest()
    {
        return isBigChest;
    }
    public Item[] GetItems(int page=0)
    {
        return items;
    }
    public Item[,] GetBigItems(int page = 0)
    {
        return null;
    }
    public int[,] GetPlacements(int page=0)
    {
        return placement;
    }

    /// <summary>
    /// x=width,y=height
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int GetIndex(int x,int y)
    {
        return placement[ y,x];
    }
    public bool IsOutOfRange(int x, int y)
    {
        if (x < 0 || x >= width) return false;
        if (y < 0 || y >= height) return false;
        else return true;
    }
    /// <summary>
    /// x=width,y=height
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public int ContainAt(Item item, int x, int y, int page=0,ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (x >= height || x < 0) return -1;
        if (y >= width || y < 0) return -1;
        if (GetIndex(x, y) == -1) return -1;
        if (items.Length <= GetIndex(x, y)) return -1;
        if (item.Compare(items[GetIndex(x, y)], mode)) return items[GetIndex(x, y)].num;
        return -1;
    }
    public bool bakCanPlaceAt(Item item, int x, int y, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {//未测试
        int size = EventCenter.WorldCenter.GetParm<int, int>("ItemIDtoSize", item.id);
        int ix = XYHelper.GetIntX(size);//宽度
        int iy = XYHelper.GetIntY(size);//高度

        int indexAtXY = GetIndex(x, y);

        if (indexAtXY == -1)
        {
            for (int i = 0; i < iy; i++)
            {
                for (int j = 0; j < ix; j++)
                {
                    if (placement[y + i, x + j] != indexAtXY) return false;
                }
            }
            return true;
        }
        else if (items[indexAtXY].Compare(item, mode))
        {
            return true;
        }
        return false;
    }

    public bool CanPlaceIgnoreCurrent(Item it, int x, int y,int page=0)
    {
        if (it.IsNumOverFlow(isBigChest)>0) return false;
        int[] temp = it.GetSize();
        int truex = temp[0];//width
        int truey = temp[1];//height
        //if (it.rota) { int a = truey; truey = truex; truex = a; }
        Item itemat = GetItemAt(x, y);
        bool canplace = true;

        if (Item.IsNullOrEmpty(itemat))
        {
            for (int i = 0; i < truey; i++)//空的
            {
                for (int j = 0; j < truex; j++)
                {
                    if (y + i >= height || x + j >= width) { canplace = false; break; }
                    if (placement[y + i, x + j] != -1)
                    {
                        canplace = false;
                        break;
                    }
                }
            }
        }
        else
        {
            int current = GetIndex(x, y);
            for (int i = 0; i < truey; i++)
            {
                for (int j = 0; j < truex; j++)
                {
                    if (y + i >= height || x + j >= width) { canplace = false; break; }
                    if (placement[y + i, x + j] != -1 && placement[y + i, x + j] != current)
                    {
                        canplace = false; break;
                    }
                }
                if (!canplace) { break; }
            }
            return canplace;
        }
        return canplace;
    }
    public Item GetItemAt(int x, int y,int page=0)
    {
        try
        {
            return items[placement[y, x]];
        }
        catch (System.Exception)
        {
            //Debug.Log("获取item失败at" + x + "," + y + ";" + placement[x, y]);
            return Item.Empty;
            //throw;
        }

    }
    public int CountItem(Item item,int page=0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (Item.IsNullOrEmpty(item)) return 0;
        int count = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].Compare(item, mode))
            {
                count += items[i].num;
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
        try
        {
            if (Item.IsNullOrEmpty(GetItemAt(x, y)))
            {
                int[] size = item.GetSize();
                for (int i = 0; i < size[0]; i++)//width
                {
                    for (int j = 0; j < size[1]; j++)//height
                    {
                        if (y + j >= height || y + j < 0) return false;
                        if (x + i >= width || x + i < 0) return false;
                        if (placement[y + j, x + i] != -1) return false;
                    }
                }
                return true;
            }
            else
            {
                if (items[placement[y, x]].Compare(item, mode))
                {
                    return true;
                }
                return false;
            }
        }
        catch (System.Exception)
        {
            Debug.Log("无法放置item at" + x + "," + y);
            return false;
            //throw;
        }
    }
    public bool CanPlaceAtEmpty(Item item, int x, int y, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        try
        {
            if (Item.IsNullOrEmpty(GetItemAt(x, y)))
            {
                int[] size = item.GetSize();
                for (int i = 0; i < size[0]; i++)//width
                {
                    for (int j = 0; j < size[1]; j++)//height
                    {
                        if (y + j >= height || y + j < 0) return false;//超出边界
                        if (x + i >= width || x + i < 0) return false;
                        if (placement[y + j, x + i] != -1) return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (System.Exception)
        {
            Debug.Log("无法放置item at" + x + "," + y);
            return false;
            //throw;
        }
    }
    /// <summary>
    /// 0 x=横坐标，1 y=纵坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int[] GetItemLeftUp(int x, int y,int page=0)
    {/// <summary>
     /// len(0)=height,len(1)=wid,x=横坐标
     /// </summary>
        try
        {
            if (placement[y,x] != -1)
            {
                int i = 0, j = 0;
                for (i = 0; i < height; i++)//向上走到该item尽头
                {
                    if (y - i < 0) { i = Mathf.Clamp(i - 1, 0, height); break; }
                    if (placement[y,x] != placement[y - i, x]) { i = Mathf.Clamp(i - 1, 0, height); break; }
                }
                for (j = 0; j < width; j++)//向左走到该item尽头
                {
                    if (x - j < 0) { j = Mathf.Clamp(j - 1, 0, width); break; }
                    if (placement[y, x] != placement[y - i, x - j]) { j = Mathf.Clamp(j - 1, 0, width); break; }
                }
                return new int[] {  x - j,y - i };
            }
            return new int[] {  x,y };
        }
        catch (System.Exception)
        {
            Debug.Log("获取item失败at" + x + "," + y);
            throw;
        }

    }
    /// <summary>
    /// 返回0=height,1=width
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int[] GetItemLeftUp(int index)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (placement[i, j] == index) { return new int[] { i, j }; }
            }
        }
        return null;
    }
    #endregion

    #region set
    public void SetSize(int x, int y, int page=0)
    {

    }

   public void SetItems(Item[] items, int[,] placements, int page=0)
   {

   }

    
    public int SafeSetItemAt(Item item, int x, int y,int page=0)
    {
        if (Item.IsNullOrEmpty(item)) { Debug.Log("safe Set null item"); return 0; }
        int overNum = item.IsNumOverFlow(isBigChest);
        if (overNum > 0)
        {
            //Debug.Log(item);
            Item tem = new Item(item);
            tem.SafeSet(item.num,isBigChest);
            SetItemAt(tem, x, y);
            return overNum;
        }
        else
        {
            //Debug.Log(item);
            SetItemAt(item, x, y);
            return 0;
        }
    }
    public bool SetItemAt(Item item, int x, int y,int page=0)
    {//x横坐标
        if (Item.IsNullOrEmpty(item)) { Debug.Log("空item"); return false; }
        try
        {
            if (placement[y,x] == -1)//设置在空区域
            {
                int[] size = item.GetSize();
                for (int i = 0; i < size[0]; i++)//width
                {
                    for (int j = 0; j < size[1]; j++)//height
                    {
                        if (placement[y + j, x + i] != -1) return false;
                    }
                }
                int empindex = findEmptyItemAt();
                if (empindex != -1)//有现有的空item利用
                {
                    for (int i = 0; i < size[0]; i++)//width
                    {
                        for (int j = 0; j < size[1]; j++)//height
                        {
                            placement[y + j, x + i] = empindex;
                        }
                    }
                    items[empindex] = item;
                }
                else
                {
                    List<Item> temp = new List<Item>(items);
                    temp.Add(item);
                    for (int i = 0; i < size[0]; i++)//width
                    {
                        for (int j = 0; j < size[1]; j++)//height
                        {
                            placement[y + j, x + i] = temp.Count - 1;
                        }
                    }
                    items = temp.ToArray();
                }
                return true;
            }
            else//设置位置有东西，直接替换该item
            {
                items[placement[y, x]] = item;
                return true;
            }
        }
        catch (System.Exception)
        {
            Debug.Log("无法在此设置item " + x + "," + y);
            throw;
        }

    }
    public bool DeleteItemAt(int x, int y,int page=0)
    {
        if (placement[y, x] == -1) return true;
        int index = placement[y, x];
        int[] size = items[placement[y, x]].GetSize();
        for (int i = 0; i < placement.GetLength(1); i++)//width
        {
            for (int j = 0; j < placement.GetLength(0); j++)//height
            {
                if (placement[j, i] == index) placement[j, i] = -1;
            }
        }
        items[index] = Item.Empty;
        return true;
    }

    /// <summary>
    /// 返回加完剩下多少item,超num上限的item加到空格子中会成功且返回0
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int AddItemAt(Item item, int x, int y,int page=0)
    {
        if (Item.IsNullOrEmpty(item)) { Debug.Log("空item" + item.ToString()); return -1; }
        try
        {
            if (CanPlaceAt(item, x, y))
            {
                if (item.Compare(GetItemAt(x, y)))//所加格子与所加物同类
                {
                    return GetItemAt(x, y).SafeAdd(item.num,item.level,isBigChest);
                }
                else
                {
                    int[] size = item.GetSize();
                    int empindex = findEmptyItemAt();
                    if (empindex != -1)//有现有的空item利用
                    {
                        for (int i = 0; i < size[0]; i++)//width
                        {
                            for (int j = 0; j < size[1]; j++)//height
                            {
                                placement[y + j, x + i] = empindex;
                            }
                        }
                        items[empindex] = item;
                    }
                    else
                    {
                        List<Item> temp = new List<Item>(items);
                        temp.Add(item);

                        for (int i = 0; i < size[0]; i++)//width
                        {
                            for (int j = 0; j < size[1]; j++)//height
                            {
                                placement[y + j, x + i] = temp.Count - 1;
                            }
                        }
                        items = temp.ToArray();
                    }

                    return 0;
                }

            }
            else return -1;
        }
        catch (System.Exception)
        {
            Debug.Log("无法在此additem " + x + "," + y);
            throw;
        }
    }
    /// <summary>
    /// 返回加完剩下多少，当格子空时返回-1
    /// </summary>
    /// <param name="i"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int AddItemNumAt(int i, int x, int y,int page=0)
    {
        try
        {
            if (!Item.IsNullOrEmpty(GetItemAt(x, y)))
            {
                return GetItemAt(x, y).SafeAdd(i,-1,isBigChest);
            }
            else return -1;
        }
        catch (System.Exception)
        {
            Debug.Log("无法在此additem " + x + "," + y);
            throw;
        }
    }

    public int AddItem(Item i, int page = 0)
    {
        return 0;
    }
    /// <summary>
    /// 会改变传入items[]对象，未测试
    /// </summary>
    /// <param name="its"></param>
    /// <returns></returns>
    public int[] AddItems(Item[] its,int page=0)
    {
        /*
         * 遍历每个item,
         * 每个item对每个格子执行一次additemat,如果该item剩余num<=0则不继续执行additemat
         * 如果所有additemat结束后仍然有剩余num，则填入num到返回的int[]中对应位置
         * */
        int[] remains = new int[its.Length];
        for (int i = 0; i < its.Length; i++)
        {
            int remain = its[i].num;
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    remain = AddItemAt(its[i],j, k);
                    if (remain == 0)
                    { break; }
                    else
                    {
                        its[i].num = remain;
                    }
                }
                if (remain == 0)
                { break; }
            }
            remains[i] = remain;
        }
        return remains;
    }
    public int SubItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return 0;
    }
    /// <summary>
    /// 返回还差几个才够减
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public int SubItemAt(Item item, int x, int y, int page=0,ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (Item.IsNullOrEmpty(item)) { Debug.Log("空item"); return -1; }
        Item slot = GetItemAt(x, y);
        if (slot.Compare(item, mode))
        {
            return SubItemNumAt(item.num, x, y,0, mode);
        }
        return -1;
    }
    public int SubItemNumAt(int item, int x, int y,int page=0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        try
        {
            Item slot = GetItemAt(x, y);
            if (!Item.IsNullOrEmpty(slot))
            {
                if (item >= slot.num) { int ans = item - slot.num; slot.num = 0; DeleteItemAt(x, y); return ans; }
                else { slot.num -= item; return 0; }

            }
            else
            {
                return 0;
            }
        }
        catch (System.Exception)
        {
            Debug.Log("无法在此subitem " + x + "," + y);
            throw;
        }
    }

    public int SetItemNumAt(int num, int x, int y,int page=0)
    {
        try
        {
            if (placement[y, x] != -1)
            {
                items[placement[y, x]].SafeSet(num,isBigChest);
                return 0;
            }
            else
            {
                return -1;
            }
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
        items = null;
    }
    /// <summary>
    /// 不可用
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        //List<string> datas = new List<string>();
        return JsonConvert.SerializeObject(this);
    }
    public static ItemPage_Data FromString(string data)
    {
        ItemPage_Data i=JsonConvert.DeserializeObject<ItemPage_Data>(data);
        i.init();
        return i;
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
}
