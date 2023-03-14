using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovCommandEffector_Mov : Effecttor_base
{
    public override Effecttor_base Copy()
    {
        return null;
    }

    /// <summary>
    /// Mov 
    ///-from	引擎名
    ///-pos  源abcdx,
    ///-to		引擎名[可缺省]
    ///-targ	  目标abcdx,
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
            string pos=parms[1].ToString().ToLower();
            string to=parms[2].ToString();
            string targ=parms[3].ToString().ToLower();
            MovScriptEngine engfrom=EventCenter.WorldCenter.GetParm<string,MovScriptEngine>(nameof(EventNames.GetMovEngineByName),from);
            MovScriptEngine engto=EventCenter.WorldCenter.GetParm<string,MovScriptEngine>(nameof(EventNames.GetMovEngineByName),to);
            MovEngineCacheType frompos,topos;
            switch (pos)
            {
                case "ax":frompos=MovEngineCacheType.AX;
                break;
                case "bx":frompos=MovEngineCacheType.BX;
                break;
                case "cx":frompos=MovEngineCacheType.CX;
                break;
                case "dx":frompos=MovEngineCacheType.DX;
                break;
                default:frompos=MovEngineCacheType.AX;break;
            }
            switch (targ)
            {
                case "ax":topos=MovEngineCacheType.AX;
                break;
                case "bx":topos=MovEngineCacheType.BX;
                break;
                case "cx":topos=MovEngineCacheType.CX;
                break;
                case "dx":topos=MovEngineCacheType.DX;
                break;
                default:topos=MovEngineCacheType.AX;break;
            }
            engto.SetCache(topos,(FP)engfrom.GetCache(frompos).Copy());
        }
        if(parms.Length==3)
        {
             string from=parms[0].ToString();
            string pos=parms[1].ToString().ToLower();
            string targ=parms[2].ToString().ToLower();
            MovScriptEngine engfrom=EventCenter.WorldCenter.GetParm<string,MovScriptEngine>(nameof(EventNames.GetMovEngineByName),from);
            MovEngineCacheType frompos,topos;
            switch (pos)
            {
                case "ax":frompos=MovEngineCacheType.AX;
                break;
                case "bx":frompos=MovEngineCacheType.BX;
                break;
                case "cx":frompos=MovEngineCacheType.CX;
                break;
                case "dx":frompos=MovEngineCacheType.DX;
                break;
                default:frompos=MovEngineCacheType.AX;break;
            }
            switch (targ)
            {
                case "ax":topos=MovEngineCacheType.AX;
                break;
                case "bx":topos=MovEngineCacheType.BX;
                break;
                case "cx":topos=MovEngineCacheType.CX;
                break;
                case "dx":topos=MovEngineCacheType.DX;
                break;
                default:topos=MovEngineCacheType.AX;break;
            }
            engfrom.SetCache(topos,(FP)engfrom.GetCache(frompos).Copy());
        }
    }

    public override void Undo(object[] parms, EventCenter caster, EventCenter[] target, EventCenter self, object buff)
    {
        throw new System.NotImplementedException();
    }
}