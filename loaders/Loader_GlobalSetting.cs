//全局设置与存档无关，不用生成到container，loader自己定时保存
//音量，分辨率，当前存档名，画质，都要注册给worldevc访问
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
/*
*   玩家ID，玩家名，
*   
*/
[System.Obsolete]
public class Loader_GlobalSetting : BaseLoader
{
    static DataNode data;//静态的数据读取方法只适用于固定的，存档无关的纯基本数据,尤其是需要泛型的数据
    public static bool GetGlobalSetting<T>(string path,out T ans)
    {
        return data.GetData<T>(path,out ans);
    }
    public static bool SetGlobalSetting<T>(string path,T d)
    {
        return data.SetOrAddData<T>(path,d);
    }

    public override void OnEventRegist(EventCenter e)
    {
       base.OnEventRegist(e);
       e.ListenEvent("SaveGlobalSetting",SaveSetting);
    }

    string path="JsonData/GlobalSetting.txt";

    public override void OnLoaderInit(int prio)
    {
        if(prio!=0)return;
        string d = FileSaver.GetFileUtility(path);//暂时用不加密的方法保存
        try
        {
            if(string.IsNullOrEmpty(d))
            {
                data=new DataNode("root");
                data.SetOrAddData<float>("volume",1);

            }
            else data=DataNode.FromString(d);
        }
        catch (System.Exception)
        {
            Debug.Log("globalsetting解析失败");
            data=new DataNode("root");
            data.SetOrAddData<float>("volume",1);
        }
        
        checkPlayerID();
    }
    public void SaveSetting()
    {
        string d=data.ToString();
       if(!FileSaver.SaveFileUtility(path,d)) 
       {
           Debug.Log("globalsetting保存失败");
       }
    }

    #region 固有数据
    void checkPlayerID()
    {
        long id=0;
        if(!GetGlobalSetting<long>("playerID",out id))
        {
            SetGlobalSetting<long>("playerID",playerIDGenerate());
        }
    }
    long playerIDGenerate()//时间码+机器码+随机码
    {
        long d= System.DateTime.Now.ToBinary();
        long machinecode=DeviceCodeGen.Gen();
        long randomcode=UnityEngine.Random.Range(0,65536);

        return d&0x1111111100000000+machinecode&0x0000000011110000+randomcode&0x0000000000001111;
    }

    #endregion
}