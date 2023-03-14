using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Eblock : MonoBehaviour, IInterectable,IEventRegister
{
    EventCenter center;
    public void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }

    public void OnEventRegist(EventCenter e)
    {
        //throw new System.NotImplementedException();
        center = e;
    }

    public int OnInterect(EventCenter source, InteractType type)
    {
        center.SendEvent<EventCenter, InteractType>("interact",source,type);
        
        Vector3 handpos = source.GetParm<Transform>(nameof(PlayerEventName.getMainHand)).position;//获取拾取者的手判断拾取距离
        if (Mathf.Abs(handpos.y - transform.position.y) > 0.5f)
        {
            if (handpos.y - transform.position.y > 0)
            {
                return 3;//下拾取
            }
            else return 2;//上拾取
        }
        return 1;
    }
}
