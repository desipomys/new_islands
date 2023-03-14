using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICompCTRL_CraftDetail : Base_UIComp_Controll
{
    public void CraftPress()
    {
        if (model != null)
            model.SendEvent(nameof(CraftViewEventName.startCraft));
        else Debug.Log("model=null");
    }
    public void autoFill()
    {

    }
    public void OnSlide(Slider v)
    {
        if(model!=null)
       model.SendEvent<int>(nameof(CraftViewEventName.UISliderMove), (int)v.value);
    }
    
}
