using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 带飞行特效子弹，用于基本枪械
/// </summary>
public class Bullet_base : MonoBehaviour,IPoolable,IEventRegister
{
    //1.沿指定方向移动
    //2.对碰撞物体施加damage
    //3.播放相应撞击特效
    //4.回收自己
    Damage dam;
    EventCenter source;
    BaseTool tool;
    Vector3 startPos;
    float range;
    //public string type;
    IObjectPool pool;
    CallEffectParm hiteffect=new CallEffectParm();


    public IObjectPool Pool { get => pool; set => pool=value; }

    public void OnPoolInit()
    {
        //throw new System.NotImplementedException();
    }

    public void OnPoolPush()
    {
        gameObject.SetActive(false);
        //throw new System.NotImplementedException();
    }

    public void OnPoolRecycle()
    {
        
        //throw new System.NotImplementedException();
    }

   
    public void onFire(BulletParm parm)
    {
        
        transform.position = parm.pos;
        startPos = parm.pos;
        range = parm.range;
        GetComponent<Rigidbody>().velocity = parm.GetSpeed();
        dam = parm.dam;
         GameObject g = EventCenter.WorldCenter.GetParm<long, GameObject>(nameof(EventNames.GetEVCbyUID), parm.sourceUUID);
        if (g != null) source=g.GetComponent<EventCenter>();
        tool = parm.tool;
        GetComponent<TrailRenderer>().Clear();

        StartCoroutine(RangeRestrict(parm.speed/(parm.range*1.2f)));
    }
    IEnumerator RangeRestrict(float tim)
    {
        yield return new WaitForSeconds(tim);
        pool.Recycle(gameObject, null);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ((collision.contacts[0].point - startPos).magnitude > (range * 1.2f)) { pool.Recycle(gameObject, null); return; }
        EventCenter e=collision.gameObject.GetComponent<EventCenter>();
        if (e != null)
        {
            e.SendEvent<Damage, EventCenter, BaseTool>(nameof(PlayerEventName.onDamage), dam, source, tool);
            //如果伤害计算完有剩余应该穿透
        }
        PlayEffect(e, collision);
        pool.Recycle(gameObject,null);
    }

    public void OnPoolPop(float time)
    {
        gameObject.SetActive(true);
        
    }

    EventCenter center;
    public void OnEventRegist(EventCenter e)
    {
        center = e;
        e.ListenEvent<BulletParm>("onFire", onFire);
    }

    public void AfterEventRegist()
    {
        
    }
    void PlayEffect(EventCenter e,Collision collision)
    {
               hiteffect.pos= collision.contacts[0].point;
        hiteffect.dir = Vector3.left;//hiteffect.pos - transform.position;
        hiteffect.type = JudgeHitEffect( e,collision); //根据碰撞物体类型和自身携带伤害值选择撞击特效
        EventCenter.WorldCenter.GetParm<CallEffectParm, GameObject>(nameof(EventNames.PlayEffect), hiteffect);
        
    }
    string JudgeHitEffect(EventCenter e, Collision c)
    {
        bool avalible=false;
        if (e != null)
        {
            EntityType et = e.GetParm<EntityType>(nameof(PlayerEventName.entity_type), out avalible);
            if (avalible)//撞击实体
            {
                
                switch (et.GetSubType())
                {
                    case EntityType.Human:
                    case EntityType.Devil:
                        return "FX_BulletHitMeat";
                        break;
                    case EntityType.Machine:
                        return "FX_BulletHitIronBig";
                        break;
                    case EntityType.Bug://虫子应该爆绿或黄血
                        return "FX_BulletHitDirt";
                        break;
                    default:
                        return "FX_BulletHitDirt";
                        break;
                }

            }
            else
            {
                T_Material tm = e.GetParm<T_Material>("getBlockMaterial", out avalible);//获取撞击方块的材质,未实装
                if (avalible)
                {

                }

                return "FX_BulletHitDirt";
            }
        }
        return "FX_BulletHitDirt";
    }

}
