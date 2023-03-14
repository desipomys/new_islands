using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSlot : Base_UIComponent,IMVConnector
{
    [Tooltip("只接受哪个page")]
    public int page;
    [Tooltip("只接受哪个x(wid)")]
    public int x;
    [Tooltip("只接受哪个y(hig)")]
    public int y;
    public UI_ModelType type;

    ItemShower shower;
    ItemSlot_big slot;
    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);
        shower = GetComponentInChildren<ItemShower>();
        slot = GetComponent<ItemSlot_big>();
        slot.ShowerInit(this);
    }
    ItemSlotOnHItArg slotonhitarg = new ItemSlotOnHItArg();
    public override void OnClick(int stat, int x, int y, int page)
    {
        base.OnClick(stat, this.x, this.y, page);
        slotonhitarg.x = this.x;
        slotonhitarg.y = this.y;
        slotonhitarg.key = stat;
        slotonhitarg.page = this.page;
        Debug.Log(slotonhitarg);

        controll.OnEvent(UIComp_EventName.itemSlotClick, slotonhitarg);
    }

    void Show(Item i)
    {
        shower.ShowWithFixSize(i);
        //shower.SetPosi(slot.GetComponent<RectTransform>());
    }
    public void OnBPChange(ItemPageChangeParm parm)
    {
        if (parm.page == page && parm.x == x && parm.y == y)
            Show(parm.item);
    }
    public void Flush()
    {
        IBackPack ib = controll.GetModel().GetParm<IBackPack>(GetDataSourceName);//刷新
        Show(ib.GetItemAt(x, y, page));
    }

    public UI_ModelType GetModelType()
    {
        return type;
    }

    public void BuildMVConnect(string viewname, EventCenter model)
    {
        controll.BuildMVConnect(viewname, model);
        model.ListenEvent<ItemPageChangeParm>(UpdateEventName, OnBPChange);
        model.ListenEvent(GetDataSourceName, Flush);
        Flush();
    }

    public void BreakMVConnect(string viewname, EventCenter model)
    {
        controll.BreakMVConnect(viewname, model);
        model.UnListenEvent<ItemPageChangeParm>(UpdateEventName, OnBPChange);
        model.UnListenEvent(GetDataSourceName, Flush);
    }
}
