using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum CraftViewEventName
{
    uuidClick,typeClick,
    /// <summary>
    /// �ϳ��䷽�ı�
    /// </summary>
    CraftingUpdate,
    ProductUpdate, CraftProcessGo, CraftNumUpdate,clearUUID,
    /// <summary>
    /// �ϳ��䷽��Ʒ�ı�
    /// </summary>
    CraftingProductUpdate,
    /// <summary>
    /// �ϳ��е��䷽�������ı�
    /// </summary>
    CraftingStart//�ϳ��ѿ�ʼ
    ,startCraft,//�����ϳ�
    UISliderMove//ui�ϵ�craftnum����
    ,cancleCraft//ȡ���ϳ�
    ,craftDone,
    /// <summary>
    /// ��ȡ�ϳ���ռ�õ�ԭ��
    /// </summary>
        GetCraftingMat,
    /// <summary>
    /// ���úϳ���ռ�õ�ԭ��
    /// </summary>
    SetCraftingMat
        , GetDone//��ȡ������
    ,SetDone//���ò�����
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
    /// ���ť����¼�
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
