using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftDetail : Base_UIComponent,IMVConnector
{
    [Tooltip("合成配方产物改变事件")]
    public string CraftProductUpdateName;//target配方产物改变事件
    //[Tooltip("target正在合成的craftdata改变事件")]
    //public string CraftingUpdateEventName;//target正在合成的craftdata改变事件
    [Tooltip("target产出格改变事件")]
    public string ProductUpdateEventName;//target产出格改变事件
    [Tooltip("target当前进度改变事件")]
    public string CraftProcessGoEventName;//
    [Tooltip("target合成num改变事件")]
    public string CraftNumUpdateEventName;
    public GameObject itemScrollViewBig;
    [Tooltip("合成产物预览格shower")]
    public ItemShower product;

    [Tooltip("合成产物预览格")]
    public ItemSlot_big productSlot; 

    public Slider craftNum;
    public Text minNum, maxNum;

    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);
        //self = itemScrollViewBig;
        
        //view.center.ListenEvent<int>(nameof(CraftViewEventName.uuidClick), OnUUIDClick);
    }
    public virtual void BuildMVConnect(string viewname, EventCenter model)
    {
        //base.BuildMVConnect(viewname, model);
        //model.ListenEvent<Craft_Data>(CraftingUpdateEventName, OnCraftingUpdate);//监听target的原料格变化、事件
        controll.BuildMVConnect(viewname, model);
        model.ListenEvent<Item>(CraftProductUpdateName, OnCraftingProductUpdate);
        model.ListenEvent<Item[]>(ProductUpdateEventName, OnProductUpdate);
        model.ListenEvent<float>(CraftProcessGoEventName, OnCraftProcessGo);
        model.ListenEvent<int, int, int>(CraftNumUpdateEventName, OnCraftNumUpdate);

    }
    public virtual void BreakMVConnect(string viewname, EventCenter model)
    {
        //base.BreakMVConnect(viewname, model);
        //model.UnListenEvent<Craft_Data>(CraftingUpdateEventName, OnCraftingUpdate);//监听target的原料格变化、事件
        controll.BreakMVConnect(viewname, model);
        model.UnListenEvent<Item>(CraftProductUpdateName, OnCraftingProductUpdate);
        model.UnListenEvent<Item[]>(ProductUpdateEventName, OnProductUpdate);
        model.UnListenEvent<float>(CraftProcessGoEventName, OnCraftProcessGo);
        model.UnListenEvent<int, int, int>(CraftNumUpdateEventName, OnCraftNumUpdate);
    }
    
    public void OnCraftingProductUpdate(Item i)
    {
        product.ShowWithFixSize(i);
        product.SetPosi(productSlot.GetComponent<RectTransform>());
    }
   
    public void OnProductUpdate(Item[] its)
    {

    }
    public void OnCraftProcessGo(float f)
    {

    }
    /// <summary>
    /// 在eblock存栏改变时调用而不是在拉动滑块时调用
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="now"></param>
    public void OnCraftNumUpdate(int min,int max,int now)
    {
        craftNum.maxValue = max;
        craftNum.minValue = min;
        craftNum.value = now;

        minNum.text = min.ToString();
        maxNum.text = max.ToString();
    }
    public UI_ModelType modleType;
    public UI_ModelType GetModelType()
    {
        return modleType;
    }
}
