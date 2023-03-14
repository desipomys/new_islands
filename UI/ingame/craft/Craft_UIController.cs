using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craft_UIController : BaseUIController
{
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.ListenEvent<int>(nameof(CraftViewEventName.uuidClick), OnUUIDClick);
        e.ListenEvent(nameof(CraftViewEventName.clearUUID), ClearUUID);
        e.ListenEvent(nameof(CraftViewEventName.cancleCraft), CancelCrafting);
    }
    public void CancelCrafting()
    {
        Item[] ib = model.GetParm<Item[]>(nameof(CraftViewEventName.GetCraftingMat));
        
        for (int i = 0; i < ib.Length; i++)
        {
            
                if (!Item.IsNullOrEmpty(ib[ i]))
                {
                   
                    int ret = target.GetParm<Item, int>(nameof(PlayerEventName.giveItem), ib[ i]);//将itemscrollviewbig中的item返还玩家,背包满则掉落
                }
            
        }
        model.SendEvent<Item[]>(nameof(CraftViewEventName.SetCraftingMat),null);
        //ib.ClearItems();
    }
    public void ClearUUID()
    {
        Debug.Log("clearuuid" + target.gameObject.name);
        IBackPack ib = model.GetParm<IBackPack>("backpack");
        Item[,] itemPage = ib.GetBigItems();
        for (int i = 0; i < itemPage.GetLength(1); i++)
        {
            for (int j = 0; j < itemPage.GetLength(0); j++)
            {
                if (!Item.IsNullOrEmpty(itemPage[j, i]))
                {
                    Debug.Log(itemPage[j, i].id + ":" + j + ":" + i);
                    int ret = target.GetParm<Item, int>(nameof(PlayerEventName.giveItem), itemPage[j, i]);//将itemscrollviewbig中的item返还玩家,背包满则掉落
                }
            }
        }
        ib.ClearItems();
    }
    public void OnUUIDClick(int uuid)
    {
        Debug.Log("onuuidclick"+target.gameObject.name);
        IBackPack ib = model.GetParm<IBackPack>("backpack");
       Item[,] itemPage = ib.GetBigItems();
        if (itemPage == null || itemPage.GetLength(0) == 0 || itemPage.GetLength(1)==0)
        { Debug.Log("itempage=null"); return; }
        for (int i = 0; i < itemPage.GetLength(1); i++)
        {
            for (int j = 0; j < itemPage.GetLength(0); j++)
            {
                if (!Item.IsNullOrEmpty(itemPage[j, i]))
                {
                    Debug.Log("返还"+itemPage[j, i].id + ":" + j + ":" + i);
                    int ret = target.GetParm<Item, int>(nameof(PlayerEventName.giveItem), itemPage[j, i]);//将itemscrollviewbig中的item返还玩家,背包满则掉落
                }
            }
        }
        ib.ClearItems();
    }
}
