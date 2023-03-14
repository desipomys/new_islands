using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader_Furnance : BaseLoader
{
    public Dictionary<int, Furnance_Data> uuid2data = new Dictionary<int, Furnance_Data>();
    public Dictionary<long,Dictionary<int,int>> mats2id=new Dictionary<long,Dictionary<int,int>>();

    string path = "";
    public override void OnEventRegist(EventCenter e)
    {
        //Debug.Log("nul");
        base.OnEventRegist(e);
        center.RegistFunc<int, Furnance_Data>(nameof(EventNames.GetFurnanceDataByID),GetDataByID);
        center.RegistFunc<Item[], Furnance_Data>(nameof(EventNames.GetFurnanceByMats), GetDataBymats);
    }

    public override void OnLoaderInit(int prio)
    {
        if (prio != 1) return;
        try
        {
            List<Furnance_Data> datas = load();
            Process(datas);
            Debug.Log("FurnanceData加载成功，有" + uuid2data.Count + "个data");
        }
        catch (System.Exception e)
        {
            Debug.LogError("FurnanceData加载失败"+e);
        }
       
    }
    List<Furnance_Data> load()
    {
        Furnance_Data[] data = Resources.LoadAll<Furnance_Data>(path);
        List<Furnance_Data> ans = new List<Furnance_Data>();
        for (int i = 0; i < data.Length; i++)
        {
            ans.Add(data[i]);
        }
        return ans;
    }
    void Process(List<Furnance_Data> fp)
    {
        uuid2data.Clear();
        mats2id.Clear();
        foreach (var item in fp)
        {
            uuid2data.Add(item.uuid, item);
            long mats=XYHelper.ToLongXY(item.mat1.id,item.mat2.id);
            if(mats2id.ContainsKey(mats))
            {
                if (!mats2id[mats].ContainsKey(item.moduel.id))//只有之前不存在此模具才加
                    mats2id[mats].Add(item.moduel.id, item.uuid);
            }
            else
            {
                Dictionary<int,int> temp=new Dictionary<int,int>();
                temp.Add(0,item.uuid);
                mats2id.Add(mats,temp);
            }
        }
    }

    #region 外部访问
    public Furnance_Data GetDataByID(int uuid)
    {
        try
        {
 return uuid2data[uuid];
        }
        catch (System.Exception)
        {
            Debug.LogError("无此UUID的furnanceData" + uuid);
            return null;
        }
       
    }
    public Furnance_Data GetDataBymats(Item[] mats)
    {
        try
        {
return getDataBymats(mats[0], mats[1], mats[2]);
        }
        catch (System.Exception)
        {
            Debug.LogError("传入mats格式不对" );
            return null;
        }
        
    }
    public Furnance_Data getDataBymats(Item mat1,Item mat2,Item moduel)
    {
        long xy = XYHelper.ToLongXY(mat1.id, mat2.id);
        if(mats2id.ContainsKey(xy))
        {
            if (mats2id[xy].ContainsKey(mat2.id))
            {
                int uuid= mats2id[xy][mat2.id];
                if (uuid2data.ContainsKey(uuid))
                    return uuid2data[uuid];
                else { Debug.LogError("无此UUID的furnanceData"+uuid); return null; }
            }
            else return null;
        }
        else return null;
    }

    #endregion
}