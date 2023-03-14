using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompCTRL_ItemScrollViewRequire : UICompCTRL_ItemScrollViewBig
{
    /// <summary>
    /// 未测试
    /// 0手空格空左键，1手空格空右键，2手空格有左键，3手空格有右键
    /// 4手有格空左键，5手有格空右键，6手有格有不等左键，7手有格有不等右键，8手有格有等左键，9手有格有等右键
    /// </summary>
    public override void OnSlotClick(int state, int x, int y, int area)
    {
        if (bp == null)
            bp = model.GetParm<IBackPack>("backpack");
        Item mouseitem = EventCenter.WorldCenter.GetParm<Item>("GetMouseItem");
        Item myitem = bp.GetItemAt(x, y, area);
        Item need = ((ItemScrollViewRequire)(this.compView)).GetNeedItem(x, y);

        switch (GetHandAndSlotStat(state, myitem, mouseitem))
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                if (myitem.ContainContent(ItemContent.OnUILeftHit))//被点击物品如果有响应脚本内容
                {
                    string[] commands = (string[])myitem.GetContent(ItemContent.OnUILeftHit);
                    if (commands != null)
                        for (int i = 0; i < commands.Length; i++)
                        {
                            //MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//默认上下文是controller的model和start engine
                        }
                }

                EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", myitem);
                bp.DeleteItemAt(x, y, area);
                break;
            case 3:
                if (myitem.ContainContent(ItemContent.OnUIRightHit))//被点击物品如果有响应脚本内容
                {
                    string[] commands = (string[])myitem.GetContent(ItemContent.OnUIRightHit);
                    if (commands != null)
                        for (int i = 0; i < commands.Length; i++)
                        {
                            //MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//默认上下文是controller的model和start engine
                        }
                }
                if (myitem.num == 1)//格中只有一个直接拿走
                {
                    EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", myitem);
                    bp.DeleteItemAt(x, y, area);
                }
                else//否则拿一半
                {
                    //Debug.Log("take half");
                    int half = myitem.num / 2;
                    Item temp1 = new Item(myitem);
                    temp1.num = half;
                    EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", temp1);
                    bp.SubItemAt(temp1, x, y, area);
                }
                break;
            case 4://判断空间够不够
                if (bp.CanPlaceAt(mouseitem, x, y, area))
                {
                    if (!Item.IsNullOrEmpty(need)&&need.Compare(mouseitem))
                    {
                        bp.SetItemAt(mouseitem, x, y, area);
                        EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", Item.Empty);
                    }
                }
                break;
            case 5:
                if (bp.CanPlaceAt(mouseitem, x, y, area))
                {
                    if (!Item.IsNullOrEmpty(need) && need.Compare(mouseitem))
                    {
                        Item temp2 = new Item(mouseitem);
                        temp2.num = 1;
                        bp.AddItemAt(temp2, x, y, area);
                        int num1 = mouseitem.num;
                        mouseitem.num = num1 - 1;
                        EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", mouseitem);
                    }
                }
                break;
            case 6:
                break;
                if (myitem.ContainContent(ItemContent.OnUILeftHit))//被点击物品如果有响应脚本内容
                {
                    string[] commands = (string[])myitem.GetContent(ItemContent.OnUILeftHit);
                    if (commands != null)
                        for (int i = 0; i < commands.Length; i++)
                        {
                            //MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//默认上下文是controller的model和start engine
                        }
                }

                if (myitem.ContainContent(ItemContent.OnUIExchange))//被点击物品如果有响应脚本内容
                {
                    string[] commands = (string[])myitem.GetContent(ItemContent.OnUIExchange);
                    if (commands != null)
                        for (int i = 0; i < commands.Length; i++)
                        {
                            //MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//默认上下文是controller的model和start engine
                        }
                }

                int[] posi = bp.GetItemLeftUp(x, y);
                Item temp = bp.GetItemAt(x, y, area);

                int[] size = temp.GetSize();
                int[] handsize = mouseitem.GetSize();
                if (handsize[0] > size[0] || handsize[1] > size[1])//被点击物品小于手持物品
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
                break;
                if (myitem.ContainContent(ItemContent.OnUIRightHit))//被点击物品如果有响应脚本内容
                {
                    string[] commands = (string[])myitem.GetContent(ItemContent.OnUIRightHit);
                    if (commands != null)
                        for (int i = 0; i < commands.Length; i++)
                        {
                            //MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//默认上下文是controller的model和start engine
                        }
                }
                break;
            case 8:
                if (myitem.ContainContent(ItemContent.OnUILeftHit))//被点击物品如果有响应脚本内容
                {
                    string[] commands = (string[])myitem.GetContent(ItemContent.OnUILeftHit);
                    if (commands != null)
                        for (int i = 0; i < commands.Length; i++)
                        {
                            //MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//默认上下文是controller的model和start engine
                        }
                }

                int num = bp.AddItemAt(mouseitem, x, y, area);//返回-1代表加item失败，且无影响
                                                              //Debug.Log("safeadd" + num);
                if (num != -1)
                {
                    mouseitem.num = num;
                    EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", mouseitem);
                }
                break;
            case 9:
                if (myitem.ContainContent(ItemContent.OnUIRightHit))//被点击物品如果有响应脚本内容
                {
                    string[] commands = (string[])myitem.GetContent(ItemContent.OnUIRightHit);
                    if (commands != null)
                        for (int i = 0; i < commands.Length; i++)
                        {
                           // MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//默认上下文是controller的model和start engine
                        }
                }
                Item tempmou = new Item(mouseitem);
                tempmou.num = 1;
                int res = bp.AddItemAt(tempmou, x, y, area);
                //int res = bp.AddItemNumAt(1, x, y, area);
                mouseitem.num = mouseitem.num - 1;
                EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", mouseitem);
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
