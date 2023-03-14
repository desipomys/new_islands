using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader_firePit : BaseLoader
{

    public Dictionary<int, FirePit_Data> uuid2data = new Dictionary<int, FirePit_Data>();
    public Dictionary<int, FirePit_Data> matid2data = new Dictionary<int, FirePit_Data>();
    public Dictionary<int,float> isFuel = new Dictionary<int, float>();

    public string path = "SC/FUNCTIONDATA/FIREPIT";
    public string fuelPath = "SC/FUNCTIONDATA/FUEL";
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        center.RegistFunc<Item, FirePit_Data>(nameof(EventNames.GetFirePitDataByMat), GetDataByMatItem);
        center.RegistFunc<int, float>(nameof(EventNames.GetFuelByID), GetFuelByID);
    }

    public override void OnLoaderInit(int prio)
    {
        if (prio != 1) return;
        List<FirePit_Data> datas = load();
        Process(datas);
        loadFuel();
    }
    void loadFuel()
    {
        isFuel.Clear();
        SC_Fuel[] data = Resources.LoadAll<SC_Fuel>(fuelPath);
        foreach (var item in data)
        {
            isFuel.Add(item.id, item.fuel);
        }
    }

    List<FirePit_Data> load()
    {
        FirePit_Data[] data = Resources.LoadAll<FirePit_Data>(path);
        List<FirePit_Data> ans = new List<FirePit_Data>();
        for (int i = 0; i < data.Length; i++)
        {
            ans.Add(data[i]);
        }
        return ans;
    }
    void Process(List<FirePit_Data> fp)
    {
        uuid2data.Clear();
        matid2data.Clear();
        foreach (var item in fp)
        {
            uuid2data.Add(item.uuid, item);
            matid2data.Add(item.mat.id, item);
        }
    }

    #region 外部访问
    /// <summary>
    /// 根据投入
    /// </summary>
    /// <param name="mat"></param>
    /// <returns></returns>
    public FirePit_Data GetDataByMatItem(Item mat)
    {
        try
        {
            return matid2data[mat.id];
        }
        catch (System.Exception)
        {
            Debug.Log("没有此item" + mat);
            return null; 
        }
        
    }
    public float GetFuelByID(int id)
    {
        if (!isFuel.ContainsKey(id)) return 0;
        return isFuel[id];
    }
    public void SetData(FirePit_Data d)
    {
        if (uuid2data.ContainsKey(d.uuid))
        { uuid2data[d.uuid] = d; }
        else
        {
            uuid2data.Add(d.uuid, d);
        }
    }
    public void Save()
    {
        //将data写回ScriptableObject,
    }

    #endregion
}
