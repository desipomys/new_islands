using System;
using System.Collections.Generic;
using UnityEngine;



public class OptEventCenter : MonoBehaviour
{
    static OptEventCenter worldCenter;
    public static OptEventCenter WorldCenter
    {
        get
        {
            return worldCenter;
        }
        set
        {
            if (worldCenter == null)
            {
                worldCenter = value;
            }
        }
    }
    //public Dictionary<string, List<Delegate>> eventDic = new Dictionary<string, List<Delegate>>();
    //public Dictionary<string, Delegate> funcDic = new Dictionary<string, Delegate>();
    //事件系统升级，添加了事件优先级，getparm覆盖,按tag发布事件等功能
    public Dictionary<TestEventName, List<Delegate_Plus>> eventDic = new Dictionary<TestEventName, List<Delegate_Plus>>();
    public Dictionary<TestEventName, List<Delegate_Plus>> funcDic = new Dictionary<TestEventName, List<Delegate_Plus>>();


    #region listen
    public void ListenEvent(TestEventName name, Action fun)
    {
        innerListenEvent(name, fun);
    }
    public void ListenEvent<T>(TestEventName name, Action<T> fun)
    {
        innerListenEvent(name, fun);
    }
    public void ListenEvent<T1, T2>(TestEventName name, Action<T1, T2> fun)
    {
        innerListenEvent(name, fun);
    }
    public void ListenEvent<T1, T2, T3>(TestEventName name, Action<T1, T2, T3> fun)
    {
        innerListenEvent(name, fun);
    }
    //FUNC，给想把自己变量注册给外部获取的组件
    public void RegistFunc<T>(TestEventName name, Func<T> fun)
    {
        innerRegistFunc(name, fun);
    }
    public void RegistFunc<T1, T2>(TestEventName name, Func<T1, T2> fun)
    {
        innerRegistFunc(name, fun);
    }

    #endregion

    #region unlisten
    public void UnListenEvent(TestEventName name, Action fun)
    {
        innerUnListenEvent(name, fun);
    }
    public void UnListenEvent<T>(TestEventName name, Action<T> fun)
    {
        innerUnListenEvent(name, fun);
    }
    public void UnListenEvent<T1, T2>(TestEventName name, Action<T1, T2> fun)
    {
        innerUnListenEvent(name, fun);
    }
    public void UnListenEvent<T1, T2, T3>(TestEventName name, Action<T1, T2, T3> fun)
    {
        innerUnListenEvent(name, fun);
    }

    public void UnRegistFunc<T>(TestEventName name, Func<T> d)//必须加判定unregist的func是否与原来监听的那个func相同
    {
        innerUnRegistFunc(name, d);
    }
    public void UnRegistFunc<T, P>(TestEventName name, Func<T, P> d)//必须加判定unregist的func是否与原来监听的那个func相同
    {
        innerUnRegistFunc(name, d);
    }
    #endregion

    #region send
    public void SendEvent(TestEventName name)
    {
        List<Delegate_Plus> dels;
        if (eventDic.TryGetValue(name,out dels))
        {
            if (dels != null)
            {
                for (int i = 0; i < dels.Count; i++)
                {
                    //Debug.Log(dels[i].del.GetType().Name+i);
                    ((Action)dels[i].del)();
                }
            }
        }
    }
    public void SendEvent<T>(TestEventName name, T parm)
    {
        List<Delegate_Plus> dels;
        if (eventDic.TryGetValue(name, out dels))
        {
            if (dels != null)
            {
                for (int i = 0; i < dels.Count; i++)
                {
                    ((Action<T>)dels[i].del)(parm);
                }
            }
        }
    }
    public void SendEvent<T1, T2>(TestEventName name, T1 parm1, T2 parm2)
    {
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = eventDic[name];
            if (dels != null)
            {
                for (int i = 0; i < dels.Count; i++)
                {
                    ((Action<T1, T2>)dels[i].del)(parm1, parm2);
                }
            }
        }
    }
    public void SendEvent<T1, T2, T3>(TestEventName name, T1 parm1, T2 parm2, T3 parm3)
    {
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = eventDic[name];
            if (dels != null)
            {
                for (int i = 0; i < dels.Count; i++)
                {
                    ((Action<T1, T2, T3>)dels[i].del)(parm1, parm2, parm3);
                }
            }
        }
    }

    public T GetParm<T>(TestEventName name)
    {
        if (funcDic.ContainsKey(name))
        {
            if (funcDic[name][0].overrided)
            {
                return ((Func<TestEventName, T>)funcDic[name][0].del)(name);
            }
            else return ((Func<T>)funcDic[name][0].del)();
        }
        else return default(T);
    }
    public T2 GetParm<T1, T2>(TestEventName name, T1 parm1)
    {
        if (funcDic.ContainsKey(name))
        {
            if (funcDic[name][0].overrided)
            {
                return ((Func<TestEventName, T1, T2>)funcDic[name][0].del)(name, parm1);
            }
            else return ((Func<T1, T2>)funcDic[name][0].del)(parm1);
        }
        else return default(T2);
    }


    #endregion
    #region send by tag

    public void SendEvent(TestEventName name, string tag)
    {
        List<Delegate_Plus> dels;
        if (eventDic.TryGetValue(name, out dels))
        {
            if (dels != null)
            {
                for (int i = 0; i < dels.Count; i++)
                {
                    if (string.IsNullOrEmpty(dels[i].tag) || dels[i].tag == tag)
                        ((Action)dels[i].del)();
                }
            }
        }
    }
    public void SendEvent<T>(TestEventName name, string tag, T parm)
    {
        List<Delegate_Plus> dels;
        if (eventDic.TryGetValue(name, out dels))
        {
            if (dels != null)
            {
                for (int i = 0; i < dels.Count; i++)
                {
                    if (string.IsNullOrEmpty(dels[i].tag) || dels[i].tag == tag)
                        ((Action<T>)dels[i].del)(parm);
                }
            }
        }
    }
    public void SendEvent<T1, T2>(TestEventName name, string tag, T1 parm1, T2 parm2)
    {
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = eventDic[name];
            if (dels != null)
            {
                for (int i = 0; i < dels.Count; i++)
                {
                    if (string.IsNullOrEmpty(dels[i].tag) || dels[i].tag == tag)
                        ((Action<T1, T2>)dels[i].del)(parm1, parm2);
                }
            }
        }
    }
    public void SendEvent<T1, T2, T3>(TestEventName name, string tag, T1 parm1, T2 parm2, T3 parm3)
    {
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = eventDic[name];
            if (dels != null)
            {
                for (int i = 0; i < dels.Count; i++)
                {
                    if (string.IsNullOrEmpty(dels[i].tag) || dels[i].tag == tag)
                        ((Action<T1, T2, T3>)dels[i].del)(parm1, parm2, parm3);
                }
            }
        }
    }

    #endregion
    #region callhiddengetparm
    /// <summary>
    /// prio无用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="prio"></param>
    /// <returns></returns>
    public T GetHiddenParm<T>(TestEventName name, int prio)
    {
        if (funcDic.ContainsKey(name))
        {
            if (funcDic[name].Count <= 1)
            {
                Debug.Log("gethiddenparm失败");
                return default(T);
            }
            return ((Func<T>)funcDic[name][1].del)();
        }
        else return default(T);
    }
    public T2 GetHiddenParm<T1, T2>(TestEventName name, int prio, T1 parm1)
    {
        if (funcDic.ContainsKey(name))
        {
            if (funcDic[name].Count <= 1)
            {
                Debug.Log("gethiddenparm失败");
                return default(T2);
            }
            return ((Func<T1, T2>)funcDic[name][1].del)(parm1);
        }
        else return default(T2);
    }
    #endregion
    #region inner
    void innerRegistFunc(TestEventName name, Delegate fun)
    {
        if (funcDic.ContainsKey(name))
        {
            //GameLogger.Log(name+"重复");
            funcDic[name][0] = new Delegate_Plus(fun, 0, "", false);
        }
        else funcDic.Add(name, new List<Delegate_Plus>() { new Delegate_Plus(fun, 0, "", false) });
    }
    void innerUnRegistFunc(TestEventName name, Delegate d)
    {
        if (funcDic.ContainsKey(name))
        {
            for (int i = 0; i < funcDic[name].Count; i++)
            {
                if (funcDic[name][i].del == d)
                    funcDic[name].RemoveAt(i);
            }

        }
    }
    int innerOverrideFunc(TestEventName name, Delegate fun, int prio, string tag)//仅允许单次重载，返回被重载的prio，-1则为失败
    {
        if (funcDic.ContainsKey(name))
        {
            if (funcDic[name][0].prio == 0)//被重载的是无prio对象
            {
                funcDic[name].Add(new Delegate_Plus(fun, prio, tag, true));
                funcDic[name].Sort();
                return 0;
            }
            else//被重载者有prio,若自己prio小于原有，则重载失败
            {
                return -1;
            }
        }
        return -1;
    }
    public void UnOverrideFunc(TestEventName name)
    {
        if (funcDic.ContainsKey(name))
        {
            if (funcDic[name].Count > 1)
                funcDic[name].RemoveAt(0);
        }
    }

    void innerListenEvent(TestEventName name, Delegate fun)
    {
        innerListenEvent(name, fun, 0, "");
    }
    void innerUnListenEvent(TestEventName name, Delegate fun)
    {
        if (eventDic.ContainsKey(name))
        {
            for (int i = 0; i < eventDic[name].Count; i++)
            {
                if (eventDic[name][i].del == fun) eventDic[name].RemoveAt(i);
            }

            if (eventDic[name].Count == 0) eventDic.Remove(name);
        }
    }
    void innerListenEvent(TestEventName name, Delegate fun, int prio, string tag)
    {
        if (eventDic.ContainsKey(name))
        {
            bool contain = false;
            for (int i = 0; i < eventDic[name].Count; i++)
            {
                if (eventDic[name][i].del == fun) contain = true;
            }
            if (!contain)
                eventDic[name].Add(new Delegate_Plus(fun, prio, tag, false));
            eventDic[name].Sort();
        }
        else { eventDic.Add(name, new List<Delegate_Plus>() { new Delegate_Plus(fun, prio, tag, false) }); }
    }
    void innerUnListenEvent(TestEventName name, Delegate fun, int prio, string tag)
    {
        innerUnListenEvent(name, fun);
    }
    #endregion

    #region newlisten
    public void ListenEvent(TestEventName name, Action a, int prio, string tag)
    {
        innerListenEvent(name, a, prio, tag);
    }
    public void ListenEvent<T>(TestEventName name, Action<T> a, int prio, string tag)
    {
        innerListenEvent(name, a, prio, tag);
    }
    public void ListenEvent<T1, T2>(TestEventName name, Action<T1, T2> a, int prio, string tag)
    {
        innerListenEvent(name, a, prio, tag);
    }
    public void ListenEvent<T1, T2, T3>(TestEventName name, Action<T1, T2, T3> a, int prio, string tag)
    {
        innerListenEvent(name, a, prio, tag);
    }

    public void RegistFunc<T>(TestEventName name, Func<T> fun, int prio, string tag)
    {
        innerRegistFunc(name, fun);
    }
    public void RegistFunc<T1, T2>(TestEventName name, Func<T1, T2> fun, int prio, string tag)
    {
        innerRegistFunc(name, fun);
    }

    public void OverrideFunc<T>(TestEventName name, Func<TestEventName, T> fun, int prio, string tag)
    {
        innerOverrideFunc(name, fun, prio, tag);
    }
    public void OverrideFunc<T1, T2>(TestEventName name, Func<TestEventName, T1, T2> fun, int prio, string tag)
    {
        innerOverrideFunc(name, fun, prio, tag);
    }
    #endregion

    #region newunlisten

    #endregion

    public virtual void Init()
    {
        //findDescriptor();
        findRegister();

    }
    public void ClearListener()
    {
        eventDic.Clear();
    }

    protected void findRegister()
    {
        
    }
}
