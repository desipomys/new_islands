using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������
/// </summary>
public class ToolPrecodition_LongKey : Precondition_base
{
    public Dictionary<KeyCode,float> allowedKey;
    
    Dictionary<KeyCode, float> pressedTime;

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
        Dictionary<KeyCode, KeyCodeStat> stats = (Dictionary<KeyCode, KeyCodeStat>)parms[1];
        foreach (var item in allowedKey)//ֻҪ��һ��keycodeͨ�����ܴ���
        {
            if (stats.ContainsKey(item.Key))
            {
                if (stats[item.Key] == KeyCodeStat.keep)
                {
                    pressedTime[item.Key] = pressedTime[item.Key] + Time.deltaTime;
                }
            }
        }
        bool succ = false;
        foreach (var item in pressedTime)
        {
            if(allowedKey[item.Key]<=item.Value)//ֻҪ��һ����ʱ����ɹ�
            {
                pressedTime[item.Key] = 0;
                succ = true;
            }
        }
        return succ;
    }
}
