using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Numerics;

public class MovCommandEffector_Add : Effecttor_base
{
    public override Effecttor_base Copy()
    {
        return null;
    }

    /// <summary>
    /// Add 
    ///-from	引擎名
    ///-pos  源abcdx,
    ///-targ	  目标abcdx,
    /// -ans    放结果的abcdx,可缺省
    /// </summary>
    /// <param name="parms"></param>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="self"></param>
    /// <param name="buff"></param>
    public override void Run(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        //from,pos,to,targ
        if(parms.Length==4)
        {
            string from=parms[0].ToString();
            string pos=parms[1].ToString().ToUpper();
            string to=parms[2].ToString().ToUpper();
            string ans=parms[3].ToString().ToLower();
            MovScriptEngine engfrom=EventCenter.WorldCenter.GetParm<string,MovScriptEngine>(nameof(EventNames.GetMovEngineByName),from);
            MovEngineCacheType frompos=(MovEngineCacheType)Enum.Parse(typeof(MovEngineCacheType),pos);
            MovEngineCacheType topos=(MovEngineCacheType)Enum.Parse(typeof(MovEngineCacheType),to);
            MovEngineCacheType ansPos=(MovEngineCacheType)Enum.Parse(typeof(MovEngineCacheType),ans);

            FP t1= (FP)engfrom.GetCache(frompos).Copy();
            FP t2= (FP)engfrom.GetCache(topos);
            t1.data=t1.Convert<BigInteger>()+t2.Convert<BigInteger>();
            engfrom.SetCache(ansPos,t1);
        }
        if(parms.Length==3)
        {
            string from=parms[0].ToString();
            string pos=parms[1].ToString().ToUpper();
            string to=parms[2].ToString().ToUpper();

            MovScriptEngine engfrom=EventCenter.WorldCenter.GetParm<string,MovScriptEngine>(nameof(EventNames.GetMovEngineByName),from);
            MovEngineCacheType frompos=(MovEngineCacheType)Enum.Parse(typeof(MovEngineCacheType),pos);
            MovEngineCacheType topos=(MovEngineCacheType)Enum.Parse(typeof(MovEngineCacheType),to);
            MovEngineCacheType ansPos=frompos;

            FP t1= (FP)engfrom.GetCache(frompos).Copy();
            FP t2= (FP)engfrom.GetCache(topos);
            t1.data=t1.Convert<BigInteger>()+t2.Convert<BigInteger>();
            engfrom.SetCache(ansPos,t1);
        }
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}