using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_ParticleBase : MonoBehaviour, IPoolable, IEventRegister
{
    IObjectPool pool;
    public IObjectPool Pool { get => pool; set => pool=value; }
    EventCenter center;
    Transform target;

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
        StartCoroutine(recycleWait(time));
    }
    IEnumerator recycleWait(float time)
    {
        yield return new WaitForSeconds(time);
        pool.Recycle(gameObject,null);
    }

    public void OnPoolPush()
    {
        gameObject.SetActive(false);
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

        ParticleSystem p = GetComponent<ParticleSystem>();
        if (p != null) p.Play();
    }
    private void Update()
    {
        if (target == null) return;
        transform.position = target.position;
    }
}
