using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieEffecter_player : MonoBehaviour,IEventRegister
{
    EventCenter center;
    
    public void AfterEventRegist()
    {
       
    }

    public void OnEventRegist(EventCenter e)
    {
        center = e;
        e.ListenEvent<float, EventCenter, BaseTool, Damage>(nameof(PlayerEventName.onDie), onDie);
        e.ListenEvent(nameof(PlayerEventName.onRespawn), respawn);

    }
    void respawn()
    {
        //将mesh的材质变回
        /*Collider[] cols = GetComponents<Collider>();
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].isTrigger = false;
        }
        GetComponent<Rigidbody>().useGravity = true;*/
        GetComponent<CharacterController>().enabled = true;
    }
    void onDie(float f, EventCenter evc, BaseTool btd, Damage d)
    {
        //将mesh的材质变红
        /*Collider[] cols=GetComponents<Collider>();
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].isTrigger = true;
        }
        GetComponent<Rigidbody>().velocity = Vector3.zero;
       GetComponent<Rigidbody>().useGravity = false;*/
        GetComponent<CharacterController>().enabled = false;
    }
}
