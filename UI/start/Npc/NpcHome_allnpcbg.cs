using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcHome_allnpcbg : MonoBehaviour//,IUIInitReciver
{/*
    public Image bound;
    BaseUIController uicontroller;
    public NPCInfoScrollView allnpcs;
     NpcHome_View father;
    public void UIInit(UICenter center, BaseUIView view)
    {
        father = (NpcHome_View)view;
        uicontroller = view.GetController();
        allnpcs = GetComponentInChildren<NPCInfoScrollView>(true);
        allnpcs.UIInit(center, view);
        allnpcs.Listen(slotonkey);
    }
    public void SetCurrent(int uiindex)
    {
        //将bound设置到trueindex的npcinfoshower上
        int[] allindex= EventCenter.WorldCenter.GetParm<int[]>(nameof(Container_PlayerData_EventNames.GetNPCSelectedIndexs));
        
        bound.rectTransform.position = allnpcs.GetShowerByIndex(allindex[uiindex]).GetComponent<RectTransform>().position;
        allnpcs.ResetIndex(allindex);
    }
    public void Show(NpcData[] alldata,int[] selectedIndex)
    {
        allnpcs.Show(alldata, selectedIndex);
    }
    public void slotonkey(NPCInfoShower shower, int x,int y,int key)
    {
        //将bound设置为正好框选住x对应的npcinfoshower
        //设置npchomeview的tempselectindex
        bound.rectTransform.position = shower.GetComponent<RectTransform>().position;

        //father.SetTempIndex(x);
    }
   */
}
