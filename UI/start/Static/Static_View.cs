using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Static_View : BaseUIView
{
    //public MovShell shell;
    [HideInInspector]
    public bool shellActive;
    public UIMouse mouse;
    public GameObject loadSceneBG;
    public TMP_Text pointDescript;//被指向物体的描述
    //PlayerNPCCharDataShower charDataShower;

    public override void UIInit(UICenter center)
    {
        base.UIInit(center);
        //menuBG = transform.Find("menuBG").gameObject;
        loadSceneBG = transform.Find("loadSceneBG").gameObject;
        //settingBG = transform.Find("SettingBG").gameObject;
       // charDataShower= transform.Find("charDataShower").gameObject.GetComponent<PlayerNPCCharDataShower>();
        
        Debug.Log(gameObject.name);
        
    }

    bool isInSave = false;
    public override bool OnESC()
    {
       return false;
    }
  
    void onLoadProcess(float f)
    {
        if (loadSceneBG == null) return;
        if (!loadSceneBG.activeInHierarchy) loadSceneBG.SetActive(true);//从game返回start会取不到loadscenebg
        Slider s = loadSceneBG.transform.Find("load").GetComponent<Slider>();
        s.value = f;
    }

    public void ShowDescriptAt(Vector3 worldPos, string text)
    {
        pointDescript.text = text;
        pointDescript.rectTransform.position = worldPos;
    }
    public void ShowNPCDetail(RectTransform targetPos,NpcData data)
    {
        /*if (data != null) charDataShower.gameObject.SetActive(true);
        else charDataShower.gameObject.SetActive(false);
        charDataShower.GetComponent<RectTransform>().position = targetPos.position;
        charDataShower.ShowNpcData(data);*/
    }
    public void UnShowNPCDetail()
    {
        //charDataShower.gameObject.SetActive(false);
    }
    public void ShowItemDetail(RectTransform targetPos, Item data)
    {

    }

    public override void OnButtonHit(int id)
    {
        switch (id)
        {
            case 0://continude
            uiCenter.ShowView("menu");
                //menuBG.SetActive(!menuBG.activeInHierarchy);
                //uiCenter.PausePlayer(menuBG.activeInHierarchy); 
                //EventCenter.WorldCenter.SendEvent<bool>(nameof(PlayerEventName.pause), menuBG.activeInHierarchy);
                break;
            case 1://command
            uiCenter.ShowView("shell");
                /*shell.gameObject.SetActive(true);
                shellActive = shell.gameObject.activeInHierarchy;
                shell.Clear();
                menuBG.SetActive(false);*/
                break;
            case 2://setting
            uiCenter.ShowView("setting");
                /*settingBG.SetActive(true);
                settingBG.GetComponent<Setting_subView>().Init(this);
                menuBG.SetActive(false);*/
                break;
            case 2001://setting close
                /*settingBG.SetActive(false);
                menuBG.SetActive(true);*/
                break;
            case 10://exit
                //MovScriptCommandResolver.Resolve2("backtostart", EventCenter.WorldCenter, null);
                break;
            default:
                break;
        }
    }

    public override void OnArriveInGameScene()
    {
        base.OnArriveInGameScene();
        mouse.OnArriveInGameScene();
        EventCenter.WorldCenter.ListenEvent<Vector3, string>("ShowDropItemDescript", ShowDescriptAt);
        EventCenter.WorldCenter.UnListenEvent<float>(nameof(EventNames.LoadSceneProcessGo), onLoadProcess);
        //uiCenter.ListenEvent<RectTransform, NpcData>(nameof(UIEventNames.ShowNPCDetail), ShowNPCDetail);
        //uiCenter.ListenEvent<RectTransform, Item>(nameof(UIEventNames.ShowItem), ShowItemDetail);
        //uiCenter.ListenEvent(nameof(UIEventNames.UnShowNPCDetail), UnShowNPCDetail);
    }
    public override void OnQuitInGameScene()
    {
        base.OnQuitInGameScene();
        mouse.OnQuitInGameScene();
        EventCenter.WorldCenter.UnListenEvent<Vector3, string>("ShowDropItemDescript", ShowDescriptAt);
        EventCenter.WorldCenter.ListenEvent<float>(nameof(EventNames.LoadSceneProcessGo), onLoadProcess);
        //uiCenter.UnListenEvent<RectTransform, NpcData>(nameof(UIEventNames.ShowNPCDetail), ShowNPCDetail);
        //uiCenter.UnListenEvent<RectTransform, Item>(nameof(UIEventNames.ShowItem), ShowItemDetail);
        //uiCenter.UnListenEvent(nameof(UIEventNames.UnShowNPCDetail), UnShowNPCDetail);
    }
    public override void OnArriveStartScene()
    {
        base.OnArriveStartScene();
        mouse.OnArriveStartScene();
        EventCenter.WorldCenter.UnListenEvent<float>(nameof(EventNames.LoadSceneProcessGo), onLoadProcess);
        //uiCenter.ListenEvent<RectTransform, NpcData>(nameof(UIEventNames.ShowNPCDetail), ShowNPCDetail);
        //uiCenter.ListenEvent<RectTransform, Item>(nameof(UIEventNames.ShowItem), ShowItemDetail);
        //uiCenter.ListenEvent(nameof(UIEventNames.UnShowNPCDetail), UnShowNPCDetail);
    }
    public override void OnQuitStartScene()
    {
        base.OnQuitStartScene();
        mouse.OnQuitStartScene();
        EventCenter.WorldCenter.ListenEvent<float>(nameof(EventNames.LoadSceneProcessGo), onLoadProcess);
        //uiCenter.UnListenEvent<RectTransform, NpcData>(nameof(UIEventNames.ShowNPCDetail), ShowNPCDetail);
        //uiCenter.UnListenEvent<RectTransform, Item>(nameof(UIEventNames.ShowItem), ShowItemDetail);
        //uiCenter.UnListenEvent(nameof(UIEventNames.UnShowNPCDetail), UnShowNPCDetail);
    }
}
