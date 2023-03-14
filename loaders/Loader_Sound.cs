using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader_Sound : BaseLoader
{
    Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();
    BaseObjectPool soundSourcePool ;

    GameObject source;
    string path = "Sound/";
    string sourcePath = "Prefabs/Base/soundSource";

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        center.ListenEvent<string,Vector3>(nameof(EventNames.PlaySoundAt), PlaySoundAt);
        center.ListenEvent<string,Transform>(nameof(EventNames.PlaySoundFor), PlaySoundFollow);
    }
    public override void OnLoaderInit(int prio)
    {
        if (prio != 0) return;
       
        try
        {
 source = Resources.Load<GameObject>(sourcePath);
        soundSourcePool = new BaseObjectPool(source, EventCenter.WorldCenter.gameObject);
 AudioClip[] all = Resources.LoadAll<AudioClip>(path);
        for (int i = 0; i < all.Length; i++)
        {
            sounds.Add(all[i].name, all[i]);
        }
            Debug.Log("声音加载成功，有" + sounds.Count + "个声音");
        }
        catch (System.Exception)
        {
            Debug.Log("声音加载失败");
            
        }
    }

    AudioClip GetSoundByName(string name)
    {
        return sounds[name];
    }
    void PlaySoundAt(string soundName,Vector3 posi)
    {
       GameObject g= soundSourcePool.Pop(null);
        g.transform.position = posi;
        AudioClip c;
        if(sounds.TryGetValue(soundName,out c))
        {
            g.GetComponent<EventCenter>().SendEvent<AudioClip>("play", c);
        }
    }
    void PlaySoundFollow(string soundName, Transform t)
    {
        GameObject g = soundSourcePool.Pop(null);
        AudioClip c;
        if (sounds.TryGetValue(soundName, out c))
        {
            g.GetComponent<EventCenter>().SendEvent<AudioClip>("play", c);
            g.GetComponent<EventCenter>().SendEvent<Transform>("follow", t);
        }
    }
}
