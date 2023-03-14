using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public enum Movement_Stat
{
    none=0,
    idle=1,//站蹲趴三者互斥
    crouch=2,
    creep=4,

    walk=8,
    run=16,
    crouchWalk = 32,
    creepWalk = 64,

    sit =128,
    lying=256,
    alarm=512,
    fly=1024,
}

public enum EntityType:uint
{//0 000 0000 0000 0000 
 //是否现代  物种   类别
    modern=0,
    fantacy=0x80000000,
    //==================

    Human=0x01000000,
    Undead = 0x02000000,
    Bug =0x03000000,
    Slime = 0x04000000,
    Devil = 0x05000000,
    Fiary = 0x06000000,
    Machine =0x07000000,
    Animal=0x08000000,

    fantacyHuman=0x81000000,
    fantacyUndead=0x82000000,
    fantacyBug=0x83000000,
    fantacySlime=0x84000000,
    fantacyDevil=0x85000000,
    fantacyFiary=0x86000000,
    fantacyMachine=0x87000000,
    fantacyAnimal=0x88000000

    //================
}
public enum DefendType
{

}

/// <summary>
/// 所有生物的数据类
/// </summary>
[Serializable]
public class Charactor_Data 
{
    public string charName;
    public float health;
    public float food;
    public float power;
    public bool isTired;

    //存储值,ingame中不变，只能在菜单界面通过皮肤、配件来改变
    public EntityType type;
    public float maxHealth;
    public float maxFood;
    public float maxPower;

    public DefendType defType;
    public float def;//防御  *伤害计算：武器基础伤害
    public float speed;
    public float runSpeed;
    public float basePowerUpRate;//基础体力恢复速度,之后结算时需要加上技能加成和装备加成
    public float baseHealthUpRate;
    public PhyArmor armor;//密度，影响弹道性伤害的表现

    //public 阵营enum

    public void SetData(CharacterEventName nam,float value)
    {
        switch (nam)
        {
            case CharacterEventName.entity_type:
                type = (EntityType)((int)value);
                break;
            case CharacterEventName.entity_hp:
                health = value;
                break;
            case CharacterEventName.entity_food:food = value;
                break;
            case CharacterEventName.entity_pow:power = value;
                break;
            case CharacterEventName.entity_tired:
                break;
            case CharacterEventName.entity_maxHp:maxHealth = value;
                break;
            case CharacterEventName.entity_maxFood:maxFood = value;
                break;
            case CharacterEventName.entity_maxPow:maxPower = value;
                break;
            case CharacterEventName.entity_def:def = value;
                break;
            case CharacterEventName.entity_spd:speed = value;
                break;
            case CharacterEventName.entity_runspd:runSpeed = value;
                break;
            case CharacterEventName.entity_powUpRate:basePowerUpRate = value;
                break;
            case CharacterEventName.entity_hpUpRate:baseHealthUpRate = value;
                break;
            case CharacterEventName.armorH:armor.H = value;
                break;
            case CharacterEventName.armorTD:
                armor.TD = value;
                break;
            default:
                break;
        }
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
    public static Charactor_Data FromString(string data)
    {
        return JsonConvert.DeserializeObject<Charactor_Data>(data);
    }
}
[Serializable]
public class Charactor_Skill_Data 
{
    public short power;//力量,近程武器工具相关
    public short knowled;//智力，合成、建造等级相关
    public short agile;//敏捷、感知，远程武器相关
    public short physi;//体质，血量

    //武器相关技能等级
    public short bladeSkill;//+刀类伤害
    public short hammerSkill;//+钝器伤害
    public short bowSkill;//弓类伤害
    public short rifleSkill;//+步枪类伤害，包括手枪,弩
    public short autoSkill;//+机枪类伤害
    //public int snipSkill;//
    public short exploSkill;//炸药类

    public short fireSkill;//火焰类  物理魔法
    public short iceSkill;//冷冻类
    public short electroSkill;//电类
    public short radioSkill;//放射类
    public short ionSkill;//等离子类(被称为魔法)

    public short posionSkill;//毒类   生化学魔法
    public short corruptionSkill;//腐蚀类
    public short chaosSkill;//混沌
    public short necromanSkill;//死灵术
    public short healSkill;//治疗术

    public short buildSkill;


    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
    public static Charactor_Skill_Data FromString(string data)
    {
        return JsonConvert.DeserializeObject<Charactor_Skill_Data>(data);
    }
}