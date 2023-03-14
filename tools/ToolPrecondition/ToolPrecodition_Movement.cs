using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPrecodition_Movement : Precondition_base
{
    public Movement_Stat[] allowedStat;
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
        Movement mv = caster.GetComponent<Movement>();
        Movement_Stat stat = mv.stat;
        bool ans = false;
        for (int i = 0; i < allowedStat.Length; i++)//只要有一个状态被允许则返回true
        {
            if (stat == allowedStat[i]) { ans = true;return ans; }
        }
        return false;
    }
}
