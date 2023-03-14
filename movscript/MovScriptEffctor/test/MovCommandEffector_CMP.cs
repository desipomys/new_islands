using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovCommandEffector_CMP : Effecttor_base
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
            string pos=parms[1].ToString().ToUpper();
            string tar=parms[2].ToString().ToUpper();
            
            MovScriptEngine engfrom=EventCenter.WorldCenter.GetParm<string,MovScriptEngine>(nameof(EventNames.GetMovEngineByName),name);
            MovEngineCacheType mpos=(MovEngineCacheType)Enum.Parse(typeof(MovEngineCacheType),pos);
            MovEngineCacheType tpos=(MovEngineCacheType)Enum.Parse(typeof(MovEngineCacheType),tar);

            int ans=(engfrom.GetCache(mpos).data==engfrom.GetCache(tpos).data)?1:0;
            engfrom.SetCache(MovEngineCacheType.ans,ans);
        
        }
        else if(parms.Length==2)
        {
            string pos=parms[0].ToString().ToUpper();
            string tar=parms[1].ToString().ToUpper();
            
            MovEngineCacheType mpos=(MovEngineCacheType)Enum.Parse(typeof(MovEngineCacheType),pos);
            MovEngineCacheType tpos=(MovEngineCacheType)Enum.Parse(typeof(MovEngineCacheType),tar);

            MovScriptEngine engfrom=(MovScriptEngine)buff;
            
            int ans=(engfrom.GetCache(mpos).data==engfrom.GetCache(tpos).data)?1:0;
            engfrom.SetCache(MovEngineCacheType.ans,ans);
        }
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}