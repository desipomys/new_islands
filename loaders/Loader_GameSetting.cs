using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Loader_GameSetting : BaseLoader
{
    GameSetting setting;
     string persistentUrl = $"{Application.persistentDataPath}/setting.cfg";
    // Start is called before the first frame update
    public override void OnLoaderInit(int prio)
    {
        if(prio!=0)return;
        base.OnLoaderInit(prio);
        string s=FileSaver.GetFileWithDecrypt(persistentUrl);
        if(string.IsNullOrEmpty(s))
        {
            setting = new GameSetting();
        }
        else setting=JsonConvert.DeserializeObject<GameSetting>(s,JsonSetting.serializerSettings);

    }
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        center.ListenEvent(nameof(EventNames.SaveGameSetting),SaveSetting);
        center.RegistFunc<GameSetting>(nameof(EventNames.GetGameSetting),()=>{return setting;});
    }

    public void SaveSetting()
    {
        string s=JsonConvert.SerializeObject(setting,JsonSetting.serializerSettings);
        FileSaver.SaveFileWithEncrypt(s,persistentUrl);
    }

}
/// <summary>
/// 游戏全局设定，音频、视频等
/// </summary>
public class GameSetting
{
    #region 音频
    public float Volume;


    #endregion

     #region 视频

    public bool IsFullScreen;

    #endregion

     #region 操作



    #endregion

}