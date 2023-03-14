using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 只适用于需要弹药的发射子弹的，不再往下抽象
/// </summary>
public class ToolEffector_FireBullet : ToolEffector
{
    /// 
    ///需提供： 子弹位置*固定几个enum
    ///   速度*excel+buff
    ///   射程*excel+buff
    ///   存在时间
    ///   子弹类型*enum，excel+buff
    ///   伤害单元*prefabs,数据来自excel+buff
    ///   表现单元
    ///   来源的tool-实时值
    ///   发射者uuid-实时值
    /// 
    // [LabelText("发射位置")]
    // public ValueSource_base bulletPos;
    // [LabelText("子弹速度")]
    // public ValueSource_base speed;
    // [LabelText("射程")]
    // public ValueSource_base range;
    // [LabelText("存在时间")]
    // public ValueSource_base time;
    // [LabelText("子弹类型")]
    // public ValueSource_base type;
    // [LabelText("伤害参数")]//返回fp[],只对应伤害值和类型
    // public ValueSource_base damageParm;

    /// <summary>
    /// 一般是个读取bulletexcel的效果器
    /// </summary>
    public ValueSource_base bulletValue;


    //public float Penatra;
    ToolCurrentAmmo currentAmmo = new ToolCurrentAmmo();

    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

   
    BulletParm parm = new BulletParm();
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        ToolEngine eng=(ToolEngine) buff;
        BaseTool tool=eng.baseTool;
        Hand hand=tool.hand;
        Item toolItem=tool.toolItem;
        Item ammoItem=currentAmmo.GetCurrentAmmo(toolItem);
        Vector3 firePos=hand.GetHoldingToolPosi(ToolPosition.silencePos);
        Vector3 targetPos=EventCenter.WorldCenter.GetParm<Vector3>(nameof(EventNames.GetMouseLookAt));
        /*
        bullet excel内容：
itemID	level	//主键，不纳入
名称	子弹prefabs	射程	速度	伤害	0-4
弹头数量	伤害类型	静息散率（10m）/m	单发散率增量	散率每秒减量	5-9
穿透力	支持的tag   10,11


        tool excel内容
itemID	level	//主键，不纳入
名称	预制名	模型名	弹夹容量	射速	0-4
伤害	射程	装弹时间	静息散率（10m）/m	单发散率增量	5-9
散率每秒减量	工具类型    10，11
弹种1	弹种2	弹种3	弹种4	弹种5	12-16		

        */
        /*
伤害：弹药配置+武器配置
伤害加成+：hand上的<key=dam+,float>buff修改
伤害加成%：hand上的<key=dam%,float>buff修改
暴击率：hand上的<key=crit,float>buff修改
暴击加成：hand上的<key=crit+%,float>buff修改
子弹预设：弹药配置或hand上的<key=bulletprefabs,str>buff修改
伤害类型：弹药配置或hand上的<key=tooldamtype,str>buff修改
工具类型：武器配置
表现单元：弹药配置
射程：弹药配置+武器配置
存在时间：弹药配置
速度：弹药配置
当前散率：basetool的dic<spread,fp>
散率形状：弹药配置+武器配置
发射位置：默认在模型“消音器位”
目标位置：默认“鼠标指向位置”
其他参数:弹药item的<tag,obj>+hand上的<key=tooldambuff,buffnamestr>buff
        */
        Dictionary<string,FP> bulletexcel=(Dictionary<string,FP>)(bulletValue.Get(caster,target[0],buff,new object[]{ammoItem}).data);
        Dictionary<string, FP> toolexcel =(Dictionary<string, FP>)(valueSource.Get(caster,target[0],buff,new object[]{toolItem}).data);

        int bulletCount=bulletexcel["弹头数量"];
        float damage=bulletexcel["伤害"].Convert<float>()+toolexcel["伤害"];

        
        float damAdd=0;
        float damPercent=0;
        float crit=0;
        float CritAdd=0;  

        FP temp=hand.GetBuffedValue(HandDataName.damAdd);
        if(temp!=null)damAdd=temp;
        temp=hand.GetBuffedValue(HandDataName.damPercent);
        if(temp!=null)damPercent=temp;
        temp=hand.GetBuffedValue(HandDataName.crit);
        if(temp!=null)crit=temp;
        temp=hand.GetBuffedValue(HandDataName.CritAdd);
        if(temp!=null)CritAdd=temp;

        string bulletprefab=bulletexcel["预制名"];
        DamageType dt=bulletexcel["伤害类型"].Convert<DamageType>();
        ToolType tt=toolexcel["工具类型"].Convert<ToolType>();
        PhyBullet pb=null;//phybullet的表现数据尚未确定
        float range=bulletexcel["射程"];
        float speed=bulletexcel["速度"].Convert<float>();
         float exitTime=range/speed;
        float spread=tool.GetData(BaseToolDataName.spread);

        Shape2D spreadShape=Shape2D.circle;  
        //Vector3 firepos=;
        //Vector3 targetpos;
        Dictionary<string,object> exd=new Dictionary<string, object>();
        Dictionary<ItemContent,object> bulletExd=ammoItem.exd;
        if(bulletExd!=null)
        foreach (var item in bulletExd)
        {
            exd.Add(item.Key.ToString(),item.Value);
        }
        temp=hand.GetBuffedValue(HandDataName.damBuff);
        if(temp!=null)
        {
            exd.Add(nameof(HandDataName.damBuff),temp);
        }


        parm.num=bulletCount;
        parm.dam.value=damage;
        parm.dam.AdditionV=damAdd;
        parm.dam.AdditionPercent=damPercent;
        parm.dam.CritPercent=crit;
        parm.dam.CritAdd=CritAdd;

        parm.dam.type=dt;
        parm.sourceUUID=caster.UUID;
        parm.tool=tool;
        
        parm.speed=speed;
        parm.range=range;
        parm.exittime=exitTime;
        parm.spreadRadius=spread;
        parm.shape=spreadShape;
        parm.type=bulletprefab;
        parm.parms=exd;
        
        parm.pos=firePos;
        parm.targetPos=targetPos;

        //发射子弹
        EventCenter.WorldCenter.SendEvent<BulletParm>(nameof(EventNames.FireBullet), parm);
    }
    


    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}

public class ToolCurrentAmmo
{
 
    public Item GetCurrentAmmo(Item toolItem)
    {
        object o = toolItem.GetContent(ItemContent.AmmoItemList);
        if (o != null)
        {
            Item[] ammos = (Item[])o;
            if (ammos.Length == 0) return Item.Empty;
            if (ammos.Length == 1)
            {
                return ammos[0];
            }
            int ind = (int)toolItem.GetContent(ItemContent.CurrentAmmoIndex);
            if (ind < 0 || ind >= ammos.Length) { Debug.LogError("index>ammo长度"); return Item.Empty; }
            return ammos[ind];
        }
        else return Item.Empty;
    }
}