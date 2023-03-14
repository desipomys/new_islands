using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class EBlock_Craft : BaseBackPack
{
    //itemPages[0]已放入的,itemPages[1]需求
    public Craft_Data Crafting;//需保存
    public Item[] Crafting_mat;//需保存
    public Item dones;//产物//需保存
    public int craftNum;//合成几个
    int remainCraftNum;//还有几个没合成完
    int currentCrafting = 0;//需保存
    float processTime = 0;//需保存
    EventCenter player;

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        center.ListenEvent<EventCenter, InteractType>("interact", OnInteract);
        center.ListenEvent<int>(nameof(CraftViewEventName.uuidClick), OnUUIDClick);
        center.ListenEvent(nameof(CraftViewEventName.clearUUID), ClearUUID);
        center.ListenEvent(nameof(CraftViewEventName.startCraft), startCrafting);
        center.ListenEvent<int>(nameof(CraftViewEventName.UISliderMove), UISliderMove);
        center.RegistFunc<Item>(nameof(CraftViewEventName.GetDone), () => { return dones; });
        center.ListenEvent<Item>(nameof(CraftViewEventName.SetDone), (Item i) => {
            dones = i; center.SendEvent<Item>(nameof(CraftViewEventName.craftDone), dones); });
        center.ListenEvent(nameof(CraftViewEventName.cancleCraft),CancleCraft);
        center.RegistFunc<Item[]>(nameof(CraftViewEventName.GetCraftingMat), () => { return Crafting_mat; });
        center.ListenEvent(nameof(CraftViewEventName.SetCraftingMat), (Item[] t)=>{ Crafting_mat = t; });
        center.RegistFunc<bool>(nameof(CraftViewEventName.isCrafting), () => { if (Crafting_mat == null || Crafting_mat.Length == 0) return false; else return true; });
        center.ListenEvent("refresh", Refresh);
        center.RegistFunc<int>(nameof(CraftViewEventName.GetCurrentCrafting),()=> { return currentCrafting; });
        foreach (var item in itemPages)
        {
            ((ItemPageBig_Data)item.Value).init();
        }
    }
    public void ClearUUID()
    {
        currentCrafting = -1;
        //Crafting = null;
        center.SendEvent<Item[]>(nameof(CraftViewEventName.CraftingUpdate), null);//触发craft配方改变事件,合成需求UI接收此事件
        center.SendEvent<Item>(nameof(CraftViewEventName.CraftingProductUpdate), Item.Empty);
    }
    public void OnUUIDClick(int uuid)
    {
        if (uuid == currentCrafting) return;
        currentCrafting = uuid;

        Craft_Data d = EventCenter.WorldCenter.GetParm<int, Craft_Data>(nameof(EventNames.GetCraftDataByUUID), uuid);
        ((ItemPageBig_Data)itemPages[1]).SetItems(d.ingredium,4);//获取craftdata填入需求物，需保存需求物以便识别能否装入
        //center.SendEvent<int>("bpFlush", 1);//触发craft原料需求改变事件

        center.SendEvent<Item[]>(nameof(CraftViewEventName.CraftingUpdate), d.ingredium);//触发craft配方改变事件,合成需求UI接收此事件
        center.SendEvent<Item>(nameof(CraftViewEventName.CraftingProductUpdate), d.product);
       
    }
    void Refresh()
    {
        Craft_Data d = EventCenter.WorldCenter.GetParm<int, Craft_Data>(nameof(EventNames.GetCraftDataByUUID), currentCrafting);
        if (d == null) {
            center.SendEvent<Item>(nameof(CraftViewEventName.CraftingProductUpdate), null);
            center.SendEvent<Craft_Data, int>(nameof(CraftViewEventName.CraftingStart), null, 0);
            center.SendEvent<Item>(nameof(CraftViewEventName.craftDone), null);
            center.SendEvent<float>(nameof(CraftViewEventName.CraftProcessGo), 0);
            return; }
        //center.SendEvent<Item[]>(nameof(CraftViewEventName.CraftingUpdate), d.ingredium);//触发craft配方改变事件,合成需求UI接收此事件
        center.SendEvent<Item>(nameof(CraftViewEventName.CraftingProductUpdate), d.product);
        center.SendEvent<Craft_Data, int>(nameof(CraftViewEventName.CraftingStart), Crafting, remainCraftNum);
        center.SendEvent<Item>(nameof(CraftViewEventName.craftDone), dones);
        center.SendEvent<float>(nameof(CraftViewEventName.CraftProcessGo), 0);

    }
    public void CancleCraft()
    {
        remainCraftNum = 0;
        processTime = 0;

        center.SendEvent<float>(nameof(CraftViewEventName.CraftProcessGo), 0);
        Crafting = null;
        //craftNum = 0;
        center.SendEvent<Craft_Data, int>(nameof(CraftViewEventName.CraftingStart), Crafting, craftNum);
        Debug.Log("取消完成");
    }
    public void UISliderMove(int a)
    {
        Debug.Log(a);
        craftNum = a;
    }
    public void startCrafting()
    {
        if (!Craft_Data.IsNullOrEmpty(Crafting)) return;
        if (craftNum == 0) return;
        int tempCraftNum = craftNum;
        //获取合成数量，当前合成配方
        Crafting = EventCenter.WorldCenter.GetParm<int, Craft_Data>(nameof(EventNames.GetCraftDataByUUID), currentCrafting);
        //扣除存栏内item
        Debug.Log("开始扣除");
        for (int i = 0; i < Crafting.ingredium.Length; i++)
        {
            Item temp = new Item(Crafting.ingredium[i]);
            temp.num *= tempCraftNum;
           int count= Contain(temp);
            if (count < temp.num) { Debug.Log("原料数量不足"); return; }
        }
        Crafting_mat = new Item[Crafting.ingredium.Length];
            for (int i = 0; i < Crafting.ingredium.Length; i++)
        {
            Item temp = new Item(Crafting.ingredium[i]);
            temp.num *= tempCraftNum;
            Debug.Log("扣除：" + temp.ToString());
            SubItem(temp);
            Crafting_mat[i] = temp;
        }
        //调整crafting,craftingmat,
        center.SendEvent<Craft_Data,int >(nameof(CraftViewEventName.CraftingStart), Crafting, tempCraftNum);

        remainCraftNum = tempCraftNum;
        processTime = 0;
    }
    public void FixedUpdate()
    {
        if (Crafting == null) return;
        if (remainCraftNum == 0||Crafting_mat==null) return;
        if (Item.IsNullOrEmpty(dones)|| dones.Compare(Crafting.product))
        {
            processTime += Time.fixedDeltaTime;
            center.SendEvent<float>(nameof(CraftViewEventName.CraftProcessGo), processTime / Crafting.time);
            if (processTime > Crafting.time)//合成完一个
            {
                remainCraftNum -= 1;
                processTime = 0;

                center.SendEvent<float>(nameof(CraftViewEventName.CraftProcessGo), 0);

                center.SendEvent<Craft_Data, int>(nameof(CraftViewEventName.CraftingStart), Crafting, remainCraftNum);
                if(Item.IsNullOrEmpty(dones))
                {
                    dones = new Item(Crafting.product);
                }
                else
                {
                    dones.SafeAdd(Crafting.product.num, Crafting.product.level);
                }
                Debug.Log("产出：" + dones.ToString());
                center.SendEvent<Item>(nameof(CraftViewEventName.craftDone), dones);

                if(remainCraftNum==0)//所有合成完毕
                {//清除crafting
                    Crafting = null;
                    craftNum = 0;
                    center.SendEvent<Craft_Data, int>(nameof(CraftViewEventName.CraftingStart), Crafting, craftNum);
                }
            }
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
        UICenter.UIWorldCenter.ShowView("craft", "backpack");
        BaseUIView chest = UICenter.UIWorldCenter.GetView("craft");
        chest.BuildMVConnect(chest.UIName, center, source);
        Debug.Log("craft BuildMVConnect");
        player = source;
    }

    public override Item[,] GetBigItems(int page = 0)
    {
        if (page == 0)
        { return base.GetBigItems(page); }
        else
        {
            Craft_Data d = EventCenter.WorldCenter.GetParm<int, Craft_Data>(nameof(EventNames.GetCraftDataByUUID), currentCrafting);
            if (d == null) return null;
            int[,] sizes = itemPages[0].GetPlacements();
            Item[,] temp = new Item[sizes.GetLength(0), sizes.GetLength(1)];
            for (int i = 0; i < d.ingredium.Length; i++)
            {
                temp[i / 4, i % 4] = d.ingredium[i];
            }
            return temp;
        }
    }

    public override void SendItemEvent(Item i, int x, int y, int index, int page, ObjInPageChangeMode mode)
    {
        base.SendItemEvent(i, x, y, index, page, mode);
        Craft_Data d = EventCenter.WorldCenter.GetParm<int, Craft_Data>(nameof(EventNames.GetCraftDataByUUID), currentCrafting);
        int maxCount = int.MaxValue;
        for (int j = 0; j < d.ingredium.Length; j++)
        {
            int num = Contain(d.ingredium[j]);
            if (num / d.ingredium[j].num < maxCount) maxCount = num / d.ingredium[j].num;
            if (maxCount == 0) break;
        }
        if (maxCount == 0) { center.SendEvent<int, int, int>(nameof(CraftViewEventName.CraftNumUpdate), 0, 0, 0); }
        else { center.SendEvent<int, int, int>(nameof(CraftViewEventName.CraftNumUpdate), 1, maxCount, 1); }
    }
    public override void FromObject(object data)
    {
        JArray temp = ((JArray)(data)).ToObject<JArray>();
        //Crafting = temp[0].ToObject<Craft_Data>();

        string tempstr = temp[0].ToString();
        Crafting = JsonConvert.DeserializeObject<Craft_Data>(tempstr);

        tempstr = temp[1].ToString();
        Crafting_mat = JsonConvert.DeserializeObject<Item[]>(tempstr);

        tempstr = temp[2].ToString();
        craftNum = int.Parse(tempstr);

        tempstr = temp[3].ToString();
        remainCraftNum = int.Parse(tempstr);

        tempstr = temp[4].ToString();
        currentCrafting = int.Parse(tempstr);

        tempstr = temp[5].ToString();
        processTime = float.Parse(tempstr);

        //remainCraftNum = temp[3].ToObject<int>();
        //currentCrafting = temp[4].ToObject<int>();
        //processTime = temp[5].ToObject<float>();

        if (temp.Count > 6)
        {
            itemPages = JsonConvert.DeserializeObject<Dictionary<int, IBackPack>>(temp[6].ToString(), JsonSetting.serializerSettings);
            foreach (var item in itemPages)
            {
                item.Value.init();
            }
        }
    }
    public override object ToObject()
    {
        //string[] temp ;
        JArray st = new JArray();
        string cSTR = JsonConvert.SerializeObject(Crafting);
        st.Add(cSTR);
        cSTR = JsonConvert.SerializeObject(Crafting_mat);
        st.Add(cSTR);
        cSTR = JsonConvert.SerializeObject(craftNum);
        st.Add(cSTR);
        cSTR = JsonConvert.SerializeObject(remainCraftNum);
        st.Add(cSTR);
        cSTR = JsonConvert.SerializeObject(currentCrafting);
        st.Add(cSTR);
        cSTR = JsonConvert.SerializeObject(processTime);
        st.Add(cSTR);

        if (itemPages != null)
        {
            st.Add(JsonConvert.SerializeObject(itemPages,JsonSetting.serializerSettings));
            //st.Add(itempageArr);
        }
        return st;
    }
}
