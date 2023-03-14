using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单纯碰撞体子弹，不带飞行特效，用于近程武器
/// </summary>
public class Bullet_collider : MonoBehaviour, IPoolable, IEventRegister
{
    Damage dam;
    EventCenter source;
    BaseTool tool;
    public string type;
    public Transform target;
    IObjectPool pool;
    CallEffectParm hiteffect = new CallEffectParm();

    HashSet<Collider> colliders = new HashSet<Collider>();

    public IObjectPool Pool { get => pool; set => pool = value; }

    public void AfterEventRegist()
    {

    }
    EventCenter center;
    public void OnEventRegist(EventCenter e)
    {
        center = e;
        e.ListenEvent<BulletParm>("onFire", onFire);
    }

    public void OnPoolInit()
    {

    }

    public void OnPoolPop(float time)
    {
        Debug.Log("激活");
        gameObject.SetActive(true);
        colliders.Clear();
    }

    public void OnPoolPush()
    {
        gameObject.SetActive(false);
        if (target != null) transform.SetParent(EventCenter.WorldCenter.transform);
        target = null;
    }

    public void OnPoolRecycle()
    {

    }

    public void onFire(BulletParm parm)
    {
        /*Debug.Log("onfire");
        transform.position = parm.pos;
        dam = parm.dam;
        target = parm.tool.transform;
        source = EventCenter.WorldCenter.GetParm<long, GameObject>("GetEntityByIndex", parm.sourceUUID).GetComponent<EventCenter>();
        tool = parm.tool;
        StartCoroutine(recycleWait(parm.exittime));*/
    }
    IEnumerator recycleWait(float t)
    {
        yield return new WaitForSeconds(t);
        pool.Recycle(gameObject,null);
    }


    public void OnTriggerEnter(Collider other)
    {

        Debug.Log("collid");
        if (!colliders.Contains(other))
        {
            EventCenter e = other.gameObject.GetComponent<EventCenter>();
            if (e != null && e.UUID != source.UUID)
            {
                Debug.Log("有evc");
                e.SendEvent<Damage, EventCenter, BaseTool>(nameof(PlayerEventName.onDamage), dam, source, tool);
                PlayEffect(other);
            }
            else if(e==null)
            {
                Debug.Log("无evc");
                PlayEffect(other);
            }
            Debug.Log("与" + other.gameObject.name + "碰撞");
            colliders.Add(other);
        }

    }
    void PlayEffect(Collider collider)
    {
        hiteffect.pos = collider.ClosestPoint(transform.position);
        hiteffect.dir = hiteffect.pos - transform.position;
        hiteffect.type = type;
        EventCenter.WorldCenter.GetParm<CallEffectParm, GameObject>(nameof(EventNames.PlayEffect), hiteffect);
    }
}
