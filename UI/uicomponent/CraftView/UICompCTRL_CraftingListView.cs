using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompCTRL_CraftingListView : Base_UIComp_Controll
{

    public override void OnEvent(UIComp_EventName eventName, object parm)
    {
        base.OnEvent(eventName, parm);
        if (eventName == UIComp_EventName.itemSlotClick)
        {
            ItemSlotOnHItArg arg = (ItemSlotOnHItArg)parm;

            if (arg.page == 1)//合成格
            {
                if(arg.key==2)//右键
                {
                    //取消当前合成
                    Debug.Log("取消合成");
                    if (model.GetParm<bool>(nameof(CraftViewEventName.isCrafting)))
                    {
                        compView.fatherView.center.SendEvent(nameof(CraftViewEventName.cancleCraft));
                        model.SendEvent(nameof(CraftViewEventName.cancleCraft));
                    }
                }
            }
            else//产出格
            {
                //只有手空格有才有用
                Item mouseitem = EventCenter.WorldCenter.GetParm<Item>("GetMouseItem");
                Item done = model.GetParm<Item>(nameof(CraftViewEventName.GetDone));
                if(!Item.IsNullOrEmpty(done)&&Item.IsNullOrEmpty(mouseitem))
                {
                    EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", done);
                    model.SendEvent<Item>(nameof(CraftViewEventName.SetDone),Item.Empty);
                }

            }
        }
        
    }
     
}
