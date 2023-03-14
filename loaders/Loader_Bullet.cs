using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 提供按名字+参数生成bullet并启用的功能，你只能让bullet携带数据出发，不能定义其撞击等事件的响应方法
/// </summary>
public class Loader_Bullet : BaseLoader
{
    //对象池管理子弹
    public Dictionary<string, BaseObjectPool> bulletPools = new Dictionary<string, BaseObjectPool>();
    string path = "Prefabs/bullet";

    public override void OnLoaderInit(int prio)
    {
        if (prio != 0) return;
        base.OnLoaderInit(prio);

        try
        {
            GameObject[] bullets = Resources.LoadAll<GameObject>(path);
            for (int i = 0; i < bullets.Length; i++)
            {
                bulletPools.Add(bullets[i].name, new BaseObjectPool(bullets[i], EventCenter.WorldCenter.gameObject));
            }
            Debug.Log("bullet加载成功,有" + bulletPools.Count + "个子弹被加载");
        }
        catch (System.Exception)
        {
            Debug.Log("bullet加载失败");
        }


    }
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        center.RegistFunc<string, GameObject>(nameof(EventNames.GetBullet), GetBullet);
        center.RegistFunc<BulletParm, GameObject>(nameof(EventNames.FireBullet), FireBullet);
    }

    GameObject GetBullet(string name)
    {
        return bulletPools[name].Pop(null);
    }
    GameObject FireBullet(BulletParm parm)
    {
        GameObject t = bulletPools[parm.type].Pop(null);
        Debug.Log("发射子弹");
        t.GetComponent<EventCenter>().SendEvent<BulletParm>("onFire", parm);
        return t;
    }
    void SimpleCollider(BulletParm parm)
    {
        LayerMask lay = LayerMask.GetMask("enemy", "neutral", "friend", "player", "Default");
        switch (parm.type)
        {
            case "sphereCollider":
               RaycastHit[] all= Physics.SphereCastAll(parm.pos, parm.range, Vector3.up,100,lay);
                for (int i = 0; i < all.Length; i++)
                {
                    if(all[i].collider.GetComponent<EventCenter>().UUID!=parm.sourceUUID)
                    {
                        //all[i].collider.GetComponent<EventCenter>().
                    }
                }
                break;
            case "boxCollider":
                RaycastHit[] all1 = Physics.BoxCastAll(parm.pos, parm.range*Vector3.one, Vector3.up,Quaternion.identity, 100, lay);
                for (int i = 0; i < all1.Length; i++)
                {
                    if (all1[i].collider.GetComponent<EventCenter>().UUID != parm.sourceUUID)
                    {
                        //all[i].collider.GetComponent<EventCenter>().
                    }
                }
                break;
            default:
                break;
        }
        
    }
}
