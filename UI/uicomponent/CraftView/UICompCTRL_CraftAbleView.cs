using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompCTRL_CraftAbleView : UICompCTRL_ItemScrollViewBig
{
    //���itemʱ�������uuid�¼�,�����view����
    public override void OnEvent(UIComp_EventName eventName, object parm)
    {
        //base.OnEvent(eventName, parm);
        if (eventName == UIComp_EventName.itemSlotClick)
        {
            model.SendEvent<int>(nameof(CraftViewEventName.uuidClick), (int)parm);
            this.compView.fatherView.center.SendEvent<int>(nameof(CraftViewEventName.uuidClick), (int)parm);//��UIҳ��view����uuidclick�¼�
            
        }
        else if (eventName==UIComp_EventName.Clear)
        {
            this.compView.fatherView.center.SendEvent(nameof(CraftViewEventName.clearUUID));//��UIҳ��view����uuidclick�¼�
            model.SendEvent(nameof(CraftViewEventName.clearUUID));
        }
    }
}
