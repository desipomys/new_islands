using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(BaseUIController))]
public class BaseUIView : MonoBehaviour,IEventRegister
{
    
    public bool active1=true;
    public float depth;
    public string UIName;
    [HideInInspector]
    public UICenter uiCenter;
    [HideInInspector]
    public BaseUIController controller;

    [HideInInspector]
    public EventCenter center;
    public EventCenter GetCenter { get{ if (center == null) return GetComponent<EventCenter>(); else return center; } private set { } }
    /// <summary>
    /// 晚于UIINIT
    /// </summary>
    /// <param name="e"></param>
    public virtual void OnEventRegist(EventCenter e)
    {
        if (!active1) return;
        //base.OnEventRegist(e);
        //e.ListenEvent<UICenter>("UIInit", UIInit);
        //e.ListenEvent("PostUIInit", PostUIInit);
        center =e;
        IEventRegister[] ies=GetComponentsInChildren<IEventRegister>();
        for (int i = 0; i < ies.Length; i++)
        {
            if (ies[i] == (IEventRegister)this) continue;
            ies[i].OnEventRegist(e);
        }

        IUIEventRegist[] allreciver = GetComponentsInChildren<IUIEventRegist>(true);//可能有些组件在uiinit时还没有active无法获取到
        for (int i = 0; i < allreciver.Length; i++)
        {
            allreciver[i].OnViewEventRegist(center);
        }
    }
    public virtual void AfterEventRegist()
    {
        if (!active1) return;
        IEventRegister[] ies=GetComponentsInChildren<IEventRegister>();
        for (int i = 0; i < ies.Length; i++)
        {
            if (ies[i] == (IEventRegister)this) continue;
            ies[i].AfterEventRegist();
        }
    }

    public virtual void UIInit(UICenter center)//不涉及数据的init
    {
        if (!active1) return;
        uiCenter = center;
        controller=GetComponent<BaseUIController>();
        initUIname();
        controller.UIInit();

        IUIInitReciver[] allreciver = GetComponentsInChildren<IUIInitReciver>(true);//可能有些组件在uiinit时还没有active无法获取到
        for (int i = 0; i < allreciver.Length; i++)
        {
            allreciver[i].UIInit(center,this);
        }
       
    }
    public virtual void PostUIInit()//涉及数据的init
    {
        
    }
    public virtual void UIDeInit()
    {
        if (!active1) return;
        IUIDeInitReciver[] allreciver = GetComponentsInChildren<IUIDeInitReciver>(true);//可能有些组件在uiinit时还没有active无法获取到
        for (int i = 0; i < allreciver.Length; i++)
        {
            allreciver[i].UIDeInit();
        }
    }
    public virtual void DoOpenAnim()
    {
        CanvasGroup c = GetComponent<CanvasGroup>();
        if(c!=null)
        {
            c.alpha = 0;
            DOTween.To(() => c.alpha, x => c.alpha = x, 1, 1);
        }
    }
    public virtual void OnUIOpen(int posi=0)
    {
        if (!active1) return;
        gameObject.SetActive(true);
        controller.OnUIOpen();
        IUIOpenReciver[] allreciver = GetComponentsInChildren<IUIOpenReciver>(true);//可能有些组件在uiinit时还没有active无法获取到
        for (int i = 0; i < allreciver.Length; i++)
        {
            allreciver[i].OnUIOpen(uiCenter, this);
        }
        DoOpenAnim();
    }
    public virtual void OnUIClose()
    {
        if (!active1) return;
        gameObject.SetActive(false);
        controller.OnUIClose();
        IUICloseReciver[] allreciver = GetComponentsInChildren<IUICloseReciver>(true);//可能有些组件在uiinit时还没有active无法获取到
        for (int i = 0; i < allreciver.Length; i++)
        {
            allreciver[i].OnUIClose(uiCenter, this);
        }
    }
   
    public virtual void OnButtonHit(int id)
    {

    }
    public virtual void OnButtonHit(string typ,int id)
    {

    }
    
    public virtual void CloseCurrentView()
    {
        uiCenter.CloseView();
    }
    /// <summary>
    /// 不要在esc关闭UI,true=UI在按下ESC会关闭
    /// </summary>
    public virtual bool OnESC()
    {
        return false;
    }
    public virtual void BuildMVConnect(string viewname,EventCenter modelSource,EventCenter modelTarget)//需改成源、目的双model
    {
        if (!active1) return;
        /*
        view监听model的ondatachange和breakconnect事件+
        设置controller的model
        调用model的buildmvconnect(controller)使其监听controller的onuichange事件
        */
        IMVConnector[] mVConnectors= GetComponentsInChildren<IMVConnector>(true);
        for (int i = 0; i < mVConnectors.Length; i++)
        {
            switch (mVConnectors[i].GetModelType())
            {
                case UI_ModelType.source:mVConnectors[i].BuildMVConnect(viewname, modelSource);
                    
                    break;
                case UI_ModelType.target:
                    mVConnectors[i].BuildMVConnect(viewname, modelTarget);
                    break;
                default:break;
            }
            
        }
        controller.SetModel(modelSource, modelTarget);
    }
    public virtual void BreakMVConnect(string viewname,EventCenter model,EventCenter target)
    {
        if (!active1) return;
        /*
        view自行向modelmvc发送unlisten（ondatachange）解除监听
        命令controller停止发送uichange事件
        */
        IMVConnector[] mVConnectors = GetComponentsInChildren<IMVConnector>(true);
        for (int i = 0; i < mVConnectors.Length; i++)
        {
            switch (mVConnectors[i].GetModelType())
            {
                case UI_ModelType.source:
                    mVConnectors[i].BreakMVConnect(viewname, model);
                    break;
                case UI_ModelType.target:
                    mVConnectors[i].BreakMVConnect(viewname, target);
                    break;
                default: break;
            }

        }
        controller.SetModel(null, null);
    }
    public virtual EventCenter GetPeerUIEVC()
    {
        return null;
    }
    public BaseUIController GetController()
    {
        if(controller==null)
        {
            controller=GetComponent<BaseUIController>();
            return controller;
        }
        else return controller;
    }
   
    public virtual void Flush()
    {
        /*
        重新获取数据显示
        */
    }
    /// <summary>
    /// 后于uiinit执行
    /// </summary>
    public virtual void OnArriveInGameScene()//
    {

    }
    public virtual void OnQuitInGameScene()
    {
        
    }
    /// <summary>
    /// 后于uiinit执行
    /// </summary>
    public virtual void OnArriveStartScene()//
    {

    }
    public virtual void OnQuitStartScene()
    {

    }

    void initUIname()
    {
        if(string.IsNullOrEmpty(UIName))
        {
            UIName = gameObject.name;
        }
    }

    
}
