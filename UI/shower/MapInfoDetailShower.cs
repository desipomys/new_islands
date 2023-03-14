using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MapInfoDetailShower : Base_Shower
{
    public Text des,nam;
    public override void ShowerInit(Base_UIComponent f)
    {
        base.ShowerInit(f);
        
    }
    public void Show(MapPrefabsData mpd)
    {
        des.text = mpd.descript;
        nam.text = mpd.GetDisplayName();
    }
}
