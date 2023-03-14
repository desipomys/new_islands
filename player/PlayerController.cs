using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum KeyCodeStat
{
    none,down,keep,up 
}
public enum KeyCodeKind
{
    None=0,
    Mouse0=1,
    Mouse1=1<<1,
    W=1<<2,
    S=1<<3,
    A=1<<4,
    D=1<<5,
    LeftShift=1<<6,
    LeftControl=1<<7,
    E=1<<8,
    Q=1<<9,
    Space=1<<10,
    Alpha1=1<<11,
    Alpha2=1<<12,
    Alpha3=1<<13,
    Alpha4=1<<14,
    R=1<<15,
    T=1<<16,
    X=1<<17
}
public class PlayerController : BaseController
{

    public KeyCode[] definedKeys=new KeyCode[]
    {KeyCode.Mouse0,KeyCode.Mouse1,
    KeyCode.W,KeyCode.S,KeyCode.A,KeyCode.D,KeyCode.LeftShift,KeyCode.LeftControl,
    KeyCode.E,KeyCode.Q,KeyCode.Space,
    KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3,KeyCode.Alpha4,KeyCode.R,KeyCode.T,KeyCode.X
    };
    KeyCodeKind kind;
    Dictionary<KeyCode, KeyCodeStat> stats=new Dictionary<KeyCode, KeyCodeStat>();

    bool isPause=false;
    Ticker longPressTick = new Ticker(0.25f);
    

    public override void OnEventRegist(EventCenter e)
    {
        center=e;

        for (int i = 0; i < definedKeys.Length; i++)
        {
            stats.Add(definedKeys[i], 0);
            //Debug.Log(definedKeys[i]);
        }
        Debug.Log("有" + definedKeys.Length + "个key,"+stats.Count);


        e.ListenEvent<bool>("setControllActive",SetActive);
        e.ListenEvent<bool>(nameof(PlayerEventName.pause), (bool p) => { isPause = p; temp = false; });
        e.ListenEvent<KeyCodeKind, Dictionary<KeyCode, KeyCodeStat>>(nameof(PlayerEventName.onKey), LongPressE);
        UICenter.UIWorldCenter.ListenEvent<string>(nameof(UIEventNames.OnOpenUI), OnPlayerOpenUI);
        UICenter.UIWorldCenter.ListenEvent<string>(nameof(UIEventNames.OnCloseUI), OnPlayerCloseUI);
        e.ListenEvent<object>(nameof(PlayerEventName.load), onLoad);
        Debug.Log("init");
    }
    //bool active=true;
    public void SetActive(bool a){enabled=a;isPause = !a; }

    public void Update()
    {
        if (isPause) return;
        
        kind=KeyCodeKind.None;
        for (int i = 0; i < stats.Count; i++)
        {
            stats[definedKeys[i]]=KeyCodeStat.none;
        }

        //if(!active)return;

        for(int i=0;i<definedKeys.Length;i++)
        {
            if (definedKeys[i] == KeyCode.Mouse0|| definedKeys[i] == KeyCode.Mouse1)//如果左键、右键于UI上则无效
            {
                if (EventSystem.current.IsPointerOverGameObject() == true) continue;
            }
            if(Input.GetKeyDown(definedKeys[i]))
            {
                kind=kind|(KeyCodeKind)(1<<i);
                stats[definedKeys[i]] =KeyCodeStat.down;
            }
            else if(Input.GetKey(definedKeys[i]))
            {
                kind = kind | (KeyCodeKind)(1 << i);
                stats[definedKeys[i]] = KeyCodeStat.keep;
            }
            else if(Input.GetKeyUp(definedKeys[i]))
            {
                kind = kind | (KeyCodeKind)(1 << i);
                stats[definedKeys[i]] = KeyCodeStat.up;
            }
        }

        center.SendEvent<KeyCodeKind, Dictionary<KeyCode, KeyCodeStat>>(nameof(PlayerEventName.onKey),kind,stats);
    }

    bool temp=false;//上次是否按下E键
    bool UIShowing = false;
    void LongPressE(KeyCodeKind kind, Dictionary<KeyCode, KeyCodeStat> dicts)
    {
        if (!kind.Contain(KeyCodeKind.E)) { longPressTick.SetReady();temp = false; return; }
        if(longPressTick.IsReady())//如果就绪则代表第一次按下E按键
        {
            if(temp)//上一次第一次按下E键到现在已经过了一个tick等待时间，
            {
                //召唤UI,召唤后选择期间不可再召唤
                //Debug.Log("OK");
                temp = false;
                UICenter.UIWorldCenter.ShowView("E");
            }
           if(dicts[KeyCode.E]==KeyCodeStat.down) temp = true;
        }
        //长按E打开UI后再按下E就能关闭当前UI
        //从E界面选择一个界面进入会暂停玩家控制，但在E界面不会
        //
    }
    void OnPlayerOpenUI(string UIname)
    {
        isPause = true;//改为发送pause事件
    }
    void OnPlayerCloseUI(string UIname)//在玩家关闭全部不影响行动的UI后被调用
    {
        isPause = false;
        //UIShowing = false;
    }
    void onLoad(object o)
    {
        if (enabled)//临时措施,需判断哪个是玩家才执行以下代码，
            Camera.main.GetComponent<EventCenter>().SendEvent<Transform>(nameof(EventNames.Track), transform);
    }
}