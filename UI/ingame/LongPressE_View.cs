using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongPressE_View : BaseUIView
{
    //当选择可选界面时，先close自己，再将目标UI入栈
    //再发送玩家打开UI事件到UI 暂停玩家行动
    public override void OnUIOpen(int pos=0)
    {
        base.OnUIOpen();
        Debug.Log("E打开");
    }
    public override void OnUIClose()
    {
        base.OnUIClose();
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))//UI打开后再次按E关闭
        {
            uiCenter.CloseView();
            //uiCenter.SendEvent<string>(nameof(UIEventNames.OnPlayerCloseUI), "E");
        }
    }

    public void OnBackpackSelect()
    {
        uiCenter.CloseView();
        
        if(EventCenter.WorldCenter.GetParm<bool>(nameof(EventNames.IsCheatMode)))
        {
            uiCenter.ShowView("backpack", "CreaterBP");
        }
        else
        {
            uiCenter.ShowView("backpack");

        }
        //uiCenter.SendEvent<string>(nameof(UIEventNames.OnPlayerOpenUI), "backpack");
    }
    public void OnBuildingSelect()
    {
        uiCenter.CloseView();
        uiCenter.ShowView("building");
       
        //uiCenter.SendEvent<string>(nameof(UIEventNames.OnPlayerOpenUI), "building");
    }
    public void OnCommunicateSelect()
    {

    }
}
