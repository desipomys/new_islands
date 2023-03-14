using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNodeShower : Base_Shower
{
    MapPrefabsData mapPrefabsData;
    //BaseUIView buv;
    MapGraphView msv;
    RectTransform myrect;

    public Text text;
    //ÓÉmapselectviewµ÷ÓÃ
    public override void ShowerInit(Base_UIComponent f)
    {
        base.ShowerInit(f);
        this.msv = (MapGraphView)f;
        //buv = view;
        myrect = GetComponent<RectTransform>();
    }

    public void Show(MapPrefabsData mpd, MapGraphData mgd)
    {
        if (mpd == null || mgd == null) { gameObject.SetActive(false); return; }
        mapPrefabsData = mpd;
        Vector3 v3;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(msv.GetComponent<RectTransform>(), new Vector3(mgd.x, mgd.y, 0), Camera.main, out v3);
        myrect.position = v3;
        text.text = mpd.mapName;
    }
    public void OnClick()
    {
       
            msv.OnNodeClick(mapPrefabsData, myrect);
            Debug.Log(mapPrefabsData.mapName);
        

    }
}
