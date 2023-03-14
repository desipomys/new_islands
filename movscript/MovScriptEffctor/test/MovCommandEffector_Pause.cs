using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovCommandEffector_Pause : Effecttor_base
{
    public override Effecttor_base Copy()
    {
        return null;
    }

    /// <summary>
    /// set 
    ///-name  引擎名
    ///-pos  源abcdx,
    ///-value	
    /// </summary>
    /// <param name="parms"></param>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="self"></param>
    /// <param name="buff"></param>
   public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        //from,pos,to,targ
        
        if(parms.Length==1)
        {
            string name=parms[0].ToString();
            MovScriptEngine engfrom=EventCenter.WorldCenter.GetParm<string,MovScriptEngine>(nameof(EventNames.GetMovEngineByName),name);
            engfrom.OnPause();

        }
        else if(parms.Length==0)
        {
            MovScriptEngine engfrom=(MovScriptEngine)buff;
            engfrom.OnPause();
        }
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}