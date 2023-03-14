using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public class EventCenter : MonoBehaviour,IDataContainer
{
    static EventCenter worldCenter;
    public static EventCenter WorldCenter
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
                worldCenter.uuid = -1;
            }
        }
    }
    [SerializeField]
    long uuid=0;
    public long UUID
    {
        get { return uuid; }
        set {
            if (uuid == 0)
            {
                uuid = value;
            }
        }
    }

    public virtual int GetDataCollectPrio => 0;

    public EventCenter friend;

    //public Dictionary<string, List<Delegate>> eventDic = new Dictionary<string, List<Delegate>>();
    //public Dictionary<string, Delegate> funcDic = new Dictionary<string, Delegate>();
    //事件系统升级，添加了事件优先级，getparm覆盖,按tag发布事件等功能
    public Dictionary<string, List<Delegate_Plus>> eventDic = new Dictionary<string, List<Delegate_Plus>>(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, List<Delegate_Plus>> funcDic = new Dictionary<string, List<Delegate_Plus>>(StringComparer.OrdinalIgnoreCase);
    

    #region listen
    public void ListenEvent(string name, Action fun)
    {
        innerListenEvent(name, fun);
    }
    public void ListenEvent<T>(string name, Action<T> fun)
    {
        innerListenEvent(name, fun);
    }
    public void ListenEvent<T1, T2>(string name, Action<T1, T2> fun)
    {
        innerListenEvent(name, fun);
    }
    public void ListenEvent<T1, T2, T3>(string name, Action<T1, T2, T3> fun)
    {
        innerListenEvent(name, fun);
    }
    public void ListenEvent<T1, T2, T3,T4>(string name, Action<T1, T2, T3,T4> fun)
    {
        innerListenEvent(name, fun);
    }
    //FUNC，给想把自己变量注册给外部获取的组件
    public void RegistFunc<T>(string name, Func<T> fun)
    {
        innerRegistFunc(name, fun);
    }
    public void RegistFunc<T1, T2>(string name, Func<T1, T2> fun)
    {
        innerRegistFunc(name, fun);
    }
     public void RegistFunc<T1, T2,T3>(string name, Func<T1, T2,T3> fun)
    {
        innerRegistFunc(name, fun);
    }

    #endregion

    #region unlisten
    public void UnListenEvent(string name, Action fun)
    {
        innerUnListenEvent(name, fun);
    }
    public void UnListenEvent<T>(string name, Action<T> fun)
    {
        innerUnListenEvent(name, fun);
    }
    public void UnListenEvent<T1, T2>(string name, Action<T1, T2> fun)
    {
        innerUnListenEvent(name, fun);
    }
    public void UnListenEvent<T1, T2, T3>(string name, Action<T1, T2, T3> fun)
    {
        innerUnListenEvent(name, fun);
    }
    public void UnListenEvent<T1, T2, T3,T4>(string name, Action<T1, T2, T3,T4> fun)
    {
        innerUnListenEvent(name, fun);
    }

    public void UnRegistFunc<T>(string name, Func<T> d)//必须加判定unregist的func是否与原来监听的那个func相同
    {
        innerUnRegistFunc(name, d);
    }
    public void UnRegistFunc<T, P>(string name, Func<T, P> d)//必须加判定unregist的func是否与原来监听的那个func相同
    {
        innerUnRegistFunc(name, d);
    }
    public void UnRegistFunc<T, P,P1>(string name, Func<T, P,P1> d)//必须加判定unregist的func是否与原来监听的那个func相同
    {
        innerUnRegistFunc(name, d);
    }

    public void ForceUnRegistFunc<T>(string name)//强行清除getparm
    {
        if (funcDic.ContainsKey(name))
        {
            funcDic.Remove(name);

        }
    }
    public void ForceUnRegistFunc<T, P>(string name)//必须加判定unregist的func是否与原来监听的那个func相同
    {
        if (funcDic.ContainsKey(name))
        {
            funcDic.Remove(name);

        }
    }
    public void ForceUnRegistFunc<T, P,P1>(string name)//必须加判定unregist的func是否与原来监听的那个func相同
    {
        if (funcDic.ContainsKey(name))
        {
            funcDic.Remove(name);

        }
    }
    /// <summary>
    /// 强制移除叫name的事件监听，除非能保证指定监听仅有一个接收者否则不要强制移除
    /// </summary>
    /// <param name="name"></param>
    public void ForceUnListenEvent(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            eventDic.Remove(name);

        }
    }
    #endregion

    #region send
    public void SendEvent(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>( eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count-1; i >=0 ; i--)//循环中元素可能被删掉，因此需倒序遍历,还有bug，需继续改正
                {
                    if(dels[i]!=null)
                    ((Action)dels[i].del)();
                }
            }
        }
        if(friend!=null&&friend!=this)SendEvent(name);
    }
    public void SendEvent<T>(string name, T parm)
    {
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>(eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count - 1; i >= 0; i--)//循环中元素可能被删掉，因此需倒序遍历
                {
                    if (dels[i] != null)
                        
                    try
                    {
                            ((Action<T>)dels[i].del)(parm);
                        }
                    catch (Exception)
                    {
                        Debug.Log("事件" + name + ":" + typeof(T).Name +  "但目标是" + dels[i].del.GetType());
                        throw;
                    }
                }
            }
        }
        if(friend!=null && friend != this) SendEvent<T>(name,parm);
    }
    public void SendEvent<T1, T2>(string name, T1 parm1, T2 parm2)
    {
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>(eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count - 1; i >= 0; i--)//循环中元素可能被删掉，因此需倒序遍历
                {
                    if (dels[i] != null)
                    {
                        try
                        {
                            ((Action<T1, T2>)dels[i].del)(parm1, parm2);
                        }
                        catch (Exception)
                        {
                            Debug.Log("事件"+name+":"+typeof(T1).Name + ":" + typeof(T2).Name + "但目标是" + dels[i].del.GetType());
                            throw;
                        }
                        
                    }
                }
            }
        }
        if(friend!=null && friend != this) SendEvent<T1,T2>(name,parm1,parm2);
    }
    public void SendEvent<T1, T2, T3>(string name, T1 parm1, T2 parm2, T3 parm3)
    {
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>(eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count - 1; i >= 0; i--)//循环中元素可能被删掉，因此需倒序遍历
                {
                    if (dels[i] != null)
                        ((Action<T1, T2, T3>)dels[i].del)(parm1, parm2, parm3);
                }
            }
        }
        if(friend!=null && friend != this) SendEvent<T1,T2,T3>(name,parm1,parm2,parm3);
    }
    public void SendEvent<T1, T2, T3,T4>(string name, T1 parm1, T2 parm2, T3 parm3,T4 parm4)
    {
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>(eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count - 1; i >= 0; i--)//循环中元素可能被删掉，因此需倒序遍历
                {
                    if (dels[i] != null)
                        ((Action<T1, T2, T3,T4>)dels[i].del)(parm1, parm2, parm3,parm4);
                }
            }
        }
        if(friend!=null && friend != this) SendEvent<T1,T2,T3,T4>(name,parm1,parm2,parm3,parm4);
    }

    public void SendEvent(string name,out bool safe)
    {
        safe = false;
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>(eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count - 1; i >= 0; i--)//循环中元素可能被删掉，因此需倒序遍历,还有bug，需继续改正
                {
                    if (dels[i] != null)
                    { ((Action)dels[i].del)(); safe = true; }
                }
            }
        }
        if (friend != null && friend != this) SendEvent(name);
    }
    public void SendEvent<T>(string name, T parm, out bool safe)
    {
        safe = false;
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>(eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count - 1; i >= 0; i--)//循环中元素可能被删掉，因此需倒序遍历
                {
                    if (dels[i] != null)
                    { ((Action<T>)dels[i].del)(parm); safe = true; }
                }
            }
        }
        if (friend != null && friend != this) SendEvent<T>(name, parm);
    }
    public void SendEvent<T1, T2>(string name, T1 parm1, T2 parm2, out bool safe)
    {
        safe = false;
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>(eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count - 1; i >= 0; i--)//循环中元素可能被删掉，因此需倒序遍历
                {
                    if (dels[i] != null)
                    { ((Action<T1, T2>)dels[i].del)(parm1, parm2); safe = true; }
                }
            }
        }
        if (friend != null && friend != this) SendEvent<T1, T2>(name, parm1, parm2);
    }
    public void SendEvent<T1, T2, T3>(string name, T1 parm1, T2 parm2, T3 parm3, out bool safe)
    {
        safe = false;
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>(eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count - 1; i >= 0; i--)//循环中元素可能被删掉，因此需倒序遍历
                {
                    if (dels[i] != null)
                    { ((Action<T1, T2, T3>)dels[i].del)(parm1, parm2, parm3); safe = true; }
                }
            }
        }
        if (friend != null && friend != this) SendEvent<T1, T2, T3>(name, parm1, parm2, parm3);
    }
    public void SendEvent<T1, T2, T3, T4>(string name, T1 parm1, T2 parm2, T3 parm3, T4 parm4, out bool safe)
    {
        safe = false;
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>(eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count - 1; i >= 0; i--)//循环中元素可能被删掉，因此需倒序遍历
                {
                    if (dels[i] != null)
                    { ((Action<T1, T2, T3, T4>)dels[i].del)(parm1, parm2, parm3, parm4); safe = true; }
                }
            }
        }
        if (friend != null && friend != this) SendEvent<T1, T2, T3, T4>(name, parm1, parm2, parm3, parm4);
    }

    public T GetParm<T>(string name)
    {
        if (funcDic.ContainsKey(name))
        {
            if (funcDic[name][0].overrided)
            {
                return ((Func<string, T>)funcDic[name][0].del)(name);
            }
            else return ((Func<T>)funcDic[name][0].del)();
        }
        else return default(T);
    }
    public T2 GetParm<T1, T2>(string name, T1 parm1)
    {
        if (funcDic.ContainsKey(name))
        {
            if (funcDic[name][0].overrided)
            {
               return ((Func<string, T1, T2>)funcDic[name][0].del)(name, parm1);
            }
            else return ((Func<T1, T2>)funcDic[name][0].del)(parm1);
        }
        else return default(T2);
    }
    public T3 GetParm<T1, T2,T3>(string name, T1 parm1,T2 parm2)
    {
        if (funcDic.ContainsKey(name))
        {
            if (funcDic[name][0].overrided)
            {
               return ((Func<string, T1, T2,T3>)funcDic[name][0].del)(name, parm1,parm2);
            }
            else return ((Func<T1, T2,T3>)funcDic[name][0].del)(parm1,parm2);
        }
        else return default(T3);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="safe">返回值是否可靠</param>
    /// <returns></returns>
    public T GetParm<T>(string name,out bool safe)//返回值是否可靠
    {
        if (funcDic.ContainsKey(name))
        {
            safe = true;
            if (funcDic[name][0].overrided)
            {

                return ((Func<string, T>)funcDic[name][0].del)(name);
            }
            else return ((Func<T>)funcDic[name][0].del)();
        }
        else { safe = false; return default(T); }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="name"></param>
    /// <param name="parm1"></param>
    /// <param name="safe">返回值是否可靠</param>
    /// <returns></returns>
    public T2 GetParm<T1, T2>(string name, T1 parm1,out bool safe)
    {
        if (funcDic.ContainsKey(name))
        {
            safe = true;
            if (funcDic[name][0].overrided)
            {
                return ((Func<string, T1, T2>)funcDic[name][0].del)(name, parm1);
            }
            else return ((Func<T1, T2>)funcDic[name][0].del)(parm1);
        }
        else { safe = false; return default(T2); }
    }
    public T3 GetParm<T1, T2,T3>(string name, T1 parm1,T2 parm2,out bool safe)
    {
        if (funcDic.ContainsKey(name))
        {
            safe = true;
            if (funcDic[name][0].overrided)
            {
                return ((Func<string, T1, T2,T3>)funcDic[name][0].del)(name, parm1,parm2);
            }
            else return ((Func<T1, T2,T3>)funcDic[name][0].del)(parm1,parm2);
        }
        else { safe = false; return default(T3); }
    }
    #endregion
    #region send by tag

    public void SendEvent(string name,string tag)
    {
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>(eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count - 1; i >= 0; i--)//循环中元素可能被删掉，因此需倒序遍历//性能热点
                {
                    //int p = Mathf.Clamp(i, 0, dels.Count - 1);
                    if (dels[i] != null)
                        if (dels[i].tag==tag)
                            ((Action)dels[i].del)();
                }
            }
        }
    }
    public void SendEvent<T>(string name, string tag, T parm)
    {
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>(eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count - 1; i >= 0; i--)//循环中元素可能被删掉，因此需倒序遍历
                {
                    //int p = Mathf.Clamp(i, 0, dels.Count - 1);
                    if (dels[i] != null)
                        if ( dels[i].tag == tag)
                            ((Action<T>)dels[i].del)(parm);
                }
            }
        }
    }
    public void SendEvent<T1, T2>(string name, string tag, T1 parm1, T2 parm2)
    {
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>(eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count - 1; i >= 0; i--)//循环中元素可能被删掉，因此需倒序遍历
                {
                    //int p = Mathf.Clamp(i, 0, dels.Count - 1);
                    if (dels[i] != null)
                        if ( dels[i].tag == tag)
                            ((Action<T1, T2>)dels[i].del)(parm1, parm2);
                }
            }
        }
    }
    public void SendEvent<T1, T2, T3>(string name, string tag, T1 parm1, T2 parm2, T3 parm3)
    {
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>(eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count - 1; i >= 0; i--)//循环中元素可能被删掉，因此需倒序遍历
                {
                    //int p = Mathf.Clamp(i, 0, dels.Count - 1);
                    if (dels[i] != null)
                        if ( dels[i].tag == tag)
                            ((Action<T1, T2, T3>)dels[i].del)(parm1, parm2, parm3);
                }
            }
        }
    }
     public void SendEvent<T1, T2, T3,T4>(string name, string tag, T1 parm1, T2 parm2, T3 parm3,T4 parm4)
    {
        if (eventDic.ContainsKey(name))
        {
            List<Delegate_Plus> dels = new List<Delegate_Plus>(eventDic[name]);
            if (dels != null)
            {
                for (int i = dels.Count - 1; i >= 0; i--)//循环中元素可能被删掉，因此需倒序遍历
                {
                    //int p = Mathf.Clamp(i, 0, dels.Count - 1);
                    if (dels[i] != null)
                        if ( dels[i].tag == tag)
                            ((Action<T1, T2, T3,T4>)dels[i].del)(parm1, parm2, parm3,parm4);
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
    public T GetHiddenParm<T>(string name, int prio)
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
        public T2 GetHiddenParm<T1, T2>(string name, int prio, T1 parm1)
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
         public T3 GetHiddenParm<T1, T2,T3>(string name, int prio, T1 parm1,T2 parm2)
        {
            if (funcDic.ContainsKey(name))
            {
                if (funcDic[name].Count <= 1)
                {
                    Debug.Log("gethiddenparm失败");
                    return default(T3);
                }
                return ((Func<T1, T2,T3>)funcDic[name][1].del)(parm1,parm2);
            }
            else return default(T3);
        }
    #endregion
    #region inner
    void innerRegistFunc(string name, Delegate fun)
    {
        if (funcDic.ContainsKey(name))
        {
            //GameLogger.Log(name+"重复");
            if (funcDic[name].Count > 0)
                funcDic[name][0] = new Delegate_Plus(fun, 0, "", false);
            else funcDic[name].Add(new Delegate_Plus(fun, 0, "", false));
        }
        else funcDic.Add(name, new List<Delegate_Plus>() { new Delegate_Plus(fun, 0, "",false) });
    }
    void innerUnRegistFunc(string name, Delegate d)
    {
        if (funcDic.ContainsKey(name))
        {
            for (int i = funcDic[name].Count-1; i >=0 ; i--)
            {
                int p = Mathf.Clamp(i, 0, funcDic[name].Count - 1);
                if (funcDic[name][p].del == d)
                    funcDic[name].RemoveAt(p);
            }

        }
    }
    int innerOverrideFunc(string name, Delegate fun, int prio, string tag)//仅允许单次重载，返回被重载的prio，-1则为失败
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
   public void UnOverrideFunc(string name)
    {
        if (funcDic.ContainsKey(name))
        {
            if (funcDic[name].Count > 1)
             funcDic[name].RemoveAt(0);
        }
    }

    void innerListenEvent(string name, Delegate fun)
    {
        innerListenEvent(name, fun, 0, "");
    }
    void innerUnListenEvent(string name, Delegate fun)
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
    void innerListenEvent(string name, Delegate fun, int prio, string tag)
    {
        if (eventDic.ContainsKey(name))
        {
            bool contain = false;
            for (int i = 0; i < eventDic[name].Count; i++)
            {
                if (eventDic[name][i].del == fun) contain = true;
            }
            if (!contain)
                eventDic[name].Add(new Delegate_Plus(fun, prio, tag,false));
            eventDic[name].Sort();
        }
        else { eventDic.Add(name, new List<Delegate_Plus>() { new Delegate_Plus(fun, prio, tag,false) }); }
    }
    void innerUnListenEvent(string name, Delegate fun, int prio, string tag)
    {
        innerUnListenEvent(name, fun);
    }
    
    #endregion

    #region newlisten
    public void ListenEvent(string name, Action a, int prio, string tag)
    {
        innerListenEvent(name, a, prio, tag);
    }
    public void ListenEvent<T>(string name, Action<T> a, int prio, string tag)
    {
        innerListenEvent(name, a, prio, tag);
    }
    public void ListenEvent<T1, T2>(string name, Action<T1, T2> a, int prio, string tag)
    {
        innerListenEvent(name, a, prio, tag);
    }
    public void ListenEvent<T1, T2, T3>(string name, Action<T1, T2, T3> a, int prio, string tag)
    {
        innerListenEvent(name, a, prio, tag);
    }
    public void ListenEvent<T1, T2, T3,T4>(string name, Action<T1, T2, T3,T4> a, int prio, string tag)
    {
        innerListenEvent(name, a, prio, tag);
    }

/// <summary>
/// 暂不可用
/// </summary>
/// <param name="name"></param>
/// <param name="fun"></param>
/// <param name="prio"></param>
/// <param name="tag"></param>
/// <typeparam name="T"></typeparam>
    public void RegistFunc<T>(string name, Func<T> fun, int prio, string tag)
    {
        innerRegistFunc(name, fun);
    }
    /// <summary>
    /// 暂不可用
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fun"></param>
    /// <param name="prio"></param>
    /// <param name="tag"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <returns></returns>
    public void RegistFunc<T1, T2>(string name, Func<T1, T2> fun, int prio, string tag)
    {
        innerRegistFunc(name, fun);
    }
    /// <summary>
    /// 咱不可用
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fun"></param>
    /// <param name="prio"></param>
    /// <param name="tag"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <returns></returns>
     public void RegistFunc<T1, T2,T3>(string name, Func<T1, T2,T3> fun, int prio, string tag)
    {
        innerRegistFunc(name, fun);
    }
    
    public void OverrideFunc<T>(string name, Func<string,T> fun, int prio, string tag)
    {
        innerOverrideFunc(name, fun, prio, tag);
    }
    public void OverrideFunc<T1, T2>(string name, Func<string,T1, T2> fun, int prio, string tag)
    {
        innerOverrideFunc(name, fun, prio, tag);
    }
    public void OverrideFunc<T1, T2,T3>(string name, Func<string,T1, T2,T3> fun, int prio, string tag)
    {
        innerOverrideFunc(name, fun, prio, tag);
    }
    #endregion

    #region newunlisten

    #endregion

    #region GetScriptGetSetter
    IScriptDataGetter iscriptDataGetter;
    IScriptFunctionCaller iscriptFunctionCaller;
    public IScriptDataGetter GetScriptDataGetter()
    {
        if(iscriptDataGetter==null)
       {iscriptDataGetter= gameObject.GetComponent<IScriptDataGetter>();
        return iscriptDataGetter;
       }
        else return iscriptDataGetter;
    }
    public IScriptFunctionCaller GetScriptFunctionCaller()
    {
        if(iscriptFunctionCaller==null)
       {iscriptFunctionCaller= gameObject.GetComponent<IScriptFunctionCaller>();
        return iscriptFunctionCaller;
       }
        else return iscriptFunctionCaller;
    }

    #endregion
    public virtual void Init()
    {
        //findDescriptor();
        findRegister();
        ListenEvent<bool>("SetActive", (bool value) => { gameObject.SetActive(value); });
    }
    public virtual void Init(long uid)
    {
        //findDescriptor();
        uuid = uid;
        Init();

    }
    
    public void ClearListener()
    {
        eventDic.Clear();
    }
    public void ClearListenerByName(string name)
    {
        if(eventDic.ContainsKey(name))
            eventDic[name].Clear();
    }
    public T AddComponentViaCenter<T>() where T:MonoBehaviour,IEventRegister
    {
        T t = gameObject.AddComponent<T>();
        t.OnEventRegist(this);
        t.AfterEventRegist();
        return t;
    }
    public void SetFriendCenter(EventCenter c){friend=c;}

    protected void findRegister()
    {
        IEventRegister[] regs = gameObject.GetComponents<IEventRegister>();
        for (int i = 0; i < regs.Length; i++)
        {
            regs[i].OnEventRegist(this);
        }
        IGetParmCutter[] cutters = gameObject.GetComponents<IGetParmCutter>();
        for (int i = 0; i < cutters.Length; i++)
        {
            cutters[i].OnGetParmCut(this);
        }
        for (int i = 0; i < regs.Length; i++)
        {
            regs[i].AfterEventRegist();
        }
    }

    void IDataContainer.FromObject(object data)
    {
        uuid = (long)data;
    }
    public override string ToString()
    {
        return uuid.ToString();
    }
    public object ToObject()
    {
        return uuid;
    }

    #region 注解驱动
    //失败的注解驱动
    /*[Obsolete]
    void findDescriptor()
    {
        MethodInfo a1 = this.GetType().GetMethod("getAction1", BindingFlags.NonPublic | BindingFlags.Instance);
        MethodInfo a2 = this.GetType().GetMethod("getAction2");
        MethodInfo a3 = this.GetType().GetMethod("getAction3");

        MethodInfo f1 = this.GetType().GetMethod("getFunc1");
        MethodInfo f2 = this.GetType().GetMethod("getFunc2");

        MonoBehaviour[] mbs = gameObject.GetComponents<MonoBehaviour>();
        for (int i = 0; i < mbs.Length; i++)//对于每个脚本
        {
            Type t = mbs[i].GetType();
            BindingFlags b = BindingFlags.Public | BindingFlags.Instance;
            MethodInfo[] mis = t.GetMethods(b);
            for (int j = 0; j < mis.Length; j++)//对于每个方法
            {
                Listen l = mis[j].GetCustomAttribute<Listen>();
                if (l != null)
                {
                    ParameterInfo[] pinfo = mis[j].GetParameters();
                    Type ret = mis[j].ReturnType;

                    if (ret != typeof(void))//有返回值
                    {
                        if (pinfo.Length > 1) continue;
                        if (pinfo.Length == 1) { }
                        else if (pinfo.Length == 0) { }

                    }
                    else//无返回值
                    {
                        Action c;
                        switch (pinfo.Length)
                        {
                            case 0:
                                c = (Action)Delegate.CreateDelegate(typeof(Action), mis[j]);
                                ListenEvent(l.name, c);
                                break;
                            case 1:
                                Debug.Log(pinfo[0].ParameterType);
                                ListenEvent(l.name, (Action)a1.MakeGenericMethod(new Type[] { pinfo[0].ParameterType }).
                                    Invoke(this, new object[] { mis[j] }));
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                        }
                    }
                }
            }
        }
    }

    Action<T> getAction1<T>(MethodInfo mi)
    {
        Debug.Log(mi);
        return (Action<T>)Action.CreateDelegate(typeof(Action<T>), mi);
    }
    Action<T1, T2> getAction2<T1, T2>(MethodInfo mi)
    {
        return (Action<T1, T2>)Delegate.CreateDelegate(typeof(Action<T1, T2>), mi);
    }
    Action<T1, T2, T3> getAction3<T1, T2, T3>(MethodInfo mi)
    {
        return (Action<T1, T2, T3>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3>), mi);
    }

    Func<T> getFunc1<T>(MethodInfo mi)
    {
        return (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), mi);
    }
    Func<T1, T2> getFunc2<T1, T2>(MethodInfo mi)
    {
        return (Func<T1, T2>)Delegate.CreateDelegate(typeof(Func<T1, T2>), mi);
    }*/
    #endregion
}

public interface IEventRegister
{

    void OnEventRegist(EventCenter e);
    void AfterEventRegist();
}
/// <summary>
///需要拦截getparm的类要实现此接口
/// </summary>
public interface IGetParmCutter
{
    void OnGetParmCut(EventCenter e);
}

public class Delegate_Plus : IComparable<Delegate_Plus>//升级版事件
{
    public bool overrided = false;
    public Delegate del;
    public int prio;//优先级,0为最低
    public string tag;//标签

    public Delegate_Plus(Delegate d, int p, string t, bool over)
    {
        del = d;
        prio = p;
        tag = t;
        overrided = over;
    }

    public int CompareTo(Delegate_Plus d)
    {
        return d.prio.CompareTo(prio);
    }
}