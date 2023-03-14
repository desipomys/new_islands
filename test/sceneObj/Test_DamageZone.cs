using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_DamageZone : MonoBehaviour
{
    public Damage dam;
    public Ticker t = new Ticker();
    public void OnTriggerStay(Collider other)
    {
        if(t.IsReady())
        {
            if(other.GetComponent<EventCenter>()!=null)
            {
                other.GetComponent<EventCenter>().SendEvent<Damage,EventCenter, BaseTool>(nameof(PlayerEventName.onDamage), dam,EventCenter.WorldCenter,null);
            }
        }
    }
}
