using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
/*
七日杀合成分类：
基础
建筑方块
资源
武器/弹药
陷阱/工具
食物/烹饪
科学
服装
家具

MC物品分类：
方块、装饰、红石、运输、杂项、食物、工具、武器、药水

合成：
    基础
    建筑
    方块
    资源
    武器/弹药
    陷阱/工具
    食物/烹饪
    装备
    配件
    杂项
物品分类：
    资源
        基础
        矿物
        金属
        化工
    工具
        小刀
        斧
        镐
        铲
        其他
    武器
        古代兵器
        古代弹药
        现代武器
        现代弹药
    食物
        食物
        现代药物
        古代药物
    装备
        古代装备
        现代装备
    配件
        古代配件
        现代配件
    方块
        古代建筑
        现代建筑
        方块
    杂项
*/

[Serializable]
[CreateAssetMenu(menuName = "dataEditor/function/合成配方")]
public class Craft_Data:Base_Functioning_Data
{
    public Item[] ingredium;
    public Item product,extra;//extra会直接给到玩家背包中，如果没有玩家接收会直接掉地上
    public CraftReqToolType ReqToolType;

    public bool isRever;//是否可反向，反向则为分解
  
    public static bool IsNullOrEmpty(Craft_Data cd)
    {
        if (cd == null) return true;
        if (Item.IsNullOrEmpty(cd.product)) return true;
        return false;
    }
    
}
/// <summary>
/// 合成所需工具类型
/// </summary>
public enum CraftReqToolType
{
    none,
    woodCut,//木头加工，原木变木板、木棍
    hammer,//矿石、石头变粉
    wrench,//扳手，

    sandWheel_1,
    sandWheel_2,
    sandWheel_3,

    metalKnife_1,
    metalKnife_2,
    metalKnife_3,

    brunSet,
    electrSet,

    speedModule,//加速升级
    
}
