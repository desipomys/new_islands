using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在VIEW事件中心初始化时调用,晚于uiinit
/// </summary>
public interface IUIEventRegist
{
    void OnViewEventRegist(EventCenter e);
}
public interface IUIInitReciver 
{
    void UIInit(UICenter center,BaseUIView view);
}
public interface IUIDeInitReciver
{
    void UIDeInit();
}
public interface IUIOpenReciver
{
    void OnUIOpen(UICenter center, BaseUIView view);
}
public interface IUICloseReciver
{
    void OnUIClose(UICenter center, BaseUIView view);
}
public interface IMVConnector
{
    UI_ModelType GetModelType();
    void BuildMVConnect(string viewname,EventCenter model);
    void BreakMVConnect(string viewname,EventCenter model);
}

/// <summary>
/// UI组件的model是源、目标、世界的哪一种
/// </summary>
public enum UI_ModelType
{
    source,target,world
}