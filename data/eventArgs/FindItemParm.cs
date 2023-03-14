using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindItemParm 
{
    public Item item;
    public int page;
    public ItemCompareMode mode;
    public FindItemParm() { }
    public FindItemParm(Item i) { item = i; }
}
