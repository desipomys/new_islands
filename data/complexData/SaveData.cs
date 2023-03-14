//存档基本数据，存档显示名，存档真实名，存档运行时间,不包含玩家信息
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

/// <summary>
/// 存档的基本信息、设置
/// </summary>
public class SaveData
{
    public string saveName;//存档显示名
    public string savePath;//存档文件名,唯一
    public System.DateTime createTime, lastPlayTime;
    public long playedTime;
    public bool isInGame;
    public bool visable = true;
    public bool IsCheatMode;
    public string minmapPath="img.png";
    public string shipAt;
    public int remainPeople;
    public int level;//添加一个int作为军衔，一个float作为经验
    public float exp;
    //为增强扩展性，添加几个数组，为方便理解限定不能存放超过5个数据
    
    Dictionary<string,FP> exd=new Dictionary<string, FP>();

        public SaveData() { }
    public SaveData(string nam) { saveName = nam;savePath = nam;remainPeople = 100; }

    public bool Compare(SaveData p)
    {
        if (savePath != p.savePath) return false;
        return true;
    }
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this,JsonSetting.serializerSettings);
    }
    public static SaveData FromString(string data)
    {
        return JsonConvert.DeserializeObject<SaveData>(data,JsonSetting.serializerSettings);
    }

}
