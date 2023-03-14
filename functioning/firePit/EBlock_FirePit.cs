using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class EBlock_FirePit : BaseBackPack
{
    //page 0mat,1fuel,2product
    float Process,MaxProcess;
    float MaxFuel, Fuel;
    bool Burning;

    public float process { get { return Process; }set { Process = value; center.SendEvent<float, float>(nameof(FirePitEventname.firePitProcessGo),Process, MaxProcess); } }
    public float fuel { get { return Fuel; } set { Fuel = value; center.SendEvent<float, float>(nameof(FirePitEventname.firePitfuelGo), Fuel, MaxFuel); } }
    public float maxProcess { get { return MaxProcess; } set { MaxProcess = value; center.SendEvent<float, float>(nameof(FirePitEventname.firePitProcessGo), Process, MaxProcess); } }
    public float maxFuel { get { return MaxFuel; } set { MaxFuel = value; center.SendEvent<float, float>(nameof(FirePitEventname.firePitfuelGo), Fuel, MaxFuel); } }
    bool burning { get { return Burning; } set { Burning = value; center.SendEvent<bool>(nameof(FirePitEventname.BuringChg), Burning); } }

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        center.ListenEvent<EventCenter, InteractType>("interact", OnInteract);
        center.RegistFunc<float[]>(nameof(FirePitEventname.GetfirePitProcess), () => { return new float[] { process, maxProcess }; });
        center.RegistFunc<float[]>(nameof(FirePitEventname.GetfirePitFuel), () => { return new float[] { fuel, MaxFuel }; });
        center.RegistFunc<bool>(nameof(FirePitEventname.GetBuring), () => { return Burning; });
        foreach (var item in itemPages)
        {
            item.Value.init();
        }
    }
    
    bool isProductAvalibe(Item i)
    {
        if (Item.IsNullOrEmpty(itemPages[0].GetItemAt(2, 0))) return true;
        if (itemPages[0].GetItemAt(2, 0).Compare(i) && itemPages[0].GetItemAt(2, 0).TrySafeAdd(i.num) <= 0) return true;
        else return false;
    }
    void StartNextProcess()
    {
        
        FirePit_Data fp = EventCenter.WorldCenter.GetParm<Item, FirePit_Data>(nameof(EventNames.GetFirePitDataByMat), itemPages[0].GetItemAt(0, 0));
        if (fp != null&&isProductAvalibe(fp.product))//如果mat里的可以烧且产出格可用
        {
            Debug.Log("可以烧"+fp.mat.id);
            process = 0;
            if (fuel <= 0) UseFuelItem();//如果当前没燃料
           
            if(fuel > 0)
            {
                burning = true;
                maxProcess = fp.time;
            }
        }
        else { Process = 0;maxProcess = 0;burning = false; }
    }
    void UseFuelItem()
    {
        if (fuel > 0) return;
        float fuelv = EventCenter.WorldCenter.GetParm<int, float>(nameof(EventNames.GetFuelByID), itemPages[0].GetItemAt(1, 0).id);
        if (fuelv > 0)
        {
            int res = itemPages[0].GetItemAt(1,0).num;
            if (res > 0)
            {
                
                MaxFuel = fuelv;
                fuel = maxFuel;
                SubItemAt(1, 1, 0);
            }
            else
            {
                burning = false;
                MaxFuel = 0;
                fuel = maxFuel;
            }
        }
        else
        {
            burning = false;
            MaxFuel = 0;
            fuel = maxFuel;
        }
    }

    public void FixedUpdate()
    {
        if (!burning) return;
        if (process >= maxProcess)
        {
            //完成一次生产
            FirePit_Data fp= EventCenter.WorldCenter.GetParm<Item, FirePit_Data>(nameof(EventNames.GetFirePitDataByMat), itemPages[0].GetItemAt(0, 0));
            if(fp!=null)
            {
                if (TrySubItemAt(fp.mat, 0, 0) >= 0)
                {
                    base.AddItemAt(new Item(fp.product), 2, 0);
                    Debug.Log("产出");
                }
            }
            StartNextProcess();
        }
        if (fuel <= 0)
        {
            //扣除一次燃料
            UseFuelItem();
            Debug.Log("消耗燃料");
            if (fuel <= 0) burning = false;
        }
        if (fuel > 0)
        {
            process += Time.fixedDeltaTime;
        }
        fuel -= Time.fixedDeltaTime;
        
       
    }
    public override void SendItemEvent(Item i, int x, int y, int index, int page, ObjInPageChangeMode mode)
    {
        base.SendItemEvent(i, x, y, index, page, mode);
        if(x==2)
        {
            if (!burning) StartNextProcess();
        }
        else if(x==1)
        {
            UseFuelItem();
            if (fuel>0&& process > 0 && burning == false) { burning = true; }
        }
        else if(x==0)
        {
            StartNextProcess();
        }
        
    }

    public void OnInteract(EventCenter source, InteractType typ)
    {
        //区分AI还是玩家
        bool safe = false;

        if (source.GetParm<bool>("isAI", out safe))
        {
            if (safe)
            {
                //是AI
                return;
            }
        }

        //是玩家打开UI
        UICenter.UIWorldCenter.ShowView("firepit", "backpack");
        BaseUIView chest = UICenter.UIWorldCenter.GetView("firepit");
        chest.BuildMVConnect(chest.UIName, center, source);
        //Debug.Log("firepit BuildMVConnect");
        //player = source;
    }
    public override object ToObject()
    {
        JArray st = new JArray();
        string cSTR = JsonConvert.SerializeObject(Process);
        st.Add(cSTR);
        cSTR = JsonConvert.SerializeObject(Fuel);
        st.Add(cSTR);
        cSTR = JsonConvert.SerializeObject(MaxProcess);
        st.Add(cSTR);
        cSTR = JsonConvert.SerializeObject(MaxFuel);
        st.Add(cSTR);


        if (itemPages != null)
        {
            st.Add(JsonConvert.SerializeObject(itemPages,JsonSetting.serializerSettings));
            
            //st.Add(itempageArr);
        }
        return st;
    }
    public override void FromObject(object data)
    {
        JArray temp = ((JArray)(data)).ToObject<JArray>();
        //Crafting = temp[0].ToObject<Craft_Data>();
    //    public float process;
    //public float fuel;
    //public float maxProcess;
    //public float maxFuel;

    string tempstr = temp[0].ToString();
     float.TryParse(tempstr,out Process);

        tempstr = temp[1].ToString();
        float.TryParse(tempstr, out Fuel);

        tempstr = temp[2].ToString();
        float.TryParse(tempstr, out MaxProcess);

        tempstr = temp[3].ToString();
        float.TryParse(tempstr, out MaxFuel);

        if (fuel > 0 && process > 0 && process < maxProcess && maxProcess > 0)
        { burning = true;Debug.Log(burning); }

        //remainCraftNum = temp[3].ToObject<int>();
        //currentCrafting = temp[4].ToObject<int>();
        //processTime = temp[5].ToObject<float>();

        if (temp.Count > 4)
        {
            itemPages = JsonConvert.DeserializeObject<Dictionary<int,IBackPack>>(temp[4].ToString(),JsonSetting.serializerSettings);
            foreach (var item in itemPages)
            {
                ((ItemPageBig_Data)item.Value).init();
            }

        }
    }

    #region set
    public override int AddItem(Item i, int page = 0)
    {
        //如果i是原料则放入原料格，是燃料则放入燃料格
        if (itemPages == null) { Debug.Log("物品栏为空"); return 0; }
        if (EventCenter.WorldCenter.GetParm<int, float>(nameof(EventNames.GetFuelByID), i.id) > 0)
        {
            if (Item.IsNullOrEmpty(itemPages[0].GetItemAt(1, 0)) || itemPages[0].GetItemAt(1, 0).Compare(i))
            {
                int temp = itemPages[0].AddItemAt(i, 1, 0);
                SendItemEvent(itemPages[0].GetItemAt(1, 0), 1, 0, 1, page, ObjInPageChangeMode.add);
                return temp;
            }
            else {
                int temp = itemPages[0].AddItemAt(i, 0, 0);
                SendItemEvent(itemPages[0].GetItemAt(0, 0), 0, 0, 0, page, ObjInPageChangeMode.add);
                return temp;
            }
        }
        else
        {
            int temp = itemPages[0].AddItemAt(i, 0, 0);
            SendItemEvent(itemPages[0].GetItemAt(0, 0), 0, 0, 0, page, ObjInPageChangeMode.add);
            return temp;
        }
    }

    public override int AddItemAt(Item i, int x, int y, int page = 0)
    {
        if (x == 0 || x == 1)
        {//如果i是原料却放入fuel格则返回-1，是燃料却放入原料格则返回-1
            if (itemPages == null) { Debug.Log("物品栏为空"); return -1; }
            int temp= itemPages[0].AddItemAt(i, x, 0);
            SendItemEvent(itemPages[0].GetItemAt(x, 0), x, 0, x, page, ObjInPageChangeMode.add);
            return temp;
        }
        return -1;
    }

    public override int AddItemNumAt(int i, int x, int y, int page = 0)
    {
        if (x == 0 || x == 1)
        {//如果i是原料却放入fuel格则返回-1，是燃料却放入原料格则返回-1
            if (itemPages == null) { Debug.Log("物品栏为空"); return -1; }
            int temp= itemPages[0].AddItemNumAt(i, x, 0);
            SendItemEvent(itemPages[0].GetItemAt(x, 0), x, 0, x, page, ObjInPageChangeMode.add);
            return temp;
        }
        return -1;
    }

    public override bool CanPlaceAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return false; }
        if (x == 0 )
        {// 如果i是原料却放入fuel格则返回 false，是燃料却放入原料格则返回 false
            
            
            return itemPages[0].CanPlaceAt(item, 0, 0);
        }
        else if(x==1)
        {
            if(EventCenter.WorldCenter.GetParm<int, float>(nameof(EventNames.GetFuelByID), item.id) > 0)
            return itemPages[0].CanPlaceAt(item, 1, 0);
        }
        return false;
    }

    public override bool CanPlaceIgnoreCurrent(Item it, int x, int y, int area = 0)
    {
        if (x == 0) return false;
        if (itemPages == null) { Debug.Log("物品栏为空"); return false; }
        return itemPages[0].CanPlaceIgnoreCurrent(it, x, 0);
    }

    public override int Contain(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return 0; }
        return itemPages[page].Contain(i,page, mode);
    }

    public override int ContainAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return 0; }
        return itemPages[page].ContainAt(item, x, y,page, mode);
    }

    public override int CountItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return 0; }
        return itemPages[page].CountItem(i, page, mode);
    }

    public override bool DeleteItemAt(int x, int y, int page = 0)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return false; }
        bool temp= itemPages[page].DeleteItemAt(x, y);
        SendItemEvent(itemPages[0].GetItemAt(x, y), x,y, x, page, ObjInPageChangeMode.delete);
        return temp;
    }

    public override bool SetItemAt(Item i, int x, int y, int page = 0)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return false; }
        if (x != 0 && x != 1 && x != 2) { return false; }
        if (EventCenter.WorldCenter.GetParm<int, float>(nameof(EventNames.GetFuelByID), i.id) > 0)//先尝试放燃料，不行再放原料
        {
            if (x != 0 && x != 1) return false;
            bool temp= itemPages[0].SetItemAt(i, x, 0);
            SendItemEvent(itemPages[0].GetItemAt(x, 0), x, 0, x, page, ObjInPageChangeMode.set);
            return temp;
        }
        else//只能放到原料格
        {
            if (x != 0) return false;
            bool tem= itemPages[0].SetItemAt(i, 0, 0);
            SendItemEvent(itemPages[0].GetItemAt(0, 0), 0, 0, 0, page, ObjInPageChangeMode.set);
            return tem;
        }
    }

    public override int SetItemNumAt(int num, int x, int y, int page = 0)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return -1; }
        if (x != 0 && x != 1 && x != 2) { return -1; }

        int tem= itemPages[0].SetItemNumAt(num, x, 0);
        SendItemEvent(itemPages[0].GetItemAt(x, 0), x, 0, x, page, ObjInPageChangeMode.set);
        return tem;
    }

    public override void SetItems(Item[] items, int[,] placements, int page = 0)
    {
        if (items == null || items.Length != 3) return;
        itemPages[0].SetItems( items, new int[3,1]);
        center.SendEvent<int>("bpFlush", 0);
    }

    public override void SetSize(int x, int y, int page = 0)
    {
        
    }

    public override int SubItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return -1; }
        int temp= itemPages[0].SubItemAt(i, 2, 0,page, mode);
        SendItemEvent(itemPages[0].GetItemAt(2, 0), 2, 0, 2, page, ObjInPageChangeMode.sub);
        return temp;
    }

    public override int SubItemAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return -1; }
        int temp= itemPages[0].SubItemAt(item, x, y, page, mode);
        SendItemEvent(itemPages[0].GetItemAt(x, 0), x, 0, x, page, ObjInPageChangeMode.sub);
        return temp;
    }
    public int SubItemAt(int num, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return -1; }
        Item tempi = new Item(itemPages[0].GetItemAt(x, 0));
        tempi.num = num;
        int temp = itemPages[0].SubItemAt(tempi, x, y, page, mode);
        SendItemEvent(itemPages[0].GetItemAt(x, 0), x, 0, x, page, ObjInPageChangeMode.sub);
        return temp;
    }
    /// <summary>
    /// 如果不够减，则不做任何处理
    /// </summary>
    /// <param name="num"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="page"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public int TrySubItemAt(Item i, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return -1; }
        int temp = ((ItemPageBig_Data)itemPages[0]).TrySubItemAt(i, x, y, mode);
        if(temp>=0)SendItemEvent(itemPages[0].GetItemAt(x, 0), x, 0, x, page, ObjInPageChangeMode.sub);
        return temp;
    }
    public int TrySubItemAt(int i, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (itemPages == null) { Debug.Log("物品栏为空"); return -1; }
        Item it = new Item(itemPages[0].GetItemAt(x, 0));
        it.num = 1;
        int temp = ((ItemPageBig_Data)itemPages[0]).TrySubItemAt(it, x, y, mode);
        if (temp >= 0) SendItemEvent(itemPages[0].GetItemAt(x, 0), x, 0, x, page, ObjInPageChangeMode.sub);
        return temp;
    }
    #endregion
}