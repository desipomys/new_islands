//主要生命周期必经之路+少许公共方法
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public class GameMainManager : BaseManager
{
    BaseManager[] allmanager;
    /// <summary>
    /// 需要接收生命周期事件的静态gobj
    /// </summary>
    public GameObject[] staticObj;

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        List<BaseManager> managers=new List<BaseManager>();

        managers.Add(GetComponent<LoaderManager>());
        managers.Add(GetComponent<ContainerManager>());

        allmanager=managers.ToArray();

        for (int i = 0; i < allmanager.Length; i++)
        {
            //allmanager[i].OnEventRegist(e);//worldevc会调用，这里不用调用
        }

        e.ListenEvent<SaveData>(nameof(EventNames.LoadSave), Onloadsave);
        e.ListenEvent(nameof(EventNames.UnLoadSave), UnLoadSave);
        e.ListenEvent(nameof(EventNames.LoadGame), OnLoadGame);
        e.ListenEvent(nameof(EventNames.UnLoadGame), UnLoadGame);
        e.ListenEvent(nameof(EventNames.Save), Save);

        e.ListenEvent(nameof(EventNames.ArriveStartScene), ArriveStartScene);
        e.ListenEvent(nameof(EventNames.ArriveInGameScene), ArriveInGameScene);
        e.ListenEvent(nameof(EventNames.QuitInGameScene), QuitInGameScene);
        e.ListenEvent(nameof(EventNames.QuitStartScene), QuitStartScene);
    }

    //在ingame,start可随时被调用
    public void Save()
    {
        string thissavename=EventCenter.WorldCenter.GetParm<string>("ThisSavePath");
        GetComponent<ContainerManager>().Save(thissavename);
        center.SendEvent(nameof(EventNames.SaveDone));
        Debug.Log("保存");
    }
    public void Onloadsave(SaveData curretSave)//选中存档双击后加载存档，同时负责start和ingame的加载
    {
        Debug.Log("生命周期事件：Onloadsave");
        GetComponent<ContainerManager>().OnLoadSave(curretSave);
        center.SendEvent(nameof(EventNames.LoadSaveDone));
    }
    public void UnLoadSave()//离开存档返回存档选择界面
    {
        Save();
        Debug.Log("生命周期事件：UnLoadSave");
        GetComponent<ContainerManager>().UnLoadSave();
        center.SendEvent(nameof(EventNames.UnLoadSaveDone));
    }
    /// <summary>
    /// 进入ingame场景后被调用，晚于onarriveingame
    /// </summary>
    public void OnLoadGame()
    {
        Debug.Log("生命周期事件：onloadgame");
        GetComponent<ContainerManager>().OnLoadGame();
        center.SendEvent(nameof(EventNames.LoadGameDone));
    }
    /// <summary>
    /// 手动离开游戏时（如点击返回开始界面）被调用
    /// </summary>
    public void UnLoadGame()
    {
        Save();
        Debug.Log("生命周期事件：Unloadgame");
        GetComponent<ContainerManager>().UnLoadGame();
        center.SendEvent(nameof(EventNames.UnLoadGameDone));
    }

    void ArriveStartScene()
    {
        Debug.Log("生命周期事件：ArriveStartScene");
        //获取当前选中savedata，加载相关数据到container或调用相关container让其自己去加载数据，
        GetComponent<ContainerManager>().OnArriveStartScene();
        sendEvent("OnArriveStartScene");
    }
    void ArriveInGameScene()//当前是ingame状态，需要跳转场景生成地图
    {
        Debug.Log("生命周期事件：ArriveInGameScene");
        GetComponent<ContainerManager>().OnArriveInGameScene();
        sendEvent("OnArriveInGameScene");
        //container不要自己去读文件
        //OnLoadGame();
    }
    void QuitInGameScene()
    {
        Debug.Log("生命周期事件：QuitInGameScene");
        GetComponent<ContainerManager>().OnQuitInGameScene();
        sendEvent("OnQuitInGameScene");
        center.ClearListenerByName(nameof(EventNames.SaveDone));
        center.ClearListenerByName(nameof(EventNames.LoadSaveDone));
        center.ClearListenerByName(nameof(EventNames.UnLoadSaveDone));
        center.ClearListenerByName(nameof(EventNames.LoadGameDone));
        center.ClearListenerByName(nameof(EventNames.UnLoadGameDone));
    }
    void QuitStartScene()
    {
        Debug.Log("生命周期事件：QuitStartScene");
        GetComponent<ContainerManager>().OnQuitStartScene();
        sendEvent("OnQuitStartScene");
        center.ClearListenerByName(nameof(EventNames.SaveDone));
        center.ClearListenerByName(nameof(EventNames.LoadSaveDone));
        center.ClearListenerByName(nameof(EventNames.UnLoadSaveDone));
        center.ClearListenerByName(nameof(EventNames.LoadGameDone));
        center.ClearListenerByName(nameof(EventNames.UnLoadGameDone));
    }

    void sendEvent(string nam)
    {
        for (int i = 0; i < staticObj.Length; i++)
        {
            staticObj[i].SendMessage(nam);
        }
    }


    private void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.D))
        {
            Dictionary<string, List<Delegate_Plus>> dic= GetComponent<EventCenter>().eventDic;
            string allkeys = " EVENT: \n";
            foreach (var item in dic)
            {
                allkeys += item.Key + "  " + item.Value.Count + "  \n";
            }

            dic= GetComponent<EventCenter>().funcDic;
            allkeys += " \n  GETPARM:\n";
            foreach (var item in dic)
            {
                allkeys += item.Key + "  " + item.Value.Count + "  \n";
            }
            Debug.Log(allkeys);
        }*/
    }

    /// <summary>
    /// 带有evc初始化的创建物体
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="posi"></param>
    /// <param name="rota"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject CreateGameObject(GameObject obj,Vector3 posi,Quaternion rota,Transform parent)
    {
        GameObject g = Instantiate(obj, posi, rota, parent);
        EventCenter e= g.GetComponent<EventCenter>();
        if (e != null) e.Init();
        return g;
    }
    /// <summary>
    /// 带有evc初始化的创建物体
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static GameObject CreateGameObject(GameObject obj)
    {
        GameObject g = Instantiate(obj);//生成的物体中，awake先于oneventregist
        EventCenter e = g.GetComponent<EventCenter>();
        if (e != null) e.Init();
        return g;
    }
}