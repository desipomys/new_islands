using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class NewSave_View : BaseUIView
{
    public Text saveName;
    GameObject failCreateSave;
    
    public override void OnButtonHit(int id)
    {
        base.OnButtonHit(id);
        switch(id)
        {
            case 0://跳过
           string succ= EventCenter.WorldCenter.GetParm<string,string>(nameof(EventNames.CreateSaveByName),saveName.text);
            if(!string.IsNullOrWhiteSpace(succ))
           {    SaveData sd = EventCenter.WorldCenter.GetParm<string, SaveData>(nameof(EventNames.GetSaveDataByIndex), succ);
            EventCenter.WorldCenter.SendEvent<SaveData>(nameof(EventNames.LoadSave), sd);
            }
            else{//建档失败
                Debug.Log("存档创建失败，已有同名存档");
                failCreateSave.SetActive(true);
                return;
            }
            SaveSetting setting=EventCenter.WorldCenter.GetParm<SaveSetting>(nameof(EventNames.GetSaveSetting));
            setting.jumpTutorial=true;
            EventCenter.WorldCenter.SendEvent<SaveSetting>(nameof(EventNames.SetSaveSetting),setting);
            UICenter.UIWorldCenter.ShowView("menu");

            break;
            case 1://不跳过
            string succ1= EventCenter.WorldCenter.GetParm<string,string>(nameof(EventNames.CreateSaveByName),saveName.text);
            if(!string.IsNullOrWhiteSpace(succ1))
           {    SaveData sd = EventCenter.WorldCenter.GetParm<string, SaveData>(nameof(EventNames.GetSaveDataByIndex), succ1);
            EventCenter.WorldCenter.SendEvent<SaveData>(nameof(EventNames.LoadSave), sd);
            }
            else{//建档失败
                Debug.Log("存档创建失败，已有同名存档");
                failCreateSave.SetActive(true);
                return;
            }

            UICenter.UIWorldCenter.ShowView("menu");
            EventCenter.WorldCenter.SendEvent<string>(nameof(EventNames.StartGameFromMap),ConstantValue.tutorialMapName);

            break;

            case 2:
                saveName.text = Random.Range(0, 100000).ToString();
                break;
        }
    }

    public override void OnUIOpen(int posi = 0)
    {
        base.OnUIOpen(posi);
        //buildconnect
        BuildMVConnect(UIName, EventCenter.WorldCenter, EventCenter.WorldCenter);
    }
    public override void OnUIClose()
    {
        base.OnUIClose();
        BreakMVConnect(UIName, EventCenter.WorldCenter, EventCenter.WorldCenter);
    }
    public override void BuildMVConnect(string viewname, EventCenter modelSource, EventCenter modelTarget)
    {
        base.BuildMVConnect(viewname, modelSource, modelTarget);

    }
    public override void BreakMVConnect(string viewname, EventCenter model, EventCenter target)
    {
        base.BreakMVConnect(viewname, model, target);
    }
}
