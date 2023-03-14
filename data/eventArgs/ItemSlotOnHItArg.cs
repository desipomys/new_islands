using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlotOnHItArg : BaseEventArg
{
    //key 左键0，右键2，中键1
    public int page, x, y, key;
    public override string ToString()
    {
        return "page:"+page + ":" + x + ":" + y + ":" + key;
    }
}
