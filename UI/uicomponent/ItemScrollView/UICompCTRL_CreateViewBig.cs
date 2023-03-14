using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompCTRL_CreateViewBig : UICompCTRL_ItemScrollViewBig
{
    public override void OnSlotClick(int state, int x, int y, int area)
    {
        //��������Ϊ��ʱ������Ϊ�����Ʒ��num=1
        //��겻Ϊ��ʱ��Ч
        //�Ҽ������Ϊ��ʱ������Ϊ�����Ʒ��num=max
        //��겻Ϊ��ʱ��Ч
       
        Item mouseitem = EventCenter.WorldCenter.GetParm<Item>("GetMouseItem");
        Item myitem = ((CreateItems_View)(compView.fatherView)).GetItemAt(x, y);

        if (Item.IsNullOrEmpty(myitem)) { EventCenter.WorldCenter.SendEvent<Item>(nameof(EventNames.SetMouseItem),Item.Empty); }

        switch (GetHandAndSlotStat(state, myitem, mouseitem))
        {
            /// <summary>
            /// δ����
            /// 0�ֿո�������1�ֿո���Ҽ���2�ֿո��������3�ֿո����Ҽ�
            /// 4���и�������5���и���Ҽ���6���и��в��������7���и��в����Ҽ���8���и��е������9���и��е��Ҽ�
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
        if (state == 1)//����ģʽ������н��Ǹ���ָ����Ʒ
        {
            if (EventCenter.WorldCenter.GetParm<bool>("IsCheatMode"))
            {
                if (!Item.IsNullOrEmpty(myitem))
                    EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", new Item(myitem));
            }
        }
    }
}
