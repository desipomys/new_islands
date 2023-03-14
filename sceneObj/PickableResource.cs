using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableResource : MonoBehaviour, IEventRegister
{
    EventCenter center;
    public Item itemToGive;//给出的物品
    public bool levelRandom=false;
    public void OnEventRegist(EventCenter e)
    {
        center = e;

        center.ListenEvent<EventCenter, InteractType>("interact", OnInterect);
    }
    public void AfterEventRegist()
    {

    }

    public void OnInterect(EventCenter source, InteractType type)
    {
        if (levelRandom) itemToGive.level = Random.Range(0, 3072);
        source.GetParm<Item, int>(nameof(PlayerEventName.giveItem), itemToGive);
        EventCenter.WorldCenter.SendEvent<long>(nameof(EventNames.RemoveEBlock), center.UUID);//销毁自己
    }
}
