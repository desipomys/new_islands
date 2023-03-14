using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate void OnUIValuechange<T>(string name, ValueChangeParm<T> value);
/// <summary>
/// 涉及写数据的就放到controller来执行，通过继承重写可以实现类似战争之人的箱子、背包每格item数量上限不同
/// </summary>
public class BaseUIController : MonoBehaviour, IEventRegister
{
   protected BaseUIView view;
    protected EventCenter model,target;
    protected IBackPack bp;
    public virtual void UIInit()
    {
        view = GetComponent<BaseUIView>();
    }
    
    public virtual void SetModel(EventCenter source,EventCenter target)
    {
        this.model = source;
        this.target = target;
        if (source == null) return;
        
    }
    public virtual EventCenter GetModel()
    {
        return model;
    }
    
    [System.Obsolete]
    public virtual void OnSlotClick(IBackPack bp,int state, int x, int y, int area)
    {
        //return;
        Item mouseitem = EventCenter.WorldCenter.GetParm<Item>("GetMouseItem");
        Item myitem = bp.GetItemAt(x, y, area);

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

                int[] posi = bp.GetItemLeftUp(x, y,area);
                Item temp = bp.GetItemAt(x, y, area);

                int[] size = temp.GetSize();
                int[] handsize = mouseitem.GetSize();
                if (handsize[0] > size[0] || handsize[1] > size[1])//被点击物品小于手持物品
                {
                    if (bp.CanPlaceIgnoreCurrent(mouseitem, x, y, area))
                    {
                        bp.DeleteItemAt(posi[0], posi[1], area);
                        bp.SetItemAt(mouseitem, x, y, area);
                        EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", temp);
                    }
                }
                else
                {
                    bp.DeleteItemAt(posi[0], posi[1], area);
                    bp.SetItemAt(mouseitem, posi[0], posi[1], area);
                    EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", temp);
                }
                break;
            case 7:
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
                            //MovScriptCommandResolver.Resolve2(commands[i], model, LoaderManager.GetLoader<Loader_MovEngine>().GetStartEngine());//默认上下文是controller的model和start engine
                        }
                }
                int res = bp.AddItemNumAt(1, x, y, area);
                mouseitem.num = mouseitem.num - 1;
                EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", mouseitem);
                break;
            default:
                break;
        }
        //Debug.Log("hh"+myitem.ToString());
        if (state == 2)//右键
        {
            if (Item.IsNullOrEmpty(mouseitem))//手空
            {
                if (Item.IsNullOrEmpty(myitem)) { }//格空
                else//格有
                {

                }
            }
            else//手有
            {
                if (Item.IsNullOrEmpty(myitem))//格空
                {

                }
                else//格有
                {

                    if (!myitem.Compare(mouseitem)) { }
                    else
                    {

                    }
                }
            }
        }
        else if (state == 0)//左键
        {
            if (Item.IsNullOrEmpty(mouseitem))//手空
            {
                if (Item.IsNullOrEmpty(myitem)) { }
                else//格子非空，交换
                {

                    // father.slotonchange(index, null);
                }
            }
            else//手有
            {
                if (Item.IsNullOrEmpty(myitem))//格空
                {

                }
                else//格有
                {

                    if (mouseitem.Compare(bp.GetItemAt(x, y, area)))
                    {

                    }
                    else//不同种物品则互换
                    {

                    }
                    //判断是否同一物品
                    //否则判断大小看能否交换
                }
            }
        }
        else if (state == 1)//作弊模式下鼠标中建是复制指向物品
        {
            if (EventCenter.WorldCenter.GetParm<bool>("IsCheatMode"))
            {
                if (!Item.IsNullOrEmpty(myitem))
                    EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", new Item(myitem));
            }
        }

    }
    [Obsolete]
    public virtual void OnSlotClick( int state, int x, int y, int area)//slot
    {
       
        if(model==null)return;
        OnSlotClick(bp, state, x, y, area);
        
    }
    
    public virtual void Flush()
    {

    }

    public virtual void OnUIOpen() { }
    public virtual void OnUIClose() { }

    /// <summary>
    /// 未测试
    /// 0手空格空左键，1手空格空右键，2手空格有左键，3手空格有右键
    /// 4手有格空左键，5手有格空右键，6手有格有不等左键，7手有格有不等右键，8手有格有等左键，9手有格有等右键
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="slotitem"></param>
    /// <param name="mouseitem"></param>
    /// <returns></returns>
    public virtual int GetHandAndSlotStat(int stats,Item slotitem,Item mouseitem)
    {
        int ans = 0;
        if (stats == 2) {ans += 1; }//右键
        if (stats == 0) {  }//左键
        if (!Item.IsNullOrEmpty(slotitem)) ans += 2;
        if (!Item.IsNullOrEmpty(mouseitem)) ans += 4;
        if (!Item.IsNullOrEmpty(slotitem)&&slotitem.Compare(mouseitem)) ans += 2;
        return ans;
    }

    public virtual void OnEventRegist(EventCenter e)
    {
        
    }

    public virtual void AfterEventRegist()
    {
        
    }
}
