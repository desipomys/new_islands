using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Light : MonoBehaviour, IPoolable, IEventRegister
{
    IObjectPool pool;
    public IObjectPool Pool { get => pool; set => pool = value; }
    EventCenter center;
    float recycletime;

    public void OnPoolInit()
    {
        
    }

    public void OnPoolPush()
    {
        gameObject.SetActive(false);
    }

    public void OnPoolPop(float time)
    {
        gameObject.SetActive(true);
        recycletime = time;
    }

    public void OnPoolRecycle()
    {
       
    }

    public void OnEventRegist(EventCenter e)
    {
        center = e;
        e.ListenEvent<CallEffectParm>("playEffect", playEffect);
    }
    void playEffect(CallEffectParm parm)//接收一个mesh，将自身mesh变为相同的mesh，然后自由落体
    {
        transform.position = parm.pos;
        GetComponent<Light>().color = (Color)parm.data;

        if (parm.time <= 0) StartCoroutine(recycleWait(recycletime));
        else StartCoroutine(recycleWait(parm.time));
    }
    IEnumerator recycleWait(float time)
    {
        yield return new WaitForSeconds(time);
        pool.Recycle(gameObject, null);
    }
    public void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }
}
