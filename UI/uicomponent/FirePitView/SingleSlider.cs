using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleSlider : Base_UIComponent, IMVConnector
{
    Slider slider;
    public UI_ModelType type;
    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);
        slider = GetComponent<Slider>();
    }

    public void OnValueChg(float v,float max)
    {
        if (max == 0) slider.value = 0;
        else slider.value = v / max;
    }

    public void BreakMVConnect(string viewname, EventCenter model)
    {
        model.UnListenEvent<float, float>(UpdateEventName, OnValueChg);
    }

    public void BuildMVConnect(string viewname, EventCenter model)
    {
        model.ListenEvent<float, float>(UpdateEventName, OnValueChg);
        float[] fs = model.GetParm<float[]>(GetDataSourceName);
        OnValueChg(fs[0], fs[1]);
    }

    public UI_ModelType GetModelType()
    {
        return type;
    }
}
