using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft;

public class Loader_Item : BaseLoader
{
    public static int ItemUIBigSize = 130;
    public static int ItemUISize = 66;

    public Dictionary<int, Item> allItems = new Dictionary<int, Item>();
    public Dictionary<int, int> idtomax = new Dictionary<int, int>();
    public Dictionary<int, int> idtosize = new Dictionary<int, int>();//x在高16位,y在低16位
    public Dictionary<int, string> idtoTexture = new Dictionary<int, string>();
    public Dictionary<int, string> idtoDescript = new Dictionary<int, string>();
    public Dictionary<int, string> idtoName = new Dictionary<int, string>();
    public Dictionary<ItemGroup, int[]> itemClasstoItems = new Dictionary<ItemGroup, int[]>();
    public Dictionary<int, ItemGroup> idtoClass = new Dictionary<int, ItemGroup>();

    public Dictionary<int, Item_Warper> allItemwarpers = new Dictionary<int, Item_Warper>();
    #region Init
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.RegistFunc<int, int>(nameof(EventNames.ItemIDtoMax), ItemIDtoMax);
        e.RegistFunc<int, int>(nameof(EventNames.ItemIDtoMaxSub), ItemIDtoMaxSub);
        e.RegistFunc<int, int>(nameof(EventNames.ItemIDtoSize), ItemIDtoSize);
        e.RegistFunc<int, Texture>(nameof(EventNames.ItemtoTexture), ItemtoTexture);
        e.RegistFunc<int, string>(nameof(EventNames.ItemIDtoDes), ItemIDtoDes);
        e.RegistFunc<int, string>(nameof(EventNames.ItemIDtoName), ItemIDtoName);
        e.RegistFunc<Item[]>(nameof(EventNames.GetAllItems), GetAllItems);
        e.RegistFunc<Item>(nameof(EventNames.GetRandomItem), GetRandomItem);
        e.RegistFunc<ItemGroup, Item[]>(nameof(EventNames.GetItemsByGroup), GrouptoItems);
        e.RegistFunc<int, ItemGroup>(nameof(EventNames.GetGroupByItemID), ItemIDtoGroup);
        e.RegistFunc<ItemGroup[]>(nameof(EventNames.GetAllItemGroup), () => { return new List<ItemGroup>(itemClasstoItems.Keys).ToArray(); });
        Debug.Log("itemloader注册完成");
    }

    string path = "JsonData/items";
    string scPath = "SC/ITEM";
    /// <summary>
    /// 从哪里加载item
    /// </summary>
    /// <returns></returns>
    List<Item_Warper> LoadItem_Warper()
    {
        //string d = Resources.Load<TextAsset>(path).text;
        Item_Warper[] allitem = Resources.LoadAll<Item_Warper>(scPath);
        List<Item_Warper> temp = new List<Item_Warper>();
        foreach (var item in allitem)
        {
            //
            FillNull(item);
            temp.Add(item);
        }
        return temp;
    }
    void FillNull(Item_Warper iw)
    {
        if (iw.maxsub > 0) { iw.item.subid = iw.maxsub; }
        if (iw.item.id == 0) { int id = 0;  int.TryParse(iw.name,out id); iw.item.id = id; }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="its"></param>
    void AdditiveLoadItem(List<Item_Warper> its)
    {

    }
    void PerprocessingItem(List<Item_Warper> its)
    {
        foreach (var item in its)
        {
            item.item.num = 1;
            if (item.max == 0) item.max = 1;
        }
    }
    public override void OnLoaderInit(int prio)
    {
        if (prio != 0) return;
        try
        {
            
            List<Item_Warper> warpers = LoadItem_Warper();
            PerprocessingItem(warpers);
            Debug.Log(warpers.Count + "个warper");
            for (int i = 0; i < warpers.Count; i++)
            {
                Item temp = Item_Warper.GetItem(warpers[i]);
                if (temp.num == 0) temp.num = 1;
                if(!allItems.ContainsKey(temp.id))
                    allItems.Add(temp.id, temp);

                if (!idtomax.ContainsKey(temp.id))
                    idtomax.Add(temp.id, warpers[i].max);
                if (warpers[i].w != 0 && warpers[i].h != 0 && !idtosize.ContainsKey(temp.id))
                    idtosize.Add(temp.id, XYHelper.ToIntXY(warpers[i].w, warpers[i].h));
                if (!idtoDescript.ContainsKey(temp.id))
                    idtoDescript.Add(temp.id, warpers[i].descript);
                if (!idtoTexture.ContainsKey(temp.id))
                    idtoTexture.Add(temp.id, warpers[i].texture);
                if (!idtoName.ContainsKey(temp.id))
                    idtoName.Add(temp.id, warpers[i].nam);

                if (!idtoClass.ContainsKey(temp.id))
                    idtoClass.Add(temp.id, warpers[i].group);
                if (!allItemwarpers.ContainsKey(temp.id))
                    allItemwarpers.Add(temp.id, warpers[i]);
            }
            Debug.Log(idtoClass.Count + "个class");
            List<List<int>> idGroups = new List<List<int>>();
            foreach (var item in idtoClass)
            {
                for (int j = 0; j <= (int)(item.Value); j++)
                {
                    if ((int)(item.Value) >= idGroups.Count)
                        idGroups.Add(new List<int>());
                    else break;
                }
                idGroups[(int)(item.Value)].Add(item.Key);
            }
            for (int i = 0; i < idGroups.Count; i++)
            {
                itemClasstoItems.Add((ItemGroup)i, idGroups[i].ToArray());
            }
            Debug.Log("有" + warpers.Count + "个item被加载");
        }
        catch (System.Exception e)
        {
            Debug.Log("Item加载失败"+e.Message);

        }


    }
    #endregion

    #region 外部访问
    //不再使用静态delegate访问方式，而是走worldcenter的事件系统
    Item GetRandomItem()
    {
        List<int> keys = new List<int>(allItems.Keys);
        return allItems[keys[UnityEngine.Random.Range(0, keys.Count)]];
    }
    public int ItemIDtoMax(int id)
    {
        int i = 0;
        if (!idtomax.TryGetValue(id, out i)) { return 20; }
        return i;
    }
    public int ItemIDtoMaxSub(int id)
    {
        Item_Warper i;
        if (allItemwarpers.TryGetValue(id, out i))
        {
            return i.maxsub;
        }
        return -1;

    }
    public int ItemIDtoSize(int id)
    {
        int i = 0x00010001;
        if (idtosize.TryGetValue(id, out i))
        { }
        else i = 0x00010001;
        //Debug.Log(id + "size="+i);
        return i;
    }
    public Texture ItemtoTexture(int id)
    {
        string i;
        idtoTexture.TryGetValue(id, out i);
        return EventCenter.WorldCenter.GetParm<string, Texture>("StrtoTexture", i);
    }
    public string ItemIDtoDes(int id)
    {
        string i;
        idtoDescript.TryGetValue(id, out i);
        return i;
    }
    public string ItemIDtoName(int id)
    {
        string i;
        idtoName.TryGetValue(id, out i);
        return i;
    }
    public ItemGroup ItemIDtoGroup(int id)
    {
        ItemGroup i = 0;
        idtoClass.TryGetValue(id, out i);
        return i;
    }
    public Item[] GrouptoItems(ItemGroup c)
    {

        List<Item> temp = new List<Item>();
        int[] ids;
        itemClasstoItems.TryGetValue(c, out ids);
        if (ids != null)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                temp.Add(allItems[ids[i]]);
            }
        }
        //Debug.Log("get items by group"+c);
        return temp.ToArray();
    }
    public Item[] GetAllItems()
    {
        List<Item> temp = new List<Item>();
        temp.AddRange(allItems.Values);
        return temp.ToArray();
    }
    #endregion
}

//专供本地存储使用

