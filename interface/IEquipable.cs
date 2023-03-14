using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// tool,装备通用的接口
/// 
/// </summary>
public interface IEquipable 
{
void OnInit(EventCenter holder,Item item);//在loader初始化后执行
void OnEquip(EventCenter holder);
void UnEquip(EventCenter holder);
void OnAnyKey(EventCenter holder, KeyCodeKind code, Dictionary<KeyCode, KeyCodeStat> stat);
void OnUse(EventCenter holder,KeyCodeStat stat);
void OnItemChange(EventCenter holder, Item item);

void OnUpdate(EventCenter holder);
/// <summary>
/// 当持有者扣血等事件发生时
/// </summary>
/// <param name="holder"></param>
/// <param name="parms"></param>
void OnHolderEvent(EventCenter holder,PlayerEventName type,object[] parms);
/// <summary>
/// 当世界事件发生
/// </summary>
/// <param name="holder"></param>
/// <param name="type">改成可用的世界事件类型</param>
/// <param name="parms"></param>
void OnWorldEvent(EventCenter holder,PlayerEventName type,object[] parms);
/// <summary>
/// 当UI事件发生，由controller发出
/// </summary>
/// <param name="holder"></param>
/// <param name="type"></param>
/// <param name="parms"></param>
void OnUIEvent(EventCenter holder,PlayerEventName type,object[] parms);
}

public interface IEquipableEngine
{
    void OnInit(EventCenter holder);
    void StartRun(object[] par);
    void OnRun();
    /// <summary>
    /// 标记在所有事件下的toolengine都会接到此调用
    /// </summary>
    /// <param name="father"></param>
    void OnEquip(EventCenter holder, IEquipable father);
    /// <summary>
    /// 标记在所有事件下的toolengine都会接到此调用
    /// </summary>
    /// <param name="father"></param>
    void UnEquip(EventCenter holder,IEquipable father);
    /// <summary>
    /// 给effector调用
    /// </summary>
    void Next();
}
