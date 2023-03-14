using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityBodyPosition
{
    hand,head,foot,center
}

public class ToolEffector_SoundEffect : ToolEffector
{
    public EntityBodyPosition position;
    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

   

    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        for (int i = 0; i < target.Length; i++)
        {
            FP soundName = valueSource.Get(caster, target[i], buff, parms);
        Vector3 pos=caster.transform.position;
            switch (position)
            {
                case EntityBodyPosition.hand:
                    pos = caster.GetParm<Vector3>(nameof(PlayerEventName.getMainHandPos));

                    break;
                case EntityBodyPosition.head:

                    break;
                case EntityBodyPosition.foot:
                    break;
                case EntityBodyPosition.center:
                    break;
                default:
                    break;
            }
            EventCenter.WorldCenter.SendEvent<string, Vector3>(nameof(EventNames.PlaySoundAt), soundName.Convert<string>(),pos);
        }
        
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
