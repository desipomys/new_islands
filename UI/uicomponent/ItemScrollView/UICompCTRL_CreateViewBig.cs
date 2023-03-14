using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompCTRL_CreateViewBig : UICompCTRL_ItemScrollViewBig
{
    public override void OnSlotClick(int state, int x, int y, int area)
    {
        //左键：鼠标为空时将手设为点击物品，num=1
        //鼠标不为空时无效
        //右键：鼠标为空时将手设为点击物品，num=max
        //鼠标不为空时无效
       
        Item mouseitem = EventCenter.WorldCenter.GetParm<Item>("GetMouseItem");
        Item myitem = ((CreateItems_View)(compView.fatherView)).GetItemAt(x, y);

        if (Item.IsNullOrEmpty(myitem)) { EventCenter.WorldCenter.SendEvent<Item>(nameof(EventNames.SetMouseItem),Item.Empty); }

        switch (GetHandAndSlotStat(state, myitem, mouseitem))
        {
            /// <summary>
            /// 未测试
            /// 0手空格空左键，1手空格空右键，2手空格有左键，3手空格有右键
            /// 4手有格空左键，5手有格空右键，6手有格有不等左键，7手有格有不等右键，8手有格有等左键，9手有格有等右键
            /// </summary>
            case 0:
            case 1:
                break;
            case 2:
                Item i = new Item(myitem);
                i.num = 1;
                EventCenter.WorldCenter.SendEvent<Item>(nameof(EventNames.SetMouseItem),i );
                break;
            case 3:
                Item i1 = new Item(myitem);
                i1.num = i1.GetMaxNum();
                EventCenter.WorldCenter.SendEvent<Item>(nameof(EventNames.SetMouseItem), i1);
                break;
            default:
                break;
        }
        //Debug.Log("hh"+myitem.ToString());
        if (state == 1)//作弊模式下鼠标中建是复制指向物品
        {
            if (EventCenter.WorldCenter.GetParm<bool>("IsCheatMode"))
            {
                if (!Item.IsNullOrEmpty(myitem))
                    EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", new Item(myitem));
            }
        }
    }
}
