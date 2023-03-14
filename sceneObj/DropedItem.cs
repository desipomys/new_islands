using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public interface IDropedItem
{
    void OnDrop(Item i, Vector3 pos, Vector3 speed);
    bool OnMerge(Item i);
}

public class DropedItem : MonoBehaviour,IInterectable,IDropedItem,IPoolable,IEventRegister
{
    public Item item;

    public IObjectPool Pool { get =>  pool ; set => pool=value; }
    IObjectPool pool;
    EventCenter center;

    public void OnDrop(Item i, Vector3 pos, Vector3 speed)
    {
        item = i;
        transform.position = pos;
        GetComponent<Rigidbody>().velocity = speed;
    }
    public bool OnMerge(Item i)
    {
        return false;
    }
    public int OnInterect(EventCenter source, InteractType type)
    {
        Debug.Log(source.gameObject.name+"拾取了"+item.ToString());
        int before = item.num;
        if (before == 0) {
            EventCenter.WorldCenter.SendEvent<long>(nameof(EventNames.RemoveEntity), center.UUID);
            return 0; }
       int remain= source.GetParm<Item, int>(nameof(PlayerEventName.giveItem), new Item(item));

        //如果拾取物件在高度上与拾取者主手距离<0.5f则播放平拾取动作，>0.5f,低于则播放蹲下拾取，高于则播放抬头拾取
        if (remain == before) { return 0; }

        item.SafeSet(remain);
        if(remain==0)
        {
            StartCoroutine(CycleWait());//等待0.125S后回收
        }

        Vector3 handpos = source.GetParm<Transform>(nameof(PlayerEventName.getMainHand)).position;//获取拾取者的手判断拾取距离
        if(Mathf.Abs(handpos.y-transform.position.y)>0.5f)
        {
            if (handpos.y - transform.position.y > 0)
            {
                return 3;//下拾取
            }
            else return 2;//上拾取
        }
        return 1;
        
    }
    WaitForSeconds wait = new WaitForSeconds(0.125f);
    IEnumerator CycleWait()
    {
        yield return wait;
        EventCenter.WorldCenter.SendEvent<long>(nameof(EventNames.RemoveEntity), center.UUID);
        //pool.Recycle(gameObject,null);//不用池，先注释掉
    }
    /// <summary>
    /// 为了优化应该在两个dropeditem碰到的时候合并彼此
    /// </summary>
    /// <param name="collision"></param>
    public void OnCollisionEnter(Collision collision)
    {
        /*IDropedItem temp = collision.gameObject.GetComponent<IDropedItem>();
        if (temp!=null)
        {
            temp.OnMerge(item);
        }*/
    }
    public void OnMouseEnter()
    {
        EventCenter.WorldCenter.SendEvent<Vector3, string>("ShowDropItemDescript", transform.position, item.GetDescript());
    }

    public void OnPoolInit()
    {
        //throw new System.NotImplementedException();
    }

    public void OnPoolPush()
    {
        //throw new System.NotImplementedException();
        gameObject.SetActive(false);
    }

    public void OnPoolPop(float time)
    {
        //throw new System.NotImplementedException();
        gameObject.SetActive(true);
    }

    public void OnPoolRecycle()
    {
        //throw new System.NotImplementedException();
    }

    public void OnEventRegist(EventCenter e)
    {
        center = e;
        e.ListenEvent<Item>("SetItem", SetItem);
        e.RegistFunc<string>(nameof(PlayerEventName.save), Save);
        e.ListenEvent<string>(nameof(PlayerEventName.load), Load);
    }

    public void AfterEventRegist()
    {
        
    }
    void SetItem(Item i)
    {
        item = i;
    }
    string Save()
    {
        return item.ToString();
    }
    void Load( string dat)
    {
        item = JsonConvert.DeserializeObject<Item>(dat);
    }
}
