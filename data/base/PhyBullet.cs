using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

[Serializable]
public class PhyBullet 
{
    /// <summary>
    /// 弹头硬度
    /// </summary>
    public float H;
    /// <summary>
    /// 单位面积动量
    /// </summary>
    public float MOA;
}
[Serializable]
public class PhyArmor
{
    /// <summary>
    /// 装甲硬度
    /// </summary>
    public float H;
    /// <summary>
    /// 等效厚度
    /// </summary>
    public float TD;
}
//%=(H*MOA/H*TD)
//V1=(1-%)^1/2*V0
//MOA1=(1-%)^2*MOA0
