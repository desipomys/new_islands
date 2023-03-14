using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using DG.Tweening;

public enum CameraShakeMode
{
    /// <summary>
    /// 来回震荡
    /// </summary>
    pingpong, decreasPingPong
}
public class Camera_Effecter : MonoBehaviour, IEventRegister
{
    EventCenter center;
    public GameObject shakePlaceHolder,rotaPlaceHolder;

   public float RotaAmplify;
    DIR dir;
    CameraShakeMode mode;

    bool isping;
    bool isShakeing = false;
    Ticker ticker = new Ticker();
    Vector3 shakeOffset;

    public void OnEventRegist(EventCenter e)//摄像机事件中心初始化，在每个arriveScene之前执行
    {
        center = e;
        e.RegistFunc<Vector3>("shakeOffset", GetShakeOffset);
        e.RegistFunc<Vector3>("ShakeRotaOffset", GetShakeRotaOffset);
        

        center.ListenEvent(nameof(EventNames.ArriveInGameScene), OnArriveInGameScene);
        center.ListenEvent(nameof(EventNames.ArriveStartScene), OnArriveStartScene);
        center.ListenEvent(nameof(EventNames.QuitInGameScene), OnQuitInGameScene);
        center.ListenEvent(nameof(EventNames.QuitStartScene), OnQuitStartScene);
    }
    public void AfterEventRegist()
    {

    }
    public void OnArriveStartScene()
    {
        initShakePlaceHolder();
        EventCenter.WorldCenter.ListenEvent<CamaraEffectParm>(nameof(EventNames.ShakeCamera), ShakeCamera);
    }
    public void OnQuitStartScene()
    {
        
        EventCenter.WorldCenter.UnListenEvent<CamaraEffectParm>(nameof(EventNames.ShakeCamera), ShakeCamera);
    }
    public void OnArriveInGameScene()
    {
        initShakePlaceHolder();
        EventCenter.WorldCenter.ListenEvent<CamaraEffectParm>(nameof(EventNames.ShakeCamera), ShakeCamera);
    }
    public void OnQuitInGameScene()
    {
        EventCenter.WorldCenter.UnListenEvent<CamaraEffectParm>(nameof(EventNames.ShakeCamera), ShakeCamera);
    }

    void initShakePlaceHolder()
    {
        shakePlaceHolder = new GameObject("shakePlaceHolder");
        shakePlaceHolder.transform.position = Vector3.zero;

        rotaPlaceHolder = new GameObject("rotaPlaceHolder");
        rotaPlaceHolder.transform.position = Vector3.zero;
    }
    void ShakeCamera(CamaraEffectParm parm)
    {
        Shake(parm.amplify, parm.time, parm.dir, parm.mode);
    }

    /// <summary>
    /// 振幅以移动距离为单位，频率是每秒pingpong几次，方向，时间
    /// </summary>
    /// <param name="amp"></param>
    /// <param name="dir"></param>
    /// <param name="time"></param>
    public void Shake(float amp, float time, DIR dir, CameraShakeMode mode = CameraShakeMode.pingpong)
    {
        /*amplify=amp;
        this.dir=dir;
        this.mode=mode;

        ticker.Reset(time);
        isShakeing=true;*/
        Debug.Log("shake");
        shakePlaceHolder.transform.position = Vector3.zero;
        shakePlaceHolder.transform.DOShakePosition(time, Vector3.up * amp,(int)(amp*100),0);

        rotaPlaceHolder.transform.position = Vector3.zero;
        rotaPlaceHolder.transform.DOShakePosition(time, UnityEngine.Random.onUnitSphere * amp, (int)(amp * 100), 0);
    }
    Vector3 nextPosi()//移动摄像机
    {
        /*Vector3 temp=Vector3.zero;
        switch (dir)
        {
            case DIR.front:
            temp=Vector3.forward;
            break;
            case DIR.left:
            temp=Vector3.left;
            break;
            case DIR.up:
            temp=Vector3.up;
            break;
            default:
            break;
        }

        if (isping)
        {
            return temp*amplify;
        }
        else
        {
            return temp*-1*amplify;
        }*/
        return shakePlaceHolder.transform.position;
    }


    void Update()
    {
        /*if(!isShakeing)return;
        if(ticker.IsReady())
        {
            shakeOffset=Vector3.zero;
            isShakeing=false;
        }*/
        //shakeOffset=nextPosi();
        //isping=!isping;
    }

    Vector3 GetShakeOffset()
    {
        return nextPosi();
    }
    Vector3 GetShakeRotaOffset()
    {
        return rotaPlaceHolder.transform.position *RotaAmplify;
    }
}