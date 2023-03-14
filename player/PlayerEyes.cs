
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEyes : BaseEyes
{

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.RegistFunc<Vector3>(nameof(PlayerEventName.getLookAt),GetLookAtPosition);
        e.RegistFunc<GameObject>(nameof(PlayerEventName.getLookAtObj), GetLookAtObj);
        e.ListenEvent<bool>(nameof(PlayerEventName.pause), (bool a) => { active = !a; });
        e.ListenEvent<KeyCodeKind, Dictionary<KeyCode, KeyCodeStat>>(nameof(PlayerEventName.onKey), changedir);
       // Debug.Log("s");
    }
    Vector3 GetLookAtPosition()
    {
        return EventCenter.WorldCenter.GetParm<Vector3>(nameof(EventNames.GetMouseLookAt));
    }
    GameObject GetLookAtObj()
    {
        return EventCenter.WorldCenter.GetParm<GameObject>(nameof(EventNames.GetMouseLookObj));
    }
    public  void changedir(KeyCodeKind key, Dictionary<KeyCode, KeyCodeStat> state)
    {
        //Debug.Log("changeDir");
        if (key.Contain(KeyCodeKind.T))
        {
            //Debug.Log("changeDir");
            if (state[KeyCode.T] == KeyCodeStat.down)
            {
                //Debug.Log("changeDir");
                EventCenter.WorldCenter.SendEvent<int>(nameof(EventNames.ChangeViewDir), 90);
            }
        }
    }

    public override EventCenter GetLookAt(EyeLookFilter filter)
    {
        return null;
    }
    public override EventCenter[] GetNotified(EyeLookFilter filter)
    {
        return null;
    }
}