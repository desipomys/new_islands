using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolEffector_Animation : ToolEffector
{
    public override Effecttor_base Copy()
    {
        throw new System.NotImplementedException();
    }

    

    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        EntityAnimator animator= self.GetComponent<EntityAnimator>();
        string nam = valueSource.Get(caster, target[0], buff, parms);
        animator.Play(nam);
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}
