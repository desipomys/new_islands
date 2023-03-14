using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthAndPowView : Base_UIComponent,IMVConnector
{
    public UI_ModelType type;
    public Slider hp,pow,powmax;
    EventCenter player;

    public void BreakMVConnect(string viewname, EventCenter model)
    {
        player = null;
        model.UnListenEvent<ValueChangeParm<float>, EventCenter, BaseTool, Damage>(nameof(PlayerEventName.onHealthChangeBy), onHealthChange);
        model.UnListenEvent<ValueChangeParm<float>>(nameof(PlayerEventName.onPowerChange), onPowerChange);
    }

    public void BuildMVConnect(string viewname, EventCenter model)
    {
        player = model;
       hp.value= model.GetParm<float>(nameof(PlayerEventName.entity_hp)) / model.GetParm<float>(nameof(PlayerEventName.entity_maxHp));
        pow.value = model.GetParm<float>(nameof(PlayerEventName.entity_pow)) / model.GetParm<float>(nameof(PlayerEventName.entity_maxPow));
        model.ListenEvent<ValueChangeParm<float>, EventCenter, BaseTool, Damage>(nameof(PlayerEventName.onHealthChangeBy), onHealthChange);
        model.ListenEvent<ValueChangeParm<float>>(nameof(PlayerEventName.onPowerChange), onPowerChange);

        Vector3 pos = Camera.main.WorldToScreenPoint(player.transform.position);
        pow.transform.position = pos;
        powmax.transform.position = pos;
    }

    public UI_ModelType GetModelType()
    {
        return type;
    }

    void onHealthChange(ValueChangeParm<float> parm, EventCenter e, BaseTool b, Damage d)
    {
        hp.value = parm.now / parm.max;
    }
    void onPowerChange(ValueChangeParm<float> parm)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(player.transform.position);
        pow.transform.position = pos;
        powmax.transform.position = pos;
        pow.value = parm.now / parm.max;
    }
}
