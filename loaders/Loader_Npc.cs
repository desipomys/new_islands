using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Loader_Npc : BaseLoader
{
   //读取npc预设的chardata文件，对外提供按npc种类enum获取chardata的方法

   static List<NpcData_Warper> npcDatas;
   static Dictionary<NpcProfession,string> professionHeadImg=new Dictionary<NpcProfession,string>();
   static Dictionary<NpcProfession,string> professionBodyImg=new Dictionary<NpcProfession,string>();
   static Dictionary<NpcProfession,string> professionModel=new Dictionary<NpcProfession,string>();
   string path="JsonData/Npc.txt";

   public static Charactor_Data GetCharDataByType(NpcProfession p,int level)
   {
      for (int i = 0; i < npcDatas.Count; i++)
      {
          if(npcDatas[i].data.level==level&&npcDatas[i].data.profession==p)
          {
             return npcDatas[i].data.char_data;
          }
      }
      return null;
   }

    public override void OnEventRegist(EventCenter e)
    {
       base.OnEventRegist(e);
       e.RegistFunc<NpcProfession, Texture>(nameof(EventNames.GetNPCHeadImg),GetNPCHeadImg);
       e.RegistFunc<NpcProfession, Texture>(nameof(EventNames.GetNPCBodyImg),GetNPCBodyImg);

    }

    public override void OnLoaderInit(int prio)
    {
        if(prio!=0)return;
        string d = FileSaver.GetFileUtility(path);
        try
        {
            if(string.IsNullOrEmpty(d))
            {
                npcDatas=new List<NpcData_Warper>();
            }
            else npcDatas=JsonConvert.DeserializeObject<List<NpcData_Warper>>(d);

            Debug.Log("NPC数据加载完成，有" + npcDatas.Count + "种NPC");
        }
        catch (System.Exception)
        {
            Debug.Log("npc预设解析失败");
            npcDatas=new List<NpcData_Warper>();
        }
        if(npcDatas!=null&&npcDatas.Count>0) initDic();
    }
    void initDic()
    {
        for (int i = 0; i < npcDatas.Count; i++)
        {
            try
            {
                professionHeadImg.Add(npcDatas[i].data.profession,npcDatas[i].headImgName);
                professionBodyImg.Add(npcDatas[i].data.profession,npcDatas[i].BodyImgName);
                professionModel.Add(npcDatas[i].data.profession,npcDatas[i].BodyModelName);
            }
            catch (System.Exception)
            {

                continue;
            }
           
        }
    }

    #region 公共区
    public Texture GetNPCHeadImg(NpcProfession pro)
    {
        if(!professionHeadImg.ContainsKey(pro))return LoaderManager.GetLoader<Loader_Texture>().StrToTexture("none");
        string temp=professionHeadImg[pro];
        return LoaderManager.GetLoader<Loader_Texture>().StrToTexture(temp);
    }
    public Texture GetNPCBodyImg(NpcProfession pro)
    {
        if (!professionBodyImg.ContainsKey(pro)) return LoaderManager.GetLoader<Loader_Texture>().StrToTexture("none");
        string temp=professionBodyImg[pro];
        return LoaderManager.GetLoader<Loader_Texture>().StrToTexture(temp);
    }

    #endregion
}


