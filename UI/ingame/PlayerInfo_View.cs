using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 手持物显示+血量体力显示
/// </summary>
[RequireComponent(typeof(BaseUIController))]
public class PlayerInfo_View : BaseUIView
{
  
    public override void UIInit(UICenter center)
    {
        base.UIInit(center);
        //InitHandSlots();
        //InitShower();
        //selectMask = transform.Find("selectMask");
        EventCenter.WorldCenter.ListenEvent(nameof(EventNames.LoadGameDone), OnLoadGameDone);
        //EventCenter.WorldCenter.ListenEvent(nameof(EventNames.), () => { BuildMVConnect(null, null); });
        center.ListenEvent<string>(nameof(UIEventNames.OnOpenUI), OnOpenUI);
        center.ListenEvent<string>(nameof(UIEventNames.OnCloseUI), OnCloseUI);
    }
    public override void UIDeInit()
    {
        base.UIDeInit();
        EventCenter.WorldCenter.UnListenEvent(nameof(EventNames.LoadGameDone), OnLoadGameDone);
        //EventCenter.WorldCenter.ListenEvent(nameof(EventNames.), () => { BuildMVConnect(null, null); });
        center.UnListenEvent<string>(nameof(UIEventNames.OnOpenUI), OnOpenUI);
        center.UnListenEvent<string>(nameof(UIEventNames.OnCloseUI), OnCloseUI);
    }
    void OnOpenUI(string uiname)
    {
        //if (uiname == "building") gameObject.SetActive(false);
    }
    void OnCloseUI(string uiname)
    {
        //if (uiname == "building") gameObject.SetActive(true);
    }
   void OnLoadGameDone()
    {
         BuildMVConnect(UIName, EventCenter.WorldCenter.GetParm<EventCenter>(nameof(EventNames.GetLocalPlayer)), null); 
    }
   

    public override void BuildMVConnect(string viewname, EventCenter model,EventCenter target)
    {
        model = EventCenter.WorldCenter.GetParm<EventCenter>(nameof(EventNames.GetLocalPlayer));
        base.BuildMVConnect(viewname, model,null);
        
    }
    public override void BreakMVConnect(string viewname, EventCenter model,EventCenter target)
    {
        model = EventCenter.WorldCenter.GetParm<EventCenter>(nameof(EventNames.GetLocalPlayer));
        base.BreakMVConnect(viewname, model,null);
        
    }
   

    
}
