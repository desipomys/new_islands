using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBlock_Chest : BaseBackPack
{
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        center.ListenEvent<EventCenter, InteractType>("interact", OnInteract);
        center.ListenEvent("buildComplete", buildComplete);
    }
    void buildComplete()
    {
        for (int i = 0; i < itemPages.Count; i++)
        {
            itemPages[i].init();
        }
    }
    public void OnInteract(EventCenter source, InteractType typ)
    {
        //区分AI还是玩家
        bool safe = false;
        
        if (source.GetParm<bool>("isAI", out safe))
        {
            if (safe)
            {
                //是AI
                return;
            }
        }
        
        //是玩家打开UI
        UICenter.UIWorldCenter.ShowView("chest", "backpack");
        BaseUIView chest = UICenter.UIWorldCenter.GetView("chest");
        chest.BuildMVConnect(chest.UIName, center, source);
    }
    public override void FromObject(object data)
    {
       
        base.FromObject(data);

    }
    public override object ToObject()
    {
       
        return base.ToObject();
    }
}
