using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToucher : BaseToucher
{

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.ListenEvent<KeyCodeKind, Dictionary<KeyCode, KeyCodeStat>>(nameof(PlayerEventName.onKey), OnKey);
    }

    public void OnKey(KeyCodeKind kind, Dictionary<KeyCode, KeyCodeStat> stat)
    {
        if(kind.Contain(KeyCodeKind.E)&&stat[KeyCode.E]==KeyCodeStat.down)//当按下E键
        {
            GameObject obj = center.GetParm<GameObject>(nameof(PlayerEventName.getLookAtObj));
            
            if (obj!=null)
            {
                IInterectable iinter = obj.GetComponent<IInterectable>();
                
                if (iinter!=null)
                {
                    
                    int i=iinter.OnInterect(center, InteractType.open);
                    if (i == 0) return;
                    center.SendEvent<float>(nameof(PlayerEventName.setMovePauseInTime), 0.25f);
                    center.SendEvent<float>(nameof(PlayerEventName.setRotaPauseInTime), 0.25f);
                    switch (i)
                    {
                        case 1://平拾取
                            //center.SendEvent<string, int>(nameof(PlayerEventName.playAnima), "pick", 1);
                            center.SendEvent<string, int>(nameof(PlayerEventName.playAnima), "pick", 1);
                            break;
                        case 2://上拾取
                            //center.SendEvent<string, int>(nameof(PlayerEventName.playAnima), "pick_up", 1);
                            center.SendEvent<string, int>(nameof(PlayerEventName.playAnima), "pick_up", 1);
                            break;
                        case 3://下拾取
                            //center.SendEvent<string, int>(nameof(PlayerEventName.playAnima), "pick_down", 1);
                            center.SendEvent<string, int>(nameof(PlayerEventName.playAnima), "pick_down", 1);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
