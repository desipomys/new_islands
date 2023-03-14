using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader_ChemicalTable : BaseLoader
{
    public Dictionary<int, Chem_Data> uuid2Chem = new Dictionary<int, Chem_Data>();
    public TwoKeyDictionary<ChemMode,int,int> modeMats2uuid=new TwoKeyDictionary<ChemMode,int,int>();

    public string path = "SC/FUNCTIONDATA/CHEM";

    public override void OnEventRegist(EventCenter e)
    {
       center=e;

    }

      public override void OnLoaderInit(int prio)
    {
        if (prio != 1) return;
        List<Chem_Data> datas = load();
        Process(datas);
        
    }
    int GetMatsHash(Item[] mats)
    {
        string s="";
        List<int> temp=new List<int>();
        for (int i = 0; i < mats.Length; i++)
        {
            temp.Add(mats[i].id);
        }
        temp.Sort();
        for (int i = 0; i < temp.Count; i++)
        {
            s+=temp[i].ToString();
        }
        return s.GetHashCode();
    }

    List<Chem_Data> load()
    {
        Chem_Data[] data = Resources.LoadAll<Chem_Data>(path);
        List<Chem_Data> ans = new List<Chem_Data>();
        for (int i = 0; i < data.Length; i++)
        {
            ans.Add(data[i]);
        }
        return ans;
    }
    void Process(List<Chem_Data> data)
    {
        uuid2Chem.Clear();
        modeMats2uuid.Clear();
       foreach (var item in data)
        {
            uuid2Chem.Add(item.uuid, item);
            
            modeMats2uuid.Add(item.mod,GetMatsHash(item.mat),item.uuid);
        }
    }

    public Chem_Data GetDataByModeMat(ChemMode mode,Item[] mat)
    {
        try
        {
            int temp=modeMats2uuid[mode][GetMatsHash(mat)];
            return uuid2Chem[temp];
        }
        catch (System.Exception)
        {
            string t="";
            for (int i = 0; i < mat.Length; i++)
            {
                t+=mat[i].id.ToString()+",";
            }
            Debug.Log("不存在"+mode+",item:"+t+"对应的Chemdata");
            
            return null;
        }
        
        
         
    }
   
}