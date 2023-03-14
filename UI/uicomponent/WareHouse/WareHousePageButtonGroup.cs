using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WareHousePageButtonGroup : Base_UIComponent,IMVConnector
{
    public Button[] buttons;

    public void BreakMVConnect(string viewname, EventCenter model)
    {
        //throw new System.NotImplementedException();
        EventCenter.WorldCenter.UnListenEvent<int>(nameof(Container_PlayerData_EventNames.WareHousePageNumChg), OnWareHouseNumChg);

    }

    public void BuildMVConnect(string viewname, EventCenter model)
    {
        EventCenter.WorldCenter.ListenEvent<int>(nameof(Container_PlayerData_EventNames.WareHousePageNumChg), OnWareHouseNumChg);
        int temp=EventCenter.WorldCenter.GetParm<int>(nameof(Container_PlayerData_EventNames.GetWareHousePageNum));
        OnWareHouseNumChg(temp);
    }

    void OnWareHouseNumChg(int num)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i<num)
            {
                buttons[i].gameObject.SetActive(true);
            }
            else break;
        }
    }

    public UI_ModelType GetModelType()
    {
        return UI_ModelType.source;
    }

    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);

    }
    public void OnButtonHit(int i)
    {
        fatherView.OnButtonHit("WareHousePage", i);
    }

}
