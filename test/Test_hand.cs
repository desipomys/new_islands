using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 测试新tool系统的桩模块，用于接收tool回传事件
/// </summary>
/*public class Test_hand : PlayerHand
{
    public GameObject gg;
    BaseTool[] tools;
    EventCenter center;
    public void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }

    public void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        center.ListenEvent<string, int>(nameof(PlayerEventName.playAnima), PlayAnima);
        center.ListenEvent<int, BaseTool>(nameof(PlayerEventName.onHandChgTool), OnChangeTool);
        //throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Awake()
    {

        GameMainManager.CreateGameObject(gg);
    }
    protected override void OnKey(KeyCodeKind keys, Dictionary<KeyCode, KeyCodeStat> stats)
    {
        //base.OnKey(keys, stats);
        Tools[currentToolIndex].OnUse(center, keys, stats);
    }
    // Update is called once per frame

    void PlayAnima(string nam, int layer)
    {
        Debug.Log("playAnim:" + name);
    }
    void OnChangeTool(int index, BaseTool tool)
    {
        //anim.SetInteger("handState", (int)(tool.idleAnim));
        Debug.Log("onChgTool:" + index+","+tool.gameObject.name);
    }
}
*/