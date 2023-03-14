using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingListView : Base_UIComponent, IMVConnector
{
    public UI_ModelType type;

    public ItemShower crafting, done;
    public ItemSlot_big craftingSlot, doneSlot;
    public Slider process;

    Item cache = new Item();
    ItemSlotOnHItArg slotonhitarg = new ItemSlotOnHItArg();

    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);
        craftingSlot.ShowerInit(this);
        doneSlot.ShowerInit(this);
    }

    public void CraftStart(Craft_Data cd,int num)
    {
        if (cd == null)
        {
            crafting.ShowWithFixSize(Item.Empty);
            return;
        }
        cache.CopyFrom(cd.product);
        cache.num *= num;
        crafting.ShowWithFixSize(cache);
    }
    public void CraftProcessGo(float f)
    {
        process.value = f;
    }
    public void craftDone(Item i)
    {
        
        if (Item.IsNullOrEmpty(i)) {Debug.Log("收到产出空"); done.ShowWithFixSize(Item.Empty); craftingSlot.enabled=false ; }
        else {  Debug.Log("收到产出"+i.ToString());done.ShowWithFixSize(i); craftingSlot.enabled=true; }
    }

    public override void OnClick(int stat, int x, int y, int page)
    {
        base.OnClick(stat, x, y, page);

        slotonhitarg.x = x;
        slotonhitarg.y = y;
        slotonhitarg.key = stat;
        slotonhitarg.page = page;

        controll.OnEvent(UIComp_EventName.itemSlotClick, slotonhitarg);
    }

    public virtual void BreakMVConnect(string viewname, EventCenter model)
    {
        controll.BreakMVConnect(viewname, model);
        model.UnListenEvent<Craft_Data, int>(nameof(CraftViewEventName.CraftingStart), CraftStart);
        model.UnListenEvent<float>(nameof(CraftViewEventName.CraftProcessGo), CraftProcessGo);
        model.UnListenEvent<Item>(nameof(CraftViewEventName.craftDone), craftDone);
        //throw new System.NotImplementedException();
    }

    public virtual void BuildMVConnect(string viewname, EventCenter model)
    {
        controll.BuildMVConnect(viewname, model);
        model.ListenEvent<Craft_Data, int>(nameof(CraftViewEventName.CraftingStart), CraftStart);
        model.ListenEvent<float>(nameof(CraftViewEventName.CraftProcessGo), CraftProcessGo);
        model.ListenEvent<Item>(nameof(CraftViewEventName.craftDone), craftDone);
        //throw new System.NotImplementedException();


    }

    public UI_ModelType GetModelType()
    {
        return type;
        //throw new System.NotImplementedException();
    }
}
