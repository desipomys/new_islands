using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ItemScrollView : MonoBehaviour//测试脚本
{
    
    // Start is called before the first frame update
    void Start()
    {
        

        //ItemScrollView tt= GetComponent<ItemScrollView>();
        //tt.UIInit(null,null);
        //
        EventCenter.WorldCenter.SendEvent("LoadSaveDone");

        EventCenter.WorldCenter.RegistFunc<Item>("GetMouseItem", () => { return mouseitem; });
        EventCenter.WorldCenter.RegistFunc<bool>("IsCheatMode", () => { return true; });
        EventCenter.WorldCenter.ListenEvent<Item>("SetMouseItem", Set_MouseItem);



    }
    Item mouseitem=new Item();

    public void Set_MouseItem(Item i)
    {
        mouseitem = i;
        EventCenter.WorldCenter.SendEvent<Item>("MouseItemChg", i);
    }
    // Update is called once per frame
   
}
