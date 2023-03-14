using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBlockHealth_Tree : BaseEBlockHealth
{
    public override void OnDamage(Damage d, EventCenter source, BaseTool tool)
    {
        //base.OnDamage(d, source, tool);
        //Debug.Log(d);
        if (tool.type == ToolType.axe && !isDead)
        {
            hparm.max = maxHealth;
            hparm.old = health;
            hparm.now = Mathf.Clamp(health - d.value, 0, maxHealth);
            health = hparm.now;

            center.SendEvent<ValueChangeParm<float>, EventCenter, BaseTool, Damage>(nameof(PlayerEventName.onHealthChangeBy), hparm, source, tool, d);

            if (hparm.now <= 0)
            {
                Debug.Log("die");
                isDead = true;
                center.SendEvent<float, EventCenter, BaseTool, Damage>(nameof(PlayerEventName.onDie), d.value, source, tool, d);
            }
        }
    }
}
