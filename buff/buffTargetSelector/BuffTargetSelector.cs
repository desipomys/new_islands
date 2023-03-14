using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum SelectorBase
{
    none,source,target
}
public enum GeomeSelector
{
    self,box,sphere
}

public abstract class TargetSelector
{
    public abstract EventCenter[] Select(EventCenter source,EventCenter target,object self, object[] rms);//object[]是事件到来的参数
}


[System.Serializable]
public class BuffTargetSelector :TargetSelector
{
    [LabelText("选择器目标")]
    public SelectorBase selectorBase;
    [LabelText("几何模式")]
    [LabelWidth(60)]
    public GeomeSelector geomeSelector;
    [LabelText("半径")]
    [LabelWidth(30)]
    public float radius;

    public BuffTargetSelector() { selectorBase = SelectorBase.none;geomeSelector = GeomeSelector.self; }

   public override EventCenter[] Select(EventCenter source,EventCenter target,object self, object[] rms)//object[]是事件到来的参数
    {
        EventCenter temp=source;
        switch (selectorBase)
        {
            case SelectorBase.none:
                break;
            case SelectorBase.source:
                temp = source;
                break;
            case SelectorBase.target:
                temp = target;
                break;
            default:
                break;
        }

        List<EventCenter> ans = new List<EventCenter>();
        switch (geomeSelector)
        {
            case GeomeSelector.self:ans.Add(temp);
                break;
            case GeomeSelector.box:
               RaycastHit[] hits= Physics.BoxCastAll(temp.transform.position, Vector3.one * radius,Vector3.forward);

                break;
            case GeomeSelector.sphere:
                break;
            default:
                break;
        }

        return ans.ToArray();
    }

    public void FromObject(dynamic dy)
    {
        selectorBase = (SelectorBase)dy.selectorBase;
        geomeSelector = (GeomeSelector)dy.geomeSelector;
        radius = (float)dy.radius;
    }
    public virtual BuffTargetSelector Copy()
    {
        BuffTargetSelector temp = new BuffTargetSelector();
        temp.radius = radius;
        temp.geomeSelector = geomeSelector;
        temp.selectorBase = selectorBase;
        return temp;
    }

    public virtual void OnBuffInit(EventCenter holder,BaseBuff bf)
    {
        
    }
}
