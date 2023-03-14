using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEParm : BaseEventArg
{
    public string type;
    public Damage dam;
    public BaseTool tool;
    public long sourceUUID;

    public PhyBullet phyBehav;

    public float time;
    public Shape3D shape;
    public float[] shapeArgs;
    public Vector3 pos;
    public Vector3 targetPos;

    public Dictionary<string,object> parms;

    /*
     *子弹预设名、伤害、伤害类型、发射工具类型、发射者
表现单元、
存在时间、AOE形状、大小参数[]，发射位置、朝向目标位置、
obj[]其他参数 
     */
}
