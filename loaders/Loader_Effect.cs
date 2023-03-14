using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader_Effect : BaseLoader
{
    //对象池管理特效
    public Dictionary<string, BaseObjectPool> effectPools = new Dictionary<string, BaseObjectPool>();
    string path = "Prefabs/Effect";

    public override void OnLoaderInit(int prio)
    {
        if (prio != 0) return;
        base.OnLoaderInit(prio);

        try
        {
            GameObject[] effects = Resources.LoadAll<GameObject>(path);
            for (int i = 0; i < effects.Length; i++)
            {
                effectPools.Add(effects[i].name, new BaseObjectPool(effects[i], EventCenter.WorldCenter.gameObject));
            }
            Debug.Log("effect加载成功,有" + effectPools.Count + "个特效被加载");
        }
        catch (System.Exception)
        {
            Debug.Log("effect加载失败");
        }

    }
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        center.RegistFunc<string, GameObject>(nameof(EventNames.GetEffect), GetEffect);
        center.RegistFunc<CallEffectParm, GameObject>(nameof(EventNames.PlayEffect), PlayEffect);
        center.RegistFunc<string, GameObject>(nameof(EventNames.GetEffectPrefabs), GetEffectPrefabs);
    }

    GameObject GetEffectPrefabs(string name)
    {
        GameObject g = GameMainManager.CreateGameObject(effectPools[name].instance);
        return g;
    }
    GameObject GetEffect(string name)
    {
        return effectPools[name].Pop(null);
    }
    GameObject PlayEffect(CallEffectParm parm)
    {
        GameObject t = effectPools[parm.type].Pop(null);
        Debug.Log("播放特效"+t.name);
        t.GetComponent<EventCenter>().SendEvent<CallEffectParm>("playEffect", parm);
        return t;
    }
}
