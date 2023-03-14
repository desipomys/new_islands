//包含玩家信息，资源数、玩家实体信息，仓库等
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

/// <summary>
/// 玩家资源数、仓库、可用NPC等信息
/// </summary>
public class PlayerData
{
    public static readonly int MaxSelected_npcNum = 5;
    public static readonly int pageWid = 19;
    public static readonly int pageHeight = 13;
    public ItemPage_Data[] wareHouse=new ItemPage_Data[1] {new ItemPage_Data(pageWid,pageHeight,true)};
    //public NpcData player=new NpcData(11,5);
    public Resource_Data resource=new Resource_Data();
    public Dictionary<int,NpcData> allNpc;
    public int[] selected_npc = new int[1] { -1 };
    public Item mouseItem=new Item();
    public string mapIndex;//当前在的小岛的index
    public DateTime time=new DateTime();//当前游戏中的时间
    public ResearchData research=new ResearchData();
    public int peopleNum = 100;

    public void AddSelectNPCSlot()
    {
        if (selected_npc == null) { selected_npc = new int[1]; }
        else if (selected_npc.Length <MaxSelected_npcNum)
        {
            List<int> temp = new List<int>(selected_npc);
            temp.Add(-1);
            selected_npc = temp.ToArray();
        }
    }
    public void AddItemPage()
    {
        List<ItemPage_Data> temp = new List<ItemPage_Data>(wareHouse);
        temp.Add(new ItemPage_Data(pageWid, pageHeight, true));
        wareHouse = temp.ToArray();
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this,Formatting.Indented);
    }
    public static PlayerData FromString(string data)
    {
        try
        {
return JsonConvert.DeserializeObject<PlayerData>(data);
        }
        catch (Exception)
        {
            
            return new PlayerData();
        }
        
    }
}
