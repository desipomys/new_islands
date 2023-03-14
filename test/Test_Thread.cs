using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Test_Thread : MonoBehaviour
{
    Thread t;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<EventCenter>().Init();
        EventCenter.WorldCenter = GetComponent<EventCenter>();

        EventCenter.WorldCenter.ListenEvent("debug", debug);
        Debug.Log(EventCenter.WorldCenter.gameObject.name);

       t =new Thread(new ThreadStart(run));
        t.Start();
    }
    void debug()
    {
        Debug.Log("debug");
    }
    void OnDestroy()
    {
        if(t!=null)
        {
            t.Abort();
        }
    }
    void run()
    {
        //发worldevc事件，直接调container方法
        for(int i=0;i<10;i++)
        {
            EventCenter.WorldCenter.SendEvent("debug");//可发送
            Thread.Sleep(1000);
        }
    }
}
