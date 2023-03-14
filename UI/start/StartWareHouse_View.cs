//仓库
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(StartWareHouse_controller))]
public class StartWareHouse_View : BaseUIView
{
    
   public ItemScrollView scrollView;//scrollview模板
    int currentPage = 0;

    public Transform scrollViewGroup;
    public ResourceShower resShower;
    public Button[] buttons;

    string path= "Prefabs/UI/inGame/unit/ItemScrollView";
    public override void UIInit(UICenter center)
    {
        base.UIInit(center);
        if (!active1) return;

        GameObject scrollview=Resources.Load<GameObject>(path);
        Debug.Log("warehouse init完成");
        /*ItemScrollView t=Instantiate(scrollview,scrollViewGroup.position,scrollViewGroup.rotation,scrollViewGroup).GetComponent<ItemScrollView>();
        t.UIInit(center,this);
        t.SetViewSize(13, 13);*/
        /*for (int i = 0; i < buttons.Length; i++)
        {
            //buttons[i].onClick.AddListener(delegate() { OnButtonClick(i); });
            
            temp.Add(t);
            
            t.Listen(onItemScrollSlotHit);
            
            //t.GetComponent<RectTransform>().position = scrollViewGroup.GetComponent<RectTransform>().position;
            t.gameObject.SetActive(false);
        }*/
        
        EventCenter.WorldCenter.ListenEvent(nameof(EventNames.LoadSaveDone),OnLoadSaveDone);
    }
    public override void OnUIOpen(int posi = 0)
    {
        base.OnUIOpen();

        BuildMVConnect(UIName, EventCenter.WorldCenter, null);
        controller.Flush();
        OnButtonClick(0);
        //获取下最新warehouse数据
    }
    public override void OnUIClose()
    {
        base.OnUIClose();

        BreakMVConnect(UIName, EventCenter.WorldCenter, null);
    }
    public override void BreakMVConnect(string viewname,EventCenter model, EventCenter target)
    {
        /*
        view自行向modelmvc发送unlisten（ondatachange）解除监听
        命令controller停止发送uichange事件
        */
        Debug.Log("BreakMVConnect");
        base.BreakMVConnect(viewname, model, target);
        //model.UnListenEvent<ItemPageChangeParm>("WareHouseItemChg",OnItemChg);//自己不再监听model的改变事件
    } 
    public override void BuildMVConnect(string viewname,EventCenter model, EventCenter target)
    {
        /*
        view监听model的ondatachange和breakconnect事件+
        调用model的buildmvconnect(controller)使其监听controller的onuichange事件
        */
        Debug.Log("BuildMVConnect");
        base.BuildMVConnect(viewname, model, target);
        /*for (int i = 0; i < scrollViews.Length; i++)//生成仓库itemscrollview
        {
            Item[] its = model.GetParm<int, Item[]>("GetWareHouse_Items", i);
            int[,] placs= model.GetParm<int, int[,]>("GetWareHouse_Placement", i);
            scrollViews[i].SetItems(its,placs);
            scrollViews[i].transform.localPosition = Vector3.zero;
        }
        scrollViews[0].gameObject.SetActive(true);
        model.ListenEvent<ItemPageChangeParm>(nameof(Container_PlayerData_EventNames.WareHouseItemChg),OnItemChg);
        */
    }

    void OnItemChg(ItemPageChangeParm parm)
    {
        if(currentPage==parm.page)
        scrollView.OnBPChange(parm);
    }
    void OnLoadSaveDone()
    {
        //BuildMVConnect(UIName, EventCenter.WorldCenter,null);
    }
    public override void OnButtonHit(string typ, int id)
    {
        base.OnButtonHit(typ, id);
        switch (typ)
        {
            case "WareHousePage":
                OnButtonClick(id);
                break;
            default:
                break;
        }
    }
    public void OnButtonClick(int index)
    {
        //Debug.Log("按下" + index);
        /*if(index<0||index>=scrollViews.Length)return;
        for (int i = 0; i < scrollViews.Length; i++)
        {
            scrollViews[i].gameObject.SetActive(false);
            
        }
        scrollViews[index].gameObject.SetActive(true);
        scrollViews[index].FlushPos();*/
        scrollView.SetPage(index);
        scrollView.Flush();
    }
   

    public override void OnQuitStartScene()
    {
        base.OnQuitStartScene();
        BreakMVConnect(UIName,EventCenter.WorldCenter,null);
        resShower.OnQuitStart();
        EventCenter.WorldCenter.UnListenEvent(nameof(EventNames.LoadSaveDone), OnLoadSaveDone);
    }
    public override void OnArriveStartScene()
    {
        base.OnArriveStartScene();
        if(EventCenter.WorldCenter.GetParm<bool>("IsInSave"))
        {
            BuildMVConnect(UIName, EventCenter.WorldCenter,null);
        }
    }

   
}
    