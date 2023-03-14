using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompCTRL_CraftAbleView : UICompCTRL_ItemScrollViewBig
{
    //点击item时触发点击uuid事件,由组件view触发
    public override void OnEvent(UIComp_EventName eventName, object parm)
    {
        //base.OnEvent(eventName, parm);
        if (eventName == UIComp_EventName.itemSlotClick)
        {
            model.SendEvent<int>(nameof(CraftViewEventName.uuidClick), (int)parm);
            this.compView.fatherView.center.SendEvent<int>(nameof(CraftViewEventName.uuidClick), (int)parm);//向UI页面view发送uuidclick事件
            
        }
        else if (eventName==UIComp_EventName.Clear)
        {
            this.compView.fatherView.center.SendEvent(nameof(CraftViewEventName.clearUUID));//向UI页面view发送uuidclick事件
            model.SendEvent(nameof(CraftViewEventName.clearUUID));
        }
    }
}
