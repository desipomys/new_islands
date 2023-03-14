using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 装备、皮肤的mesh都在这获取
/// </summary>
public class Loader_EquipAndSkin : BaseLoader
{

    //需提供根据抽象装备名找具体装备obj[]的功能
    Dictionary<string, GameObject> name2meshobj = new Dictionary<string, GameObject>();
    Dictionary<int, Dictionary<PlayerEquipPos, string>> item2meshname = new Dictionary<int, Dictionary<PlayerEquipPos, string>>();
    string path = "SC/EQUIP/";
    string eqpath = "Prefabs/playerEQ/player_fullCloth";
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.RegistFunc<string[], GameObject[]>(nameof(EventNames.GetEQObjByabsName), getEQObj);
        e.RegistFunc<Item[], GameObject[]>(nameof(EventNames.GetEQObjByItem), getEQObjItem);
    }
    public override void OnLoaderInit(int prio)
    {
        if (prio != 1) return;
      //  LoadAndProcessEQMeshObj();
       // process(load());
    }

    SC_Equipment[] load()
    {
        return Resources.LoadAll<SC_Equipment>(path);
    }
    void process(SC_Equipment[] eqs)
    {
       
        foreach (var item in eqs)
        {
            if(!item2meshname.ContainsKey(item.itemID))
            {
                item2meshname.Add(item.itemID, new Dictionary<PlayerEquipPos, string>());
            }
            for (int i = 0; i < item.partPos.Length; i++)
            {
                //如果该id下没有该partpos则新增，有则替换
                if (!item2meshname[item.itemID].ContainsKey(item.partPos[i]))
                {
                    item2meshname[item.itemID].Add(item.partPos[i], item.partname[i]);
                }
                else
                {
                    item2meshname[item.itemID][item.partPos[i]]= item.partname[i];
                }
            }
        }
        Debug.Log(item2meshname.Count + "个装备加载成功");
    }

    void LoadAndProcessEQMeshObj()
    {
        GameObject g = Resources.Load<GameObject>(eqpath);
        SkinnedMeshRenderer[] sks = g.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var item in sks)
        {
            name2meshobj.Add(item.gameObject.name, item.gameObject);
        }
        Debug.Log(name2meshobj.Count+"个clothMesh加载成功");
    }

    public GameObject[] getEQObj(string[] absnames)
    {
        if (absnames == null || absnames.Length <= 0) return null;
        List<GameObject> temp = new List<GameObject>();
        for (int i = 0; i < absnames.Length; i++)
        {
            if(name2meshobj.ContainsKey(absnames[i]))
                temp.Add(name2meshobj[absnames[i]]);
        }
        return temp.ToArray();
    }
    public GameObject[] getEQObjItem(Item[] absnames)
    {//
        Dictionary<PlayerEquipPos, string> temp = new Dictionary<PlayerEquipPos, string>();
        foreach (var item in absnames)
        {
            if(item2meshname.ContainsKey(item.id))
            {
                foreach (var its in item2meshname[item.id])
                {
                    if (temp.ContainsKey(its.Key))
                    {
                        temp[its.Key] = its.Value;
                    }
                    else temp.Add(its.Key, its.Value);
                }
            }
        }
        return getEQObj(new List<string>(temp.Values).ToArray());
    }

}
