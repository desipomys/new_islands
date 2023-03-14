using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ItemPageChangeParm:BaseEventArg
{
    public Item item;
    /// <summary>
    /// ������
    /// </summary>
    public int x;
    /// <summary>
    /// ������
    /// </summary>
    public int y;
    /// <summary>
    /// �ı�֮ǰitempagedata�и�x,yλ�ö�Ӧ��item[]�е�index
    /// </summary>
    public int index;
    public int page;
    public ObjInPageChangeMode mode;
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this,Formatting.Indented);
    }
}