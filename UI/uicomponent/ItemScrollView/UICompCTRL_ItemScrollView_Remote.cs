using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompCTRL_ItemScrollView_Remote : Base_UIComp_Controll
{
    public BaseUIController target;
    public override void OnEvent(UIComp_EventName eventName, object parm)
    {
        if (target == null) { Debug.LogError("target为空");return; }
        base.OnEvent(eventName, parm);
        if (eventName == UIComp_EventName.itemSlotClick)
        {
            ItemSlotOnHItArg arg = (ItemSlotOnHItArg)parm;
            Debug.Log("aaa");
            target.OnSlotClick(arg.key, arg.x, arg.y, arg.page);
        }
    }

    public override EventCenter GetModel()
    {
        return target.GetModel();
    }
}
