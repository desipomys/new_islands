using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompCTRL_ItemScrollViewBig : Base_UIComp_Controll
{
    protected IBackPack bp;
    public override void OnEvent(UIComp_EventName eventName, object parm)
    {
        base.OnEvent(eventName, parm);
        if (eventName == UIComp_EventName.itemSlotClick)
        {
            ItemSlotOnHItArg arg = (ItemSlotOnHItArg)parm;
            Debug.Log(arg);
            OnSlotClick(arg.key, arg.x, arg.y, arg.page);
        }
    }

    public virtual void OnSlotClick(int state, int x, int y, int area)
    {
        if (bp == null)
            bp = model.GetParm<IBackPack>("backpack");
        Item mouseitem = EventCenter.WorldCenter.GetParm<Item>("GetMouseItem");
        Item myitem = bp.GetItemAt(x, y, area);

        switch (GetHandAndSlotStat(state, myitem, mouseitem))
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                if (myitem.ContainContent(ItemContent.OnUILeftHit))//�������Ʒ�������Ӧ�ű�����
                {
                    string[] commands = (string[])myitem.GetContent(ItemContent.OnUILeftHit);
                    if (commands != null)
                        for (int i = 0; i < commands.Length; i++)
                        {
                            //MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//Ĭ����������controller��model��start engine
                        }
                }

                EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", myitem);
                bp.DeleteItemAt(x, y, area);
                break;
            case 3:
                if (myitem.ContainContent(ItemContent.OnUIRightHit))//�������Ʒ�������Ӧ�ű�����
                {
                    string[] commands = (string[])myitem.GetContent(ItemContent.OnUIRightHit);
                    if (commands != null)
                        for (int i = 0; i < commands.Length; i++)
                        {
                            //MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//Ĭ����������controller��model��start engine
                        }
                }
                if (myitem.num == 1)//����ֻ��һ��ֱ������
                {
                    EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", myitem);
                    bp.DeleteItemAt(x, y, area);
                }
                else//������һ��
                {
                    //Debug.Log("take half");
                    int half = myitem.num / 2;
                    Item temp1 = new Item(myitem);
                    temp1.num = half;
                    EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", temp1);
                    bp.SubItemAt(temp1, x, y, area);
                }
                break;
            case 4://�жϿռ乻����
                if (bp.CanPlaceAt(mouseitem, x, y, area))
                {
                    bp.SetItemAt(mouseitem, x, y, area);
                    EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", Item.Empty);
                }
                break;
            case 5:
                if (bp.CanPlaceAt(mouseitem, x, y, area))
                {
                    Item temp2 = new Item(mouseitem);
                    temp2.num = 1;
                    bp.AddItemAt(temp2, x, y, area);
                    int num1 = mouseitem.num;
                    mouseitem.num = num1 - 1;
                    EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", mouseitem);
                }
                break;
            case 6:
                if (myitem.ContainContent(ItemContent.OnUILeftHit))//�������Ʒ�������Ӧ�ű�����
                {
                    string[] commands = (string[])myitem.GetContent(ItemContent.OnUILeftHit);
                    if (commands != null)
                        for (int i = 0; i < commands.Length; i++)
                        {
                            //MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//Ĭ����������controller��model��start engine
                        }
                }

                if (myitem.ContainContent(ItemContent.OnUIExchange))//�������Ʒ�������Ӧ�ű�����
                {
                    string[] commands = (string[])myitem.GetContent(ItemContent.OnUIExchange);
                    if (commands != null)
                        for (int i = 0; i < commands.Length; i++)
                        {
                            //MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//Ĭ����������controller��model��start engine
                        }
                }

                Debug.Log(x + ":" + y + ":" + area);
                int[] posi = bp.GetItemLeftUp(x, y,area);
                Item temp = bp.GetItemAt(x, y, area);

                int[] size = temp.GetSize();
                int[] handsize = mouseitem.GetSize();
                if (handsize[0] > size[0] || handsize[1] > size[1])//�������ƷС���ֳ���Ʒ
                {
                    if (bp.CanPlaceIgnoreCurrent(mouseitem, x, y, area))
                    {
                        bp.DeleteItemAt(posi[0], posi[1], area);
                        bp.AddItemAt(mouseitem, x, y, area);
                        EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", temp);
                    }
                }
                else
                {
                    bp.DeleteItemAt(posi[0], posi[1], area);
                    bp.AddItemAt(mouseitem, posi[0], posi[1], area);
                    EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", temp);
                }
                break;
            case 7:
                if (myitem.ContainContent(ItemContent.OnUIRightHit))//�������Ʒ�������Ӧ�ű�����
                {
                    string[] commands = (string[])myitem.GetContent(ItemContent.OnUIRightHit);
                    if (commands != null)
                        for (int i = 0; i < commands.Length; i++)
                        {
                            //MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//Ĭ����������controller��model��start engine
                        }
                }
                break;
            case 8:
                if (myitem.ContainContent(ItemContent.OnUILeftHit))//�������Ʒ�������Ӧ�ű�����
                {
                    string[] commands = (string[])myitem.GetContent(ItemContent.OnUILeftHit);
                    if (commands != null)
                        for (int i = 0; i < commands.Length; i++)
                        {
                            //MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//Ĭ����������controller��model��start engine
                        }
                }

                int num = bp.AddItemAt(mouseitem, x, y, area);//����-1�����itemʧ�ܣ�����Ӱ��
                                                              //Debug.Log("safeadd" + num);
                if (num != -1)
                {
                    mouseitem.num = num;
                    EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", mouseitem);
                }
                break;
            case 9:
                if (myitem.ContainContent(ItemContent.OnUIRightHit))//�������Ʒ�������Ӧ�ű�����
                {
                    string[] commands = (string[])myitem.GetContent(ItemContent.OnUIRightHit);
                    if (commands != null)
                        for (int i = 0; i < commands.Length; i++)
                        {
                            //MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//Ĭ����������controller��model��start engine
                        }
                }
                Item tempmou = new Item(mouseitem);
                tempmou.num = 1;
                int res = bp.AddItemAt(tempmou, x, y, area);
                mouseitem.num = mouseitem.num - 1;
                EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", mouseitem);
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

    /// <summary>
    /// δ����
    /// 0�ֿո�������1�ֿո���Ҽ���2�ֿո��������3�ֿո����Ҽ�
    /// 4���и�������5���и���Ҽ���6���и��в��������7���и��в����Ҽ���8���и��е������9���и��е��Ҽ�
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="slotitem"></param>
    /// <param name="mouseitem"></param>
    /// <returns></returns>
    public virtual int GetHandAndSlotStat(int stats, Item slotitem, Item mouseitem)
    {
        int ans = 0;
        if (stats == 2) { ans += 1; }//�Ҽ�
        if (stats == 0) { }//���
        if (!Item.IsNullOrEmpty(slotitem)) ans += 2;
        if (!Item.IsNullOrEmpty(mouseitem)) ans += 4;
        if (!Item.IsNullOrEmpty(slotitem) && slotitem.Compare(mouseitem)) ans += 2;
        return ans;
    }
}
