using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

//container负责管理生成后的物体，包括接收数据对现有物体进行更改
//不接收loaderinit事件

/// <summary>
/// container不要自己去读文件
/// </summary>
public class BaseContainer : IEventRegister
{
    protected EventCenter center;
    public virtual void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }
    //晚于loader初始化
    public virtual void OnEventRegist(EventCenter e)
    {
        center = e;
    }
    public virtual void OnUpdate()
    {
        
    }
    /// <summary>
    /// 从savedata加载数据
    /// </summary>
    /// <param name="save"></param>
   public virtual void OnLoadSave(SaveData save)//改名loadSaveDone
    {

    }
    public virtual void UnLoadSave()
    {

    }
    /// <summary>
    /// ingame的加载数据+初始化阶段
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    public virtual void OnLoadGame(MapPrefabsData data,int index)
    {

    }
    /// <summary>
    /// ingame的生成阶段
    /// </summary>
    /// <param name="index"></param>
    public virtual void OnBuildGame(int index)
    {

    }
    public virtual void UnLoadGame(int index)
    {

    }
    public virtual void OnArriveStartScene()
    {

    }
    public virtual void OnQuitStartScene()
    {

    }
    public virtual void OnArriveInGameScene()
    {

    }
    public virtual void OnQuitInGameScene()
    {

    }

    public virtual void Save(string path)
    {

    }

    /// <summary>
    /// 从字符串加载数据
    /// </summary>
    /// <param name="data"></param>
    public virtual void LoadData(string data)
    {

    }

}
