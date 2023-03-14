using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovCommandEffector_Debug : Effecttor_base
{
    public override Effecttor_base Copy()
    {
        return null;
    }

    /// <summary>
   
    /// </summary>
    /// <param name="parms"></param>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="self"></param>
    /// <param name="buff"></param>
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        //from,pos,to,targ
        for (int i = 0; i < parms.Length; i++)
        {
            Debug.Log(parms[i]);
        }
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}