using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_None : MonoBehaviour, IPoolable, IEventRegister
{
    IObjectPool pool;
    public IObjectPool Pool { get => pool; set => pool = value; }
    EventCenter center;
    public Transform target;
    float recycletime;

    public void AfterEventRegist()
    {

    }

    public void OnEventRegist(EventCenter e)
    {
        center = e;
        e.ListenEvent<CallEffectParm>("playEffect", playEffect);
    }

    public void OnPoolInit()
    {

    }

    public void OnPoolPop(float time)
    {
        gameObject.SetActive(true);
        recycletime = time;
    }
    IEnumerator recycleWait(float time)
    {
        yield return new WaitForSeconds(time);
        pool.Recycle(gameObject,null);
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

    void playEffect(CallEffectParm parm)
    {
        transform.position = parm.pos;
        transform.LookAt(parm.pos + parm.dir);

        target = parm.target;
        if (parm.speed != null)
        {
            Rigidbody rg = GetComponent<Rigidbody>();
            if (rg != null)
            {
                rg.velocity = parm.speed;
            }
        }
        if (target != null) { transform.SetParent(target); transform.localPosition = Vector3.zero; }
        if (parm.time <= 0) StartCoroutine(recycleWait(recycletime));
        else StartCoroutine(recycleWait(parm.time));
    }
    private void Update()
    {
        //if (target == null) return;
        //transform.position = target.position;
    }
}
