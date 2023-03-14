using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovCommandEffector_Load : Effecttor_base
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
        
        if(parms.Length==3)
        {
            string name=parms[0].ToString();
            string key=parms[1].ToString();
            string pos=parms[2].ToString().ToUpper();
            
            MovScriptEngine engfrom=EventCenter.WorldCenter.GetParm<string,MovScriptEngine>(nameof(EventNames.GetMovEngineByName),name);
            MovEngineCacheType mpos=(MovEngineCacheType)Enum.Parse(typeof(MovEngineCacheType),pos);

            engfrom.SetCache(mpos,engfrom.GetData(key));

        }
        else if(parms.Length==2)
        {
            string key=parms[0].ToString();
            string pos=parms[1].ToString().ToUpper();
            
            MovEngineCacheType mpos=(MovEngineCacheType)Enum.Parse(typeof(MovEngineCacheType),pos);

            MovScriptEngine engfrom=(MovScriptEngine)buff;
            //engfrom.SetData(key,engfrom.GetCache(mpos).data);
            engfrom.SetCache(mpos,engfrom.GetData(key));
        }
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}