using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对应一个entity_*_*.txt文件
/// </summary>
public class Entity
{
    public long UUID;
    /// <summary>
    /// 实体预制名
    /// </summary>
    public string name;
    public float x, y, z;
    public float Rx, Ry, Rz;
    public Entity() { }
    public Entity(string nam, Vector3 pos)
    {
        name = nam;
        SetPos(pos);
    }
    public void SetPos(Vector3 pos)
    {
        x = pos.x; y = pos.y; z = pos.z;
    }
    public void SetRota(Vector3 rota)
    {
        Rx = rota.x;Ry = rota.y;Rz = rota.z;
    }
}
public class EntityFile
{
    public Dictionary<long, HashSet<long>> ChunkPos_UUIDS = new Dictionary<long, HashSet<long>>();//chunk坐标-UUID，chunk坐标是全局chunk坐标
    public Dictionary<long, string> datas = new Dictionary<long, string>();
    public Dictionary<long, Entity> entitys = new Dictionary<long, Entity>();
    /// <summary>
    /// 添加单个chunk
    /// </summary>
    /// <param name="chunkpos"></param>
    /// <param name="uuids"></param>
    /// <param name="datas"></param>
    public void AddChunk(long chunkpos, HashSet<long> uuids, Dictionary<long, Entity> e)
    {
        ChunkPos_UUIDS.Add(chunkpos, uuids);
        foreach (var item in e)
        {
            entitys.Add(item.Key, item.Value);
        }
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

//container负责管理生成后的物体，包括接收数据对现有物体进行更改
/*
 * 生成功能放在loader_prefabs，不用在这里实现生成功能
 * 
 */
public class Container_Entity : BaseContainer
{
    int entityLoadDistance = 3;//实体加载距离,单位chunk

    Dictionary<long, string> UUID_Data = new Dictionary<long, string>();//全世界的实体数据缓存
    Dictionary<long, EventCenter> UUID_EntityEVC { get { return eventCenterContainer.UUID_EVC; } set { } }
    Dictionary<long, Entity> UUID_Entity = new Dictionary<long, Entity>();

    Dictionary<long, HashSet<long>> ChunkPos_UUIDs = new Dictionary<long, HashSet<long>>();//小chunk坐标对其上的实体uuid，需对每个生物实体进行跟踪
    //性能受限，非区块加载者的生物每1S更新一次


    //long currentAvalible = 0;//要保存到map文件夹里
    Container_EventCenter eventCenterContainer;
    public override void AfterEventRegist()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnEventRegist(EventCenter e)
    {
        //throw new System.NotImplementedException();
        base.OnEventRegist(e);
        center.ListenEvent<long, int>(nameof(EventNames.BigChunkLoaded), OnBigChunkLoaded);//同步加载模式下先于chunkupdate
        center.ListenEvent<long, int>(nameof(EventNames.ChunkUpdate), OnChunkUpdate);//监听container_terrian的加卸载事件
        eventCenterContainer = ContainerManager.GetContainer<Container_EventCenter>();
    }

    public override void OnLoadSave(SaveData data)
    {
        
    }
    public override void LoadData(string data)
    {

    }
    public override void OnLoadGame(MapPrefabsData data, int index)
    {
        if (index != 1) return;
        base.OnLoadGame(data, index);

        center.RegistFunc<string, Vector3, GameObject>(nameof(EventNames.CreateEntityByName), CreateEntityByName);
        center.RegistFunc<long, GameObject>("GetEntityByIndex", GetEntityByIndex);
        center.ListenEvent<long, Vector3, Vector3>(nameof(EventNames.SetEntityToCaChe), SetToCaChe);
        center.ListenEvent<long>(nameof(EventNames.RemoveEntity),RemoveEntity);
        //读取所有建筑数据根据预设生成相应NPC
        //如果已经有entity数据就直接加载
    }
    public override void UnLoadGame(int ind)
    {
        if (ind != 0) return;
        base.UnLoadGame(ind);

        center.UnRegistFunc<string, Vector3, GameObject>(nameof(EventNames.CreateEntityByName), CreateEntityByName);
        center.UnRegistFunc<long, GameObject>("GetEntityByIndex", GetEntityByIndex);
        center.UnListenEvent<long, Vector3, Vector3>(nameof(EventNames.SetEntityToCaChe), SetToCaChe);
        center.UnListenEvent<long>(nameof(EventNames.RemoveEntity), RemoveEntity);

        //base.UnLoadGame();
        UUID_Data.Clear();
        UUID_Entity.Clear();
        //UUID_EntityEVC.Clear();
        ChunkPos_UUIDs.Clear();
    }
    public override void Save(string path)
    {
        base.Save(path);
        long[] bigchunks = center.GetParm<long[]>(nameof(EventNames.GetAllActiveChunkFile));
        for (int i = 0; i < bigchunks.Length; i++)
        {
            OnBigChunkLoaded(bigchunks[i], 2);
        }
    }

    [Obsolete]
    void cacheFarEntity()
    {
        /*List<KeyValuePair<long,EventCenter>> cache=new List<KeyValuePair<long,EventCenter>>();
        List<KeyValuePair<long,EventCenter>> active=new List<KeyValuePair<long,EventCenter>>();

        Vector3[] loadCenters=center.GetParm<Vector3[]>("GetAllPlayerPosi");
        foreach (var item in entitys)
        {
            for (int i = 0; i < loadCenters.Length; i++)
            {
                if((item.Value.transform.position-loadCenters[i]).magnitude>
                entityLoadDistance*Chunk.BlockSize*Chunk.ChunkSize)//激活实体超出距离
                {
                    cache.Add(item);
                    continue;
                }
            }
        }

        foreach (var item in cachedEntitys)
        {
            for (int i = 0; i < loadCenters.Length; i++)
            {
                if((item.Value.transform.position-loadCenters[i]).magnitude<
                entityLoadDistance*Chunk.BlockSize*Chunk.ChunkSize)//缓存实体回到加载范围
                {
                    active.Add(item);
                    continue;
                }
            }
        }*/
    }

    Dictionary<long, string> SaveChunkEntityData(long xy)
    {
        Dictionary<long, string> data = new Dictionary<long, string>();
        HashSet<long> chunkuuids = ChunkPos_UUIDs[xy];
        foreach (var item in chunkuuids)
        {
            data.Add(item, UUID_EntityEVC[item].GetParm<string>(nameof(PlayerEventName.save)));
        }
        return data;
    }

    public void OnChunkUpdate(long xy, int stat)//加载时晚于bigchunkupdate，卸载时早于bigchunkload
    {//1加载2卸载3保存
        if (stat != 1 && stat != 2 && stat != 3) return;
        if (!ChunkPos_UUIDs.ContainsKey(xy)) return;


        if (stat == 1)//从uuid_data取数据生成entityobj
        {
            foreach (var item in ChunkPos_UUIDs[xy])
            {
                if (UUID_EntityEVC.ContainsKey(item))
                {
                    UUID_EntityEVC[item].SendEvent<bool>(nameof(PlayerEventName.SetActive), true);
                }
                else
                {
                    GameObject g = center.GetParm<string, GameObject>("GetEntityByName", UUID_Entity[item].name);
                    g.GetComponent<EventCenter>().UUID = UUID_Entity[item].UUID;
                    g.transform.position = new Vector3(UUID_Entity[item].x, UUID_Entity[item].y, UUID_Entity[item].z);
                    g.transform.rotation = Quaternion.Euler(UUID_Entity[item].Rx, UUID_Entity[item].Ry, UUID_Entity[item].Rz);
                    g.GetComponent<EventCenter>().SendEvent<string>(nameof(PlayerEventName.load), UUID_Data[item]);
                    UUID_EntityEVC.Add(UUID_Entity[item].UUID, g.GetComponent<EventCenter>());
                }
            }

        }
        else if (stat == 2)//卸载entityObj，保存此chunk实体数据到UUIDData
        {
            foreach (var item in ChunkPos_UUIDs[xy])
            {
                if (UUID_Data.ContainsKey(item))
                {
                    UUID_Data[item] = UUID_EntityEVC[item].GetParm<string>(nameof(PlayerEventName.save));
                    UUID_Entity[item].SetPos(UUID_EntityEVC[item].transform.position);
                }
                else
                {
                    UUID_Data.Add(item, UUID_EntityEVC[item].GetParm<string>(nameof(PlayerEventName.save)));

                    UUID_Entity[item].SetPos(UUID_EntityEVC[item].transform.position);
                }
                UUID_EntityEVC[item].SendEvent<bool>("SetActive", false);
            }

        }
        else if (stat == 3)//只保存不卸载
        {
            foreach (var item in ChunkPos_UUIDs[xy])
            {
                if (UUID_Data.ContainsKey(item))
                {
                    UUID_Data[item] = UUID_EntityEVC[item].GetParm<string>(nameof(PlayerEventName.save));
                    UUID_Entity[item].SetPos(UUID_EntityEVC[item].transform.position);
                }
                else
                {
                    UUID_Data.Add(item, UUID_EntityEVC[item].GetParm<string>(nameof(PlayerEventName.save)));

                    UUID_Entity[item].SetPos(UUID_EntityEVC[item].transform.position);
                }
                //UUIDentitys[item].SendEvent<bool>("SetActive", false);
            }
        }
    }
    public void OnBigChunkLoaded(long xy, int stat)//当某个chunks文件被加载时,0加载，1卸载,2只保存不卸载；加载时早于chunkupdate,卸载时晚于chunkupdate
    {//xy是bigchunk坐标
        switch (stat)
        {
            case 0:
                string dat = FileSaver.GetEntityFile(center.GetParm<string>(nameof(EventNames.ThisSavePath)), xy.GetX(), xy.GetY());
                if (string.IsNullOrEmpty(dat)) return;
                EntityFile f = JsonConvert.DeserializeObject<EntityFile>(dat);
                dat = null;
                //读取文件，将数据add到当前字典中
                if (f == null) { Debug.Log("空entity"); }
                else
                {
                    foreach (var item in f.ChunkPos_UUIDS)
                    {
                        ChunkPos_UUIDs.Add(item.Key, item.Value);
                    }
                    foreach (var item in f.datas)
                    {
                        if (!UUID_Data.ContainsKey(item.Key))
                        { UUID_Data.Add(item.Key, item.Value); }
                        else UUID_Data[item.Key] = item.Value;

                        if (!UUID_Entity.ContainsKey(item.Key))
                        {
                            UUID_Entity.Add(item.Key, f.entitys[item.Key]);
                        }
                        else UUID_Entity[item.Key] = f.entitys[item.Key];
                    }//EVC尚未生成，在chunkupdate处生成
                }
                break;
            case 1:

                List<long> needUnLoadChunk = new List<long>();
                EntityFile f1 = new EntityFile();
                long chunkPos = XYHelper.ToLongXY(xy.GetX() * Container_Terrain.chunkSizePerFile, xy.GetY() * Container_Terrain.chunkSizePerFile);
                //将该bigchunk包含的所有小chunk坐标上的实体转化为str数据，保存
                for (int i = 0; i < Container_Terrain.chunkSizePerFile; i++)
                {
                    for (int j = 0; j < Container_Terrain.chunkSizePerFile; j++)
                    {
                        long thischunkpos = XYHelper.ToLongXY(chunkPos.GetX() + i, chunkPos.GetY() + j);
                        Dictionary<long, Entity> e = new Dictionary<long, Entity>();

                        foreach (var item in ChunkPos_UUIDs[thischunkpos])
                        {
                            if (!UUID_EntityEVC.ContainsKey(item))
                            { f1.datas.Add(item, UUID_Data[item]); }//如果未实例化则直接取数据缓存
                            else {
                                f1.datas.Add(item, UUID_EntityEVC[item].GetParm<string>(nameof(PlayerEventName.save)));
                                UUID_Entity[item].SetPos(UUID_EntityEVC[item].transform.position);
                                UUID_Entity[item].SetRota(UUID_EntityEVC[item].transform.rotation.eulerAngles);
                            }//已实例化的则取实例数据

                            e.Add(item, UUID_Entity[item]);
                        }
                        f1.AddChunk(thischunkpos, ChunkPos_UUIDs[thischunkpos], e);
                        needUnLoadChunk.Add(thischunkpos);
                    }
                }

                FileSaver.SetEntityFile(center.GetParm<string>(nameof(EventNames.ThisSavePath)), xy.GetX(), xy.GetY(), f1.ToString());
                f1 = null;

                foreach (var item in needUnLoadChunk)
                {
                    RemoveChunk(item);
                }

                break;
            case 2://只保存不卸载

                EntityFile f2 = new EntityFile();
                long chunkPos2 = XYHelper.ToLongXY(xy.GetX() * Container_Terrain.chunkSizePerFile, xy.GetY() * Container_Terrain.chunkSizePerFile);
                //将该bigchunk包含的所有小chunk坐标上的实体转化为str数据，保存
                for (int i = 0; i < Container_Terrain.chunkSizePerFile; i++)
                {
                    for (int j = 0; j < Container_Terrain.chunkSizePerFile; j++)
                    {
                        long thischunkpos = XYHelper.ToLongXY(chunkPos2.GetX() + i, chunkPos2.GetY() + j);
                        Dictionary<long, Entity> e = new Dictionary<long, Entity>();

                        if (ChunkPos_UUIDs.ContainsKey(thischunkpos))
                        {
                            foreach (var item in ChunkPos_UUIDs[thischunkpos])
                            {
                                if (!UUID_EntityEVC.ContainsKey(item))
                                { f2.datas.Add(item, UUID_Data[item]); }//如果未实例化则直接取数据缓存
                                else
                                {
                                    f2.datas.Add(item, UUID_EntityEVC[item].GetParm<string>(nameof(PlayerEventName.save)));
                                    UUID_Entity[item].SetPos(UUID_EntityEVC[item].transform.position);
                                    UUID_Entity[item].SetRota(UUID_EntityEVC[item].transform.rotation.eulerAngles);
                                }//已实例化的则取实例数据


                                e.Add(item, UUID_Entity[item]);
                            }
                            f2.AddChunk(thischunkpos, ChunkPos_UUIDs[thischunkpos], e);
                        }
                    }
                }

                FileSaver.SetEntityFile(center.GetParm<string>(nameof(EventNames.ThisSavePath)), xy.GetX(), xy.GetY(), f2.ToString());
                break;
        }
    }

    /// <summary>
    /// 只添加数据不新增obj/evc，只能加在已加载的chunk中
    /// </summary>
    /// <param name="UUID"></param>
    /// <param name="e"></param>
    /// <param name="data"></param>
    void AddEntityData(Entity e, string data)
    {
        long chunkpos = Chunk.WorldPosToChunkPos(new Vector2(e.x, e.z));
        if (!ChunkPos_UUIDs.ContainsKey(chunkpos))
        {
            Debug.Log("此chunk未加载" + chunkpos.GetX() + ":" + chunkpos.GetY());
            ChunkPos_UUIDs.Add(chunkpos, new HashSet<long>());
        }
        UUID_Data.Add(e.UUID, data);
        UUID_Entity.Add(e.UUID, e);

        ChunkPos_UUIDs[chunkpos].Add(e.UUID);

    }
    /// <summary>
    /// 移除entity包括数据、obj/EVC
    /// </summary>
    /// <param name="uuid"></param>
    void RemoveEntity(long uuid)
    {
        bool haveEntity = false;
        foreach (var item in ChunkPos_UUIDs)
        {
            if (item.Value.Contains(uuid)) { item.Value.Remove(uuid); haveEntity = true; }
        }
        if (!haveEntity) return;

        UUID_EntityEVC[uuid].SendEvent(nameof(PlayerEventName.remove));
        Debug.Log("删除" + uuid);
        GameObject.Destroy(UUID_EntityEVC[uuid].gameObject);
        UUID_EntityEVC.Remove(uuid);
        UUID_Data.Remove(uuid);
        UUID_Entity.Remove(uuid);

    }
    /// <summary>
    /// 移除一整个chunk上的实体，删除数据+evc+obj
    /// </summary>
    /// <param name="chunkpos"></param>
    void RemoveChunk(long chunkpos)
    {
        //移除前可能需发个事件通知目标物体断开事件连接
        foreach (var item in ChunkPos_UUIDs[chunkpos])
        {
            GameObject.Destroy(UUID_EntityEVC[item].gameObject);
            UUID_EntityEVC.Remove(item);
            UUID_Data.Remove(item);
            UUID_Entity.Remove(item);
        }
        ChunkPos_UUIDs.Remove(chunkpos);
    }

    #region 外部注册方法
   
    //需重写
    GameObject CreateEntityByName(string name, Vector3 pos)
    {
        GameObject g = center.GetParm<string, GameObject>(nameof(EventNames.GetEntityByName), name);
        long nowuuid = center.GetParm<long>(nameof(EventNames.GetMaxUUID));
        g.transform.position = pos;
        

        Entity e = new Entity(name, pos);
        e.UUID = nowuuid;
        string dat = g.GetComponent<EventCenter>().GetParm<string>(nameof(PlayerEventName.save));
        AddEntityData(e, dat);

        UUID_EntityEVC.Add(e.UUID, g.GetComponent<EventCenter>());
        g.GetComponent<EventCenter>().UUID= e.UUID;
        return g;
    }
    GameObject GetEntityByIndex(long index)
    {
        if (index == -1) return EventCenter.WorldCenter.gameObject;
        EventCenter g;
        if (UUID_EntityEVC.TryGetValue(index, out g))
            return g.gameObject;
        else//实体表中找不到就去玩家表中找
        {
            return ContainerManager.GetContainer<Container_Player>().GetPlayerByIndex(index).gameObject;
        }
    }
    void SetToCaChe(long index, Vector3 oldpos, Vector3 newpos)//将index对应的实体从活动实体中移除
    {

    }

    #endregion



    #region 外部响应方法

    /*void OnChunkActive(long xy)
    {
        //active chunk上实体
    }

    void OnChunkSleep(long xy)
    {
        //unactive chunk上实体
    }*/
    #endregion

}
