using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PlayerBackpack : BaseBackPack
{
    public int Wid, Hig;
    //public Item[] HandItems, Equipments, Parts, Skins;
    private ItemPageBig_Data HandItems{get{return (ItemPageBig_Data)itemPages[-1];} set{}}

    public override void OnEventRegist(EventCenter e)
    {
        InitPageSize();
        center = e;
        center.RegistFunc<IBackPack>(nameof(PlayerEventName.backpack), () => { return this; });
        center.RegistFunc<int>("get_bpSize", () => { return itemPages.Count; });
        center.RegistFunc<int, int[,]>("Getbp_Placement", (int p) => { return itemPages[p].GetPlacements(); });
        center.RegistFunc<int, Item[]>(nameof(PlayerEventName.Getbp_Items), GetItems);
        center.RegistFunc<Item, int>(nameof(PlayerEventName.giveItem), (Item i) => { return AddItem(i); });
        center.ListenEvent<ItemPageChangeParm>(nameof(PlayerEventName.setItem), (ItemPageChangeParm p) => { SetItemAt(p.item, p.x, p.y, p.page); });
        center.RegistFunc<FindItemParm, int>(nameof(PlayerEventName.containItem), (FindItemParm i) => { return Contain(i.item, i.page, i.mode); });
        /*center.RegistFunc<ItemPageChangeParm,int>(nameof(PlayerEventName.giveItem),(ItemPageChangeParm p)=> { return AddItem(p.item, p.page); } );*/
        center.RegistFunc<FindItemParm, int>(nameof(PlayerEventName.takeItem), (FindItemParm p) => { return SubItem(p.item, p.page, p.mode); });
        //center.ListenEvent<BaseUIController, BaseUIView>(nameof(PlayerEventName.buildMVConnect), BuildMVConnect);
        //center.ListenEvent<BaseUIController, BaseUIView>(nameof(PlayerEventName.breakMVConnect), BreakMVConnect);
        center.ListenEvent<ItemPageChangeParm>(nameof(PlayerEventName.dropBPItemAt), DropItem);
    }
    void InitPageSize()
    {
        itemPages = new Dictionary<int, IBackPack>();
        itemPages.Add(0,new ItemPage_Data(Hig,Wid));
        itemPages.Add(-1,new ItemPageBig_Data(1,3));//hand
        itemPages.Add(-2,new ItemPageBig_Data(1,3));//equip
        itemPages.Add(-3,new ItemPageBig_Data(1,3));//part
        itemPages.Add(-4,new ItemPageBig_Data(1,3));//skin
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            string placementInfo = "";
            ItemPage_Data temp=(ItemPage_Data)itemPages[0];
            for (int i = 0; i < temp.height; i++)
            {
                for (int j = 0; j < temp.width; j++)
                {
                    placementInfo += temp.placement[i, j].ToString() + " ";
                }
                placementInfo += "\n";
            }
            Debug.Log(placementInfo);
        }
    }

    public override Item[] GetItems(int page = 0)
    {
        return itemPages[page].GetItems(page);
    }


    #region get
    public override int Contain(Item item, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (Item.IsNullOrEmpty(item)) return 0;
        int count = 0;
        
        if (page == int.MaxValue)
        {
            count = HandItems.Contain(item, page,mode);
            return base.Contain(item, page, mode) + count;
        }
        else if (page == -1)
        {
            count = HandItems.Contain(item,page, mode);
            return count;
        }
        else
        {
            return base.Contain(item, page, mode);
        }

    }
    public override int ContainAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (page == -1)
        {

            return HandItems.ContainAt(item, x, y,page, mode);

        }
        else return base.ContainAt(item, x, y, page, mode);
    }
    public override Item GetItemAt(int x, int y, int page = 0)
    {
        return base.GetItemAt(x, y, page);
    }
    public override int CountItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {

        return Contain(i, page, mode);
    }
    public override bool CanPlaceAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (Item.IsNullOrEmpty(item)) return true;
        return base.CanPlaceAt(item, x, y, page, mode);
    }
    public override bool CanPlaceIgnoreCurrent(Item it, int x, int y, int page = 0)
    {
        if (page == -1) return true;
        return base.CanPlaceIgnoreCurrent(it, x, y, page);
    }

    public override int[] GetItemLeftUp(int x, int y, int page = 0)
    {
        Debug.Log(x + ":" + y + ":" + page);
        if (page == -1) return new int[] { x ,y};
        else return itemPages[page].GetItemLeftUp(x, y);
    }
    #endregion

    #region set
    public override bool SetItemAt(Item i, int x, int y, int page = 0)
    {
        if (Item.IsNullOrEmpty(i)) { return false; }
       
         return base.SetItemAt(i, x, y, page);
    }
    public override bool DeleteItemAt(int x, int y, int page = 0)
    {
        Debug.Log(x + ":" + y + ":" + page);
        
            return base.DeleteItemAt(x, y, page);
        
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="page">=int.max时代表加到所有page里</param>
    /// <returns></returns>
    public override int AddItem(Item item, int page = int.MaxValue)
    {
        //Debug.Log("加");
        if (Item.IsNullOrEmpty(item)) return 0;
        Debug.Log("加非空" + item.ToString());
        int res = item.num;
       

        if(page==int.MaxValue)
        {
            foreach (var pack in itemPages)
            {
                int[,] size=pack.Value.GetPlacements();
                for (int i = 0; i < size.GetLength(0); i++)//hig,y
                {
                    for (int j = 0; j < size.GetLength(1); j++)//wid,x
                    {
                        if(pack.Value.CanPlaceAt(item,j,i,page,ItemCompareMode.excludeNum))
                        { 
                            Item newitem=new Item(item);
                            newitem.num=res;
                            res=pack.Value.AddItemAt(newitem,j,i,page);
                        }
                        if(res==0)break;
                    }
                    if(res==0)break;
                }
                if(res==0)break;
            }
        }
        else{
            IBackPack pack=itemPages[page];
            int[,] size=pack.GetPlacements();
            for (int i = 0; i < size.GetLength(0); i++)//hig,y
            {
                for (int j = 0; j < size.GetLength(1); j++)//wid,x
                {
                    if(pack.CanPlaceAt(item,j,i,page,ItemCompareMode.excludeNum))
                    { 
                        Item newitem=new Item(item);
                        newitem.num=res;
                        res=pack.AddItemAt(newitem,j,i,page);
                    }
                    if(res==0)break;
                }
                if(res==0)break;
            }
        }


        /*
        if (page == -1 || page == int.MaxValue)
        {

            for (int i = 0; i < HandItems.Length; i++)
            {
                if (item.Compare(HandItems[i]))
                {
                    res = HandItems[i].SafeAdd(res, item.level);
                    SendItemEvent(HandItems[i], i, 0, i, -1, ObjInPageChangeMode.add);
                }
            }

        }
        if (page != -1 || page == int.MaxValue)
        {
            for (int p = 0; p < itemPages.Count; p++)
            {
                if (page != int.MaxValue && page != p) continue;

                for (int i = 0; i < itemPages[p].width; i++)//找相同item加,len(0)=height,len(1)=wid
                {
                    for (int j = 0; j < itemPages[p].height; j++)
                    {
                        if (itemPages[p].ContainAt(item, i, j) != -1)
                        {
                            //Debug.Log("same Item at"+i+":"+j+" res="+res);
                            int index = itemPages[p].GetIndex(j, i);
                            res = itemPages[p].GetItemAt(i, j).SafeAdd(res, item.level);
                            SendItemEvent(itemPages[p].GetItemAt(i, j), i, j, index, p, ObjInPageChangeMode.add);
                            //Debug.Log("res=" + res);
                            if (res <= 0) break;
                        }
                    }
                    if (res <= 0) break;
                }
                if (res > 0)//还有没加完的找空位加
                {
                    for (int i = 0; i < itemPages[p].width; i++)
                    {
                        for (int j = 0; j < itemPages[p].height; j++)
                        {
                            if (itemPages[p].CanPlaceAtEmpty(item, i, j))
                            {
                                item.num = res;
                                //Debug.Log("empty Item at" + i + ":" + j + " res=" + res);
                                int index = itemPages[p].GetIndex(i, j);
                                res = itemPages[p].SafeSetItemAt(new Item(item), i, j);
                                SendItemEvent(itemPages[p].GetItemAt(i, j), i, j, index, p, ObjInPageChangeMode.set);
                                //Debug.Log("res=" + res);
                                if (res <= 0) break;
                            }
                        }
                        if (res <= 0) break;
                    }
                }
                if (res > 0)//未整理前还有没加完的整理后再找空位加
                {

                }
                if (res <= 0) break;
            }
        }
        */
        return res;
    }
    public override int AddItemAt(Item i, int x, int y, int page = 0)
    {

        return base.AddItemAt(i, x, y, page);
    }
    public override int AddItemNumAt(int i, int x, int y, int page = 0)
    {
       return base.AddItemNumAt(i, x, y, page);
    }
    public override int SubItem(Item item, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        //Debug.Log("待减item="+item);
        if (Item.IsNullOrEmpty(item)) return 0;
        if (EventCenter.WorldCenter.GetParm<EventCenter>(nameof(EventNames.GetLocalPlayer)).UUID == center.UUID &&
            EventCenter.WorldCenter.GetParm<bool>(nameof(EventNames.IsCheatMode))) return 0;
        //未做完
        int res = item.num;
        

        
        /*if (page == int.MaxValue)
        {
            for (int i = 0; i < HandItems.Length; i++)
            {
                if (item.Compare(HandItems[i], mode))
                {
                    res = HandItems[i].SafeSub(res);
                    SendItemEvent(HandItems[i], i, 0, i, -1, ObjInPageChangeMode.sub);
                }
            }
            for (int k = 0; k < itemPages.Count; k++)
            {
                for (int i = 0; i < itemPages[k].width; i++)//找相同item加,len(0)=height,len(1)=wid
                {
                    for (int j = 0; j < itemPages[k].height; j++)
                    {
                        if (itemPages[k].ContainAt(item, i, j) != -1)
                        {
                            int index = itemPages[k].GetIndex(i, j);
                            res = itemPages[k].SubItemNumAt(res, i, j,page, mode);
                            SendItemEvent(itemPages[k].GetItemAt(i, j), i, j, index, k, ObjInPageChangeMode.sub);
                            if (res == 0) break;
                        }
                    }
                    if (res == 0) break;
                }
            }

        }
        else if (page == -1)
        {
            //int res = item.num;
            for (int i = 0; i < HandItems.Length; i++)
            {
                if (item.Compare(HandItems[i], mode))
                {
                    res = HandItems[i].SafeSub(res);
                    SendItemEvent(HandItems[i], i, 0, i, -1, ObjInPageChangeMode.sub);
                }
            }
            //return 0;
        }
        else if (page >= 0)
        {
            //int res = item.num;
            for (int i = 0; i < itemPages[page].width; i++)//找相同item加,len(0)=height,len(1)=wid
            {
                for (int j = 0; j < itemPages[page].height; j++)
                {
                    if (itemPages[page].ContainAt(item, i, j,page, mode) != -1)
                    {
                        int index = itemPages[page].GetIndex(i, j);
                        res = itemPages[page].SubItemNumAt(res, i, j,page, mode);
                        SendItemEvent(itemPages[page].GetItemAt(i, j), i, j, index, page, ObjInPageChangeMode.sub);
                        if (res == 0) break;
                    }
                }
                if (res == 0) break;
            }

        }*/
        return res;
    }
    public override int SubItemAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return base.SubItemAt(item, x, y, page, mode);
    }

    public override int SetItemNumAt(int num, int x, int y, int page = 0)
    {
        return SetItemNumAt(num, x, y, page);
    }

    #endregion

    public void DropItem(ItemPageChangeParm p)
    {
        if (p.page != -1 && (p.page >= this.itemPages.Count || p.page < 0)) return;
        
        IBackPack ib=itemPages[p.page];
        Item temp=ib.GetItemAt(p.x,p.y);
        ib.SetItemAt(Item.Empty,p.x,p.y);
        center.SendEvent<Item>(nameof(PlayerEventName.dropItem), temp);
        SendItemEvent(Item.Empty, p.x, p.y, ib.GetPlacements()[p.x,p.y], p.page, ObjInPageChangeMode.delete);
        /*if (p.page == -1)
        {


            center.SendEvent<Item>(nameof(PlayerEventName.dropItem), HandItems.GetItemAt(p.x,p.y));
            HandItems.SetItemAt(Item.Empty,p.x,p.y) ;
            SendItemEvent(Item.Empty, p.x, 0, p.x, -1, ObjInPageChangeMode.delete);
        }
        else
        {
            if (!itemPages[p.page].IsOutOfRange(p.x, p.y))
            {

                center.SendEvent<Item>(nameof(PlayerEventName.dropItem), itemPages[p.page].GetItemAt(p.x, p.y));
                int index = itemPages[p.page].GetIndex(p.x, p.y);
                itemPages[p.page].SetItemAt(Item.Empty, p.x, p.y);
                SendItemEvent(Item.Empty, p.x, p.y, index, p.page, ObjInPageChangeMode.delete);
            }
        }*/
    }

    public override void FromObject(object data)
    {
        JArray temp = ((JArray)(data)).ToObject<JArray>();
        Wid = int.Parse( temp[0].ToObject<string>());
        Hig = int.Parse(temp[1].ToObject<string>());
        itemPages = JsonConvert.DeserializeObject<Dictionary<int, IBackPack>>(temp[2].ToObject<string>(), JsonSetting.serializerSettings);
        /*for (int i = 0; i < temp.Length - 1; i++)
        {
            itemPages[i] = JsonConvert.DeserializeObject<ItemPage_Data>(temp[i + 1]);
        }*/
        int[,] sizes = itemPages[-1].GetPlacements();
        for (int i = 0; i < sizes.GetLength(0); i++)
        {
            for (int j = 0; j < sizes.GetLength(1); j++)//wid
            {
                SendItemEvent(itemPages[-1].GetItemAt(j, i), j, i, j, -1, ObjInPageChangeMode.set);
            }
        }
        

    }
    public override object ToObject()
    {
        //string[] temp ;
        List<string> st = new List<string>();
        st.Add(Wid.ToString());
        st.Add(Hig.ToString());
        st.Add(JsonConvert.SerializeObject(itemPages, JsonSetting.serializerSettings));
        return JArray.FromObject(st);
    }

}
