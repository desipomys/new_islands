using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 还需要完善
/// </summary>
public class BaseObjectPool :IObjectPool
{
    public int incresePerNeed=5;//每次缺obj时生成多少个
    public float recycleTime=5f;
    public GameObject instance;
    public Queue<GameObject> unActiveObj = new Queue<GameObject>();
    public HashSet<GameObject> activeObj = new HashSet<GameObject>();

    GameObject father;

    public BaseObjectPool(GameObject g,GameObject father){ this.father = father; Init(g);}
    public virtual void Init(GameObject g)
    {
        tw = new Ticker(recycleTime);
        instance = g;
        IPoolable[] p = instance.GetComponents<IPoolable>();
        if(p!=null)
            for (int i = 0; i < p.Length; i++)
            {
                p[i].Pool = this;//instan不会被放入池中
                //p[i].OnPoolInit();
                //p[i].OnPoolPush();
            }
    }
    public virtual void Clear()
    {
        List<GameObject> temp = new List<GameObject>(activeObj);
        temp.AddRange(unActiveObj);
        for (int i = 0; i < temp.Count; i++)
        {
            GameObject.Destroy(temp[i]);
        }
    }
    //原本只支持定时回收对象，不支持对象自定义回收时间,现在是对象自行计算,到时间调用recycle回收
    public virtual void Recycle(GameObject obj,object parm)
    {
        IPoolable[] p = obj.GetComponents<IPoolable>();
        if(p!=null)
            for (int i = 0; i < p.Length; i++)
            {
                p[i].OnPoolPush();
            }
        unActiveObj.Enqueue(obj);
        activeObj.Remove(obj);
    }
    /// <summary>
    /// 从池中取出一个Obj
    /// </summary>
    /// <returns></returns>
    public virtual GameObject Pop(object parm)
    {
        if (unActiveObj.Count > 0)//够则通过接口发送poolpop
        {
            GameObject temp = unActiveObj.Dequeue();
            IPoolable[] p = temp.GetComponents<IPoolable>();
            if(p!=null)
                for (int i = 0; i < p.Length; i++)
                {
                    p[i].OnPoolPop(recycleTime);
                }
            activeObj.Add(temp);
            return temp;
        }
        else//不够则生成N个再POP一次
        {
            for (int i = 0; i < incresePerNeed; i++)
            {
                GameObject g = GameMainManager.CreateGameObject(instance,father.transform.position,father.transform.rotation,father.transform);
                
                IPoolable[] p = g.GetComponents<IPoolable>();
                if(p!=null)
                    for (int j = 0; j < p.Length; j++)
                    {
                        p[j].Pool = this;
                        p[j].OnPoolInit();
                        p[j].OnPoolPush();
                    }
                unActiveObj.Enqueue(g);
            }
            return Pop(parm);
        }
    }
    Ticker tw;
    public void FixedUpdate()
    {
        /*if(tw.IsReady())//每隔recycleTime回收一个activeobj,待修改
        {   
            if(activeObj.Count==0)return;
            GameObject g=activeObj.Dequeue();
            IPoolable[] p = g.GetComponents<IPoolable>();
            for (int i = 0; i < p.Length; i++)
            {
                p[i].OnPoolRecycle();
            }
            unActiveObj.Enqueue(g);
        }*/
    }
    #region 优化
    //统计一定时间内pool的使用情况，以动态缩放pool size、increaseperneed

    #endregion
}
public interface IObjectPool
{
    void Init(GameObject g);
    void Clear();
    void Recycle(GameObject g,object parm=null);
   GameObject Pop(object parm);
}
public interface IPoolable
{
    IObjectPool Pool { get; set; }
    /// <summary>
    /// 当对象池完成初始化
    /// </summary>
    void OnPoolInit();
    /// <summary>
    /// 当对象被放入池里
    /// </summary>
    void OnPoolPush();
   /// <summary>
   /// 当对象从池里被取出（先于poolpopinit事件）
   /// </summary>
   /// <param name="time">建议的回收时间</param>
    void OnPoolPop(float time);
    /// <summary>
    /// 当对象被池回收
    /// </summary>
    void OnPoolRecycle();

}
