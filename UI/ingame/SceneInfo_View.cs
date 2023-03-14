using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneInfo_View : BaseUIView
{
    public SpreadIndicator spreadIndicator;

    public override void OnUIOpen(int posi = 0)
    {
        base.OnUIOpen(posi);

    }
    public override void OnUIClose()
    {
        base.OnUIClose();
    }
    public override void OnArriveInGameScene()
    {
        base.OnArriveInGameScene();
        //监听世界的spreadchg事件
        EventCenter.WorldCenter.ListenEvent<float>(nameof(PlayerEventName.onSpreadChg), spreadIndicator.SetSpread);
    }
    
    public override void OnQuitInGameScene()
    {
        base.OnQuitInGameScene();
        spreadIndicator.OnQuitInGameScene();
        EventCenter.WorldCenter.UnListenEvent<float>(nameof(PlayerEventName.onSpreadChg), spreadIndicator.SetSpread);
    }
}
