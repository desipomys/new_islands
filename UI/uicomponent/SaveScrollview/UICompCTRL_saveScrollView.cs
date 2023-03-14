using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompCTRL_saveScrollView : Base_UIComp_Controll
{
    public void EnterSave(string path)
    {
        SaveData sd = EventCenter.WorldCenter.GetParm<string, SaveData>("GetSaveDataByIndex", path);
        EventCenter.WorldCenter.SendEvent<SaveData>("LoadSave", sd);

        uiCenter.ShowView("menu");
    }

}
