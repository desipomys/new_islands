using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete]
public class Test_Backpack : BaseBackPack
{
    public ItemPageBig_Data bigItems;
    public override void OnEventRegist(EventCenter e)
    {
        //base.OnEventRegist(e);
        center = e;
        center.RegistFunc<IBackPack>("backpack", () => { return this; });
        //center.ListenEvent<BaseUIController, BaseUIView>("buildMVConnect", BuildMVConnect);
        //center.ListenEvent<BaseUIController, BaseUIView>("breakMVConnect", BreakMVConnect);
    }
    public void Start()
    {
        //GetComponent<EventCenter>().Init();        
    }
    

    public override void SetSize(int h, int w, int page = 0)
    {
        /*List<ItemPage_Data> temp = new List<ItemPage_Data>();
        for (int i = 0; i < page-1; i++)
        {
            temp.Add(new ItemPage_Data(h, w));
        }
        itemPages = temp.ToArray();
        bigItems = new ItemPageBig_Data(h, w);*/
    }
    public void SetItems(Item[,] items, int page = 0)
    {
        bigItems.SetItems(items);
    }

    #region set
    public override bool SetItemAt(Item i, int x, int y, int page = 0)
    {
        if (page == 0)
            return base.SetItemAt(i, x, y, page);
        else
        {

            bool p = bigItems.SetItemAt(i, x, y);
            SendItemEvent(bigItems.GetItemAt(x, y), x, y, x, page, ObjInPageChangeMode.set);
            return p;
        }
    }
    
    public override int AddItemAt(Item i, int x, int y, int page = 0)
    {
        if(page==0)
        return base.AddItemAt(i, x, y, page);
        else
        {
           
            int p=bigItems.AddItemAt(i, x, y);
            SendItemEvent(bigItems.GetItemAt(x, y), x, y,x, page, ObjInPageChangeMode.add);
            return p;
        }
    }
    public override int AddItemNumAt(int i, int x, int y, int page = 0)
    {
        if(page==0)
            return base.AddItemNumAt(i, x, y, page);
        else
        {
            
            int p = bigItems.AddItemNumAt(i, x, y);
            SendItemEvent(bigItems.GetItemAt(x, y), x, y,x, page, ObjInPageChangeMode.add);
            return p;
        }
    }
    public override bool DeleteItemAt(int x, int y, int page = 0)
    {
        if (page == 0)
            return base.DeleteItemAt( x, y, page);
        else
        {
           
            bool p = bigItems.DeleteItemAt(x, y);
            SendItemEvent(bigItems.GetItemAt(x, y), x, y,x, page, ObjInPageChangeMode.delete);
            return p;
        }
    }

    public override int AddItem(Item item, int page = 0)
    {
        if (page == 0)
            return base.AddItem(item, page);
        else
        {
            return 0;
        }
    }
   
    public override int SubItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return 0;
    }
    public override int SubItemAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (page == 0)
            return base.SubItemAt(item,x, y, page);
        else
        {
            
            int p = bigItems.SubItemAt(item,x, y,page,mode);
            SendItemEvent(bigItems.GetItemAt(x, y), x, y,x, page, ObjInPageChangeMode.sub);
            return p;
        }
    }
    public override int SetItemNumAt(int num, int x, int y, int page = 0)
    {
        if (page == 0)
            return base.SetItemNumAt(num,x, y, page);
        else
        {
            
            int p = bigItems.SetItemNumAt(num,x, y);
            SendItemEvent(bigItems.GetItemAt(x, y), x, y,x, page, ObjInPageChangeMode.set);
            return p;
        }
    }

    #endregion

    #region get
    public override int Contain(Item item, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (page == 0)
            return base.Contain(item, page,mode);
        else
        {
            return bigItems.Contain(item,page,mode);
        }
    }
    public override int ContainAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (page == 0)
            return base.ContainAt(item, x, y,page,mode);
        else
        {
            return bigItems.ContainAt(item, x, y,page,mode);
        }
    }
    public override Item GetItemAt(int x, int y, int page = 0)
    {
        if (page == 0)
            return base.GetItemAt( x, y, page);
        else
        {
            return bigItems.GetItemAt( x, y);
        }
    }
    public override int CountItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (page == 0)
            return base.CountItem(i, page, mode);
        else
        {
            return bigItems.CountItem(i,page,mode);
        }
    }
    public override bool CanPlaceAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (page == 0)
            return base.CanPlaceAt(item,x, y, page,mode);
        else
        {
            return bigItems.CanPlaceAt(item,x, y,page,mode);
        }
      
    }
    public override bool CanPlaceIgnoreCurrent(Item it, int x, int y, int page = 0)
    {
        if (page == 0)
            return base.CanPlaceIgnoreCurrent(it,x, y, page);
        else
        {
            return bigItems.CanPlaceIgnoreCurrent(it,x, y);
        }
    }
    public override int[] GetItemLeftUp(int x, int y, int page = 0)
    {
        if (page == 0)
            return base.GetItemLeftUp( x, y, page);
        else
        {
            return bigItems.GetItemLeftUp( x, y);
        }
    }
    public override Item[,] GetBigItems(int page = 0)
    {
        if (page == 0) return base.GetBigItems(page);
        else return bigItems.items;
    }
    #endregion

}
