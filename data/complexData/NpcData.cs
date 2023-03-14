using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 并不是用于NPC实体的数据类，只能作为实体的建造数据
/// </summary>
public class NpcData
{
    public static readonly int skinSize = 3;
    public static readonly int partSize = 3;
    public static readonly int equipSize = 3;
    public static readonly int handSize = 3;
    public static readonly int backpackWid = 7;
    public static readonly int DefalutBackpackHig = 6;
    public static readonly int backpackHig = 5;
    
    public static readonly int partPage = 100;
    public static readonly int equipPage = 101;
    public static readonly int skinPage = 102;
    public static readonly int handPage = 103;

    public string npcName;
    public int level;//NPC根据等级和职业来生成charaterdata预设
    public NpcProfession profession;
    public Charactor_Data char_data=new Charactor_Data();
    public Charactor_Skill_Data char_skill=new Charactor_Skill_Data();
    //可加npc装备
    public Item[] part=new Item[partSize];//page 100
    public Item[] equip=new Item[equipSize];//page 101
    public Item[] skin=new Item[skinSize];//page 102
    public Item[] hand = new Item[handSize];//page 103

    public ItemPage_Data bp = new ItemPage_Data(backpackHig,backpackWid);//page1
    public ItemPage_Data defaultItems=new ItemPage_Data(DefalutBackpackHig,backpackWid);//page0
    //还需要加入buff数据

    public NpcData() { }
    public NpcData(string nam) { npcName = nam; }
    public NpcData(string nam,int lv) { npcName = nam; level = lv; }
    public NpcData(string nam, int lv,NpcProfession pro) { npcName = nam; level = lv; profession = pro; }
    //public NpcData(int w,int h) { bp = new ItemPage_Data( h,w); }

    public void WriteData(EventCenter npcGameObj)
    {
        //从NPCObj中获取数据写入此NPCdata
    }
}
public class NpcData_Warper
{
    public NpcData data;
    public string headImgName,BodyImgName,BodyModelName;//显示图像、模型名
    public NpcData_Warper() { }
    public NpcData_Warper(NpcData d) { data = d; }
}
public enum NpcProfession
{
    None,
    Carrier, Miner,//搬东西的 矿工 
    Assaulter, LMG, HMG_1, HMG_2, HMG_3, //突击兵 轻机枪手 重机枪三人小组
    Scout, Sniper, shotguner, RPG, MORTAR_1, MORTAR_2,//侦察兵 狙击手 重装兵 RPG 迫击炮二人小队
    engineer, Medics, flamdrower, //工程师  医疗兵 喷火兵

    //本地雇佣兵团
    warrior, archer, witch, heavywarrior, holyknight, ninja, alchemch//战士，弓兵，法师,重步兵，圣骑，忍者，炼金
}
