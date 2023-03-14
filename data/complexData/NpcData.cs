using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����������NPCʵ��������ֻ࣬����Ϊʵ��Ľ�������
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
    public int level;//NPC���ݵȼ���ְҵ������charaterdataԤ��
    public NpcProfession profession;
    public Charactor_Data char_data=new Charactor_Data();
    public Charactor_Skill_Data char_skill=new Charactor_Skill_Data();
    //�ɼ�npcװ��
    public Item[] part=new Item[partSize];//page 100
    public Item[] equip=new Item[equipSize];//page 101
    public Item[] skin=new Item[skinSize];//page 102
    public Item[] hand = new Item[handSize];//page 103

    public ItemPage_Data bp = new ItemPage_Data(backpackHig,backpackWid);//page1
    public ItemPage_Data defaultItems=new ItemPage_Data(DefalutBackpackHig,backpackWid);//page0
    //����Ҫ����buff����

    public NpcData() { }
    public NpcData(string nam) { npcName = nam; }
    public NpcData(string nam,int lv) { npcName = nam; level = lv; }
    public NpcData(string nam, int lv,NpcProfession pro) { npcName = nam; level = lv; profession = pro; }
    //public NpcData(int w,int h) { bp = new ItemPage_Data( h,w); }

    public void WriteData(EventCenter npcGameObj)
    {
        //��NPCObj�л�ȡ����д���NPCdata
    }
}
public class NpcData_Warper
{
    public NpcData data;
    public string headImgName,BodyImgName,BodyModelName;//��ʾͼ��ģ����
    public NpcData_Warper() { }
    public NpcData_Warper(NpcData d) { data = d; }
}
public enum NpcProfession
{
    None,
    Carrier, Miner,//�ᶫ���� �� 
    Assaulter, LMG, HMG_1, HMG_2, HMG_3, //ͻ���� ���ǹ�� �ػ�ǹ����С��
    Scout, Sniper, shotguner, RPG, MORTAR_1, MORTAR_2,//���� �ѻ��� ��װ�� RPG �Ȼ��ڶ���С��
    engineer, Medics, flamdrower, //����ʦ  ҽ�Ʊ� ����

    //���ع�Ӷ����
    warrior, archer, witch, heavywarrior, holyknight, ninja, alchemch//սʿ����������ʦ,�ز�����ʥ����ߣ�����
}
