//挂载玩家上，当玩家从一个区块进入另一区块时触发区块更新
//也可挂载于普通实体，跨区块时不更新，但时跨到sleep区和cache区时会将自己存储起来

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

[RequireComponent(typeof(CharacterEntity))]
public class EntityInChunkTracer : MonoBehaviour, IEventRegister
{
    Vector3 oldPosi;
   public bool isNeedLoad;//移动是否会造成区块加载
    EventCenter center;
    Ticker FlushTicker=new Ticker(0.125f);
    public void OnEventRegist(EventCenter e)
    {
        center=e;
        oldPosi = transform.position-Vector3.one*Chunk.ChunkSize*2;
        //throw new NotImplementedException();
    }

    public void AfterEventRegist()
    {
        //throw new NotImplementedException();
    }


    void Update()
    {

        if(Chunk.WorldPosToChunkPos(oldPosi)!=Chunk.WorldPosToChunkPos(transform.position)&&FlushTicker.IsReady())//限制每秒最多发送8次
        {        
            if(isNeedLoad)//会导致地形加载新区块的实体
            {
                Debug.Log("进入新区块"+Chunk.WorldPosToChunkPos(transform.position).GetX()+","+ Chunk.WorldPosToChunkPos(transform.position).GetY());
                EventCenter.WorldCenter.SendEvent(nameof(EventNames.UpdateActiveChunk));
            }    
            else//如果走到未加载地区则将自己设为unactive的实体
            {
                if(!EventCenter.WorldCenter.GetParm<Vector3,bool>(nameof(EventNames.IsHereLoaded),transform.position))
                {
                    long id=center.GetParm<long>("getUUID");
                    EventCenter.WorldCenter.SendEvent<long,Vector3,Vector3>(nameof(EventNames.SetEntityToCaChe),id,oldPosi,transform.position);
                }
            }
            oldPosi =transform.position;
        }
        
    }
}