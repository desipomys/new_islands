using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 是view,接收生命周期事件,监听时间改变事件
/// </summary>
public class DayNightEnv : MonoBehaviour
{
    public float DaySunPow, NightMoonPow;
    public Color DaySunColor,NightMoonColor,AfterNoonColor;
    public Color DayShadowColor, NightShadowColor, AfterNoonShadowColor;
    public Color DayAmbitColor, NightAmbitColor, AfterNoonAmbitColor;

    public Gradient SunGrad,MoonGrad,DayAmbitGrad, NightAmbitGrad,DayShadowGrad,NightShadowGrad,DayIntensityGrad,NightIntensityGrad;

    float currentSunPow;
    Color currentSun, currentShadow,currentAmbit;
    public Light light;

    Ticker w = new Ticker(1);
    int time = 0;
    public int cycleLen=60;

    DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,6,0,0);
    private void FixedUpdate()
    {
        if(w.IsReady())
        {
            
            dt=dt.AddMinutes(60*24/cycleLen);
            //Debug.Log("现在是：" + dt);
            EventCenter.WorldCenter.SendEvent<DateTime>(nameof(EventNames.OnMapTimeChg), dt);
           /* time++;
            float f = Mathf.PingPong(time, cycleLen) / cycleLen;
            currentSun = (f>0.5f?Color.Lerp(AfterNoonColor, NightMoonColor,(f-0.5f)*2): Color.Lerp(DaySunColor, AfterNoonColor, f  * 2));
            currentShadow = (f > 0.5f ? Color.Lerp(AfterNoonShadowColor, NightShadowColor, (f - 0.5f) * 2) : Color.Lerp(DayShadowColor, AfterNoonShadowColor, f * 2));
            currentAmbit = (f > 0.5f ? Color.Lerp(AfterNoonAmbitColor, NightAmbitColor, (f - 0.5f) * 2) : Color.Lerp(DayAmbitColor, AfterNoonAmbitColor, f * 2));
            currentSunPow = Mathf.Lerp(DaySunPow, NightMoonPow, Mathf.PingPong(time, cycleLen) / cycleLen);

            light.color = currentSun;
            light.intensity = currentSunPow;
            RenderSettings.subtractiveShadowColor = currentShadow;
            RenderSettings.ambientLight = currentAmbit;

            f = Mathf.Repeat(Mathf.Repeat(time, cycleLen) / cycleLen*180+90,180);
            light.transform.rotation = Quaternion.Euler(new Vector3(f, -30, 0));*/
        }
    }
    public void OnMapTimeChg(DateTime date)
    {
        bool isDay = date.Hour > 5 && date.Hour < 19;
        float datef = (date.Hour*60+date.Minute)*1f/(24*60);
        light.color = SunGrad.Evaluate(datef);
        light.intensity =DayIntensityGrad.Evaluate(datef).a;
        RenderSettings.subtractiveShadowColor = DayShadowGrad.Evaluate(datef);
        RenderSettings.ambientLight = DayAmbitGrad.Evaluate(datef);

        float f = Mathf.Repeat(datef*300 + 90, 150);
        light.transform.rotation = Quaternion.Euler(new Vector3(f+15, -30, 0));
        //RenderSettings.ambientLight
    }
    void SetMapTime(DateTime d) { dt = d; Debug.Log("设置时间"+d); }

    public void OnArriveStartScene()//进入start场景之后
    {
        //Debug.Log("OnArriveInGameScene");
    }
    public void OnQuitInGameScene()//离开ingame场景之前
    {
        //Debug.Log("OnArriveInGameScene");
        EventCenter.WorldCenter.UnListenEvent<DateTime>(nameof(EventNames.OnMapTimeChg), OnMapTimeChg);
        EventCenter.WorldCenter.UnListenEvent<DateTime>(nameof(EventNames.SetMapTime), SetMapTime);
    }
    public void OnArriveInGameScene()//进入ingame场景之后
    {
        EventCenter.WorldCenter.ListenEvent<DateTime>(nameof(EventNames.OnMapTimeChg), OnMapTimeChg);
        EventCenter.WorldCenter.ListenEvent<DateTime>(nameof(EventNames.SetMapTime), SetMapTime);
        //dt.AddHours(6);
        //获取游戏当前时间
        Debug.Log("监听setmaptime成功");
    }
    public void OnQuitStartScene()//离开start场景之前
    {
        //Debug.Log("qss");
        
    }
    
}
