//当前存档主信息
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Sirenix.OdinInspector;

/// <summary>
/// 包括难度等的存档创建设置，运行时很少有改动的需求
/// </summary>
public class SaveSetting
{
    //在教程之前设置
    [LabelText("跳过教程")]
    public bool jumpTutorial;


    //在教程之后设置
    /// <summary>
    /// 难度
    /// </summary>
    [Range(0,3)]
    [LabelText("难度")]
    public int hard;

    [LabelText("科技树难度")]
    public int TechHard;

    [Range(0,5)]
    [LabelText("初始集装箱物资数")]
    public int InitCageNum;
    

//可能应该放在mapprefabs中
    #region 矿物丰度
/// <summary>
    /// 魔力丰度
    /// </summary>
    [Range(0,1f)]
    [LabelText("魔力丰度")]
    public float ManaRich;

    [Range(0,5f)]
    [LabelText("铁丰度")]
    public float ironRich;
    [Range(0,5f)]
    [LabelText("铜丰度")]
    public float copperRich;
    [Range(0,5f)]
    [LabelText("铅丰度")]
    public float leadRich;
    [Range(0,5f)]
    [LabelText("金丰度")]
    public float goldRich;
    [Range(0,5f)]
    [LabelText("原油丰度")]
    public float oilRich;

    [Range(0,5f)]
    [LabelText("树木丰度")]
    public float woodRich;

    #endregion
}

public class Container_SaveData : BaseContainer
{
    SaveSetting setting;
    SaveData data;
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        //不要在init时就加载playerdata，只有选择进入了某个存档后才开始加载

        center.ListenEvent<float>("addPlayedTime", addPlayedTime);
        center.ListenEvent<System.DateTime>("chgLastTime", this.chgLastTime);
    }

    public override void UnLoadSave()//界面切换造成的unload
    {
        base.UnLoadSave();

        data = null;
        center.ForceUnRegistFunc<bool>(nameof(EventNames.IsInSave));
        center.ForceUnRegistFunc<string>(nameof(EventNames.ThisSaveName));
        center.ForceUnRegistFunc<string>(nameof(EventNames.ThisSavePath));
        center.ForceUnRegistFunc<System.DateTime>("SaveCreateTime");
        center.ForceUnRegistFunc<double>("SavePlayedTime");
        center.ForceUnRegistFunc<bool>(nameof(EventNames.IsInGame));
        center.ForceUnRegistFunc<bool>(nameof(EventNames.IsCheatMode));
        center.ForceUnRegistFunc<int>("RemainPeople");
        center.ForceUnRegistFunc<SaveData>(nameof(EventNames.GetCurrentSaveData));
        center.ForceUnListenEvent(nameof(EventNames.SetCheat));
        center.ForceUnListenEvent(nameof(EventNames.SetSaveSetting));
        center.ForceUnRegistFunc<SaveSetting>(nameof(EventNames.GetSaveSetting));
        
        
        Debug.Log("savedata清理完成");

    }
    public override void OnLoadSave(SaveData save)
    {
        data = save;
        LoadSetting(data.savePath);
        center.RegistFunc<bool>(nameof(EventNames.IsInSave), () => { return data != null; });
        center.RegistFunc<string>(nameof(EventNames.ThisSaveName), () => { return data.saveName; });
        center.RegistFunc<string>(nameof(EventNames.ThisSavePath), () => { return data.savePath; });
        center.RegistFunc<System.DateTime>("SaveCreateTime", () => { return data.createTime; });
        center.RegistFunc<double>("SavePlayedTime", () => { return data.playedTime; });
        center.RegistFunc<bool>(nameof(EventNames.IsInGame), () => { return data.isInGame; });
        center.RegistFunc<bool>(nameof(EventNames.IsCheatMode), () => { return data.IsCheatMode; });
        center.RegistFunc<int>("RemainPeople", () => { return data.remainPeople; });
        center.RegistFunc<SaveData>(nameof(EventNames.GetCurrentSaveData), () => { return data; });
        center.ListenEvent<bool>(nameof(EventNames.SetCheat), (bool b) => { data.IsCheatMode = b; });
        center.ListenEvent<SaveSetting>(nameof(EventNames.SetSaveSetting),(SaveSetting s)=>{setting=s;});
        center.RegistFunc<SaveSetting>(nameof(EventNames.GetSaveSetting),()=>{return setting;});
        Debug.Log("savedata加载完成");
    }
    void LoadSetting(string savePath)
    {
       string s= FileSaver.GetSaveSetting(savePath);
       if(string.IsNullOrEmpty(s))
       {
        setting=new SaveSetting();
       }
       else
       setting=JsonConvert.DeserializeObject<SaveSetting>(s,JsonSetting.serializerSettings);
    }
    public override void Save(string path)
    {
        base.Save(path);
        data.shipAt = ContainerManager.GetContainer<Container_PlayerData>().GetMapIndex();
        FileSaver.SaveSaveData(path, data.ToString());
        FileSaver.SetSaveSetting(path,JsonConvert.SerializeObject(setting,JsonSetting.serializerSettings));
    }
    public override void OnArriveInGameScene()
    {
        base.OnArriveInGameScene();

    }
    public override void OnQuitInGameScene()
    {
        base.OnQuitInGameScene();

    }
    public override void OnLoadGame(MapPrefabsData data, int index)
    {
        base.OnLoadGame(data,index);
        this.data.isInGame = true;
    }
    public override void UnLoadGame(int ind)
    {
        if (ind != 0) return;
        base.UnLoadGame(ind);
        data.isInGame = false;
    }

    #region 外部可访问方法

    void addPlayedTime(float t)
    {
        data.playedTime += (long)t;
    }
    void chgLastTime(System.DateTime t)
    {
        data.lastPlayTime = t;
    }
    public bool IsInGame() { return data.isInGame; }
    
    public DateTime Now()//获取这个存档当前的时间
    {
        return data.lastPlayTime;
    }

    #endregion
}