using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器的持有类型枚举，如果持有时的idle动画相同则视为同一类
/// </summary>
public enum PlayerHandAnimaState
{
    Idle=1,
    Pistol,
    Rifle,
    MG,
    RPG,
    Spear,
    Sheild,
    Flame,
    Axe,
}


public class PlayerAnimator : EntityAnimator
{
    Animator animator;
    Animator anim { get {if(animator==null) animator = GetComponent<Animator>(); return animator; } set { } }
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        

        center.ListenEvent<float, EventCenter, BaseTool, Damage>(nameof(PlayerEventName.onDie), onDie);
        center.ListenEvent(nameof(PlayerEventName.onRespawn), onRespawn);
        center.ListenEvent<Movement_Stat, float, DIR>(nameof(PlayerEventName.move), onMove);
        center.ListenEvent<string, int>(nameof(PlayerEventName.playAnima), PlayAnimation);
        //center.ListenEvent<int,BaseTool>(nameof(PlayerEventName.onHandChgTool), OnChangeTool);
    }

    public override void OnAnimationEvent(string eventEnum)
    {
        base.OnAnimationEvent(eventEnum);

        switch (eventEnum)
        {
            case "end":
                //AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(0);//还需转换为upper在持有当前武器下此动画的状态
                //string nam = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                //Debug.Log(lowerNameToUpperName(nam));
                //anim.CrossFade(lowerNameToUpperName(nam), 0.25f, 1, Mathf.Repeat(asi.normalizedTime, 1));
                Movement_Stat stat = (Movement_Stat)anim.GetInteger("WalkStat");
                //Debug.Log(nam+" "+stat);
                //如果当前在播的动画属于当前状态机
                if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name .ToUpper().Contains(stat.ToString().ToUpper()))
                {
                    anim.SetTrigger("IsAnyActive");
                }else anim.SetTrigger("IsAnyActive");

                break;
            default:
                break;
        }
        
    }

    #region 事件响应区
    public void PlayAnimation(string nam,int layer)
    {
        Debug.Log("play " + nam);
        Movement_Stat stat = (Movement_Stat)anim.GetInteger("WalkStat");
        //Debug.Log(nam+" "+stat);
        if(nam.ToUpper().Contains(stat.ToString().ToUpper()))
        {//如果要播的动画属于当前状态机
            
            anim.ResetTrigger("IsAnyActive");
        }
        else { anim.SetTrigger("IsAnyActive"); }
        anim.Play(nam.ToLower(), layer,0);
        anim.Play(nam.ToLower(), 0, 0);
        
    }
    public void SetAnyActive() { //Debug.Log("set");
        anim.ResetTrigger("IsAnyActive"); }

    /*void OnChangeTool(int index, BaseTool tool)
    {
        /*anim.SetInteger("handState", (int)(tool.GetData().GetInt("idleAnim")));

        AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(0);//还需转换为upper在持有当前武器下此动画的状态
        string nam = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        //Debug.Log(lowerNameToUpperName(nam));
        anim.CrossFade(lowerNameToUpperName(nam), 0.25f, 1, Mathf.Repeat(asi.normalizedTime, 1));
    }*/
    void onDie(float f, EventCenter evc, BaseTool btd, Damage d)
    {
        anim.enabled = false;
    }
    void onRespawn()
    {
        anim.enabled = true;
    }
    void onMove(Movement_Stat stat, float speed, DIR dir)
    {
        
    }
    public void OnWalkStatChg(Movement_Stat stat)
    {
        //Debug.Log(stat);
        if (anim == null) anim = GetComponent<Animator>();
        anim.SetTrigger("IsAnyActive");//trigger被用过了之后就会重设为false
        anim.SetInteger("WalkStat", (int)stat);
    }
    #endregion

    string lowerNameToUpperName(string nam)
    {
        //return "";
        switch (anim.GetInteger("handState"))
        {
            case 0:
                return nam;
                break;
            default:
                return ((PlayerHandAnimaState)(anim.GetInteger("handState"))).ToString()+"_" + nam;
             break;
        }
    }
}
