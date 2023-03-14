using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Sirenix.Serialization;

public enum UIEventNames
{
    OnCloseUI, OnOpenUI,


    ShowNPCDetail, UnShowNPCDetail,ShowText, ShowItem
}

public class UICenter : EventCenter
{
    static UICenter uiWorldCenter;
    public static UICenter UIWorldCenter
    {
        get
        {
            if (uiWorldCenter == null)
            {

                GameObject cavas = GameObject.Find("canvasGroup");
                uiWorldCenter = cavas.GetComponent<UICenter>();
                return uiWorldCenter;
            }
            return uiWorldCenter;
        }
        private set { uiWorldCenter = value; }
    }

    UIMouse mouse;
    public string defaultOpenUIName;
    
    public string[] viewsName;
    
    public string[] keepAliveName;

    Dictionary<string, BaseUIView> views = new Dictionary<string, BaseUIView>(StringComparer.OrdinalIgnoreCase);
    public Stack<BaseUIView> uistack = new Stack<BaseUIView>();
    public Stack<BaseUIView> secondStatck = new Stack<BaseUIView>();

    public List<BaseUIView> keepAlive = new List<BaseUIView>();

    int pausePlayerCount = 0;
    string path = "Prefabs/UI/UIView/";

    public override void Init()
    {
        Debug.Log("UIInit at " + SceneManager.GetActiveScene().name);
        findRegister();
        findMouse();
        findAndInitAllView();
        registToWorld();
        finalInit();
    }
    void finalInit()//显示默认view
    {
        ShowView(defaultOpenUIName);

        /* ListenEvent<string>(nameof(UIEventNames.OnPlayerOpenUI),
             onPlayerOpenUI);
         ListenEvent<string>(nameof(UIEventNames.OnPlayerCloseUI),
             onPlayerCloseUI);*/
    }
    void findMouse()
    {
        mouse = gameObject.GetComponentInChildren<UIMouse>();
    }
    /// <summary>
    /// 读取所有可用预制，从中选出viewnames+keepalive指定的预制
    /// </summary>
    /// <returns></returns>
    List<BaseUIView> LoadFromPrefabs()
    {
        GameObject[] gsa = Resources.LoadAll<GameObject>(path);
        Dictionary<string, GameObject> viewDic = new Dictionary<string, GameObject>();
        foreach (GameObject item in gsa)
        {
            GameObject temp = Instantiate(item, transform);
            temp.name = item.name;
            temp.SetActive(false);
            viewDic.Add(item.name, temp);
        }

        List<BaseUIView> ans = new List<BaseUIView>();
        foreach (var item in viewsName)
        {
            if(viewDic.ContainsKey(item))
            {
                ans.Add(viewDic[item].GetComponent<BaseUIView>());
            }
        }

        keepAlive.Clear();
        for (int i = 0; i < keepAliveName.Length; i++)
        {
            if (viewDic.ContainsKey(keepAliveName[i]))
            { keepAlive.Add(viewDic[keepAliveName[i]].GetComponent<BaseUIView>());
            }
        }

        return ans;
    }
    void findAndInitAllView()
    {
        List<BaseUIView> allview = LoadFromPrefabs();
        
        /*int count = transform.childCount;
        //Debug.Log(count);
        for (int i = 0; i < count; i++)
        {
            BaseUIView tt = transform.GetChild(i).GetComponent<BaseUIView>();
            if (tt != null)
                allview.Add(tt);
        }*/

        for (int i = 0; i < allview.Count; i++)
        {
            allview[i].UIInit(this);
            if(!string.IsNullOrEmpty( allview[i].UIName))//无视UINAME为空的
                views.Add(allview[i].UIName, allview[i]);
        }
        for (int i = 0; i < allview.Count; i++)
        {
            if (!string.IsNullOrEmpty(allview[i].UIName))//无视UINAME为空的
                allview[i].GetComponent<EventCenter>().Init();
        }
        for (int i = 0; i < allview.Count; i++)
        {
            if (string.IsNullOrEmpty(allview[i].UIName))//无视UINAME为空的
            { continue; }

            allview[i].PostUIInit();
            if (allview[i].gameObject.activeInHierarchy)
                allview[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < keepAlive.Count; i++)
        {
            keepAlive[i].OnUIOpen();
        }

        Debug.Log("有" + allview.Count + "个UI");
    }

    void onPlayerOpenUI(string uiname)
    {
        WorldCenter.SendEvent<bool>(nameof(EventNames.PauseLocalPlayer), true);
    }
    void onPlayerCloseUI(string uiname)
    {
        WorldCenter.SendEvent<bool>(nameof(EventNames.PauseLocalPlayer), false);
    }
    public UIMouse GetMouse()
    {
        return mouse;
    }

    public BaseUIView ShowAndGetView(string name)
    {
        ShowView(name);
        return GetView(name);
    }

    public BaseUIView GetView(string name)
    {
        BaseUIView view;
        if (views.TryGetValue(name, out view))
        {
            return view;
        }
        return null;
    }

    public void ShowView(string name)//入栈一个UI
    {
        BaseUIView view = GetView(name);
        if (view != null)
        {
            if (uistack.Count > 0 && !keepAlive.Contains(uistack.Peek()))
            {
                uistack.Peek().OnUIClose();
                SendEvent<string>(nameof(UIEventNames.OnCloseUI), uistack.Peek().UIName);
            }

            uistack.Push(view);
            view.OnUIOpen();
            SendEvent<string>(nameof(UIEventNames.OnOpenUI), view.UIName);

            if (secondStatck.Count > 0 && !keepAlive.Contains(secondStatck.Peek()) && secondStatck.Peek() != null)
            {//双UI支持
                secondStatck.Peek().OnUIClose();
                SendEvent<string>(nameof(UIEventNames.OnCloseUI), secondStatck.Peek().UIName);
            }
            secondStatck.Push(null);

            SendEvent(nameof(UIEventNames.UnShowNPCDetail));

        }
        //return view;
        //Debug.Log(uistack.Count);
    }
    public void ShowView(string view1name, string view2name)
    {
        BaseUIView view = GetView(view1name);
        if (view != null)
        {
            if (uistack.Count > 0 && !keepAlive.Contains(uistack.Peek()))
            {
                uistack.Peek().OnUIClose();
                SendEvent<string>(nameof(UIEventNames.OnCloseUI), uistack.Peek().UIName);
            }

            uistack.Push(view);
            view.OnUIOpen();
            SendEvent<string>(nameof(UIEventNames.OnOpenUI), view.UIName);
        }

        view = GetView(view2name);
        if (view != null)
        {
            if (secondStatck.Count > 0 && !keepAlive.Contains(secondStatck.Peek()) && secondStatck.Peek() != null)
            {
                secondStatck.Peek().OnUIClose();
                SendEvent<string>(nameof(UIEventNames.OnCloseUI), secondStatck.Peek().UIName);
            }

            secondStatck.Push(view);
            view.OnUIOpen();
            SendEvent<string>(nameof(UIEventNames.OnOpenUI), view.UIName);
        }
        SendEvent(nameof(UIEventNames.UnShowNPCDetail));
    }

    /// <summary>
    /// 关闭当前UI的无返回值版
    /// </summary>
    public void CloseView()
    {

        CloseCurrentView();
        //Debug.Log(uistack.Count);
    }
    /// <summary>
    /// 功能同closeview，关闭当前UI
    /// </summary>
    /// <returns></returns>
    public BaseUIView CloseCurrentView()//出栈一个UI
    {
        if (uistack.Count == 0) return null;
        BaseUIView view = uistack.Pop();
        if (view != null)
        {
            if (!keepAlive.Contains(view))
            {
                view.OnUIClose();
                SendEvent<string>(nameof(UIEventNames.OnCloseUI), view.UIName);
            }
        }
        if (uistack.Count > 0)
        {
            uistack.Peek().OnUIOpen();
            SendEvent<string>(nameof(UIEventNames.OnOpenUI), uistack.Peek().UIName);
        }

        BaseUIView view1 = secondStatck.Pop();
        if (view1 != null)
        {
            if (!keepAlive.Contains(view1))
            {
                view1.OnUIClose();
                SendEvent<string>(nameof(UIEventNames.OnCloseUI), view1.UIName);
            }
        }
        if (secondStatck.Count > 0 && secondStatck.Peek() != null)
        {
            secondStatck.Peek().OnUIOpen();
            SendEvent<string>(nameof(UIEventNames.OnOpenUI), secondStatck.Peek().UIName);
        }

        return view;
    }
    public void MVConnectRequest(string viewName, EventCenter model,EventCenter target)
    {
        if (!views.ContainsKey(viewName)) return;
        views[viewName].BuildMVConnect(viewName, model,target);
        //model.SendEvent<BaseUIController,BaseUIView>("buildMVConnect",views[viewName].GetController(),views[viewName]);
    }


    public void OnLoaderInit(int index)
    {
        if (index == 3)
        {
            //Init();

        }
    }
    void registToWorld()
    {


        //EventCenter.WorldCenter.ListenEvent<string>("OnQuitScene",OnQuitScene);
        /*EventCenter.WorldCenter.ListenEvent("QuitStartScene", OnQuitStartScene);
        EventCenter.WorldCenter.ListenEvent("ArriveInGameScene", OnArriveInGameScene);
        EventCenter.WorldCenter.ListenEvent("QuitInGameScene", OnQuitInGameScene);
        EventCenter.WorldCenter.ListenEvent("ArriveStartScene", OnArriveStartScene);*/

        EventCenter.WorldCenter.RegistFunc<BaseUIView>("CloseCurrentView", CloseCurrentView);
        EventCenter.WorldCenter.ListenEvent<string>("ShowView", ShowView);
        EventCenter.WorldCenter.RegistFunc<string, BaseUIView>("GetView", GetView);
    }

    #region 生命周期方法

    public void OnQuitAnyScene()
    {
        //EventCenter.WorldCenter.UnListenEvent<string>("OnQuitScene",OnQuitScene);

        /*EventCenter.WorldCenter.UnListenEvent("QuitStartScene", OnQuitStartScene);
        EventCenter.WorldCenter.UnListenEvent("ArriveInGameScene", OnArriveInGameScene);
        EventCenter.WorldCenter.UnListenEvent("QuitInGameScene", OnQuitInGameScene);
        EventCenter.WorldCenter.UnListenEvent("ArriveStartScene", OnArriveStartScene);*/

        EventCenter.WorldCenter.UnRegistFunc<BaseUIView>("CloseCurrentView", CloseCurrentView);
        EventCenter.WorldCenter.UnListenEvent<string>("ShowView", ShowView);
        EventCenter.WorldCenter.UnRegistFunc<string, BaseUIView>("GetView", GetView);
        uiWorldCenter = null;
        //view自己会去监听worldevc的onquit
        foreach (var item in views)
        {
            item.Value.UIDeInit();
        }
    }

    public void OnArriveStartScene()
    {
        List<BaseUIView> views = new List<BaseUIView>(this.views.Values);
        for (int i = 0; i < views.Count; i++)
        {
            views[i].OnArriveStartScene();
        }
    }
    public void OnQuitStartScene()
    {
        List<BaseUIView> views = new List<BaseUIView>(this.views.Values);
        for (int i = 0; i < views.Count; i++)
        {
            views[i].OnQuitStartScene();
        }
        OnQuitAnyScene();
    }
    public void OnArriveInGameScene()
    {
        List<BaseUIView> views = new List<BaseUIView>(this.views.Values);
        for (int i = 0; i < views.Count; i++)
        {
            views[i].OnArriveInGameScene();
        }


    }
    public void OnQuitInGameScene()
    {
        List<BaseUIView> views = new List<BaseUIView>(this.views.Values);
        for (int i = 0; i < views.Count; i++)
        {
            views[i].OnQuitInGameScene();
        }
        OnQuitAnyScene();
    }

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            List<bool> uiesc = new List<bool>();

            BaseUIView view = uistack.Count == 0 ? null : uistack.Peek();
            if (view != null)
            {
                uiesc.Add(view.OnESC());
            }

            view = secondStatck.Count == 0 ? null : secondStatck.Peek();
            if (view != null)
            {
                uiesc.Add(view.OnESC());
            }
            Debug.Log(uistack.Count + ":" + secondStatck.Count);

            if (uiesc.Count == 0)
            {
                for (int j = 0; j < keepAlive.Count; j++)
                {
                    keepAlive[j].OnESC();
                }
            }
            else
            {
                for (int i = 0; i < uiesc.Count; i++)
                {
                    if (!uiesc[i])//只要存在一个不可以esc关的UI就传onesc给所有keepalive
                    {

                        for (int j = 0; j < keepAlive.Count; j++)
                        {
                            keepAlive[j].OnESC();
                        }
                        //return;
                        return;
                    }
                }
            }
            if(uiesc.Count>1)CloseCurrentView();//menu界面不能用esc关闭，只能通过退出存档回到存档选择界面
            //
        }
    }

    public void PausePlayer(bool t)
    {
        if (t) pausePlayerCount++;
        else pausePlayerCount--;
        Debug.Log("当前count=" + pausePlayerCount);
        if (pausePlayerCount > 0) { EventCenter.WorldCenter.SendEvent<bool>(nameof(PlayerEventName.pause), true); }
        else { EventCenter.WorldCenter.SendEvent<bool>(nameof(PlayerEventName.pause), false); }
    }
}
//public interface IUI