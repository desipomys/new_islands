using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceShower:Base_UIComponent
{
    [Tooltip("是否响应玩家资源数据改变,falsew为不响应")]
    public bool isolate=true;
    public Text material;
    public Text manPower;
    public Text energy;
    public Text information;
   //public Text metal;
    //public Text chemical;
    //public Text magic;

    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);
        //监听container_playerdata的resource改变事件
        //EventCenter.WorldCenter.GetParm<Resource_Data>("");
        if (isolate)
        {
            EventCenter.WorldCenter.ListenEvent<Resource_Data>("ResourceChange", OnDataChange);
            OnDataChange(EventCenter.WorldCenter.GetParm<Resource_Data>("GetResource"));
        }
        //EventCenter.WorldCenter.ListenEvent("QuitStartScene", OnQuitStart);
    }
    public void Show(Resource_Data arg)
    {
        if (arg == null) {
            material.text = "";
            manPower.text = "";
            energy.text = "";
            information.text = "";
            return; }
        material.text=arg.material.ToString();
        manPower.text=arg.manPower.ToString();
        energy.text=arg.energy.ToString();
        information.text=arg.information.ToString();
        
    }
    
    public void OnDataChange(Resource_Data arg)
    {
        if (arg == null) return;
        material.text=arg.material.ToString();
        manPower.text=arg.manPower.ToString();
        energy.text=arg.energy.ToString();
        information.text=arg.information.ToString();
       
    }
    public void OnQuitStart()
    {
        //切断连接
        if (isolate)
        {
            Debug.Log("切断连接");
            EventCenter.WorldCenter.UnListenEvent<Resource_Data>("ResourceChange", OnDataChange);
            //EventCenter.WorldCenter.UnListenEvent("OnQuitStart", OnQuitStart);
        }
    }
    public void OnLoadSaveDone()
    {

    }
}