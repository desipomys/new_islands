using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 加载scObj
/// </summary>
public class Loader_Tool : BaseLoader
{
    string toolpath = "SC/Tool/Tool";
    Dictionary<string, BaseTool> tools = new Dictionary<string, BaseTool>();//要改成对象池，存的都是已经init好的obj
    BaseTool barehand;

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);

        e.RegistFunc<string, BaseTool>(nameof(EventNames.GetToolByName), GetToolByName);
        e.RegistFunc<string,BaseTool>(nameof(EventNames.GetBareHand), GetBareHand);
        e.RegistFunc<Item,bool>(nameof(EventNames.IsBareHand), IsBareHand);
    }

    public override void OnLoaderInit(int prio)
    {
        if (prio != 0) return;
        try
        {
            tools.Clear();
            Process(Load());
            Debug.Log("tool加载完成，有"+tools.Count+"个tool(不包括barehand)");
        }
        catch (System.Exception)
        {

            Debug.Log("tool加载失败");
            throw;
        }


    }
    BaseTool[] Load()
    {
        barehand = Resources.Load<BaseTool>("SC/Tool/bareHand");
        return Resources.LoadAll<BaseTool>(toolpath);
    }
    void Process(BaseTool[] bt)
    {
        for (int i = 0; i < bt.Length; i++)
        {
            tools.Add(bt[i].name, bt[i]);
        }
    }

   
    BaseTool GetToolByName(string i)
    {
        BaseTool temp;
        if (tools.TryGetValue(i, out temp))
        {
            ///临时措施，性能不佳
            temp = DataExtension.DeepCopy(temp);
            return temp;
        }
        else
        {
            return null;
        }
    }
    BaseTool GetBareHand(string barehandName)
    {
        return barehand;
    }
    bool IsBareHand(Item i)
    {
        BaseTool temp;
        if (!Item.IsNullOrEmpty(i)&&tools.TryGetValue(i.GetItemName(), out temp))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
