using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftDetail : Base_UIComponent,IMVConnector
{
    [Tooltip("�ϳ��䷽����ı��¼�")]
    public string CraftProductUpdateName;//target�䷽����ı��¼�
    //[Tooltip("target���ںϳɵ�craftdata�ı��¼�")]
    //public string CraftingUpdateEventName;//target���ںϳɵ�craftdata�ı��¼�
    [Tooltip("target������ı��¼�")]
    public string ProductUpdateEventName;//target������ı��¼�
    [Tooltip("target��ǰ���ȸı��¼�")]
    public string CraftProcessGoEventName;//
    [Tooltip("target�ϳ�num�ı��¼�")]
    public string CraftNumUpdateEventName;
    public GameObject itemScrollViewBig;
    [Tooltip("�ϳɲ���Ԥ����shower")]
    public ItemShower product;

    [Tooltip("�ϳɲ���Ԥ����")]
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
        //model.ListenEvent<Craft_Data>(CraftingUpdateEventName, OnCraftingUpdate);//����target��ԭ�ϸ�仯���¼�
        controll.BuildMVConnect(viewname, model);
        model.ListenEvent<Item>(CraftProductUpdateName, OnCraftingProductUpdate);
        model.ListenEvent<Item[]>(ProductUpdateEventName, OnProductUpdate);
        model.ListenEvent<float>(CraftProcessGoEventName, OnCraftProcessGo);
        model.ListenEvent<int, int, int>(CraftNumUpdateEventName, OnCraftNumUpdate);

    }
    public virtual void BreakMVConnect(string viewname, EventCenter model)
    {
        //base.BreakMVConnect(viewname, model);
        //model.UnListenEvent<Craft_Data>(CraftingUpdateEventName, OnCraftingUpdate);//����target��ԭ�ϸ�仯���¼�
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
    /// ��eblock�����ı�ʱ���ö���������������ʱ����
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
