using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLifeCycle : MonoBehaviour,IEventRegister
{
    //这里只应该发送事件或INIT，不应该处理逻辑
    Fsm lifecyclefsm;
    
    EventCenter uievc;
    void Awake()//开始时执行，跳场景时不执行
    {
        List<GameObject> temp=new List<GameObject>();//找自己，如果有两个则销毁自己
        try
        {
            temp.AddRange(GameObject.FindGameObjectsWithTag("starter"));
            if (temp.Count>1)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        catch (System.Exception)
        {
            
        }

        EventCenter evc = GetComponent<EventCenter>();//世界中心建立
        //evc.UUID=1;
        EventCenter.WorldCenter=evc;
        evc.Init();//注册各组件事件 ,读取数据

        if(SceneManager.GetActiveScene().name==ConstantValue.startSceneName)
            OnArriveStartScene();
        else
        {
            SaveData sd = new SaveData("test");
            sd.visable = false;
            sd.isInGame = true;
            evc.SendEvent<SaveData>(nameof(EventNames.LoadSave),sd);//加载一个测试用的savedata
            OnArriveInGameScene();
        }
        //GameObject mainCamera = Camera.main.gameObject;//摄像机初始化
        //mainCamera.GetComponent<EventCenter>().Init();

        //GameObject cavas = GameObject.Find("canvasGroup");//UI初始化
        //cavas.GetComponent<EventCenter>().Init();
    }


    public void OnEventRegist(EventCenter e)
    {
        //e.ListenEvent<string>("OnQuitScene",OnQuitScene);//事件源是scenejumper，想处理quitscene事件只要去worldevc监听就好不需要由lifecycle转发
        //e.ListenEvent<string>("OnArriveScene",OnArriveScene);

    }
    public void AfterEventRegist()
    {

    }


    public void OnQuitStartScene()//离开start场景之前
    {
        GameObject mainCamera = Camera.main.gameObject;//摄像机初始化
        mainCamera.GetComponent<EventCenter>().SendEvent(nameof(EventNames.QuitStartScene));

        GameObject cavas = GameObject.Find("canvasGroup");//UI初始化
        if (cavas != null) cavas.GetComponent<UICenter>().OnQuitStartScene();

        EventCenter.WorldCenter.SendEvent(nameof(EventNames.QuitStartScene));
    }

    public void OnArriveInGameScene()//进入ingame场景之后
    {
        GameObject mainCamera = Camera.main.gameObject;//摄像机初始化
        mainCamera.GetComponent<EventCenter>().Init();
        mainCamera.GetComponent<EventCenter>().SendEvent(nameof(EventNames.ArriveInGameScene));

        GameObject cavas = GameObject.Find("canvasGroup");//UI初始化
        if(cavas!=null) cavas.GetComponent<UICenter>().Init();
        if (cavas != null) cavas.GetComponent<UICenter>().OnArriveInGameScene();

        EventCenter.WorldCenter.SendEvent(nameof(EventNames.ArriveInGameScene));
        EventCenter.WorldCenter.SendEvent(nameof(EventNames.LoadGame));
    }
    public void OnQuitInGameScene()//离开ingame场景之前
    {
        GameObject mainCamera = Camera.main.gameObject;//摄像机初始化
        mainCamera.GetComponent<EventCenter>().SendEvent(nameof(EventNames.QuitInGameScene));

        GameObject cavas = GameObject.Find("canvasGroup");//UI初始化
        if (cavas != null) cavas.GetComponent<UICenter>().OnQuitInGameScene();

        EventCenter.WorldCenter.SendEvent(nameof(EventNames.QuitInGameScene));
       
    }
    public void OnArriveStartScene()//进入start场景之后
    {
        GameObject mainCamera = Camera.main.gameObject;//摄像机初始化
        mainCamera.GetComponent<EventCenter>().Init();
        mainCamera.GetComponent<EventCenter>().SendEvent(nameof(EventNames.ArriveStartScene));

        GameObject cavas = GameObject.Find("canvasGroup");//UI初始化
        if(cavas!=null) cavas.GetComponent<EventCenter>().Init();
        if (cavas != null) cavas.GetComponent<UICenter>().OnArriveStartScene();

        EventCenter.WorldCenter.SendEvent(nameof(EventNames.ArriveStartScene));       
    }

    public void OnApplicationQuit()
    {
        bool avalible=true;
        /*if(EventCenter.WorldCenter.GetParm<bool>("IsInGame",out avalible))//在游戏中退出
        {
            if (avalible)
            {
                //保存游戏地图数据
                
            }
        }*/

        if (EventCenter.WorldCenter.GetParm<bool>(nameof(EventNames.IsInSave), out avalible))//在存档中退出
        {
            if (!avalible) return;
            EventCenter.WorldCenter.SendEvent<float>("addPlayedTime", Time.time);
            EventCenter.WorldCenter.SendEvent<System.DateTime>("chgLastTime", System.DateTime.Now);

            EventCenter.WorldCenter.SendEvent(nameof(EventNames.Save));//ingame时的数据也在此保存
        }
        
    }

}
