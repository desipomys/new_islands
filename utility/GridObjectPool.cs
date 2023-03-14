using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridObjectPool :IObjectPool
{
    public float DestroyTime=60f;
    GameObject prefabs;
    GameObject father;
   Dictionary<long,GameObject> pool=new Dictionary<long, GameObject>();
    Dictionary<long,float> objunActiveTime=new Dictionary<long, float>();//保存该long最近一次被回收的时间
    //可以再加个对象池管理超时destroy掉的物体

    public GridObjectPool(GameObject g,GameObject father){prefabs=g;this.father=father;}
    public void Init(GameObject g)
    {
        prefabs=g;
    }
    public void Clear()
    {
        foreach (var item in pool)
        {
            GameObject.Destroy(item.Value);
        }
        pool.Clear();
        objunActiveTime.Clear();
    }
    public void Recycle(GameObject g,object parm)
    {
        if(g==null){
            RecycleWithNoObj(Convert.ToInt64(parm));
            return;
        }
        long xy=Convert.ToInt64(parm);
        IPoolable[] p = g.GetComponents<IPoolable>();
        if(p==null)return;
        for (int i = 0; i < p.Length; i++)
        {
            p[i].OnPoolPush();
        }
        if(objunActiveTime.ContainsKey(xy))
        {//已被回收过的再次被回收
            objunActiveTime[xy]=Time.time;
            //Debug.Log("回收1 "+xy.GetX()+":"+xy.GetY());
        }
        else
        {
            objunActiveTime.Add(xy,Time.time);
            //Debug.Log("回收2 "+xy.GetX()+":"+xy.GetY());
        }
    }
    void RecycleWithNoObj(long xy)
    {
        if(!pool.ContainsKey(xy))return;
        GameObject g=pool[xy];
        if(g==null)return;
        
        IPoolable[] p = g.GetComponents<IPoolable>();
        if(p==null)return;
        for (int i = 0; i < p.Length; i++)
        {
            p[i].OnPoolPush();
        }
        if(objunActiveTime.ContainsKey(xy))
        {
            objunActiveTime[xy]=Time.time;
            Debug.Log("回收1 "+xy.GetX()+":"+xy.GetY());
        }
        else
        {
            objunActiveTime.Add(xy,Time.time);
            Debug.Log("回收2 "+xy.GetX()+":"+xy.GetY());
        }
    }
    public GameObject Get(long coord)
    {
        if (pool.ContainsKey(coord))
            return pool[coord];
        else return null;
    }
    public GameObject Pop(object parm)
    {
        //if(parm.GetType()!=typeof(long))return null;
        long xy=Convert.ToInt64(parm);
        objunActiveTime.Remove(xy);
       return InnerPop(xy);
    }
    /// <summary>
    /// 此方法旨在减少装箱拆箱
    /// </summary>
    /// <param name="xy"></param>
    /// <returns></returns>
    GameObject InnerPop(long xy)
    {
        if(pool.ContainsKey(xy))
            {
                //Debug.Log("取出"+xy.GetX()+":"+xy.GetY());
                return pool[xy];
                }
            else{
                //Debug.Log("生成"+xy.GetX()+":"+xy.GetY());
                GameObject ng=GameMainManager.CreateGameObject(prefabs,father.transform.position,father.transform.rotation,father.transform);
                 IPoolable[] p = ng.GetComponents<IPoolable>();
                 if(p!=null)
                    for (int j = 0; j < p.Length; j++)
                    {
                        p[j].Pool = this;
                        p[j].OnPoolInit();
                        p[j].OnPoolPush();
                    }
                pool.Add(xy,ng);
                return InnerPop(xy);
            }
    }

    public void OnUpdate()
    {
        List<long> unActiveTooLongObj=new List<long>();
        foreach(var item in objunActiveTime)
        {
            if(Time.time-item.Value>DestroyTime)//已经unactive了超过60S
            {
                unActiveTooLongObj.Add(item.Key);
            }
        }
        foreach (var item in unActiveTooLongObj)
        {
            if(pool[item]!=null)
                GameObject.Destroy(pool[item]);
           pool.Remove(item);
           objunActiveTime.Remove(item);
           Debug.Log("销毁"+item.GetX()+":"+item.GetY());
        }

    }
}
