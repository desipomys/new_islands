using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScatteView_Big : Base_UIComponent,IMVConnector
{
    public RectTransform selectMask,showerGroup;
    public ItemSlot_big[] slots;
    public ItemShower[] showers;
    public int page;
    public UI_ModelType modelType;

    string path = "Prefabs/UI/inGame/unit/ItemShow";

    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);
        InitSlots();
        InitShower();
        
    }

    void onItemChange(ItemPageChangeParm parm)
    {
        if (parm.page != -1) return;//page-1是手的固定位置
        if (parm.x >= 0 && parm.x < showers.Length)
        {
            showers[parm.x].ShowWithFixSize(parm.item);
            showers[parm.x].SetCenterPosi( slots[parm.x].GetComponent<RectTransform>());
        }
    }
    void onSelectChg(int target, BaseTool bt)
    {
        StartCoroutine(synPosiWait(selectMask, slots[target].transform));
    }
    IEnumerator synPosiWait(Transform a, Transform t)
    {
        yield return new WaitForEndOfFrame();
        a.position = t.position;
    }
    IEnumerator synShowerPosiWait(ItemShower its,RectTransform tr)
    {
        yield return new WaitForEndOfFrame();
        its.SetCenterPosi(tr);
    }
    void InitShower()
    {
        GameObject Shower = Resources.Load<GameObject>(path);
        showers = new ItemShower[slots.Length];
        for (int i = 0; i < showers.Length; i++)
        {
            showers[i] = Instantiate(Shower, slots[i].transform.position, slots[i].transform.rotation, showerGroup).GetComponent<ItemShower>();
            showers[i].UIInit(true);
            showers[i].ShowWithFixSize(Item.Empty);
            showers[i].SetCenterPosi(slots[i].GetComponent<RectTransform>());
        }
    }
    void InitSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ShowerInit(this);
            slots[i].SetIndex(0,i, 0);
        }
    }

    ItemSlotOnHItArg slotonhitarg = new ItemSlotOnHItArg();
    public override void OnClick(int stat, int x, int y, int page)
    {
        base.OnClick(stat, x, y, page);
        slotonhitarg.x = x;
        slotonhitarg.y = y;
        slotonhitarg.page = this.page;
        slotonhitarg.key = stat;
        Debug.Log(slotonhitarg.ToString());

        controll.OnEvent(UIComp_EventName.itemSlotClick, slotonhitarg);
    }

    public void BreakMVConnect(string viewname, EventCenter model)
    {
        //throw new System.NotImplementedException();
        model.UnListenEvent<ItemPageChangeParm>(UpdateEventName, onItemChange);
        model.UnListenEvent<int, BaseTool>(nameof(PlayerEventName.onHandChgTool), onSelectChg);
        controll.BreakMVConnect(viewname, model);
    }

    public void BuildMVConnect(string viewname, EventCenter model)
    {
        //throw new System.NotImplementedException();
        model.ListenEvent<ItemPageChangeParm>(UpdateEventName, onItemChange);
        model.ListenEvent<int, BaseTool>(nameof(PlayerEventName.onHandChgTool), onSelectChg);

        Item[] its = model.GetParm<int, Item[]>("Getbp_Items", -1);
        int selecting = model.GetParm<int>(nameof(PlayerEventName.getCurrentHandHolding));
        for (int i = 0; i < its.Length; i++)
        {
            showers[i].ShowWithFixSize(its[i]);
            StartCoroutine(synShowerPosiWait(showers[i], slots[i].GetComponent<RectTransform>()));
            
        }
        StartCoroutine(synPosiWait(selectMask, slots[selecting].transform));
        controll.BuildMVConnect(viewname, model);
    }

    public UI_ModelType GetModelType()
    {
        return modelType;
        //throw new System.NotImplementedException();
    }
}
