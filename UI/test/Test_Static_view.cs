using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Static_view : BaseUIView
{
    public EventCenter bp;
    public ItemScrollView itemScrollView;

    public ItemScrollView_big itemScrollView_Big;

    public MovShell shell;
    public UIMouse mouse;
    public override void UIInit(UICenter center)
    {

        base.UIInit(center);

        BaseBackPack bap = GetComponent<BaseBackPack>();
        ItemPage_Data tpd = new ItemPage_Data(4, 5);
        tpd.SetItemAt(new Item(1, 1), 1, 1);
        tpd.SetItemAt(new Item(1, 1), 1, 2);
        bap.SetItems(tpd.items,tpd.placement);
        //BuildMVConnect(UIName,)
        GameObject.Find("backpack").GetComponent<EventCenter>().Init();
        GameObject.Find("backpack").GetComponent<Test_Backpack>().SetSize(4, 5);
    }
    public override void OnUIOpen(int posi = 0)
    {
        Debug.Log("jjjj");
        base.OnUIOpen(posi);
        BuildMVConnect(UIName, GetComponent<EventCenter>(), GameObject.Find("backpack").GetComponent<EventCenter>());
    }

    bool isInSave = false;
    private void Update()
    {
        
    }
}
