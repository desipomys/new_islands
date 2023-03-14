using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerBuilder_View : BaseUIView
{//现在只能放置eblock
    public Transform GroupRoot;
    public BuildScrollView view;
    public bool InVirtualMode = false;

    GameObject sketchShadow;
    

    int currentTech = 0;
    int currentClass = 0;

    /// <summary>
    /// 当前选择的建筑方块的index
    /// </summary>
    int currentSelected =0;

    public override void UIInit(UICenter center)
    {
        base.UIInit(center);

        //view.UIInit(0);
        //view.Listen(onItemScrollSlotHit);
   
    }
    public override void OnUIOpen(int posi = 0)
    {
        
        BuildMVConnect(UIName, EventCenter.WorldCenter.GetParm<EventCenter>(nameof(EventNames.GetLocalPlayer)),EventCenter.WorldCenter);
        base.OnUIOpen(posi);
        uiCenter.PausePlayer(true);
        UpdateCurrent();
    }
    public override void OnUIClose()
    {
        base.OnUIClose();
        BreakMVConnect(UIName, EventCenter.WorldCenter.GetParm<EventCenter>(nameof(EventNames.GetLocalPlayer)), EventCenter.WorldCenter);
        InVirtualMode = false;
        uiCenter.PausePlayer(false);
        Debug.Log("关闭建筑面板");
    }

    /// <summary>
    /// 获取所有等级的eblock/bblock建筑生成UI按钮
    /// 建立连接时根据玩家建筑等级显示可用的建筑，锁定不可用的建筑
    /// 
    /// </summary>
    /// <param name="viewname"></param>
    /// <param name="model"></param>
    public override void BuildMVConnect(string viewname, EventCenter model, EventCenter target)
    {
        base.BuildMVConnect(viewname, model,target);
        
    }
    
   
    public override bool OnESC()
    {
        InVirtualMode = false;
        return true;
    }

    void UpdateCurrent()
    {
        //以当前T和类别取要显示的eblock或bblock
        if (currentClass == 2)//方块
        {

        }
        else    
        {//Debug.Log("等级与类别"+currentTech + ":" + currentClass);
           /* Entity_BlockModel[] ebs = EventCenter.WorldCenter.GetParm<int, int, Entity_BlockModel[]>(nameof(EventNames.GetEBlockByLvAndClass), currentTech, currentClass);
            Debug.Log(ebs.Length + "个");
            view.SetBuildings(ebs);*/
        }
    }
    
    public void SetScrollViewVisable(bool stat)
    {
        view.gameObject.SetActive(stat);
    }
   
    
}
