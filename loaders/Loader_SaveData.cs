//按名获取存档数据(savedata),不包括ingame数据
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using System;
using System.IO;

public class Loader_SaveData : BaseLoader
{
    static Dictionary<string,SaveData> allSave=new Dictionary<string,SaveData>();//以path为key

    #region Init
    public override void OnEventRegist(EventCenter e)
    {
       center=e;

        e.RegistFunc<Dictionary<string, SaveData>>(nameof(EventNames.GetAllSaveData), GetAllSaveData);
        e.RegistFunc<string,SaveData>(nameof(EventNames.GetSaveDataByName), GetSaveDataByName);
        e.RegistFunc<string, SaveData>(nameof(EventNames.GetSaveDataByIndex), GetSaveDataByIndex);
        e.ListenEvent(nameof(EventNames.CreateSave), CreateSave);
        e.RegistFunc<string,string>(nameof(EventNames.CreateSaveByName), CreateSaveByName);
        e.RegistFunc<string>(nameof(EventNames.CreateSaveAndGetName), CreateSaveAndGetName);
    }

    string path= "Saves/";
    public override void OnLoaderInit(int prio)//先于oneventreg
    {   
        //用filesaver
        //找到save目录下所有property.sav,读取后反序列化为savedata，并检验存档完整性

        //测试区
        //CreateNewSave("save1");
        if(prio!=0)return;
        try
        {
            string[] allsaveproerPath =FileSaver.GetAllSavePropertyPath(Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources", path));

            for (int i = 0; i < allsaveproerPath.Length; i++)
            {
                string savedata = FileSaver.GetFileWithFullPath(Path.Combine(allsaveproerPath[i],"proper.txt"));
                //Debug.Log(savedata);
                SaveData sd = SaveData.FromString(savedata);
                if(sd!=null&&sd.visable)
                    allSave.Add(sd.savePath,sd);
            }
            Debug.Log("有" + allSave.Count + "个存档,"+(allsaveproerPath.Length-allSave.Count)+"个存档损坏或被隐藏");
        }
        catch (System.Exception)
        {
            Debug.Log("存档加载失败");
        }
        
    }
    #endregion
    public Dictionary<string, SaveData> GetAllSaveData()
    {
        //List<SaveData> temp = new List<SaveData>(allSave.Values);
        return allSave;
    }
    public int GetSaveCount()
    {
        return allSave.Count;
    }
    public SaveData GetSaveDataByName(string name)
    {
        SaveData s;
        try
        {
            allSave.TryGetValue(name,out s);
        }
        catch (System.Exception)
        {
            Debug.Log("获取存档失败"+name);
            return null;
        }
        
        return s;
    }
    public SaveData GetSaveDataByIndex(string path)
    {
        //if (index < 0 || index >= allSave.Count) return null;
        //List<SaveData> temp = new List<SaveData>(allSave.Values);
        return allSave[path];
    }
    public void CreateSave()
    {
        CreateSaveAndGetName();
    }
    public string CreateSaveByName(string name)
    {
        if(!string.IsNullOrWhiteSpace(name))
           {
            if(allSave.ContainsKey(name))
            {
                  return "";  
            }
            else {return CreateNewSave(name);}
           } 
            else 
            {
                
                return CreateSaveAndGetName();
            }
    }
    public string CreateSaveAndGetName()
    {
        int index = UnityEngine.Random.Range(0, 100000);
        while (allSave.ContainsKey(index.ToString()))
        {
            index = UnityEngine.Random.Range(0, 100000);
        }
        string temp = CreateNewSave(index.ToString());
        if (string.IsNullOrEmpty(temp))
        {
            return "";
        }
        else return temp;
    }

    public string CreateNewSave(string name)//未进入存档时调用，所以无需告诉container
    {
        SaveData sd;
        try
        {
            sd = new SaveData();
            sd.saveName = name;
            sd.createTime = System.DateTime.Now;
            sd.lastPlayTime = System.DateTime.Now;
            sd.isInGame = false;
             sd.savePath = checkAvaliablePath(name);//按照存档名生成可用的存档路径名
            allSave.Add(sd.savePath, sd);
            createSaveFolder(sd);
            center.SendEvent<string>(nameof(EventNames.OnCreateSave), sd.savePath);
        }
        catch (System.Exception)
        {
            Debug.Log("存档创建失败");
            return "";
        }
        return sd.savePath;
    }

    public void DeleteSave(string path)
    {
       SaveData sd= GetSaveDataByIndex(path);
        allSave.Remove(sd.savePath);

        FileSaver.DeleteSave(sd.savePath);
        center.SendEvent<string>(nameof(EventNames.OnDeleteSave), sd.savePath);
    }

    string checkAvaliablePath(string path)
    {
        string p = path;
        try
        {
           
            while(Directory.Exists(System.IO.Path.Combine(System.Environment.CurrentDirectory, "Assets/Resources/Saves/", p)))
            {
                p += "_";
            }
        }
        catch (Exception)
        {
            Debug.Log("存档创建失败");
            throw;
        }
        
        return p;
    }
    void createSaveFolder(SaveData sd)
    {
        //创建data.sav,ingame.sav,proper.sav和map文件夹
        PlayerData pd = new PlayerData();
        MovScriptEngine eng = EventCenter.WorldCenter.GetParm<MovScriptEngine>("GetStartEngine");

        string sdstr = sd.ToString();
        if(!FileSaver.SaveFileUtility(path + sd.savePath+"/proper.txt", sdstr))Debug.Log("proper");
        string engstr = eng==null?"":eng.ToString();
        if(!FileSaver.SaveFileUtility(path + sd.savePath+"/ingame.txt", engstr))Debug.Log("ingame");
        string pdstr = pd.ToString();
        if(!FileSaver.SaveFileUtility(path + sd.savePath + "/data.txt", pdstr))Debug.Log("data");
        //创建mapPrefabsData到文件

    }
}