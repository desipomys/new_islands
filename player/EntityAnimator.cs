using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画帧事件
/// </summary>
public enum AnimationEventNames
{
    OnFistHit1,OnFistHit2,OnGunFire,//玩家攻击动画
    OnAttack
    
}

public class EntityAnimator : MonoBehaviour,IEventRegister
{
    EventCenter evc;
    protected EventCenter center { get { if (evc == null) evc = GetComponent<EventCenter>();return evc; }set { evc = value; } }
    public virtual void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }

    public virtual void OnEventRegist(EventCenter e)
    {
       center=e;
    }

    public virtual void Play(string name)
    {

    }
    private void Update()
    {
        
    }

    public virtual void OnAnimationEvent(string eventEnum)
    {
        center.SendEvent<string>(nameof(PlayerEventName.onAnimationEvent),eventEnum);
    }

    public override string ToString()
    {
        //返回dic的序列化数据，dic key为组件名，value为调用组件tostring的值
        return null;
    }
    public virtual void FromString(string data)
    {
        //序列化为dic,用此字典调用所有idatacontainer组件的fromstring
        //组件自取key对应的value
    }
}
