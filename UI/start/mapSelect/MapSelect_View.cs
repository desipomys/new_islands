using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapSelect_UIController))]
public class MapSelect_View : BaseUIView
{
    public override void OnUIOpen(int posi = 0)
    {
        base.OnUIOpen(posi);
        BuildMVConnect(UIName, EventCenter.WorldCenter, null);
    }
    public override void OnUIClose()
    {
        base.OnUIClose();
        BreakMVConnect(UIName, EventCenter.WorldCenter, null);
    }

}
