using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Player : MonoBehaviour,IEventRegister,IPoolable
{
    Transform target;
    AudioSource source;
    EventCenter center;
    IObjectPool pool;
    public IObjectPool Pool { get => pool; set => pool=value; }

    public void AfterEventRegist()
    {
        //hrow new System.NotImplementedException();
    }

    public void OnEventRegist(EventCenter e)
    {
        // throw new System.NotImplementedException();
        center = e;
        source = GetComponent<AudioSource>();
        center.ListenEvent<AudioClip>("play", Play);
        center.ListenEvent<Transform>("follow", (Transform t) => { target = t; });
    }

    public void OnPoolInit()
    {
        //throw new System.NotImplementedException();
    }

    public void OnPoolPop(float time)
    {
        //throw new System.NotImplementedException();
        //StartCoroutine(cycleWait(time));
        gameObject.SetActive(true);
    }
    IEnumerator cycleWait(float time)
    {
        yield return new WaitForSeconds(time);
        pool.Recycle(gameObject,null);
    }

    public void OnPoolPush()
    {
        target = null;
        gameObject.SetActive(false);
        //throw new System.NotImplementedException();
    }

    public void OnPoolRecycle()
    {
        //throw new System.NotImplementedException();
    }
    public void Update()
    {
        if (!source.isPlaying) pool.Recycle(gameObject,null);
        if (target == null) return;
        transform.position = target.position;
    }
    public void Play(AudioClip clip)
    {
        //source.clip=clip;
        //source.Play();
        source.PlayOneShot(clip);
        //source.on
    }
}
