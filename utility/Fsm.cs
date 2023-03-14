using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Fsm : IFsmStat
{//支持fsm嵌套
    public Dictionary<string, IFsmStat> allstats = new Dictionary<string, IFsmStat>();
    public IFsmStat current;
    public Fsm parent;
    public string currentStatName;
    public bool ispause;

    public string GetCurrentStatName()
    {
        return currentStatName;
    }

    public virtual void OnInit(Fsm fsm)
    {
        parent = fsm;
    }
    public virtual void OnResume(Fsm fsm)
    {
        current.OnResume(this);
        ispause = false;
    }
    public virtual void OnPause(Fsm fsm)
    {
        current.OnPause(this);
        ispause = true;
    }
    public virtual void OnEnter(Fsm fsm)
    {
        current.OnEnter(this);
    }
    public virtual void OnLeave(Fsm fsm)
    {
        current.OnLeave(this);
    }
    public virtual void OnUpdate(Fsm fsm)
    {
        if (ispause) return;
        current.OnUpdate(this);
    }
    public virtual void OnDelete(Fsm fsm)
    {
        foreach (var item in allstats)
        {
            item.Value.OnDelete(this);
        }
    }

    public virtual void AddStat(string name, IFsmStat stat)
    {
        if (stat == this) return;
        if (allstats.Count == 0) { current = stat; currentStatName = name; }
        allstats.Add(name, stat);
    }
    public virtual void ChangeStat(string targetStat)
    {
        if (string.IsNullOrEmpty(targetStat)) return;
        if (allstats.ContainsKey(targetStat))
        {
            current.OnLeave(this);
            current = allstats[targetStat];
            currentStatName = targetStat;
            current.OnEnter(this);
        }
    }
    public virtual bool ChangeStatFromTo(string from, string to)
    {
        if (currentStatName != from) return false;
        else
        {
            ChangeStat(to);
            return true;
        }
    }
    public virtual void SetStartStat(string name)
    {
        try
        {
            current = allstats[name];
            currentStatName = name;
        }
        catch
        {
            return;
        }
    }
}


public interface IFsmStat
{
    //void OnInit(Action<Fsm> onenter,Action<Fsm> onleave,Action<Fsm> onupdate);
    void OnEnter(Fsm fsm);
    void OnLeave(Fsm fsm);
    void OnUpdate(Fsm fsm);
    void OnPause(Fsm fsm);
    void OnResume(Fsm fsm);
    void OnDelete(Fsm fsm);
}

public class FsmStat : IFsmStat
{
    Action<Fsm> onEnter, onLeave, onUpdate, onPause, onResume, onDelete;

    #region 构造函数

    public FsmStat(Action<Fsm> onenter, Action<Fsm> onleave, Action<Fsm> onupdate, Action<Fsm> onpause, Action<Fsm> OnResume, Action<Fsm> OnDelete)
    {
        onEnter = onenter;
        onLeave = onleave;
        onUpdate = onupdate;
        onPause = onpause;
        this.onResume = onResume;
        onDelete = OnDelete;
    }
    public FsmStat(Action<Fsm> onenter, Action<Fsm> onleave, Action<Fsm> onupdate)
    {
        onEnter = onenter;
        onLeave = onleave;
        onUpdate = onupdate;
    }
    public FsmStat(Action<Fsm> onenter)
    {
        onEnter = onenter;
    }

    #endregion

    public void OnEnter(Fsm fsm)
    {
        onEnter(fsm);
    }
    public void OnLeave(Fsm fsm)
    {
        onLeave(fsm);
    }
    public void OnUpdate(Fsm fsm)
    {
        onUpdate(fsm);
    }
    public void OnPause(Fsm fsm)
    {
        onPause(fsm);
    }
    public void OnResume(Fsm fsm)
    {
        onResume(fsm);
    }
    public void OnDelete(Fsm fsm)
    {
        onDelete(fsm);
    }
}