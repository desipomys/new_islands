using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_EvcOverride : MonoBehaviour
{
    EventCenter center;
    OptEventCenter optcenter;
    // Start is called before the first frame update
    void init()
    {
        center = GetComponent<EventCenter>();
        center.Init();
    }
    void optinit()
    {
        optcenter = GetComponent<OptEventCenter>();
        optcenter.Init();
    }
    void Start()
    {
        init();
        //optinit();
        test();
        //opttest();
    }
    void test()
    {
       
    }
    void pressTest()
    {
        //center.RegistFunc<int>("test", targetmethod);
        for (int i = 0; i < 10000; i++)
        {
            int p = center.GetParm<int>("test");

            //center.OverrideFunc<int>("test", overridemethod, 10, "");

            p += center.GetParm<int>("test");

            //center.UnOverrideFunc("test");

            //p-= center.GetParm<int>("test");
            //测试按tag发布

            center.SendEvent("eventa");
            center.SendEvent("event", 1);

            center.SendEvent("eventa", "a");
            center.SendEvent("eventa", "b");
        }
    }
    private void Update()
    {

        //optPressTest();
        pressTest();

    }

    void opttest()
    {
        optcenter.RegistFunc<int>(TestEventName.test, targetmethod);

        Debug.Log(optcenter.GetParm<int>(TestEventName.test));

        optcenter.OverrideFunc<int>(TestEventName.test, overridemethod, 10, "");

        Debug.Log(optcenter.GetParm<int>(TestEventName.test));
        Debug.Log(optcenter.GetHiddenParm<int>(TestEventName.test, 0));

        //测试按tag发布
        optcenter.ListenEvent(TestEventName.eventa, oneventa, 0, "a");
        optcenter.ListenEvent(TestEventName.eventa, oneventatag, 0, "b");
        optcenter.ListenEvent<int>(TestEventName.eventb, one);

        optcenter.SendEvent(TestEventName.eventa);

        optcenter.SendEvent(TestEventName.eventa, "a");
        optcenter.SendEvent(TestEventName.eventa, "b");
    }
    void optPressTest()
    {
        for (int i = 0; i < 10000; i++)
        {
            int p = optcenter.GetParm<int>(TestEventName.test);

            //center.OverrideFunc<int>("test", overridemethod, 10, "");

            p += optcenter.GetParm<int>(TestEventName.test);

            //center.UnOverrideFunc("test");

            //p-= center.GetParm<int>("test");
            //测试按tag发布

            optcenter.SendEvent(TestEventName.eventa);
            optcenter.SendEvent(TestEventName.eventb, 1);

            optcenter.SendEvent(TestEventName.eventa, "a");
            optcenter.SendEvent(TestEventName.eventa, "b");
        }
    }
    int pp = 0;
    void one(int p)
    {
        pp += p;
    }
    void oneventa()
    {
        pp += 1;
    }
    void oneventatag()
    {
        pp -= 1;
    }

    int overridemethod(string nam)
    {
        return center.GetHiddenParm<int>("test", 0) + 1;
    }
    int overridemethod(TestEventName nam)
    {
        return optcenter.GetHiddenParm<int>(nam, 0) + 1;
    }


    int targetmethod()
    {
        return 1;
    }

}
