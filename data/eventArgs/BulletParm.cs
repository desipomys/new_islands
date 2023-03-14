using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;


public enum Shape2D
{
    box,circle
}
public enum Shape3D
{
    cube,Sphere,cylind,
}

public class BulletParm:BaseEventArg
{
    //要求所有evenarg的子类都可以序列化在网络传输
    public string type;
    public Damage dam;
    public BaseTool tool;
    public long sourceUUID;

    public PhyBullet phyBehav;

    public int num;
    public float range;
    public float speed;
    public float spreadRadius;
    public Shape2D shape;
    public float exittime;
    public Vector3 pos;
    public Vector3 targetPos;
    /// <summary>
    /// 子弹预设名
    /// </summary>

    //子弹预设名、伤害、伤害类型、发射工具类型、发射者
    //表现单元、
    //射程、速度、散率、发射位置、目标位置、

    public Dictionary<string,object> parms;
    //public Action<Collision> hitCallback;//获取返回的bulletgameobject后自己监听其collision事件

    public Vector3 GetSpeed()
    {
        Vector3 fromto = targetPos - pos;
        Vector2 temp;
        switch (shape)
        {
            case Shape2D.box:
                temp = new Vector2( UnityEngine.Random.value * spreadRadius, UnityEngine.Random.value * spreadRadius);
                break;
            case Shape2D.circle:
                temp = UnityEngine.Random.insideUnitCircle * spreadRadius;
                break;
            default:
                temp = Vector2.one;
                break;
        }
        //法线点
        
        Vector3 norm =  (targetPos-pos).normalized;
        Vector3 planeX ;
        Vector3 planeY;
        float x1 = -norm.x;
        float z1 = -norm.z;
        if(Mathf.Abs(norm.y)<0.00001f)
        {
            planeY = Vector3.up;
            planeX = Vector3.Cross(planeY, norm);
        }
        else
        {
            float y = (-norm.x*x1-norm.z*z1) / norm.y ;
            Debug.Log(y);
            planeX = new Vector3(x1,y,z1).normalized;
            planeY = Vector3.Cross(norm, planeX).normalized;
        }
        Debug.Log(planeX   );
        Debug.Log(planeY);
        //求以fromto作为法向量的平面的两个基，*temp.x,temp.y
        return fromto+(planeX*temp.x+planeY*temp.y);
    }

    public override string ToString()
    {
        
        return JsonConvert.SerializeObject(this,JsonSetting.serializerSettings);
    }
    public static BulletParm FromString(string s)
    {
        
        return JsonConvert.DeserializeObject<BulletParm>(s, JsonSetting.serializerSettings);
    }
}