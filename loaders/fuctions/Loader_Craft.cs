using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader_Craft : BaseLoader
{
    public Dictionary<int, Craft_Data> uuid2Craft = new Dictionary<int, Craft_Data>();
    public Dictionary<ItemGroup, List<int>> type2UUID = new Dictionary<ItemGroup, List<int>>();

    public override void OnEventRegist(EventCenter e)
    {
        //Debug.Log("nul");
        base.OnEventRegist(e);
        center.RegistFunc<ItemGroup, int[]>(nameof(EventNames.GetCraftUUIDByGroup), GetUUIDByGroup);
        center.RegistFunc<int, Craft_Data>(nameof(EventNames.GetCraftDataByUUID), GetDataByUUID);
    }

    public string path = "SC/FUNCTIONDATA/CRAFT";

    List<Craft_Data> loadCraft()
    {
        try
        {
            Craft_Data[] all = Resources.LoadAll<Craft_Data>(path);
            uuid2Craft.Clear();
            List<Craft_Data> ans = new List<Craft_Data>();
            for (int i = 0; i < all.Length; i++)
            {
                Craft_Data cd = all[i];
                ans.Add(cd);

            }
            return ans;
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            throw;
        }

    }
    /// <summary>
    /// 将默认值等写入data
    /// </summary>
    /// <param name="cd"></param>
    void FineTuneData(Craft_Data cd)
    {

    }
    void ProcessCraft(List<Craft_Data> datas)
    {
        try
        {
            foreach (var item in datas)
            {
                FineTuneData(item);
                uuid2Craft.Add(item.uuid, item);

                ItemGroup ig = center.GetParm<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), item.product.id);
                if (type2UUID.ContainsKey(ig))
                {
                    type2UUID[ig].Add(item.uuid);
                }
                else
                {
                    type2UUID.Add(ig, new List<int>() { item.uuid });
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            throw;
        }

    }

    public override void OnLoaderInit(int prio)
    {
        if (prio != 1) return;
        try
        {
            List<Craft_Data> datas = loadCraft();
            ProcessCraft(datas);
            Debug.Log("加载craftData完成，共有" + datas.Count + "个craftData");
        }
        catch (System.Exception)
        {
            Debug.Log("加载craftData失败");
        }

    }

    #region 外部访问
    //按产物item类别取uuid[]
   
    public int[] GetUUIDByGroup(ItemGroup ig)
    {
        if (type2UUID.ContainsKey(ig))
            return type2UUID[ig].ToArray();
        else return null;
    }
    //按uuid取craftdata
    public Craft_Data GetDataByUUID(int uuid)
    {
        if (uuid2Craft.ContainsKey(uuid))
            return uuid2Craft[uuid];
        else return null;
    }
    #endregion
}
