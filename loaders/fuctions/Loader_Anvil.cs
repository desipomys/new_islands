using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader_Anvil : BaseLoader
{
    public Dictionary<int, Anvil_Data> uuid2Anvil = new Dictionary<int, Anvil_Data>();
    public Dictionary<AnvilMode,Dictionary<int,int>> mode2uuid=new Dictionary<AnvilMode,Dictionary<int,int>>();

    public string path = "SC/FUNCTIONDATA/ANVIL";

    public override void OnEventRegist(EventCenter e)
    {
       center=e;

    }

      public override void OnLoaderInit(int prio)
    {
        if (prio != 1) return;
        List<Anvil_Data> datas = load();
        Process(datas);
        
    }

    List<Anvil_Data> load()
    {
        Anvil_Data[] data = Resources.LoadAll<Anvil_Data>(path);
        List<Anvil_Data> ans = new List<Anvil_Data>();
        for (int i = 0; i < data.Length; i++)
        {
            ans.Add(data[i]);
        }
        return ans;
        
        
    }
    void Process(List<Anvil_Data> data)
    {
        uuid2Anvil.Clear();
        mode2uuid.Clear();
        if (data == null) return;
       foreach (var item in data)
        {
            uuid2Anvil.Add(item.uuid, item);
            
            if(mode2uuid.ContainsKey(item.mode))
            {
                if(!mode2uuid[item.mode].ContainsKey(item.mat.id))
                {
                    mode2uuid[item.mode].Add(item.mat.id,item.uuid);
                }    
            }
            else{
               Dictionary<int,int> temp= new Dictionary<int,int>();
               temp.Add(item.mat.id,item.uuid);
                mode2uuid.Add(item.mode,temp);
            }
        }
    }

    public Anvil_Data GetDataByModeMat(AnvilMode mode,Item mat)
    {
        if(mode2uuid.ContainsKey(mode))
        {
            if(mode2uuid[mode].ContainsKey(mat.id))
            {
                int uuid= mode2uuid[mode][mat.id];
                if(uuid2Anvil.ContainsKey(uuid))
                {return uuid2Anvil[uuid];}
                
            }
        }
        Debug.Log("不存在"+mode+":"+mat.id+"对应的anvildata");
         return null;
    }
   
}