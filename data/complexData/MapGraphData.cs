using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[Serializable]
//用于开始界面显示地图连接关系及计算在地图间移动成本
[CreateAssetMenu(menuName = "Map/地图连接关系")]
public class MapGraphData :Base_SCData
{
    public string mapName;
   
    public int x, y;//节点在地图中的坐标
    public int landTime;//登陆次数
    public MapEdgeData[] edges;//与该节点相连的边

    public MapExtraData exd;
}
[Serializable]
/// <summary>
/// 地图中连接两节点的一条边
/// </summary>
public class MapEdgeData
{
   
    public string target;
    public Resource_Data moveCost;//只是去目的地的成本，返回的成本在另外一个graphData中
    public float moveTimeCost;//移动用时
    public float distance;//距离，以后移动消耗和移动用时根据这个算
    [DictionaryDrawerSettings()]
    [ShowInInspector]
    public Dictionary<string,float> edgeTag;

}
[Serializable]
public class MapExtraData
{
    public int danger;

}
