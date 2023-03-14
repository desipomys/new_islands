using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePitFlameEffect : MonoBehaviour, IEventRegister
{
    public Light light;
    public ParticleSystem partical;
    void BuringChg(bool b)
    {
        light.enabled = b;

        var v = partical.emission;
        v.enabled=b;
    }
    public void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }

    public void OnEventRegist(EventCenter e)
    {
        e.ListenEvent<bool>(nameof(FirePitEventname.BuringChg), BuringChg);
    }
}
