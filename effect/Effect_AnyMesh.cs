using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_AnyMesh : MonoBehaviour, IPoolable, IEventRegister
{
    IObjectPool pool;
    public IObjectPool Pool { get => pool; set => pool=value; }
    EventCenter center;
    float recycletime;

    public void AfterEventRegist()
    {
        
    }

    public void OnEventRegist(EventCenter e)
    {
        center = e;
        e.ListenEvent<CallEffectParm>("playEffect", playEffect);
        e.ListenEvent<Mesh,Material>("chgMesh", ChangeMesh);
    }

    public void OnPoolInit()
    {
       //throw new System.NotImplementedException();
    }

    public void OnPoolPop(float time)
    {
        gameObject.SetActive(true);
        recycletime = time;
    }

    public void OnPoolPush()
    {
        gameObject.SetActive(false);
    }

    public void OnPoolRecycle()
    {
       // throw new System.NotImplementedException();
    }

    void playEffect(CallEffectParm parm)//接收一个mesh，将自身mesh变为相同的mesh，然后自由落体
    {
        transform.position = parm.pos;
        
        if (parm.time <= 0) StartCoroutine(recycleWait(recycletime));
        else StartCoroutine(recycleWait(parm.time));
    }
    IEnumerator recycleWait(float time)
    {
        yield return new WaitForSeconds(time);
        pool.Recycle(gameObject, null);
    }
    public void ChangeMesh(Mesh m,Material mat)
    {
        GetComponent<MeshFilter>().sharedMesh = m;
        GetComponent<MeshRenderer>().sharedMaterial = mat;
        Bounds b = m.bounds;
        GetComponent<BoxCollider>().center = b.center;
        GetComponent<BoxCollider>().size = b.size;
    }
}
