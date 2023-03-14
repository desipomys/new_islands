using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ItemPageChangeParm:BaseEventArg
{
    public Item item;
    /// <summary>
    /// 横坐标
    /// </summary>
    public int x;
    /// <summary>
    /// 纵坐标
    /// </summary>
    public int y;
    /// <summary>
    /// 改变之前itempagedata中该x,y位置对应的item[]中的index
    /// </summary>
    public int index;
    public int page;
    public ObjInPageChangeMode mode;
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this,Formatting.Indented);
    }
}