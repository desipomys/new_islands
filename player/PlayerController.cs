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
        Debug.Log("��" + definedKeys.Length + "��key,"+stats.Count);


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
            if (definedKeys[i] == KeyCode.Mouse0|| definedKeys[i] == KeyCode.Mouse1)//���������Ҽ���UI������Ч
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

    bool temp=false;//�ϴ��Ƿ���E��
    bool UIShowing = false;
    void LongPressE(KeyCodeKind kind, Dictionary<KeyCode, KeyCodeStat> dicts)
    {
        if (!kind.Contain(KeyCodeKind.E)) { longPressTick.SetReady();temp = false; return; }
        if(longPressTick.IsReady())//�������������һ�ΰ���E����
        {
            if(temp)//��һ�ε�һ�ΰ���E���������Ѿ�����һ��tick�ȴ�ʱ�䣬
            {
                //�ٻ�UI,�ٻ���ѡ���ڼ䲻�����ٻ�
                //Debug.Log("OK");
                temp = false;
                UICenter.UIWorldCenter.ShowView("E");
            }
           if(dicts[KeyCode.E]==KeyCodeStat.down) temp = true;
        }
        //����E��UI���ٰ���E���ܹرյ�ǰUI
        //��E����ѡ��һ������������ͣ��ҿ��ƣ�����E���治��
        //
    }
    void OnPlayerOpenUI(string UIname)
    {
        isPause = true;//��Ϊ����pause�¼�
    }
    void OnPlayerCloseUI(string UIname)//����ҹر�ȫ����Ӱ���ж���UI�󱻵���
    {
        isPause = false;
        //UIShowing = false;
    }
    void onLoad(object o)
    {
        if (enabled)//��ʱ��ʩ,���ж��ĸ�����Ҳ�ִ�����´��룬
            Camera.main.GetComponent<EventCenter>().SendEvent<Transform>(nameof(EventNames.Track), transform);
    }
}