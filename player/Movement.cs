using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;

public class Movement : MonoBehaviour, IEventRegister, IDataContainer
{

    protected EventCenter center;
    //Func<CharactorInGame_Data> getPlayerData;//优化，把chardata的获取方法弄到这里缓存
    public Movement_Stat stat = Movement_Stat.idle;
    protected bool CanMove { get { return !movePauseHandler.IsPause; } set { } }
    protected bool CanRota { get { return !rotaPauseHandler.IsPause; } set { } }

    public int GetDataCollectPrio => 0;

   public PauseHandler movePauseHandler=new PauseHandler(), rotaPauseHandler = new PauseHandler();

    protected Rigidbody rgb;
    protected CharacterController character;

    public virtual void OnEventRegist(EventCenter e)
    {
        center = e;
        rgb = GetComponent<Rigidbody>();
        character = GetComponent<CharacterController>();
        e.ListenEvent<KeyCodeKind, Dictionary<KeyCode, KeyCodeStat>>(nameof(PlayerEventName.onKey), Move);
        e.ListenEvent(nameof(PlayerEventName.onDie), onDie);
        e.ListenEvent(nameof(PlayerEventName.onRespawn), respawn);
        e.ListenEvent<bool>(nameof(PlayerEventName.pause), OnPause);
        e.ListenEvent<float>(nameof(PlayerEventName.setMovePauseInTime), SetMovPauseInTime);
        e.ListenEvent<float>(nameof(PlayerEventName.setRotaPauseInTime), SetRotaPauseInTime);
        e.RegistFunc<Movement_Stat>(nameof(PlayerEventName.getMoveStat), () => { return stat; });
    }
    public virtual void AfterEventRegist(EventCenter e)
    {
        //getPlayerData= e.FetchGetParm<Charactor_Data>("char_Data");
    }

    protected int pressedMove = 0;
    public virtual void Move(KeyCodeKind key, Dictionary<KeyCode, KeyCodeStat> state)
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

        if ((key & KeyCodeKind.LeftShift) != KeyCodeKind.None)
        {

            if (stat.Contain(Movement_Stat.creep) || stat.Contain(Movement_Stat.crouch))//趴下不能跑
            {

            }
            else
            {
                stat = stat.Add(Movement_Stat.run);
                //stat = stat & (~Movement_Stat.crouch);//取消蹲下，但不取消趴
                //stat.Add(Movement_Stat.idle);
                runspeed = center.GetParm<float>("entity_runspd");
            }
            //return;
        }
        else
        {
            stat = stat & ~Movement_Stat.run;
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

    protected void onDie()
    {
        movePauseHandler.DoPause(true);
    }
    protected virtual void OnPause(bool p)
    {
        movePauseHandler.DoPause(p);
        rotaPauseHandler.DoPause(p);
        ResetStat();
    }
    protected void SetMovPauseInTime(float time)
    {
        movePauseHandler.DoPauseFor(time);
    }
    protected void SetRotaPauseInTime(float time)
    {
        //Debug.Log("设置停止转动" + time + "秒");
        if (rgb != null)
        { rgb.angularVelocity = Vector3.zero; }
        rotaPauseHandler.DoPauseFor(time);
    }
    protected void respawn()
    {
        movePauseHandler.DoPause(false);
        ResetStat();
    }
    protected void ResetStat()
    {//重发一次移动事件重置动画等组件
        center.SendEvent<Movement_Stat, float, DIR>(nameof(PlayerEventName.move), Movement_Stat.idle, 0, DIR.none);
    }

    public void AfterEventRegist()
    {
        //throw new NotImplementedException();
    }

    public void FromObject(object data)
    {
        //Debug.Log(data.GetType());
        string[] temp = ((JArray)data).ToObject<string[]>();
        transform.position = Vector3.zero.FromString(temp[0]);
        adjustPosi();
        transform.localScale = Vector3.zero.FromString(temp[2]);
        rotaPauseHandler = PauseHandler.FromString(temp[3]);
        movePauseHandler = PauseHandler.FromString(temp[4]);

        stat = (Movement_Stat)Enum.Parse(typeof(Movement_Stat), temp[5]);


        center.SendEvent<Movement_Stat, float, DIR>(nameof(PlayerEventName.move), stat, 0, DIR.none);
    }
    void adjustPosi()//调整位置的Y以免陷进地里
    {
        Vector3 temp = transform.position;
        temp.y = Chunk.WorldPosToBlockH(temp) + 0.5f;
        if (transform.position.y < temp.y)
            transform.position = temp;
    }
    public object ToObject()
    {
        string[] temp = new string[] { transform.position.ToString("f2"), transform.rotation.ToString(),
         transform.localScale.ToString(),rotaPauseHandler.ToString(),movePauseHandler.ToString(),stat.ToString()};
        return temp;
    }
}
