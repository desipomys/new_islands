//地图元数据，记载哪里是矿区哪里是建筑群哪里是任务点
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 地图预设特殊地点生成数据
/// </summary>
public class MapPointMetaData
{
    /// <summary>
    /// 中心位置
    /// </summary>
    public Vector3Int pos { get; set; }//生成前为空，地图生成完后才有值
    public Vector3Int size { get; set; }//不指定具体位置，生成地图时才指定
    public MapMetaShape shape;
    public int level;//规模
    public bool flatGround;//生成时是否需平整地面
    public MapMetaType type;
    public MapMetaProper proper;
    public string name;

    //生成条件
    public MapGenCondition condition;
}
/// <summary>
/// 地图预设建筑的生成条件
/// </summary>
public enum MapGenCondition
{
    /// <summary>
    /// 平地
    /// </summary>
    flatarea,
    /// <summary>
    /// 山谷
    /// </summary>
    valley,
    /// <summary>
    /// 山顶
    /// </summary>
    moutaintop,
    //nearwater
}
public enum MapMetaShape
{
    sphere,rect
}
public enum MapMetaType
{
    //地点内容
    none,
    ore,//资源区
    building,//独栋建筑
    vilige//建筑群

}
public enum MapMetaProper
{
    //地点属性
    none,
    landPoint,
    friendHome,
    tacticalPoint,
    enemyHome
}

//楼房层数
//矿物规模
//村落规模
