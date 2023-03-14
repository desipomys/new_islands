//选择存档界面
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class ChoseSave_View : BaseUIView
{
    public SaveScrollView saves;
    Transform animaRoot;
    public Text deleteText;
    public GameObject deleteBG;


    public override void UIInit(UICenter center)
    {
        base.UIInit(center);
        Debug.Log("UIINI次数");
      
        animaRoot = transform.Find("holder");
    }
    public override void OnUIOpen(int posi=0)
    {
        base.OnUIOpen();
        BuildMVConnect(UIName, EventCenter.WorldCenter, null);

        //reuseOldSaveShower();
        //genSaveSlotsByData();

        /*if (allSaveSlot.Length > 0)
        {
            StartCoroutine(slotAliasWait(pathToIndex[selectingSave]));
        }*/
    }
    public override void DoOpenAnim()
    {
        //animaRoot.position += Vector3.right * 1920;
        //DotWeenHelper.DoMoveXTest(animaRoot, -1920, 0.5f);
    }
    public override void OnButtonHit(int id)
    {
        base.OnButtonHit(id);
        switch (id)
        {
            case 0:uiCenter.CloseView();
                break;
            default:
                break;
        }
    }


    public void DeleteSave()
    {
        if (string.IsNullOrEmpty(saves.GetSelectingSave())) return;
        deleteText.text = "确定删除存档\"" + EventCenter.WorldCenter.GetParm<string, SaveData>("GetSaveDataByIndex", saves.GetSelectingSave()).saveName + "\"?";
        deleteBG.SetActive(true);
    }
    public void certainDeleteSave()
    {
        LoaderManager.GetLoader<Loader_SaveData>().DeleteSave(saves.GetSelectingSave());
        saves.Flush();//重新生成一遍saveslot
        
    }

    
    public override void OnQuitInGameScene()
    {
        base.OnQuitInGameScene();

    }
    public override void OnQuitStartScene()
    {
        base.OnQuitStartScene();
        
    }
}
