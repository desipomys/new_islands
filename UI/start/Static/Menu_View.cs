using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_View : BaseUIView
{
    public Button resume;//1
    public Button cmd;//2
    public Button setting;//3
    public Button quit;//4结算并退出游戏

    public void OnUIClose(UICenter center, BaseUIView view)
    {
        
    }

    public void OnUIOpen(UICenter center, BaseUIView view)
    {
       
    }
    public override void OnButtonHit(int id)
    {
        base.OnButtonHit(id);
        switch (id)
        {
            case 1:uiCenter.CloseView();
            break;
            case 2:uiCenter.ShowView("shell");
            break;
            case 3:uiCenter.ShowView("setting");
            break;
            case 4:
            break;
            default:
            break;
        }
    }

    public void UIInit(UICenter center, BaseUIView view)
    {
      
    }
    public void Init(BaseUIView view)
    {
        
    }

   public void Close()
    {
       
    }
}
