using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 给hand这类玩家上需要存储数据的组件实现
/// 也可给存档时需要保存数据的组件实现
/// 应用场景：按名生成后需要用数据初始化的prefabs上的组件，与EntityDataCollector配合使用，比如玩家的backpack组件、实体的health组件
/// </summary>
public interface IDataContainer 
{
    int GetDataCollectPrio { get; }
    /// <summary>
    /// tostring()
    /// (过时)实体一般以string化的dic str,str 的形式返回数据
    /// </summary>
    /// <returns></returns>
    //string ToString();
    /// <summary>
    /// (过时)先反序列化为 string[]，再从数组按顺序取数据反序列化到变量
    /// </summary>
    /// <param name="data"></param>
    void FromObject(object data);
    //void DeSerailize(Dictionary<string,string> data);
    object ToObject();
}
