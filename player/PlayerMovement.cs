using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 比基本移动组件多了朝向跟随鼠标的功能
/// </summary>
public class PlayerMovement : Movement
{
    public float angVelocity=360;//角度/S
    public override void OnEventRegist(EventCenter e)
    {
        rgb = GetComponent<Rigidbody>();
        base.OnEventRegist(e);
        character = GetComponent<CharacterController>();
        //e.ListenEvent<bool>(nameof(PlayerEventName.creepuse),onPause)
    }
    protected override void OnPause(bool p)
    {
        base.OnPause(p);
        //rig.angularVelocity = Vector3.zero;
    }
    public override void Move(KeyCodeKind key, Dictionary<KeyCode, KeyCodeStat> state)
    {
        if (!CanMove) return;
        
        float speed = center.GetParm<float>("entity_spd") * Time.deltaTime;
        float runspeed = 1;
        DIR dir = DIR.none;
        bool statChged = false;//状态是否改变
        //stat = Movement_Stat.none;

        if (key.Contain(KeyCodeKind.LeftControl))//改变姿势
        {
            if (state[KeyCode.LeftControl] == KeyCodeStat.down && !(key.Contain(KeyCodeKind.W) || key.Contain(KeyCodeKind.S) || key.Contain(KeyCodeKind.A) || key.Contain(KeyCodeKind.D)))
            {
                switch (stat & (Movement_Stat.idle | Movement_Stat.crouch | Movement_Stat.creep))
                {
                    case Movement_Stat.idle://站蹲趴彼此互斥

                        stat = stat & ~Movement_Stat.idle;
                        stat = stat | Movement_Stat.crouch;

                        break;
                    case Movement_Stat.crouch:
                        stat = stat & ~Movement_Stat.crouch;
                        stat = stat | Movement_Stat.creep;
                        break;
                    case Movement_Stat.creep:
                        stat = stat & ~Movement_Stat.creep;
                        stat = stat | Movement_Stat.idle;
                        break;
                    default:
                        stat = stat.Remove(Movement_Stat.crouch);
                        stat = stat.Remove(Movement_Stat.creep);
                        stat = stat.Add(Movement_Stat.idle);
                        break;
                }
                //停止move0.5s
                statChged = true;
                SetMovPauseInTime(0.5f);
            }

            //return;
        }
        else if (key.Contain(KeyCodeKind.Space) && state[KeyCode.Space] == KeyCodeStat.down)
        {
            switch (stat & (Movement_Stat.idle | Movement_Stat.crouch | Movement_Stat.creep))
            {
                case Movement_Stat.idle://站蹲趴彼此互斥

                    break;
                case Movement_Stat.crouch:
                    stat = stat.Remove(Movement_Stat.crouch);
                    stat = stat | Movement_Stat.idle; statChged = true; SetMovPauseInTime(0.5f);
                    break;
                case Movement_Stat.creep:
                    stat = stat & ~Movement_Stat.creep;
                    stat = stat | Movement_Stat.crouch; statChged = true; SetMovPauseInTime(0.5f);
                    break;
                default:
                    stat = stat.Remove(Movement_Stat.crouch);
                    stat = stat.Remove(Movement_Stat.creep);
                    stat = stat.Add(Movement_Stat.idle); statChged = true; SetMovPauseInTime(0.5f);
                    break;
            }
        }

        if ((key & KeyCodeKind.LeftShift) != KeyCodeKind.None&&state[KeyCode.LeftShift]==KeyCodeStat.down)//按下跑键
        {
            stat = stat.Add(Movement_Stat.run);
            if (stat.Contain(Movement_Stat.creep) || stat.Contain(Movement_Stat.crouch))//趴下不能跑
            {

            }
            else if(!center.GetParm<bool>(nameof(CharacterEventName.entity_tired)))
            {
                stat = stat.Add(Movement_Stat.run);
                
            }
            else { stat = stat & ~Movement_Stat.run; }
            //return;
        }
        else if ((key & KeyCodeKind.LeftShift) != KeyCodeKind.None && state[KeyCode.LeftShift] == KeyCodeStat.up)
        {
            stat = stat & ~Movement_Stat.run;
        }
        if (center.GetParm<bool>(nameof(CharacterEventName.entity_tired)))
        {
            stat = stat.Remove(Movement_Stat.run);
        }
        if (stat.Contain(Movement_Stat.run))
        { //stat = stat & (~Movement_Stat.crouch);//取消蹲下，但不取消趴
          //stat.Add(Movement_Stat.idle);
            runspeed = center.GetParm<float>("entity_runspd");
        }


        //以下是移动
        //if(state!=KeyCodeStat.keep)return;
        pressedMove = Mathf.Clamp(pressedMove - 1, 0, 2);
        stat = stat.Remove(Movement_Stat.walk);

        if (key.Contain(KeyCodeKind.W) || key.Contain(KeyCodeKind.S) || key.Contain(KeyCodeKind.A) || key.Contain(KeyCodeKind.D))
        {
            pressedMove = 2;
            if (stat.Contain(Movement_Stat.crouch))
            {
                stat = stat.Remove(Movement_Stat.crouch);//取消蹲下，但不取消趴
                stat = stat.Add(Movement_Stat.idle); statChged = true;
                SetMovPauseInTime(0.5f);
            }
        }

        if (stat.Contain(Movement_Stat.creep)) { runspeed *= 0.25f; }//调整蹲趴时的移速
        else if (stat.Contain(Movement_Stat.crouch)) { runspeed *= 0; }

        if (key.Contain(KeyCodeKind.W) && state[KeyCode.W] == KeyCodeStat.keep)
        {
            stat = stat | Movement_Stat.walk;
            dir = dir | DIR.front;

            if (false) { rgb.velocity = transform.forward * speed * runspeed; }
            else character.Move(transform.forward * speed * runspeed - Vector3.up * 9.8f * Time.deltaTime);
        }
        else if (key.Contain(KeyCodeKind.S) && state[KeyCode.S] == KeyCodeStat.keep)
        {
            stat = stat | Movement_Stat.walk;
            dir = dir & (~DIR.front);
            if (false) { rgb.velocity = -transform.forward * speed * runspeed; }
            else character.Move(-transform.forward * speed * runspeed - Vector3.up * 9.8f * Time.deltaTime);
        }

        if (key.Contain(KeyCodeKind.A) && state[KeyCode.A] == KeyCodeStat.keep)
        {
            stat = stat | Movement_Stat.walk;
            dir = dir | DIR.left;
            if (false) { rgb.velocity = -transform.right * speed * runspeed; }
            else character.Move(-transform.right * speed * runspeed - Vector3.up * 9.8f * Time.deltaTime);
        }
        else if (key.Contain(KeyCodeKind.D) && state[KeyCode.D] == KeyCodeStat.keep)
        {
            stat = stat | Movement_Stat.walk;
            dir = dir & (~DIR.left);
            if (false) { rgb.velocity = transform.right * speed * runspeed; }
            else character.Move(transform.right * speed * runspeed - Vector3.up * 9.8f * Time.deltaTime);
        }

        if (pressedMove == 2 || statChged)
        {
            dir = dir | DIR.none;
            center.SendEvent<Movement_Stat, float, DIR>(nameof(PlayerEventName.move), stat, speed * runspeed, dir);
        }
    }

    public void Update()
    {
        if (!CanMove) return;

        try
        {
            if (CanRota)
            {
                
                Vector3 lookAt = EventCenter.WorldCenter.GetParm<Vector3>(nameof(EventNames.GetViewPosi));
                //if (lookAt == Vector3.zero)
                    //Debug.Log(lookAt);
                //Debug.Log(default(Vector3));
                if ((lookAt - transform.position).magnitude < 1f) return;
                lookAt.y = transform.position.y;
                transform.LookAt(lookAt);
                
            }
        }
        catch (System.Exception)
        {
            Debug.Log("监视移动失败");
            return;
        }
       
    }
    
}
