//监听其他组件变化事件，将背包、npc状态的变化翻译成GOAP的state
//保存一些ai感知世界的变量，比如危险系数，危险类型，团队人数等
//根据变化的场景添加/移除一些action,实现IGoap响应Goap内部事件，继承mono和ieventregist
//getMoveCost(vector3 target)
//getwaitCost(float)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//每种ai要继承此类重写creategoalstate方法
public class BaseGoapSensor : MonoBehaviour,IEventRegister,IDataContainer,IGoap {

    public int dangerLevel,dangerType,defendPower,attackPower;
    public NPCTeam myTeam;
    DataNode data=new DataNode("root");
   protected GoapAgent agent;
   protected EventCenter center;
    public void OnEventRegist(EventCenter c)
    {
        center=c;
        c.ListenEvent<NPCCommand>("SendCommand",OnReciveCommand);
        agent=GetComponent<GoapAgent>();
    }
    public void AfterEventRegist()
    {
        throw new NotImplementedException();
    }

    void FixedUpdate()
    {
        //距离队伍任一成员太远自动脱离队伍
    }
    public virtual HashSet<KeyValuePair<string,object>> createGoalState ()
    {
        //根据情况给出goal
        return null;
    }

    public virtual void OnJoinTeam(NPCTeam t)
    {
        
    }
    public virtual void OnLeftTeam(NPCTeam t)
    {
        
    }

    public virtual void OnReciveCommand(NPCCommand command)
    {

    }

    protected Ticker t=new Ticker(5f);

    public virtual int GetDataCollectPrio => 0;

    public void OnHit(Damage d,EventCenter source)
    {
        dangerLevel+=(int)d.value;

        if(t.IsReady())
        {
            //replan
            
            agent.RePlan();
        }
        else
        {
            t.ReTick();
        }
    }
    public void SetData<T>(string name,T d)
    {
        data.SetData<T>(name,d);
    }
    public T GetData<T>(string name)
    {
        T ans;
        data.GetData<T>(name,out ans);
        return ans; 
    }

   

    

    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        throw new NotImplementedException();
    }

    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
        throw new NotImplementedException();
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
    {
        throw new NotImplementedException();
    }

    public void actionsFinished()
    {
        throw new NotImplementedException();
    }

    public void planAborted(GoapAction aborter)
    {
        throw new NotImplementedException();
    }

    public bool moveAgent(GoapAction nextAction)
    {
        throw new NotImplementedException();
    }

    public object ToObject()
    {
        return null;
    }
    public void FromObject(object data)
    {
        throw new NotImplementedException();
    }
}

public class NPCTeam//不是阵营，只是临时形成的队伍
{
    public int id;
    public EventCenter[] npcs;//玩家也可以加入
}
public class NPCCommand
{
    public NPCCommandType type;
    public Dictionary<string,object> datas;
}
public enum NPCCommandType
{
    //行为
    areaDefend,entityGaurd,captureFlag,collectSth,buildDefend,

    //守则
    dontPick,dontAttack,passiveAttackOnly,freeAttack,dontMakeNosie,dontUnloadGoods,
    dontMove
}