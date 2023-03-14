using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropItemArea : MonoBehaviour
{
    public void OnPointerClick()
    {
        EventCenter playerevc = EventCenter.WorldCenter.GetParm<EventCenter>(nameof(EventNames.GetLocalPlayer));
        Item mouseitem= EventCenter.WorldCenter.GetParm<Item>(nameof(EventNames.GetMouseItem));
        playerevc.SendEvent<Item>(nameof(PlayerEventName.dropItem),mouseitem);//以玩家为中心丢出物体
        EventCenter.WorldCenter.SendEvent<Item>(nameof(EventNames.SetMouseItem), Item.Empty);
    }

   
}
