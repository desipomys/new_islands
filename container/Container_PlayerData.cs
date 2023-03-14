//存档的全局数据，包括玩家属性，NPC列表，仓库，是否在map中，当前进入的map设置...

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class Container_PlayerData : BaseContainer, IBackPack
{
    PlayerData data;//不会接收ingame期间玩家gobj的背包数据变化，退出ingame时才会保存玩家ingame数据到此
    NPCBPAccsser nPCBPAccsser;
    ItemPageChangeParm itemChangeArg = new ItemPageChangeParm();
    NPCDataChangeParm npcChangeArg = new NPCDataChangeParm();
    ItemPageChangeParm npcItemChgArg = new ItemPageChangeParm();
    ValueChangeParm<int> peopleChgArg = new ValueChangeParm<int>();

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.RegistFunc<IBackPack>("backpack", () => { return this; });
        e.RegistFunc<int, IBackPack>(nameof(Container_PlayerData_EventNames.NPCbackpack), (int a) => { nPCBPAccsser = new NPCBPAccsser(GetNPCByIndex(a),SendNPCEvent,a,GetUIIndexByTrueIndex(a)); return nPCBPAccsser; });
        e.RegistFunc<bool>("IsInSave", () => { return data != null; });
        //不要在init时就加载playerdata，只有选择进入了某个存档后才开始加载
    }
    public override void OnLoadSave(SaveData save)//刚进入存档时调用
    {
        getPlayerData(save.savePath);
        dataInit();
        regEvent();
        //center.SendEvent("LoadSaveDone");
        Debug.Log("playerdata加载完成");
    }
    public override void UnLoadSave()
    {
        base.UnLoadSave();
        data = null;
        unRegEvent();
        Debug.Log("playerdata清理完成");
    }
    public override void Save(string path)
    {
        Debug.Log("save playerdata");
        base.Save(path);
        if (data != null)
        {
            if (!Item.IsNullOrEmpty(data.mouseItem))
            {
                //data.wareHouse[0].a将item+到仓库中
            }
            FileSaver.SavePlayerData(path, data.ToString());
        }
    }
    public override void OnLoadGame(MapPrefabsData data, int index)
    {
        if (index != 0) return;
        base.OnLoadGame(data, index);

    }
    public override void UnLoadGame(int ind)
    {
        if (ind != 0) return;
        base.UnLoadGame(ind);
    }

    public ItemPage_Data getWareHouse(int index)
    {
        return data.wareHouse[index];
    }


    void getPlayerData(string path)
    {
        try
        {
            string dat = FileSaver.GetFileInSave(path, "data");
            data = PlayerData.FromString(dat);
            playerDataInit(data);
            if (data == null)
            {
                data = new PlayerData();
                Debug.Log("playerdata加载失败");
            }
        }
        catch (System.Exception)
        {
            data = new PlayerData();
            playerDataInit(data);
            Debug.Log("playerdata加载失败");
        }


    }
    void playerDataInit(PlayerData pd)
    {
        if (string.IsNullOrEmpty(pd.mapIndex))
        {
            try
            {
                pd.mapIndex = center.GetParm<MapPrefabsData[]>(nameof(EventNames.GetMapprefabDatas))[0].mapName;
            }
            catch (Exception)
            {
                pd.mapIndex = "";
            }

        }
    }
    void regEvent()
    {
        EventCenter.WorldCenter.RegistFunc<Item>(nameof(EventNames.GetMouseItem), () => { return data.mouseItem; });
        EventCenter.WorldCenter.ListenEvent<Item>(nameof(EventNames.SetMouseItem), Set_MouseItem);
        EventCenter.WorldCenter.RegistFunc<Resource_Data>("GetResource", () => { return data.resource; });

        EventCenter.WorldCenter.ListenEvent<string, float>(nameof(EventNames.SetRes), SetRes_res);
        /*EventCenter.WorldCenter.ListenEvent<float>("SetRes_buildmat", SetRes_buildmat);
        EventCenter.WorldCenter.ListenEvent<float>("SetRes_energy", SetRes_energy);
        EventCenter.WorldCenter.ListenEvent<float>("SetRes_food", SetRes_food);
        EventCenter.WorldCenter.ListenEvent<float>("SetRes_metal", SetRes_metal);
        EventCenter.WorldCenter.ListenEvent<float>("SetRes_chemi", SetRes_chemi);
        EventCenter.WorldCenter.ListenEvent<float>("SetRes_magic", SetRes_magic);*/

        EventCenter.WorldCenter.ListenEvent<string, string>("Mov_setres", Mov_setres);

        //center.RegistFunc<NpcData>(nameof(Container_PlayerData_EventNames.GetPlayerCharData), () => { return data.player; });
        //center.ListenEvent<NpcData>(nameof(Container_PlayerData_EventNames.SetPlayerCharData), (NpcData nd) => { data.player = nd; });
        center.ListenEvent<int, int>(nameof(Container_PlayerData_EventNames.SetNPCSelectIndex), SetNPCSelectIndex);

        center.RegistFunc<int, NpcData>(nameof(Container_PlayerData_EventNames.GetNPCByUIIndex), GetNPCByUIIndex);
        center.RegistFunc<int, NpcData>(nameof(Container_PlayerData_EventNames.GetNPCByIndex), GetNPCByIndex);
        center.RegistFunc<Dictionary<int, NpcData>>(nameof(Container_PlayerData_EventNames.GetAllNPCData), () => { return data.allNpc; });
        //center.RegistFunc<NpcData[]>(nameof(Container_PlayerData_EventNames.GetAllSelectedNPCData), () => { return data.allNpc; });
        center.RegistFunc<int[]>(nameof(Container_PlayerData_EventNames.GetNPCSelectedIndexs), () => { return data.selected_npc; });
        center.RegistFunc<string>(nameof(Container_PlayerData_EventNames.GetMapIndex), () => { return data.mapIndex; });
        center.ListenEvent<string>(nameof(Container_PlayerData_EventNames.MovShipIndex), (string s) =>
        {
            string temp = data.mapIndex;
            data.mapIndex = s;
            center.SendEvent<ShipPosChgParm>(nameof(Container_PlayerData_EventNames.OnMapIndexChg), new ShipPosChgParm(temp, data.mapIndex));
        });
        center.RegistFunc<int, Item[]>("GetWareHouse_Items", (int p) => { return data.wareHouse[p].items; });
        center.RegistFunc<int, int[,]>("GetWareHouse_Placement", (int p) => { return data.wareHouse[p].placement; });
        //EventCenter.WorldCenter.ListenEvent("QuitStartScene", onQuitStartScene);

        center.RegistFunc<IBackPack>("backpack", () => { return this; });
        center.RegistFunc<MapPrefabsData>(nameof(EventNames.GetShipAtMapData), GetShipAtMapData);
        center.ListenEvent<Resource_Data>(nameof(Container_PlayerData_EventNames.UseResource), UseResource);
        center.RegistFunc<ResearchData>(nameof(EventNames.GetReserchData), () => { return data.research; });
        center.ListenEvent<ResearchData>(nameof(EventNames.SetReserchData), SetReserchData);

        center.ListenEvent<NpcData>(nameof(Container_PlayerData_EventNames.AddNPC), AddNPC);
        center.ListenEvent(nameof(Container_PlayerData_EventNames.AddNPCSlot), AddNPCSlot);
        center.RegistFunc<int>(nameof(Container_PlayerData_EventNames.GetWareHousePageNum), GetWareHousePageNum);

        //BaseUIView buv=center.GetParm<string, BaseUIView>("GetView", "wareHouse");
        //BuildMVConnect(null, buv);
    }
    void unRegEvent()
    {
        EventCenter.WorldCenter.ForceUnRegistFunc<Item>("GetMouseItem");
        center.UnListenEvent<Item>("SetMouseItem", Set_MouseItem);
        EventCenter.WorldCenter.ForceUnRegistFunc<Resource_Data>("GetResource");

        EventCenter.WorldCenter.UnListenEvent<string, float>(nameof(EventNames.SetRes), SetRes_res);
        /*EventCenter.WorldCenter.UnListenEvent<float>("SetRes_buildmat", SetRes_buildmat);
        EventCenter.WorldCenter.UnListenEvent<float>("SetRes_energy", SetRes_energy);
        EventCenter.WorldCenter.UnListenEvent<float>("SetRes_food", SetRes_food);
        EventCenter.WorldCenter.UnListenEvent<float>("SetRes_metal", SetRes_metal);
        EventCenter.WorldCenter.UnListenEvent<float>("SetRes_chemi", SetRes_chemi);
        EventCenter.WorldCenter.UnListenEvent<float>("SetRes_magic", SetRes_magic);*/

        EventCenter.WorldCenter.UnListenEvent<string, string>("Mov_SetResource", Mov_setres);

        center.ForceUnRegistFunc<NpcData>(nameof(Container_PlayerData_EventNames.GetPlayerCharData));
        center.ForceUnListenEvent(nameof(Container_PlayerData_EventNames.SetPlayerCharData));
        center.ForceUnListenEvent(nameof(Container_PlayerData_EventNames.SetNPCSelectIndex));

        center.ForceUnRegistFunc<int, NpcData>(nameof(Container_PlayerData_EventNames.GetNPCByUIIndex));
        center.ForceUnRegistFunc<int, NpcData>(nameof(Container_PlayerData_EventNames.GetNPCByIndex));
        center.ForceUnRegistFunc<NpcData[]>(nameof(Container_PlayerData_EventNames.GetAllNPCData));
        center.ForceUnRegistFunc<NpcData[]>(nameof(Container_PlayerData_EventNames.GetAllSelectedNPCData));
        center.ForceUnRegistFunc<int[]>(nameof(Container_PlayerData_EventNames.GetNPCSelectedIndexs));
        center.ForceUnRegistFunc<string>(nameof(Container_PlayerData_EventNames.GetMapIndex));
        center.ForceUnListenEvent(nameof(Container_PlayerData_EventNames.MovShipIndex));
        center.ForceUnRegistFunc<int, Item[]>("GetWareHouse_Items");
        center.ForceUnRegistFunc<int, int[,]>("GetWareHouse_Placement");
        //EventCenter.WorldCenter.UnListenEvent("QuitStartScene", onQuitStartScene);

        center.ForceUnRegistFunc<IBackPack>("backpack");
        center.UnRegistFunc<MapPrefabsData>(nameof(EventNames.GetShipAtMapData), GetShipAtMapData);
        center.UnListenEvent<Resource_Data>(nameof(Container_PlayerData_EventNames.UseResource), UseResource);
        center.ForceUnRegistFunc<ResearchData>(nameof(EventNames.GetReserchData));
        center.ForceUnListenEvent(nameof(EventNames.SetReserchData));
        center.UnListenEvent<NpcData>(nameof(Container_PlayerData_EventNames.AddNPC), AddNPC);
        center.UnRegistFunc<int>(nameof(Container_PlayerData_EventNames.GetWareHousePageNum), GetWareHousePageNum);
        //BaseUIView buv = center.GetParm<string, BaseUIView>("GetView", "wareHouse");
        //BreakMVConnect(null,buv);
    }




    public override void OnQuitStartScene()
    {
        if (!Item.IsNullOrEmpty(data.mouseItem))
        {
            AddItem(data.mouseItem);
            data.mouseItem = Item.Empty;
        }
    }
    public override void OnArriveStartScene()
    {

    }

    void dataInit()
    {//发送初始数据加载完的第一次变化事件，应让UI自己来取数据显示
        sendResEvent();
        sendMouseEvent();
    }
    #region 读取方法区
    int GetWareHousePageNum()
    {
        return data.wareHouse.Length;
    }
    NpcData GetNPCByUIIndex(int index)//uiindex
    {
        try
        {
            //if (index == -1) return data.player;
            if (data.selected_npc[index] == int.MinValue) return null;
            return data.allNpc[data.selected_npc[index]];
        }
        catch (System.Exception)
        {
            return null;
        }

    }
    NpcData GetNPCByIndex(int index)
    {
        try
        {
            if (index == int.MinValue) return null;
            //if (index == -1) return data.player;
            return data.allNpc[index];
        }
        catch (System.Exception)
        {
            return null;
        }

    }
    int GetUIIndexByTrueIndex(int trueindex)
    {
        if (trueindex < 0) return -1;
        for (int i = 0; i < data.selected_npc.Length; i++)
        {
            if (data.selected_npc[i] == trueindex) return i;
        }
        return -1;
    }
    NpcData[] GetAllSelectedNPC()
    {
        List<NpcData> nds = new List<NpcData>();
        for (int i = 0; i < data.selected_npc.Length; i++)
        {
            nds.Add(data.allNpc[data.selected_npc[i]]);
        }
        return nds.ToArray();
    }

    public MapPrefabsData GetShipAtMapData()
    {
        Container_MapPrefabData loader = ContainerManager.GetContainer<Container_MapPrefabData>();
        MapPrefabsData mpd = loader.GetMapDataByName(data.mapIndex);
        if (mpd != null)
        { return mpd; }
        else
        {
            return null;
        }
    }
    public string GetMapIndex()
    {
        return data.mapIndex;
    }
    #endregion
    #region 写入方法区
    void SetRes_res(string typ, float t)
    {
        switch (typ)
        {
            case "m":
                data.resource.material = t;
                break;
            case "e":
                data.resource.energy = t;
                break;
            case "p":
                data.resource.manPower = t;
                break;
            case "i":
                data.resource.information = t;
                break;

            default:
                break;
        }

        sendResEvent();
    }
    /*void SetRes_buildmat(float t) { data.resource.build_mat = t; onResChange(); }
    void SetRes_energy(float t) { data.resource.energy = t; onResChange(); }
    void SetRes_food(float t) { data.resource.food = t; onResChange(); }
    void SetRes_metal(float t) { data.resource.metal = t; onResChange(); }
    void SetRes_chemi(float t) { data.resource.chemical = t; onResChange(); }
    void SetRes_magic(float t) { data.resource.magic = t; onResChange(); }*/

    void Set_MouseItem(Item i) { data.mouseItem = i; Debug.Log("设置手" + i.id); center.SendEvent<Item>("MouseItemChg", i); }
    /// <summary>
    /// 提供给脚本访问的方法
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    void Mov_setres(string target, string value)
    {
        //等待新command解析器支持外部方法时解封
        /*
         * try
        {
            float temp = float.Parse(value);
            switch (target)
            {
                case "res":
                    SetRes_res(temp);
                    break;
                case "buildmat":
                    SetRes_buildmat(temp);
                    break;
                case "energy":
                    SetRes_energy(temp);
                    break;
                case "food":
                    SetRes_food(temp);
                    break;
                case "metal":
                    SetRes_metal(temp);
                    break;
                case "chemical":
                    SetRes_chemi(temp);
                    break;
                case "magic":
                    SetRes_magic(temp);
                    break;
                default:
                    break;
            }
            //MovScriptCommandResolver.ExecuteResult = true;
        }
        catch (System.Exception)
        {

            MovScriptCommandResolver.ExecuteResult = false;
        }
        */
    }

    void AddNPCSlot()
    {
        data.AddSelectNPCSlot();
        SendNPCSelectChgEvent(data.selected_npc);
    }
    void SetNPCSelectIndex(int uiindex, int targetindex)
    {
        Debug.Log("改变SELECT" + uiindex + ":" + targetindex);
        if (data.selected_npc == null) { data.selected_npc = new int[1]; }
        if (targetindex == int.MinValue)
        {
            data.selected_npc[uiindex] = targetindex;
            SendNPCSelectChgEvent(data.selected_npc);
            SendNPCEvent(null, null, -1, uiindex, NPCDataChgPOS.all, ObjInPageChangeMode.set);
        }//将当前UIINDEX设为空
        else
        {
            foreach (var item in data.selected_npc)
            {
                if (item == targetindex) return;
            }
            data.selected_npc[uiindex] = targetindex;
            SendNPCSelectChgEvent(data.selected_npc);
            SendNPCEvent(data.allNpc[targetindex], null, targetindex, uiindex, NPCDataChgPOS.all, ObjInPageChangeMode.set);
        }
    }

    void UseResource(Resource_Data res)
    {
        data.resource -= res;
        sendResEvent();
    }
    void SetReserchData(ResearchData d)
    {

    }
    void SetPeopleNum(int p)
    {
        int a = data.peopleNum;
        data.peopleNum = p;
        sendPeopleChg(a, p);
    }
    void SetShipPos(string land)
    {
        string old = data.mapIndex;
        data.mapIndex = land;
        sendShipChg(old);
    }
    #endregion
    public void SetSize(int h, int w, int page = 0)
    {
        List<ItemPage_Data> temp = new List<ItemPage_Data>();
        for (int i = 0; i < page; i++)
        {
            temp.Add(new ItemPage_Data(h, w));
        }
        data.wareHouse = temp.ToArray();
    }
    public void SetItems(Item[] items, int[,] placements, int page = 0)
    {
        data.wareHouse[page].SetItems(items, placements);
    }


    #region  backpackset
    public void ClearItems(int page = 0)
    {
        throw new NotImplementedException();
    }
    //0-2仓库
    public virtual bool SetItemAt(Item i, int x, int y, int page = 0)
    {
        int[] xy = data.wareHouse[page].GetItemLeftUp(x, y);
        bool p = data.wareHouse[page].SetItemAt(i, x, y);
        SendItemEvent(data.wareHouse[page].GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.set);
        return p;
    }
    public virtual bool DeleteItemAt(int x, int y, int page = 0)
    {
        int[] xy = data.wareHouse[page].GetItemLeftUp(x, y);
        bool p = data.wareHouse[page].DeleteItemAt(x, y);
        SendItemEvent(data.wareHouse[page].GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.delete);
        return p;
    }

    public virtual int AddItem(Item item, int page = 0)
    {
        int p = item.num;
        for (int i = 0; i < data.wareHouse.Length; i++)
        {
            for (int j = 0; j < data.wareHouse[i].height; j++)
            {
                for (int k = 0; k < data.wareHouse[i].width; k++)
                {

                }
            }
        }
        return p;
    }
    public virtual int AddItemAt(Item i, int x, int y, int page = 0)
    {
        int[] xy = data.wareHouse[page].GetItemLeftUp(x, y);
        int p = data.wareHouse[page].AddItemAt(i, x, y);
        SendItemEvent(data.wareHouse[page].GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.add);

        return p;
    }
    public virtual int SubItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return 0;
    }
    public virtual int SubItemAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        int[] xy = data.wareHouse[page].GetItemLeftUp(x, y);
        int p = data.wareHouse[page].SubItemAt(item, x, y);
        SendItemEvent(data.wareHouse[page].GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.sub);

        return p;
    }
    public virtual int SetItemNumAt(int num, int x, int y, int page = 0)
    {
        int[] xy = data.wareHouse[page].GetItemLeftUp(x, y);
        int p = data.wareHouse[page].SetItemNumAt(num, x, y);
        SendItemEvent(data.wareHouse[page].GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.set);

        return p;
    }
    public int AddItemNumAt(int i, int x, int y, int page = 0)
    {
        int[] xy = data.wareHouse[page].GetItemLeftUp(x, y);
        int p = data.wareHouse[page].AddItemNumAt(i, x, y);

        SendItemEvent(data.wareHouse[page].GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.add);
        return p;
    }
    #endregion
    public void init() { }
    #region  backpackget
    public bool IsBigChest()
    {
        return true;
    }
    public virtual int Contain(Item item, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        int count = 0;
        if (page == -1)
        {
            for (int i = 0; i < data.wareHouse.Length; i++)
            {
                count += data.wareHouse[page].Contain(item, page, mode);
            }
            return count;
        }
        else
        {
            count += data.wareHouse[page].Contain(item, page, mode);
            return count;
        }
    }
    public virtual int ContainAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return data.wareHouse[page].ContainAt(item, x, y);
    }
    public virtual Item GetItemAt(int x, int y, int page = 0)
    {
        return data.wareHouse[page].GetItemAt(x, y);
    }
    public virtual int CountItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return data.wareHouse[page].CountItem(i, page, mode);
    }
    public virtual bool CanPlaceAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return data.wareHouse[page].CanPlaceAt(item, x, y, page, mode);
    }
    public int[] GetItemLeftUp(int x, int y, int page = 0)
    {
        //throw new System.NotImplementedException();
        return data.wareHouse[page].GetItemLeftUp(x, y);
    }
    public virtual bool CanPlaceIgnoreCurrent(Item it, int x, int y, int page = 0)
    {
        return data.wareHouse[page].CanPlaceIgnoreCurrent(it, x, y);
    }
    public Item[] GetItems(int page = 0)
    {

        return data.wareHouse[page].items;
    }
    public int[,] GetPlacements(int page = 0)
    {
        return data.wareHouse[page].placement;
    }

    public Item[,] GetBigItems(int page = 0)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region 玩家和NPC皮肤、装备、道具
    int findUIIndexByTrueIndex(int trueindex)
    {
        try
        {
            for (int i = 0; i < data.selected_npc.Length; i++)
            {
                if (trueindex == data.selected_npc[i]) return i;
            }
            return -1;
        }
        catch (System.Exception)
        {
            return -1;

        }


    }
    //返回设置的物品是否放置在指定位置
    /*public bool SetPlayerSkin(Item i, int index)
    {
        if (index >= NpcData.skinSize || index < 0) return false;
        if (data.player.skin == null) data.player.skin = new Item[NpcData.skinSize];
        if (!Item.IsNullOrEmpty(i) && center.GetParm<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), i.id) != ItemGroup.skin) return false;//如果i不属于skin
        data.player.skin[index] = i;
        SendNPCEvent(data.player, null, -1, -1, NPCDataChgPOS.skin, ObjInPageChangeMode.set);
        return true;
    }
    public bool SetPlayerEquip(Item i, int index)
    {
        if (index >= NpcData.equipSize || index < 0) return false;
        if (data.player.equip == null) data.player.equip = new Item[NpcData.equipSize];
        if (!Item.IsNullOrEmpty(i)&&center.GetParm<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), i.id) != ItemGroup.equip) return false;//如果i不属于skin
        data.player.equip[index] = i;
        SendNPCEvent(data.player, null, -1, -1, NPCDataChgPOS.equip, ObjInPageChangeMode.set);
        return true;
    }
    public bool SetPlayerProps(Item i, int index)
    {
        if (index >= NpcData.partSize || index < 0) return false;
        if (data.player.part == null) data.player.part = new Item[NpcData.partSize];
        if (!Item.IsNullOrEmpty(i) && center.GetParm<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), i.id) != ItemGroup.part) return false;//如果i不属于skin
        data.player.part[index] = i;
        SendNPCEvent(data.player, null, -1, -1, NPCDataChgPOS.part, ObjInPageChangeMode.set);
        return true;
    }
    public bool SetPlayerSkin(Item[] i)
    {
        if (i.Length > NpcData.skinSize) return false;
        for (int j = 0; j < i.Length; j++)
        {
            if (!Item.IsNullOrEmpty(i[j]) && center.GetParm<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), i[j].id) != ItemGroup.skin) { return false; }
        }
        data.player.skin = i;
        SendNPCEvent(data.player, null, -1, -1, NPCDataChgPOS.skin, ObjInPageChangeMode.set);
        return true;
    }
    public bool SetPlayerEquip(Item[] i)
    {
        if (i.Length > NpcData.equipSize) return false;
        for (int j = 0; j < i.Length; j++)
        {
            if (!Item.IsNullOrEmpty(i[j]) && center.GetParm<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), i[j].id) != ItemGroup.equip) { return false; }
        }
        data.player.equip = i;
        SendNPCEvent(data.player, null, -1, -1, NPCDataChgPOS.equip, ObjInPageChangeMode.set);
        return true;
    }
    public bool SetPlayerProps(Item[] i)
    {
        if (i.Length > NpcData.partSize) return false;
        for (int j = 0; j < i.Length; j++)
        {
            if (!Item.IsNullOrEmpty(i[j]) && center.GetParm<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), i[j].id) != ItemGroup.part) { return false; }
        }
        data.player.part = i;
        SendNPCEvent(data.player, null, -1, -1, NPCDataChgPOS.part, ObjInPageChangeMode.set);
        return true;
    }
    */
    public bool SetNPCSkin(int trueindex, int uiindex, Item i, int index)
    {
        //if (trueindex == -1) return SetPlayerSkin(i, index);
        if (index >= NpcData.skinSize || index < 0) return false;
        //if (data.player.skin == null) data.player.skin = new Item[NpcData.skinSize];
        if (!Item.IsNullOrEmpty(i) && center.GetParm<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), i.id) != ItemGroup.skin) return false;//如果i不属于skin
        data.allNpc[trueindex].skin[index] = i;
        SendNPCEvent(data.allNpc[trueindex], null, trueindex, uiindex, NPCDataChgPOS.skin, ObjInPageChangeMode.set);
        return true;
    }
    public bool SetNPCEquip(int trueindex, int uiindex, Item i, int index)
    {
        //if (trueindex == -1) return SetPlayerEquip(i, index);
        if (index >= NpcData.equipSize || index < 0) return false;
        //if (data.player.equip == null) data.player.equip = new Item[NpcData.equipSize];
        if (!Item.IsNullOrEmpty(i) && center.GetParm<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), i.id) != ItemGroup.equip) return false;//如果i不属于skin
        data.allNpc[trueindex].equip[index] = i;
        SendNPCEvent(data.allNpc[trueindex], null, trueindex, uiindex, NPCDataChgPOS.equip, ObjInPageChangeMode.set);
        return true;
    }
    public bool SetNPCProps(int trueindex, int uiindex, Item i, int index)
    {
        //if (trueindex == -1) return SetPlayerProps(i, index);
        if (index >= NpcData.partSize || index < 0) return false;
        //if (data.player.part == null) data.player.part = new Item[NpcData.partSize];
        if (!Item.IsNullOrEmpty(i) && center.GetParm<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), i.id) != ItemGroup.part) return false;//如果i不属于skin
        data.allNpc[trueindex].part[index] = i;
        SendNPCEvent(data.allNpc[trueindex], null, trueindex, uiindex, NPCDataChgPOS.part, ObjInPageChangeMode.set);
        return true;
    }
    public bool SetNPCSkin(int trueindex, int uiindex, Item[] i)
    {
        //if (trueindex == -1) return SetPlayerSkin(i);
        if (i.Length > NpcData.skinSize) return false;
        for (int j = 0; j < i.Length; j++)
        {
            if (!Item.IsNullOrEmpty(i[j]) && center.GetParm<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), i[j].id) != ItemGroup.skin) { return false; }
        }
        data.allNpc[trueindex].skin = i;
        SendNPCEvent(data.allNpc[trueindex], null, trueindex, uiindex, NPCDataChgPOS.skin, ObjInPageChangeMode.set);
        return true;
    }
    public bool SetNPCEquip(int trueindex, int uiindex, Item[] i)
    {
        //if (trueindex == -1) return SetPlayerEquip(i);
        if (i.Length > NpcData.equipSize) return false;
        for (int j = 0; j < i.Length; j++)
        {
            if (!Item.IsNullOrEmpty(i[j]) && center.GetParm<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), i[j].id) != ItemGroup.equip) { return false; }
        }
        data.allNpc[trueindex].equip = i;
        SendNPCEvent(data.allNpc[trueindex], null, trueindex, uiindex, NPCDataChgPOS.equip, ObjInPageChangeMode.set);
        return true;
    }
    public bool SetNPCProps(int trueindex, int uiindex, Item[] i)
    {
        //if (trueindex == -1) return SetPlayerProps(i);
        if (i.Length > NpcData.partSize) return false;
        for (int j = 0; j < i.Length; j++)
        {
            if (!Item.IsNullOrEmpty(i[j]) && center.GetParm<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), i[j].id) != ItemGroup.part) { return false; }
        }
        data.allNpc[trueindex].part = i;
        SendNPCEvent(data.allNpc[trueindex], null, trueindex, uiindex, NPCDataChgPOS.part, ObjInPageChangeMode.set);
        return true;
    }

    public Item GetNPCSkin(int trueIndex, int skinIndex)
    {
        if (trueIndex == -1) return null;
        else return data.allNpc[trueIndex].skin[skinIndex];
    }
    public Item GetNPCEquip(int trueIndex, int equipIndex)
    {
        if (trueIndex == -1) return null;
        else return data.allNpc[trueIndex].equip[equipIndex];
    }
    public Item GetNPCPart(int trueIndex, int partIndex)
    {
        if (trueIndex == -1) return null;
        else return data.allNpc[trueIndex].part[partIndex];
    }
    public void AddNPC(NpcData a)
    {
        if (a == null) return;
        int max = 0;
        if (data.allNpc == null) data.allNpc = new Dictionary<int, NpcData>();
        foreach (var item in data.allNpc)
        {
            if (item.Key > max) max = item.Key;
        }
        data.allNpc.Add(max + 1, a);
        SendNPCListEvent(data.allNpc);
    }
    public void AddWareHousePage()
    {
        data.AddItemPage();
        sendItemPageNumChgEvent();
    }

    #region 玩家和NPC的背包内容get
    public bool IsNPCBigChest(int trueindex)
    {
        return false;
    }
    public int NPCContain(int trueindex, Item item, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {

        int count = 0;
        if (page == -1)
        {
            for (int i = 0; i < data.allNpc.Count; i++)
            {
                count += data.allNpc[trueindex].bp.Contain(item, page, mode);
            }
            return count;
        }
        else
        {
            count += data.allNpc[trueindex].bp.Contain(item, page, mode);
            return count;
        }
    }
    public int NPCContainAt(int trueindex, Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        //if (trueindex == -1) return data.player.bp.ContainAt(item, x, y);
        return data.allNpc[trueindex].bp.ContainAt(item, x, y);
    }
    public Item GetNPCItemAt(int trueindex, int x, int y, int page = 0)
    {
        //if (trueindex == -1) return null;
        return data.allNpc[trueindex].bp.GetItemAt(x, y);
    }
    public int CountNPCItem(int trueindex, Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        //if (trueindex == -1) return null;
        return data.allNpc[trueindex].bp.CountItem(i, page, mode);
    }
    public bool NPCCanPlaceAt(int trueindex, Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        //if (trueindex == -1) return false;
        return data.allNpc[trueindex].bp.CanPlaceAt(item, x, y, page, mode);
    }
    public int[] GetNPCItemLeftUp(int trueindex, int x, int y, int page = 0)
    {
        //if (trueindex == -1) return data.player.bp.GetItemLeftUp(x, y);
        return data.allNpc[trueindex].bp.GetItemLeftUp(x, y);
    }
    public bool NPCCanPlaceIgnoreCurrent(int trueindex, Item it, int x, int y, int page = 0)
    {
        //if (trueindex == -1) return data.player.bp.CanPlaceIgnoreCurrent(it, x, y);
        return data.allNpc[trueindex].bp.CanPlaceIgnoreCurrent(it, x, y);
    }
    public Item[] GetNPCItems(int trueindex, int page = 0)
    {
        //if (trueindex == -1) return data.player.bp.items;
        return data.allNpc[trueindex].bp.items;
    }
    public int[,] GetNPCPlacements(int trueindex, int page = 0)
    {
        //if (trueindex == -1) return data.player.bp.placement;
        return data.allNpc[trueindex].bp.placement;
    }

    public Item[,] GetNPCBigItems(int trueindex, int page = 0)
    {
        throw new System.NotImplementedException();
    }


    #endregion
    #region 玩家和NPC的背包内容set
    public bool SetNPCItemAt(int trueindex, Item i, int x, int y, int page = 0)
    {
        ItemPage_Data temp;
        NpcData nd;
        if (trueindex == -1) { return false; }
        else { temp = data.allNpc[trueindex].bp; nd = data.allNpc[trueindex]; }
        int[] xy = temp.GetItemLeftUp(x, y);
        bool p = temp.SetItemAt(i, x, y);

        AssembleItemChgArg(npcItemChgArg, temp.GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.set);
        SendNPCEvent(nd, npcItemChgArg, trueindex, findUIIndexByTrueIndex(trueindex), NPCDataChgPOS.bp, ObjInPageChangeMode.set);
        return p;
    }
    public bool DeleteNPCItemAt(int trueindex, int x, int y, int page = 0)
    {
        ItemPage_Data temp;
        NpcData nd;
        if (trueindex == -1) { return false; }
        else { temp = data.allNpc[trueindex].bp; nd = data.allNpc[trueindex]; }
        int[] xy = temp.GetItemLeftUp(x, y);
        bool p = temp.DeleteItemAt(x, y);

        AssembleItemChgArg(npcItemChgArg, temp.GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.delete);
        SendNPCEvent(nd, npcItemChgArg, trueindex, findUIIndexByTrueIndex(trueindex), NPCDataChgPOS.bp, ObjInPageChangeMode.set);

        return p;
    }

    public int AddNPCItem(int trueindex, Item item, int page = 0)
    {
        int p = item.num;
        for (int i = 0; i < data.allNpc.Count; i++)
        {
            for (int j = 0; j < data.wareHouse[i].height; j++)
            {
                for (int k = 0; k < data.wareHouse[i].width; k++)
                {

                }
            }
        }
        return p;
    }
    public int AddNPCItemAt(int trueindex, Item i, int x, int y, int page = 0)
    {
        ItemPage_Data temp;
        NpcData nd;
        if (trueindex == -1) { return 0; }
        else { temp = data.allNpc[trueindex].bp; nd = data.allNpc[trueindex]; }
        int[] xy = temp.GetItemLeftUp(x, y);
        int p = temp.AddItemAt(i, x, y);


        AssembleItemChgArg(npcItemChgArg, temp.GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.add);
        SendNPCEvent(nd, npcItemChgArg, trueindex, findUIIndexByTrueIndex(trueindex), NPCDataChgPOS.bp, ObjInPageChangeMode.set);
        return p;
    }
    public int SubNPCItem(int trueindex, Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        return 0;
    }
    public int SubNPCItemAt(int trueindex, Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        ItemPage_Data temp;
        NpcData nd;
        if (trueindex == -1) { return 0; }
        else { temp = data.allNpc[trueindex].bp; nd = data.allNpc[trueindex]; }
        int[] xy = temp.GetItemLeftUp(x, y);
        int p = temp.SubItemAt(item, x, y);

        AssembleItemChgArg(npcItemChgArg, temp.GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.sub);
        SendNPCEvent(nd, npcItemChgArg, trueindex, findUIIndexByTrueIndex(trueindex), NPCDataChgPOS.bp, ObjInPageChangeMode.set);
        return p;
    }
    public int SetNPCItemNumAt(int trueindex, int num, int x, int y, int page = 0)
    {
        ItemPage_Data temp;
        NpcData nd;
        if (trueindex == -1) { return 0; }
        else { temp = data.allNpc[trueindex].bp; nd = data.allNpc[trueindex]; }
        int[] xy = temp.GetItemLeftUp(x, y);
        int p = temp.SetItemNumAt(num, x, y);


        AssembleItemChgArg(npcItemChgArg, temp.GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.set);
        SendNPCEvent(nd, npcItemChgArg, trueindex, findUIIndexByTrueIndex(trueindex), NPCDataChgPOS.bp, ObjInPageChangeMode.set);
        return p;
    }
    public int AddNPCItemNumAt(int trueindex, int i, int x, int y, int page = 0)
    {
        ItemPage_Data temp;
        NpcData nd;
        if (trueindex == -1) { return 0; }
        else { temp = data.allNpc[trueindex].bp; nd = data.allNpc[trueindex]; }
        int[] xy = temp.GetItemLeftUp(x, y);
        int p = temp.AddItemNumAt(i, x, y);


        AssembleItemChgArg(npcItemChgArg, temp.GetItemAt(x, y), xy[0], xy[1], page, ObjInPageChangeMode.add);
        SendNPCEvent(nd, npcItemChgArg, trueindex, findUIIndexByTrueIndex(trueindex), NPCDataChgPOS.bp, ObjInPageChangeMode.set);
        return p;
    }
    #endregion

    #endregion





    void AssembleItemChgArg(ItemPageChangeParm parm, Item i, int x, int y, int page, ObjInPageChangeMode mode)
    {
        parm.item = i;
        parm.x = x;
        parm.y = y;
        parm.page = page;
        parm.mode = mode;
    }
    void sendItemPageNumChgEvent()
    {
        center.SendEvent<int>(nameof(Container_PlayerData_EventNames.WareHousePageNumChg), data.wareHouse.Length);
    }
    void sendResEvent()
    {
        //Debug.Log("onreschange");
        center.SendEvent<Resource_Data>(nameof(Container_PlayerData_EventNames.ResourceChange), data.resource);
    }
    void sendMouseEvent()
    {
        center.SendEvent<Item>(nameof(Container_PlayerData_EventNames.MouseItemChg), data.mouseItem);
    }
    void sendPeopleChg(int old, int now)
    {
        peopleChgArg.old = old;
        peopleChgArg.now = now;
        center.SendEvent<ValueChangeParm<int>>(nameof(Container_PlayerData_EventNames.PeopleChg), peopleChgArg);
    }
    void sendDateChg()
    {
        center.SendEvent<DateTime>(nameof(Container_PlayerData_EventNames.DateChg), data.time);
    }
    void sendShipChg(string old)
    {
        center.SendEvent<ShipPosChgParm>(nameof(Container_PlayerData_EventNames.OnMapIndexChg), new ShipPosChgParm(old, data.mapIndex));
    }
    protected virtual void SendItemEvent(Item i, int x, int y, int page, ObjInPageChangeMode mode)
    {
        AssembleItemChgArg(itemChangeArg, i, x, y, page, mode);
        center.SendEvent<ItemPageChangeParm>(nameof(Container_PlayerData_EventNames.WareHouseItemChg), itemChangeArg);
    }
    protected virtual void SendNPCEvent(NpcData data, ItemPageChangeParm parm, int trueindex, int uiindex, NPCDataChgPOS pos, ObjInPageChangeMode mode)
    {
        Debug.Log("发送NPCdata改变事件");
        npcChangeArg.data = data;
        npcChangeArg.trueIndex = trueindex;
        npcChangeArg.UIindex = uiindex;
        npcChangeArg.bpchg = parm;
        npcChangeArg.pos = pos;
        npcChangeArg.mode = mode;
        center.SendEvent<NPCDataChangeParm>(nameof(Container_PlayerData_EventNames.NPCDataChg), npcChangeArg);
    }
    /// <summary>
    /// npc列表改变时，在某npc增减时发送
    /// </summary>
    /// <param name="data"></param>
    protected virtual void SendNPCListEvent(Dictionary<int, NpcData> data)
    {
        center.SendEvent<Dictionary<int, NpcData>>(nameof(Container_PlayerData_EventNames.NPCListChg), data);
    }
    /// <summary>
    /// npc选择改变时
    /// </summary>
    /// <param name="data"></param>
    protected virtual void SendNPCSelectChgEvent(int[] data)
    {
        center.SendEvent<int[]>(nameof(Container_PlayerData_EventNames.NPCSelectChg), data);
    }

}
public class NPCBPAccsser : IBackPack
{
    int trueindex,uiindex;
    NpcData Npc;
    Action<NpcData,ItemPageChangeParm, int, int, NPCDataChgPOS, ObjInPageChangeMode> bpEvent;
    public NPCBPAccsser(NpcData data, Action<NpcData,ItemPageChangeParm,int,int, NPCDataChgPOS,ObjInPageChangeMode> act,int trueindex,int uiindex)
    {
        Npc = data;
        bpEvent = act;
        this.trueindex = trueindex;
        this.uiindex = uiindex;
    }
    ItemPageChangeParm changeParm = new ItemPageChangeParm();
    void AssembleItemChgArg(ItemPageChangeParm parm, Item i, int x, int y, int page, ObjInPageChangeMode mode)
    {
        parm.item = i;
        parm.x = x;
        parm.y = y;
        parm.page = page;
        parm.mode = mode;
    }
    void SendItemEvent(Item i, int x, int y, int page, ObjInPageChangeMode mode)
    {
        AssembleItemChgArg(changeParm, i, x, y, page, mode);
        bpEvent(Npc, changeParm, trueindex, uiindex, NPCDataChgPOS.bp, mode);
    }
    public void init() { }
    public int AddItem(Item i, int page = 0)
    {
        throw new NotImplementedException();
    }

    public int AddItemAt(Item i, int x, int y, int page = 0)
    {
        if (page == 0) return 0;
        if (page == 1)
        {
            int[] xy = Npc.bp.GetItemLeftUp(x, y);
            int ans = Npc.bp.AddItemAt(i, x, y);
            SendItemEvent(Npc.bp.GetItemAt(x, y), xy[0], xy[1], 1, ObjInPageChangeMode.add);
            return ans;
        }
        if(page==100&&x<(Npc.part==null?0:Npc.part.Length))
        {
            int ans = Npc.part[x].SafeAdd(i);
            SendItemEvent(Npc.part[x], x, y, page, ObjInPageChangeMode.add);
            return ans;
        }
        if (page == 101 && x < (Npc.equip == null ? 0 : Npc.equip.Length))
        {
            int ans = Npc.equip[x].SafeAdd(i);
            SendItemEvent(Npc.equip[x], x, y, page, ObjInPageChangeMode.add);
            return ans;
        }
        if (page == 102 && x < (Npc.skin == null ? 0 : Npc.skin.Length))
        {
            int ans = Npc.skin[x].SafeAdd(i);
            SendItemEvent(Npc.skin[x], x, y, page, ObjInPageChangeMode.add);
            return ans;
        }
        if (page == 103 && x < (Npc.hand == null ? 0 : Npc.hand.Length))
        {
            int ans = Npc.hand[x].SafeAdd(i);
            SendItemEvent(Npc.hand[x], x, y, page, ObjInPageChangeMode.add);
            return ans;
        }
        return 0;
    }

    public int AddItemNumAt(int i, int x, int y, int page = 0)
    {
        if (page == 0) return 0;
        if (page == 1)
        {
            int[] xy = Npc.bp.GetItemLeftUp(x, y);
            int ans = Npc.bp.AddItemNumAt(i, x, y);
            SendItemEvent(Npc.bp.GetItemAt(x, y), xy[0], xy[1], 1, ObjInPageChangeMode.add);
            return ans;
        }
        if (page == 100 && x < (Npc.part == null ? 0 : Npc.part.Length))
        {
            int ans = Npc.part[x].SafeAdd(i, -1);
            SendItemEvent(Npc.part[x], x, y, page, ObjInPageChangeMode.add);
            return ans;
        }
        if (page == 101 && x < (Npc.equip == null ? 0 : Npc.equip.Length))
        {
            int ans = Npc.equip[x].SafeAdd(i,-1);
            SendItemEvent(Npc.equip[x], x, y, page, ObjInPageChangeMode.add);
            return ans;
        }
        if (page == 102 && x < (Npc.skin == null ? 0 : Npc.skin.Length))
        {
            int ans = Npc.skin[x].SafeAdd(i, -1);
            SendItemEvent(Npc.skin[x], x, y, page, ObjInPageChangeMode.add);
            return ans;
        }
        if (page == 103 && x < (Npc.hand == null ? 0 : Npc.hand.Length))
        {
            int ans = Npc.hand[x].SafeAdd(i, -1);
            SendItemEvent(Npc.hand[x], x, y, page, ObjInPageChangeMode.add);
            return ans;
        }
        return 0;
    }

    public bool CanPlaceAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (page == 0) return false;
        if (page == 1)
        { return Npc.bp.CanPlaceAt(item, x, y, page, mode); }
        if (page == 100 && x < (Npc.part == null ? 0 : Npc.part.Length))
        {
            if (Item.IsNullOrEmpty(Npc.part[x])) return true;
            else return Npc.part[x].Compare(item, mode);
        }
        if (page == 101 && x < (Npc.equip == null ? 0 : Npc.equip.Length))
        {
            if (Item.IsNullOrEmpty(Npc.equip[x])) return true;
            else return Npc.equip[x].Compare(item, mode);
        }
        if (page == 102 && x < (Npc.skin == null ? 0 : Npc.skin.Length))
        {
            if (Item.IsNullOrEmpty(Npc.skin[x])) return true;
            else return Npc.skin[x].Compare(item, mode);
        }
        if (page == 103 && x < (Npc.hand == null ? 0 : Npc.hand.Length))
        {
            if (Item.IsNullOrEmpty(Npc.hand[x])) return true;
            else return Npc.hand[x].Compare(item, mode);
        }
        return false;
    }

    public bool CanPlaceIgnoreCurrent(Item it, int x, int y, int page = 0)
    {
        if (page == 0) return false;
       if(page==1) return Npc.bp.CanPlaceIgnoreCurrent(it, x, y);
        if (page == 100 && x < (Npc.part == null ? 0 : Npc.part.Length))
        {
            return true;
        }
        if (page == 101 && x < (Npc.equip == null ? 0 : Npc.equip.Length))
        {
            return true;
        }
        if (page == 102 && x < (Npc.skin == null ? 0 : Npc.skin.Length))
        {
            return true;
        }
        if (page == 103 && x < (Npc.hand == null ? 0 : Npc.hand.Length))
        {
            return true;
        }
        return false;
    }

    public void ClearItems(int page = 0)
    {
        if (page == 0) return;
        if (page == 1) Npc.bp.ClearItems();
        if (page == 100)
        {
            Npc.part = new Item[NpcData.partSize];
        }
        if (page == 101)
        {
            Npc.equip = new Item[NpcData.equipSize];
        }
        if (page == 102 )
        {
            Npc.skin = new Item[NpcData.skinSize];
        }
        if (page == 103)
        {
            Npc.hand = new Item[NpcData.handSize];
        }
    }

    public int Contain(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (page == 0) return 0;
        if (page == 1) return Npc.bp.Contain(i, page, mode);
        Item[] temp=null;
        if (page == 100) temp = Npc.part; 
        if (page == 101) temp = Npc.equip;
        if (page == 102) temp = Npc.skin;
        if (page == 103) temp = Npc.hand;
        if (temp == null) return 0;
        int contain = 0;
        for (int j = 0; j < temp.Length; j++)
        {
            if (i.Compare(temp[j], mode)) contain += temp[j].num;
        }
        return contain;
    }

    public int ContainAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (page == 0||Item.IsNullOrEmpty(item)) return 0;
        if(page==1) return Npc.bp.ContainAt(item,x,y, page, mode);
        Item[] temp = null;
        if (page == 100) temp = Npc.part;
        if (page == 101) temp = Npc.equip;
        if (page == 102) temp = Npc.skin;
        if (page == 103) temp = Npc.hand;
        if (temp == null) return 0;
        if (item.Compare(temp[x])) return temp[x].num;
        return 0;
    }

    public int CountItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (page == 0) return 0;
        if (page == 1) return Npc.bp.CountItem(i, page, mode);
        Item[] temp = null;
        if (page == 100) temp = Npc.part;
        if (page == 101) temp = Npc.equip;
        if (page == 102) temp = Npc.skin;
        if (page == 103) temp = Npc.hand;
        if (temp == null) return 0;
        int count=0;
        for (int j = 0; j < temp.Length; j++)
        {
            if (temp[j]!=null&&temp[j].Compare(i))
            {
                count += temp[j].num;
            }
        }
        return count;
    }

    public bool DeleteItemAt(int x, int y, int page = 0)
    {
        if (page == 0) return false;
        if (page == 1)
        {
            int[] xy = Npc.bp.GetItemLeftUp(x, y);
            bool ans = Npc.bp.DeleteItemAt(x, y);
            //Debug.LogError("删除成功");
            SendItemEvent(Npc.bp.GetItemAt(x, y), xy[0], xy[1], 1, ObjInPageChangeMode.delete);
            return ans;
        }
        if (page == 100 && x < (Npc.part == null ? 0 : Npc.part.Length))
        {
            Npc.part[x] = Item.Empty;
            SendItemEvent(Npc.part[x], x, y, page, ObjInPageChangeMode.delete);
            return true;
        }
        if (page == 101 && x < (Npc.equip == null ? 0 : Npc.equip.Length))
        {
            Npc.equip[x] = Item.Empty;
            SendItemEvent(Npc.equip[x], x, y, page, ObjInPageChangeMode.delete);
            return true;
        }
        if (page == 102 && x < (Npc.skin == null ? 0 : Npc.skin.Length))
        {
            Npc.skin[x] = Item.Empty;
            SendItemEvent(Npc.skin[x], x, y, page, ObjInPageChangeMode.delete);
            return true;
        }
        if (page == 103 && x < (Npc.hand == null ? 0 : Npc.hand.Length))
        {
            Npc.hand[x] = Item.Empty;
            SendItemEvent(Npc.hand[x], x, y, page, ObjInPageChangeMode.delete);
            return true;
        }
        return false;

    }

    public Item[,] GetBigItems(int page = 0)
    {
        return null;
    }

    public Item GetItemAt(int x, int y, int page = 0)
    {
        if (page == 0) return Npc.defaultItems.GetItemAt(x,y);
        if (page == 1) return Npc.bp.GetItemAt(x, y);
        Item[] temp = null;
        if (page == 100) temp = Npc.part;
        if (page == 101) temp = Npc.equip;
        if (page == 102) temp = Npc.skin;
        if (page == 103) temp = Npc.hand;
        if (temp == null) return Item.Empty;
        return temp[x];
    }

    public int[] GetItemLeftUp(int x, int y, int page = 0)
    {
        if (page == 0) return Npc.defaultItems.GetItemLeftUp(x, y);
        if (page == 1) return Npc.bp.GetItemLeftUp(x, y);
        else return new int[] { x, y };
    }

    public Item[] GetItems(int page = 0)
    {
        if (page == 0) return Npc.defaultItems.items;
        if (page == 1) return Npc.bp.items;
        Item[] temp = null;
        if (page == 100) temp = Npc.part;
        if (page == 101) temp = Npc.equip;
        if (page == 102) temp = Npc.skin;
        if (page == 103) temp = Npc.hand;
        return temp;
    }

    public int[,] GetPlacements(int page = 0)
    {
        if (page == 0) return Npc.defaultItems.placement;
        if (page == 1) return Npc.bp.placement;
        else return null;
    }

    public bool IsBigChest()
    {
        return false;
    }

    public bool SetItemAt(Item i, int x, int y, int page = 0)
    {
        if (page == 0) return false;
        if (page == 1)
        {
            int[] xy = Npc.bp.GetItemLeftUp(x, y);
            bool ans = Npc.bp.SetItemAt(i, x, y);
            SendItemEvent(Npc.bp.GetItemAt(x, y), xy[0], xy[1], 1, ObjInPageChangeMode.set);
            return ans;
        }
        if (page == 100 && x < (Npc.part == null ? 0 : Npc.part.Length))
        {
            Npc.part[x] = i;
            SendItemEvent(Npc.part[x], x, y, page, ObjInPageChangeMode.set);
        }
        if (page == 101 && x < (Npc.equip == null ? 0 : Npc.equip.Length))
        {
            Npc.equip[x] = i;
            SendItemEvent(Npc.equip[x], x, y, page, ObjInPageChangeMode.set);
        }
        if (page == 102 && x < (Npc.skin == null ? 0 : Npc.skin.Length))
        {
            Npc.skin[x] = i;
            SendItemEvent(Npc.skin[x], x, y, page, ObjInPageChangeMode.set);
        }
        if (page == 103 && x < (Npc.hand == null ? 0 : Npc.hand.Length))
        {
            Npc.hand[x] = i;
            SendItemEvent(Npc.hand[x], x, y, page, ObjInPageChangeMode.set);
        }
        return true;
    }

    public int SetItemNumAt(int num, int x, int y, int page = 0)
    {
        if (page == 0) return 0;
        if (page == 1)
        {
            int[] xy = Npc.bp.GetItemLeftUp(x, y);
            int ans = Npc.bp.SetItemNumAt(num, x, y);
            SendItemEvent(Npc.bp.GetItemAt(x, y), xy[0], xy[1], 1, ObjInPageChangeMode.set);
            return ans;
        }
        if (page == 100 && x < (Npc.part == null ? 0 : Npc.part.Length))
        {
            Npc.part[x].SafeSet(num);
            SendItemEvent(Npc.part[x], x, y, page, ObjInPageChangeMode.set);
        }
        if (page == 101 && x < (Npc.equip == null ? 0 : Npc.equip.Length))
        {
            Npc.equip[x].SafeSet(num);
            SendItemEvent(Npc.equip[x], x, y, page, ObjInPageChangeMode.set);
        }
        if (page == 102 && x < (Npc.skin == null ? 0 : Npc.skin.Length))
        {
            Npc.skin[x].SafeSet(num);
            SendItemEvent(Npc.skin[x], x, y, page, ObjInPageChangeMode.set);
        }
        if (page == 103 && x < (Npc.hand == null ? 0 : Npc.hand.Length))
        {
            Npc.hand[x].SafeSet(num);
            SendItemEvent(Npc.hand[x], x, y, page, ObjInPageChangeMode.set);
        }
        return 0;
    }

    public void SetItems(Item[] items, int[,] placements, int page = 0)
    {
        if (page == 0)  Npc.defaultItems.SetItems(items,placements);
        if (page == 1) Npc.bp.SetItems(items, placements);

        if (page == 100 && items.Length!= Npc.part.Length) Npc.part=items;
        if (page == 101 && items.Length != Npc.equip.Length) Npc.equip = items;
        if (page == 102 && items.Length != Npc.skin.Length) Npc.skin = items;
        if (page == 103 && items.Length != Npc.hand.Length) Npc.hand = items;
    }

    public void SetSize(int x, int y, int page = 0)
    {
       
    }

    public int SubItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        throw new NotImplementedException();
    }

    public int SubItemAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum)
    {
        if (page == 0) return 0;
        if (page == 1)
        {
            int ans = Npc.bp.SubItemAt(item, x, y);
            SendItemEvent(Npc.bp.GetItemAt(x, y), x, y, 1, ObjInPageChangeMode.sub);
            return ans;
        }
        if (page == 100 && x < (Npc.part == null ? 0 : Npc.part.Length))
        {
            int ans = Npc.part[x].SafeSub(item.num);
            SendItemEvent(Npc.part[x], x, y, page, ObjInPageChangeMode.sub);
            return ans;
        }
        if (page == 101 && x < (Npc.equip == null ? 0 : Npc.equip.Length))
        {
            int ans = Npc.equip[x].SafeSub(item.num);
            SendItemEvent(Npc.equip[x], x, y, page, ObjInPageChangeMode.sub);
            return ans;
        }
        if (page == 102 && x < (Npc.skin == null ? 0 : Npc.skin.Length))
        {
            int ans = Npc.skin[x].SafeSub(item.num);
            SendItemEvent(Npc.skin[x], x, y, page, ObjInPageChangeMode.sub);
            return ans;
        }
        if (page == 103 && x < (Npc.hand == null ? 0 : Npc.hand.Length))
        {
            int ans = Npc.hand[x].SafeSub(item.num);
            SendItemEvent(Npc.hand[x], x, y, page, ObjInPageChangeMode.sub);
            return ans;
        }
        return 0;
    }
}