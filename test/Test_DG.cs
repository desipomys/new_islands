using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Test_DG : MonoBehaviour,IEventRegister
{
    public float distan;
    public void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }

    public void OnEventRegist(EventCenter e)
    {
        e.ListenEvent<KeyCodeKind, Dictionary<KeyCode, KeyCodeStat>>("onKey", Move);
        //throw new System.NotImplementedException();
    }

    public virtual void Move(KeyCodeKind key, Dictionary<KeyCode, KeyCodeStat> state)
    {
        if(key.Contain(KeyCodeKind.Space)&&state[KeyCode.Space]==KeyCodeStat.down)
        {
            DotWeenHelper.DoMoveXTest(transform, 5, 1);
        }
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //DotWeenHelper.DoMoveXTest(transform, distan, 1);
           Tween t= GetComponent<Camera>().DOShakePosition(1,new Vector3(0,2,0));
            t.SetLoops<Tween>(5);
           
            t.Play<Tween>();
        }
    }

}
