using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Test_ObjectPool : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject g,father;
    GridObjectPool pool;
    void Start()
    {
       //testPool();
        //testXY();
    }
    void testXY()
    {
        int a=1;
        long xy=Convert.ToInt64(a);
        Debug.Log(xy+","+xy.GetX()+","+xy.GetY());
    }
    void testPool()
    {
         pool=new GridObjectPool(g,father);
        List<GameObject> lg=new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            lg.Add(pool.Pop(i));
        }
        for (int i = 0; i < 5; i++)
        {
            pool.Recycle(lg[i],i);
        }
        for (int i = 0; i < 5; i++)
        {
            lg[i]=pool.Pop(i);
        }
        for (int i = 0; i < 2; i++)
        {
            pool.Recycle(lg[i],i);
        }
        ticker2.IsReady();
    }

    // Update is called once per frame
    Ticker ticker=new Ticker(1);
    Ticker ticker2=new Ticker(6);
    void Update()
    {
        if(ticker.IsReady())
        {
            for (int i = 0; i < 5; i++)
            {
                int x=UnityEngine.Random.Range(0,5);
                int y=UnityEngine.Random.Range(0,5);
                pool.Pop(XYHelper.ToLongXY(x,y));
            }
            for (int i = 0; i < 5; i++)
            {
                int x=UnityEngine.Random.Range(0,5);
                int y=UnityEngine.Random.Range(0,5);
                pool.Recycle(null,XYHelper.ToLongXY(x,y));
            }
        }
        if(ticker2.IsReady())
        {
            pool.OnUpdate();
        }
        
    }

}
