using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDetail_UIController : BaseUIController
{
    public int trueIndex, UIIndex;
    public void SetNPCIndex(int t,int u)
    {
        trueIndex = t;
        UIIndex = u;
        bp = EventCenter.WorldCenter.GetParm<int, IBackPack>(nameof(Container_PlayerData_EventNames.NPCbackpack), trueIndex);
    }
    public override EventCenter GetModel()
    {
        return this.view.center;
    }

    public override void OnSlotClick(int state, int x, int y, int area)
    {
        if (area > 0)
        {
            base.OnSlotClick(bp, state, x, y, area);
        }
    }
}
