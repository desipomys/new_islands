using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum CraftViewEventName
{
    uuidClick,typeClick,
    /// <summary>
    /// 合成配方改变
    /// </summary>
    CraftingUpdate,
    ProductUpdate, CraftProcessGo, CraftNumUpdate,clearUUID,
    /// <summary>
    /// 合成配方产品改变
    /// </summary>
    CraftingProductUpdate,
    /// <summary>
    /// 合成中的配方、数量改变
    /// </summary>
    CraftingStart//合成已开始
    ,startCraft,//启动合成
    UISliderMove//ui上的craftnum拉动
    ,cancleCraft//取消合成
    ,craftDone,
    /// <summary>
    /// 获取合成中占用的原料
    /// </summary>
        GetCraftingMat,
    /// <summary>
    /// 设置合成中占用的原料
    /// </summary>
    SetCraftingMat
        , GetDone//获取产出格
    ,SetDone//设置产出格
    ,isCrafting,
    GetCurrentCrafting
}

public class Craft_View : BaseUIView
{
    public RectTransform group;
    public override void UIInit(UICenter center)
    {
        base.UIInit(center);

    }
    /// <summary>
    /// 类别按钮点击事件
    /// </summary>
    /// <param name="id"></param>
    public override void OnButtonHit(int id)
    {
        base.OnButtonHit(id);
        center.SendEvent<ItemGroup>(nameof(CraftViewEventName.typeClick), (ItemGroup)id);
    }
    public override void BuildMVConnect(string viewname, EventCenter modelSource, EventCenter modelTarget)
    {
        base.BuildMVConnect(viewname, modelSource, modelTarget);
        modelSource.SendEvent("refresh");
    }
}
