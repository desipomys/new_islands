using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPrecodition_Key : Precondition_base
{
    public Dictionary<KeyCode, KeyCodeStat> allowedKey;
    public override Precondition_base Copy()
    {
        throw new System.NotImplementedException();
    }

    public override void FromObject(dynamic dy)
    {
        throw new System.NotImplementedException();
    }

    public override bool Judge(EventCenter caster, EventCenter target, object self, object[] parms)
    {
        KeyCodeKind keys = (KeyCodeKind)parms[0];
        Dictionary<KeyCode, KeyCodeStat> stats = (Dictionary<KeyCode, KeyCodeStat>)parms[1];
        foreach (var item in allowedKey)//只要有一个keycode通过就能触发
        {
            if(stats.ContainsKey(item.Key))
            {
                if (stats[item.Key] == item.Value) return true;
            }
        }
        return false;
    }
}
