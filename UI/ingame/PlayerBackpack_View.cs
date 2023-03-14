using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBackpack_View : BaseUIView
{
    public ItemScrollView scrollView;
    int currentPage = 0;
    public override void UIInit(UICenter center)
    {
        base.UIInit(center);
        scrollView.UIInit(center,this);
        //scrollView.Listen(onItemScrollSlotHit);//不用监听了
        
    }

    public override void OnUIOpen(int posi = 0)
    {
        base.OnUIOpen(posi);
     
        BuildMVConnect( UIName,EventCenter.WorldCenter.GetParm<EventCenter>(nameof(EventNames.GetLocalPlayer)),null);
        scrollView.FlushPos();
        uiCenter.PausePlayer(true);
    }
    public override void OnUIClose()
    {
        base.OnUIClose();
        BreakMVConnect(UIName, EventCenter.WorldCenter.GetParm<EventCenter>(nameof(EventNames.GetLocalPlayer)),null);
        uiCenter.PausePlayer(false);
    }

    //关闭自己，发送玩家关闭所有UI事件
    public override bool OnESC()
    {
        //uiCenter.SendEvent<string>(nameof(UIEventNames.OnPlayerCloseUI), "backpack");
        return true;
        //Debug.Log("关闭背包");
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            uiCenter.CloseCurrentView();
        }
    }

    /*void OnItemChg(ItemPageChangeParm parm)
    {
        if(parm.page>=0&&parm.page==currentPage)
        scrollView.OnBPChange(parm);
    }*/

    public override void BuildMVConnect(string viewname, EventCenter modelSource,EventCenter modelTarget)
    {
        base.BuildMVConnect(viewname, modelSource, modelTarget);
        /*int bpSize = model.GetParm<int>("get_bpSize");//让组件自己初始化
        //for (int i = 0; i < scrollViews.Length; i++)//生成仓库itemscrollview
        {
            Item[] its = model.GetParm<int, Item[]>("Getbp_Items", 0);
            int[,] placs = model.GetParm<int, int[,]>("Getbp_Placement", 0);
            scrollView.SetItems(its, placs);
            //scrollViews[i].transform.localPosition = Vector3.zero;
        }*/
        //scrollView.gameObject.SetActive(true);
        //model.ListenEvent<ItemPageChangeParm>(nameof(PlayerEventName.bpChg), OnItemChg);//不在这里监听
        //controller.SetModel(modelSource,modelTarget);
    }
    /*public override void BreakMVConnect(string viewname, EventCenter model, EventCenter target)
    {
        //Debug.Log("BreakMVConnect");
        //controller.SetModel(null,null);//使controller不再发送事件
        //model.UnListenEvent<ItemPageChangeParm>(nameof(PlayerEventName.bpChg), OnItemChg);//自己不再监听model的改变事件
    }*/
}
