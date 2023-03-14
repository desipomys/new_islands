//最开始界面
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class MainStart_View : BaseUIView
{
    public override void OnUIOpen(int posi = 0)
    {
        base.OnUIOpen();
        if(EventCenter.WorldCenter.GetParm<bool>("IsInSave"))//如果已在存档中则跳转maingameview
        {
            uiCenter.ShowView("menu");
        }
    }
    public override void OnButtonHit(int id)
    {
        base.OnButtonHit(id);
        switch (id)
        {
            case 0://新存档
                uiCenter.ShowView("NewSave");
                break;
            case 1://继续游戏
                uiCenter.ShowView("continue");
                break;
            case 2://设置
                uiCenter.ShowView("setting");
                break;
            case 3://退出
                break;
            default:
                break;
        }
    }
}
    