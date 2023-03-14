using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container_EventCenter : BaseContainer
{
   public Dictionary<long, EventCenter> UUID_EVC = new Dictionary<long, EventCenter>();
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
    }
    public override void OnLoadGame(MapPrefabsData data, int index)
    {
        base.OnLoadGame(data, index);
        if(index!=1)
        {
            center.RegistFunc<long, EventCenter>(nameof(EventNames.GetEVCbyUID), GetCenter);
        }
    }
    EventCenter GetCenter(long uuid)
    {
        return UUID_EVC[uuid];
    }
}
