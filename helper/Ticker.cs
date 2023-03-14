using System;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class Ticker
{
    public float waitTime;
    [HideInInspector]
    public float cache;
    float lastRunTime;

    //刚创建时就是ready的
    public Ticker()
    {

    }
    public Ticker(float time)
    {
        waitTime=time;
    }
    /// <summary>
    /// 重新计时
    /// </summary>
    public void ReTick()
    {
        cache=0;
        lastRunTime = Time.time;
    }
    /// <summary>
    /// 将等待时间设为time并重新计时
    /// </summary>
    /// <param name="time"></param>
    public void Reset(float time)
    {
        waitTime=time;
        ReTick();
    }
    /// <summary>
    /// 直接将计时设为就绪
    /// </summary>
    public void SetReady()
    {
        cache = waitTime + float.Epsilon;
    }
    /// <summary>
    /// 是否到时间（重置计时）
    /// </summary>
    /// <returns></returns>
    public bool IsReady()
    {
        cache += (Time.time - lastRunTime);
        bool ans=(cache>waitTime);
        if(ans)
        {
            cache=0;
        }
        lastRunTime = Time.time;
        return ans;
    }
    /// <summary>
    /// 是否到时间了（不重置计时）
    /// </summary>
    /// <returns></returns>
    public bool TestReady()
    {
        return (cache > waitTime);
    }


    public override string ToString()
    {
        string[] datas = new string[] { waitTime.ToString(), Mathf.Clamp(cache+(Time.time-lastRunTime),0,float.MaxValue).ToString() };
        return JsonConvert.SerializeObject(datas);
    }
    public Ticker Deserialize(string data)
    {
        Ticker t = new Ticker();
        t.FromString(data);
        return t;
    }
    public void FromString(string data)
    {
        string[] ans = JsonConvert.DeserializeObject<string[]>(data);
        waitTime = float.Parse(ans[0]);
        cache = float.Parse(ans[1]);
    }
}

[Serializable]
public class Waiter
{
    public float waitRemain;
    [HideInInspector]
     public float lastRunTime;
    public void AddWaitTime(float t)
    {
        waitRemain = t;
        lastRunTime = Time.time;
    }
    public void StackWaitTime(float i)
    {
        waitRemain += i;
    }
    public bool IsReady()
    {
        if (waitRemain <= 0) return true;

        waitRemain =Mathf.Clamp(waitRemain-(Time.time - lastRunTime),0,100000);
        lastRunTime = Time.time;
        return waitRemain <= 0;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
    public static Waiter FromString(string p)
    {
        return JsonConvert.DeserializeObject<Waiter>(p);
    }
}