using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIComp_EventName
{
    itemSlotClick,Clear,
    ammoArrayClick
}

public class Base_UIComp_Controll: MonoBehaviour
{
    public EventCenter model;
    protected Base_UIComponent compView;
    protected UICenter uiCenter;
    public virtual void UIInit(UICenter center, BaseUIView view,Base_UIComponent comp)
    {
        compView = comp;
        uiCenter = center;
    }
    public virtual void OnEvent(UIComp_EventName eventName, object parm)
    {

    }
    public virtual void OnSlide(float oldValue, float newValue, int index, int page) { }
    public virtual void BuildMVConnect(string viewname, EventCenter model)
    {
        this.model = model;
    }
    public virtual void BreakMVConnect(string viewname, EventCenter model)
    {
        this.model = null;
    }
    public virtual EventCenter GetModel()
    {
        return model;
    }
}
