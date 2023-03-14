using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可接收攻击单元，当该单元符合要求时返回攻击者物品,依赖血量组件
/// </summary>
public class ResourceObject : MonoBehaviour,IEventRegister
{
    EventCenter center;
    public Item itemToGive;//给出的物品
    public Item finalGive;//死亡后给出的物品
    public float healPerGive;//每扣多少血给一次物品
    public ToolType type;

    public void OnEventRegist(EventCenter e)
    {
        center=e;
        center.ListenEvent<ValueChangeParm<float>,EventCenter,BaseTool,Damage>(nameof(PlayerEventName.onHealthChangeBy),OnHealChg);
        center.ListenEvent<float,EventCenter,BaseTool,Damage>(nameof(PlayerEventName.onDie),OnDie);
    }
    public void AfterEventRegist()
    {

    }
   
    public void OnHealChg(ValueChangeParm<float> damage, EventCenter evc, BaseTool toolDriver,Damage du)
    {
        Debug.Log(du.value);
        if(toolDriver.type!=type)return;
        if (healPerGive == 0) return;
        int oldgive=Mathf.FloorToInt((damage.max-damage.old)/healPerGive);
        int newgive=Mathf.FloorToInt((damage.max-damage.now)/healPerGive);
        for (int i = 0; i < newgive-oldgive; i++)
        {
            evc.GetParm<Item, int>(nameof(PlayerEventName.giveItem),itemToGive);
        }
        
    }
    public void OnDie(float damage,EventCenter source,BaseTool tool,Damage du)
    {
        if(tool.type!=type)return;
        source.GetParm<Item, int>(nameof(PlayerEventName.giveItem),finalGive);  
    }
}
