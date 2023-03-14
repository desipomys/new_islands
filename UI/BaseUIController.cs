using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate void OnUIValuechange<T>(string name, ValueChangeParm<T> value);
/// <summary>
/// �漰д���ݵľͷŵ�controller��ִ�У�ͨ���̳���д����ʵ������ս��֮�˵����ӡ�����ÿ��item�������޲�ͬ
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

                int[] posi = bp.GetItemLeftUp(x, y,area);
                Item temp = bp.GetItemAt(x, y, area);

                int[] size = temp.GetSize();
                int[] handsize = mouseitem.GetSize();
                if (handsize[0] > size[0] || handsize[1] > size[1])//�������ƷС���ֳ���Ʒ
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
                int res = bp.AddItemNumAt(1, x, y, area);
                mouseitem.num = mouseitem.num - 1;
                EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", mouseitem);
                break;
            default:
                break;
        }
        //Debug.Log("hh"+myitem.ToString());
        if (state == 2)//�Ҽ�
        {
            if (Item.IsNullOrEmpty(mouseitem))//�ֿ�
            {
                if (Item.IsNullOrEmpty(myitem)) { }//���
                else//����
                {

                }
            }
            else//����
            {
                if (Item.IsNullOrEmpty(myitem))//���
                {

                }
                else//����
                {

                    if (!myitem.Compare(mouseitem)) { }
                    else
                    {

                    }
                }
            }
        }
        else if (state == 0)//���
        {
            if (Item.IsNullOrEmpty(mouseitem))//�ֿ�
            {
                if (Item.IsNullOrEmpty(myitem)) { }
                else//���ӷǿգ�����
                {

                    // father.slotonchange(index, null);
                }
            }
            else//����
            {
                if (Item.IsNullOrEmpty(myitem))//���
                {

                }
                else//����
                {

                    if (mouseitem.Compare(bp.GetItemAt(x, y, area)))
                    {

                    }
                    else//��ͬ����Ʒ�򻥻�
                    {

                    }
                    //�ж��Ƿ�ͬһ��Ʒ
                    //�����жϴ�С���ܷ񽻻�
                }
            }
        }
        else if (state == 1)//����ģʽ������н��Ǹ���ָ����Ʒ
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
    /// δ����
    /// 0�ֿո�������1�ֿո���Ҽ���2�ֿո��������3�ֿո����Ҽ�
    /// 4���и�������5���и���Ҽ���6���и��в��������7���и��в����Ҽ���8���и��е������9���и��е��Ҽ�
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="slotitem"></param>
    /// <param name="mouseitem"></param>
    /// <returns></returns>
    public virtual int GetHandAndSlotStat(int stats,Item slotitem,Item mouseitem)
    {
        int ans = 0;
        if (stats == 2) {ans += 1; }//�Ҽ�
        if (stats == 0) {  }//���
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
