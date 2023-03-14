using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container_DropItem : BaseContainer
{
    string dropPath = "Prefabs/dropItem/dropItem";
    List<GameObject> dropObj = new List<GameObject>();

    BaseObjectPool dropPool;

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.ListenEvent<Item,Vector3,Vector3>(nameof(EventNames.DropItem), Drop);
        e.ListenEvent<long, int>(nameof(EventNames.ChunkUpdate), OnUpdateChunk);
    }
    public override void OnLoadGame(MapPrefabsData data, int index)
    {
        base.OnLoadGame(data, index);
        dropObj.AddRange(Resources.LoadAll<GameObject>(dropPath));
        dropPool = new BaseObjectPool(dropObj[0], EventCenter.WorldCenter.gameObject);

        //加载{vec3,item}列表，监听update
    }
    public override void UnLoadGame(int ind)
    {
        if (ind != 0) return;
        base.UnLoadGame(ind);
        dropPool.Clear();
    }
    /// <summary>
    /// 当stat=1(加载完edge)时找到在此chunk里的item,实例化之,stat=2则缓存之
    /// </summary>
    /// <param name="xy"></param>
    /// <param name="stat"></param>
    void OnUpdateChunk(long xy,int stat)
    {
        if(stat==1)
        {
            //Debug.Log(xy.GetX() + ":" + xy.GetY() + "load edge done");
        }
        else if(stat==2)
        {
            //Debug.Log(xy.GetX() + ":" + xy.GetY() + "recycle");
        }
    }

    public void Drop(Item item,Vector3 pos,Vector3 speed)
    {
        if (Item.IsNullOrEmpty(item)) return;
        /*GameObject g= dropPool.Pop(null);
         g.GetComponent<IDropedItem>().OnDrop(item, pos, speed);*/
       GameObject g= center.GetParm<string, Vector3, GameObject>(nameof(EventNames.CreateEntityByName), "dropItem", pos);
        g.GetComponent<EventCenter>().SendEvent<Item>("SetItem", item);
    }
}
