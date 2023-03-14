using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Loader_EntityBlock : BaseLoader
{

    //获取所有建筑等级
    //根据等级获取所有entityblock的名称、图标，大小属性，简介
    Dictionary<int, int> typ_lv = new Dictionary<int, int>();
    Dictionary<int, Entity_BlockModel> id_model = new Dictionary<int, Entity_BlockModel>();
    Dictionary<int,List< Entity_BlockModel>> lv_model = new Dictionary<int,List< Entity_BlockModel>>();
    Dictionary<string, GameObject> allEBlock = new Dictionary<string, GameObject>();
    Dictionary<int, string> typ_strTyp = new Dictionary<int, string>();
    Dictionary<int, int> typ_size = new Dictionary<int, int>();

    string path = "Prefabs/terrain/EBlock"; 
    string modelPath = "SC/EBLOCKMODLE";
    public override void OnEventRegist(EventCenter e)
    {
        e.RegistFunc<int, int>(nameof(EventNames.GetEBlockSizeByID), GetEBlockSizeByID);
        e.RegistFunc<int, GameObject>(nameof(EventNames.GetEBlockObjByID), GetEBlockObjByID);
        e.RegistFunc<int[]>(nameof(EventNames.GetAllEBlockID), GetAllEBlockID);
        e.RegistFunc<int, Entity_BlockModel[]>(nameof(EventNames.GetEBlockByLv), GetEBlockByLV);
        e.RegistFunc<int,int, Entity_BlockModel[]>(nameof(EventNames.GetEBlockByLvAndClass), GetEBlockByLVAndClass);
        e.RegistFunc<int, Entity_BlockModel>(nameof(EventNames.GetEBlockByID), GetEBlockByID);
    }
    public override void OnLoaderInit(int prio)
    {
        
        if (prio != 0) return;
        base.OnLoaderInit(prio);
        try
        {
            //string modelEb = Resources.Load<TextAsset>(modelPath).text;//model文件只取typ和size的对应
            SC_Entity_BlockModel[] scmodels = Resources.LoadAll<SC_Entity_BlockModel>(modelPath);
            Entity_BlockModel[] models = new Entity_BlockModel[scmodels.Length];
            for (int i = 0; i < scmodels.Length; i++)
            {
                models[i] = scmodels[i].model;
            }
                //JsonConvert.DeserializeObject<Entity_BlockModel[]>(modelEb);
            for (int i = 0; i < models.Length; i++)
            {
                //typ_strTyp.Add(models[i].typ, models[i].strTyp);
                if(!typ_size.ContainsKey(models[i].typ))
                    typ_size.Add(models[i].typ, XYHelper.ToCoord3(models[i].x, models[i].y, models[i].h));
                if(!lv_model.ContainsKey(models[i].lv))
                {
                    List<Entity_BlockModel> temp = new List<Entity_BlockModel>();
                    temp.Add(models[i]);
                    lv_model.Add(models[i].lv, temp);
                }
                else
                {
                    lv_model[models[i].lv].Add(models[i]);
                }
            }

           
            GameObject[] eblocks = Resources.LoadAll<GameObject>(path);
            for (int i = 0; i < eblocks.Length; i++)
            {
                BaseEntityBlock beb = eblocks[i].GetComponent<BaseEntityBlock>();
                if(beb!=null&&beb.block.typ>0)
                {
                    typ_strTyp.Add(beb.block.typ, eblocks[i].name);
                    allEBlock.Add(eblocks[i].name, eblocks[i]);
                } 
            }

            for (int i = 0; i < models.Length; i++)
            {
                if(typ_strTyp.ContainsKey(models[i].typ))
                    models[i].strTyp = typ_strTyp[models[i].typ];
            }
            Debug.Log("EBlock加载成功,有"+lv_model.Count+"个等级共" + allEBlock.Count + "个实例EBlock被加载");
        }
        catch (System.Exception)
        {
            Debug.Log("EBlock实例加载失败");
            throw;
            //
        }
    }
    public int GetEBlockSizeByID(int id)
    {
        //Debug.Log("取得size" + id + ":" + typ_size[id]);
        return typ_size[id];
    }
    public int[] GetAllEBlockID()
    {
        return new List<int>(typ_strTyp.Keys).ToArray();
    }
    public GameObject GetEBlockObjByID(int id)
    {
        return GameMainManager.CreateGameObject( allEBlock[typ_strTyp[id]]);
    }
    public Entity_BlockModel GetEBlockByID(int id)
    {
        return id_model[id];
    }
    public Entity_BlockModel[] GetEBlockByLV(int lv)
    {
        Debug.Log("有"+lv_model.Count+"个等级"+lv);
        return lv_model[lv].ToArray();
    }
    public Entity_BlockModel[] GetEBlockByLVAndClass(int lv,int c)
    {
        List<Entity_BlockModel> ebs = new List<Entity_BlockModel>();
        //Debug.Log(new List<int>( lv_model.Keys).ToArray());
        for (int i = 0; i < lv_model[lv].Count; i++)
        {
            if (lv_model[lv][i].classTyp == c) ebs.Add(lv_model[lv][i]);
        }
        return ebs.ToArray();
    }
    public GameObject GetEBlockObjByName(string nam)
    {
        return GameMainManager.CreateGameObject( allEBlock[nam]);
    }
}
