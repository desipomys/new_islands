using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractType
{
    open,
    AIopen
}
/// <summary>
/// 交互接口，被交互物品可能没有evc，所以直接用接口
/// 
/// </summary>
public interface IInterectable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="type"></param>
    /// <returns>返回0代表无效，1成功，2上拾取成功，3下拾取成功</returns>
    int OnInterect(EventCenter source, InteractType type);
    
}