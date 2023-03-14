//只提供按名获取预设,不包括effect和bullet
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;

public class Loader_Prefabs : BaseLoader
{
    string entitypath="Prefabs/entity/";
    string blockpath="Prefabs/block/";
    string naturepath="Prefabs/nature/";
    string modelpath = "Prefabs/Models/";

    Dictionary<string,GameObject> entitys=new Dictionary<string,GameObject>();
    Dictionary<string,GameObject> blocks=new Dictionary<string,GameObject>();
    Dictionary<string,GameObject> natures=new Dictionary<string,GameObject>();
    Dictionary<string, GameObject> models = new Dictionary<string, GameObject>();

    public override void OnEventRegist(EventCenter e)
    {
       base.OnEventRegist(e);
       //e.ListenEvent("SaveGlobalSetting",SaveSetting);
        e.RegistFunc<string,GameObject>(nameof(EventNames.GetEntityByName),GetEntityByName);
        e.RegistFunc<string,GameObject>("GetBlockByid",GetBlockByid);
        e.RegistFunc<string,GameObject>("GetNatureByName",GetNatureByName);
        e.RegistFunc<string, GameObject>(nameof(EventNames.GetModelByName), GetModelByName);
    }

    public override void OnLoaderInit(int prio)
    {
        if(prio!=0)return;
        try
        {
            GameObject[] entity=Resources.LoadAll<GameObject>(entitypath);
                for (int i = 0; i < entity.Length; i++)
                {
                    entitys.Add(entity[i].name,entity[i]);
                }

                GameObject[] block=Resources.LoadAll<GameObject>(blockpath);
                for (int i = 0; i < block.Length; i++)
                {
                    blocks.Add(block[i].name,block[i]);
                }

                GameObject[] nature=Resources.LoadAll<GameObject>(naturepath);
                for (int i = 0; i < nature.Length; i++)
                {
                    natures.Add(nature[i].name,nature[i]);
                }
            GameObject[] model = Resources.LoadAll<GameObject>(modelpath);
            for (int i = 0; i < model.Length; i++)
            {
                models.Add(model[i].name, model[i]);
            }
        }
        catch (System.Exception)
        {
            
            Debug.Log("prefabs加载失败");
        }

       
    }

    #region 外部注册方法
    GameObject GetModelByName(string name)
    {
        GameObject temp;
        if (models.TryGetValue(name, out temp))
        {
            return GameMainManager.CreateGameObject(temp);
        }
        else
        {
            return null;
        }

    }
    GameObject GetEntityByName(string name)
    {
        GameObject temp;
        if(entitys.TryGetValue(name,out temp))
        {
            return GameMainManager.CreateGameObject(temp);
        }
        else
        {
            return null;
        }
        
    }
    GameObject GetBlockByid(string name)
    {
        GameObject temp;
        if(blocks.TryGetValue(name,out temp))
        {
            return GameObject.Instantiate(temp);
        }
        else
        {
            return null;
        }
        
    }

    GameObject GetNatureByName(string name)
    {
        GameObject temp;
        if(natures.TryGetValue(name,out temp))
        {
            return GameObject.Instantiate(temp);
        }
        else
        {
            return null;
        }
        
    }
    #endregion
}
