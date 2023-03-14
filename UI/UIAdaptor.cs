using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAdaptor:MonoBehaviour,IEventRegister
{//挂在worldevc下
    UICenter center;
    public void OnEventRegist(EventCenter e)
    {
        
        e.ListenEvent<int>("OnLoaderInit",OnLoaderInit);
    }
    public void AfterEventRegist()
    {

    }

    void OnArriveScene(string name)
    {
        if(name==ConstantValue.ingameSceneName)
        {
            center=GameObject.Find("canvasgroup").GetComponent<UICenter>();
        }
        else
        {
            center=GameObject.Find("canvasgroup").GetComponent<UICenter>();
        }
        center.Init();
    }
    void OnLoaderInit(int index)
    {
        center.OnLoaderInit(index);

    }
}