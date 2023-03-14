using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_UIComponent : MonoBehaviour,IUIInitReciver,IUIOpenReciver,IUICloseReciver,IUIDeInitReciver,IUIEventRegist
{
    /// <summary>
    /// 调用叫什么名字的getparm可得到数据源
    /// </summary>
    public string GetDataSourceName,UpdateEventName;

    [HideInInspector]
    public Base_UIComp_Controll controll;
    [HideInInspector]
    public BaseUIView fatherView;

    public virtual void UIInit(UICenter center, BaseUIView view)
    {
        fatherView = view;
        controll = GetComponent<Base_UIComp_Controll>();
        if (controll == null) { Debug.Log("UI组件"+gameObject.name+"无controll"); return; }
        controll.UIInit(center, view,this);
    }
    public virtual void UIDeInit()
    {
        
    }

    public virtual void SetPage(int pag) { }

    public virtual void OnClick(int stat,int x,int y,int page) { }//由shower调用的
    public virtual void OnEvent(object parm) { }
    public virtual void OnEvent(object p1,object p2) { }
    public virtual void OnEvent(object p1, object p2,object p3) { }
    public virtual void OnSlide(float oldValue,float newValue,int index,int page) { }
    public virtual void OnTextChg(string str,int index,int page) { }
    public virtual void OnTextEndEdit(string str, int index, int page) { }

    public virtual void OnUIOpen(UICenter center, BaseUIView view)
    {
        
    }

    public virtual void OnUIClose(UICenter center, BaseUIView view)
    {
       
    }

    public virtual void OnViewEventRegist(EventCenter e)
    {
        
    }
}
