using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class XAndY 
{
    public int x,y;//第y行，第x列
    public XAndY() { }
    public XAndY(int[] a) { x = a[1];y = a[0]; }//0=height（行）,1=width（列）
}
