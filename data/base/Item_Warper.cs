using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[Serializable]
[CreateAssetMenu(menuName = "dataEditor/Item_Warper")]
public class Item_Warper : Base_SCData
{
    public Item item;
    public string nam;
    public int max;
    public int maxsub;
    public int w;//在UI的大小
    public int h;
    public Resource_Data typeValues = new Resource_Data();
    public string texture;
    public string descript;
    public ItemGroup group;
    
    public Item_Warper() { }
    public Item_Warper(Item i) { item = i; }

    public static Item GetItem(Item_Warper w)
    {
        return w.item;
    }
}