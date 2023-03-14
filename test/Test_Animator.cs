using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Animator : MonoBehaviour
{
    PlayerAnimator anim;
    void Start()
    {
        anim = GetComponent<PlayerAnimator>();
        GetComponent<EventCenter>().ListenEvent<string>(nameof(PlayerEventName.onAnimationEvent), onAnimaEvent);
    }
    void onAnimaEvent(string s)
    {
        Debug.Log(s);
        //if (s == "end") { anim.SetAnyActive(); }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            GetComponent<Animator>().SetTrigger("IsAnyActive");//trigger=true
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            GetComponent<Animator>().ResetTrigger("IsAnyActive");//trigger=false
        }

       if(Input.GetKeyDown(KeyCode.Q))
        {
            anim.OnWalkStatChg(Movement_Stat.idle);
        }
       else if (Input.GetKeyDown(KeyCode.W))
        {
            anim.OnWalkStatChg(Movement_Stat.walk);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            anim.OnWalkStatChg(Movement_Stat.run);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            anim.OnWalkStatChg(Movement_Stat.crouch);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            anim.OnWalkStatChg(Movement_Stat.creep);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            anim.OnWalkStatChg(Movement_Stat.lying);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("play " );
            anim.PlayAnimation("idle_attack", 1);
            anim.PlayAnimation("idle_attack", 0);
            anim.SetAnyActive();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("play ");
            anim.PlayAnimation("walk_attack", 1);
            anim.PlayAnimation("walk_attack", 0);
            anim.SetAnyActive();
        }
    }
    
}
