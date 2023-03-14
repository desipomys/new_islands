using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovCommandEffector_JMP : Effecttor_base
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
        
        if(parms.Length==2)
        {
            string name=parms[0].ToString();
            string ind=parms[1].ToString().ToUpper();
            MovScriptEngine engfrom=EventCenter.WorldCenter.GetParm<string,MovScriptEngine>(nameof(EventNames.GetMovEngineByName),name);
            
            int indexInt;
            if(int.TryParse(ind,out indexInt))
            {

            }
            else
            {
                if(!engfrom.aliasIndex.ContainsKey(ind))throw new Exception("eng没有此index");
                indexInt=engfrom.aliasIndex[ind];
            }
           engfrom.Next(indexInt);

        }
        else if(parms.Length==1)
        {
            string ind=parms[1].ToString().ToUpper();
            MovScriptEngine engfrom=(MovScriptEngine)buff;
           int indexInt;
            if(int.TryParse(ind,out indexInt))
            {

            }
            else
            {
                if(!engfrom.aliasIndex.ContainsKey(ind))throw new Exception("eng没有此index");
                indexInt=engfrom.aliasIndex[ind];
            }
           engfrom.Next(indexInt);
        }
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}