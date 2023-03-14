//主游戏界面，选择仓库还是NPC还是开战等
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class MainGame_View : BaseUIView
{
    public ResourceShower resShower;
    public Text gameDate, peoples;
    public Transform map,mapbuttonMask;
    //public MapSelectView mapSelect;

    int selectIslandIndex=0;

    public override void UIInit(UICenter center)
    {
        base.UIInit(center);


        //StartCoroutine(maskWait());
    }
    public override bool OnESC()
    {
       return base.OnESC();
    }

    public override void OnUIOpen(int posi = 0)
    {
        
        bool isAvliable=false;
        if (EventCenter.WorldCenter.GetParm<bool>(nameof(EventNames.IsInGame), out isAvliable))
        {
            if (isAvliable)//isingame=true
            {
                //显示加载存档进度条
                EventCenter.WorldCenter.SendEvent<string>(nameof(EventNames.JumpToScene), ConstantValue.ingameSceneName);
            }
        }
        else
        {
            base.OnUIOpen();
            //mapSelect.ListenOnGoTo(GoToOrEnterIsland);
            BuildMVConnect(UIName, EventCenter.WorldCenter,null);
            StartCoroutine(StartWait());
        }
    }
    IEnumerator StartWait()
    {
        yield return null;
        //SelectIsLand(0);
    }

    public override void OnUIClose()
    {
        base.OnUIClose();
        BreakMVConnect(UIName, EventCenter.WorldCenter,null);
    }
    public override void BreakMVConnect(string viewname,EventCenter model,EventCenter target)
    {
        /*
        view自行向modelmvc发送unlisten（ondatachange）解除监听
        命令controller停止发送uichange事件
        */
        //model.SendEvent("unlistenMVConnect");
        base.BreakMVConnect(viewname, model, target);
        //mapSelect.BreakMVConnect(viewname, model);
    } 
    public override void BuildMVConnect(string viewname,EventCenter model,EventCenter target)
    {
        /*
        view监听model的ondatachange和breakconnect事件+
        调用model的buildmvconnect(controller)使其监听controller的onuichange事件
        */
        //需监听peoplenum、游戏内时间改变事件
        base.BuildMVConnect(viewname, model, target);
        //mapSelect.BuildMVConnect(viewname, model);
    }
    //[Obsolete]
   public void LeaveSave()
    {
        EventCenter.WorldCenter.SendEvent("Save");
        EventCenter.WorldCenter.SendEvent("UnLoadSave");
        uiCenter.CloseCurrentView();
    }
    [Obsolete]
    public void GoToOrEnterIsland(string mapName)
    { 
        
        //如果点击的是船只所在点则登陆
        string shipatMap = EventCenter.WorldCenter.GetParm<string>(nameof(Container_PlayerData_EventNames.GetMapIndex));
        if (mapName == shipatMap)
        {

            EventCenter.WorldCenter.SendEvent<string>("JumpToScene", ConstantValue.ingameSceneName);
        }
        else
        {
            //船只移动到指向点，需根据图计算路径，扣除资源
            //先暂时禁止mapselectview的点击
            //然后开启全局协程，让船只一步一步向目标点移动
            //到达目的地后解除点击禁止
            EventCenter.WorldCenter.SendEvent<string>(nameof(Container_PlayerData_EventNames.MovShipIndex), mapName);
        }
       
    }
    public override void OnButtonHit(int id)
    {
        base.OnButtonHit(id);
        switch (id)
        {//0map,1warehouse2npc 3mulit
            //4setting 5quest 6mail 7 trade
            case 0:uiCenter.ShowView("map");
                break;
            case 1:
                uiCenter.ShowView("warehouse");
                break;
            case 2:
                uiCenter.ShowView("NPC");
                break;
            case 3:
                uiCenter.ShowView("map");
                break;
            case 4:
                uiCenter.ShowView("setting");
                break;
            case 5:
                uiCenter.ShowView("quest");
                break;
            case 6:
                uiCenter.ShowView("map");
                break;
            case 7:
                uiCenter.ShowView("map");
                break;
            default:
                break;
        }
    }

    public void SelectIsLand(int index)
    {
        //selectIslandIndex = index;
        //mapbuttonMask.position = map.GetChild(selectIslandIndex).position;
        //ContainerManager.GetContainer<Container_MapPrefabData>().SetSelected(selectIslandIndex);
    }
    public override void OnQuitStartScene()
    {
        base.OnQuitStartScene();
        BreakMVConnect(UIName, EventCenter.WorldCenter,null);
        resShower.OnQuitStart();
    }
}
    