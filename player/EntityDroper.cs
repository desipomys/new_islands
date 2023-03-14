using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDroper : MonoBehaviour,IEventRegister
{
    EventCenter center;
    public void AfterEventRegist()
    {
        
    }

    public void OnEventRegist(EventCenter e)
    {
        center = e;
        center.ListenEvent<Item>(nameof(PlayerEventName.dropItem), OnDropItem);
    }

   void OnDropItem(Item i)
    {
        Vector3 handpos = center.GetParm<Vector3>(nameof(PlayerEventName.getMainHandPos));
        EventCenter.WorldCenter.SendEvent<Item, Vector3, Vector3>(nameof(EventNames.DropItem), i, handpos, Vector3.zero);
    }
}
