using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 对应一个entityblock_*_*文件
/// </summary>
public class EntityBlockFile
{
    public List<Entity_Block> blocks;
    public Dictionary<long, string> datas;//uuid-data,可能要改成long,jobject对，以提供在没生成实体时修改数据的功能

    public void Add(Entity_Block eb,string data)
    {
        if (blocks == null) blocks = new List<Entity_Block>();
        if (datas == null) datas = new Dictionary<long, string>();
        blocks.Add(eb);
        datas.Add(eb.UUID,data);
    }
    public void remove(Vector3Int pos)
    {
        long uuid=0;
        for (int i = 0; i < blocks.Count; i++)
        {
            if(blocks[i].posH==pos.y&&blocks[i].posX==pos.x&&blocks[i].posY==pos.z)
            {
                uuid=blocks[i].UUID;
                blocks.Remove(blocks[i]);
                break;
            }
        }
        if(uuid!=0)datas.Remove(uuid);
        
    }


    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class Container_EntityBlock : BaseContainer
{
    /// <summary>
    /// uuid到其eblock中心的映射
    /// </summary>
    Dictionary<long, long> UUID_blockpos = new Dictionary<long, long>();//不参与存储,center=所在点的bpos
    Dictionary<long, long> blockPos_UUID = new Dictionary<long, long>();//blockPos是世界坐标，不参与存储

    Dictionary<long, EventCenter> UUID_blockEvc { get { return eventCenterContainer.UUID_EVC; } set { } }//分块加载仅作用于此

    //================存储内容
    Dictionary<long, Dictionary<long, Entity_Block>> ChunkPos_eblocks = new Dictionary<long, Dictionary<long, Entity_Block>>();
    Dictionary<long, string> UUID_Data = new Dictionary<long, string>();//临时保存文件时用

    GameObject terrianRoot;
    TerrainGenerator generator = new TerrainGenerator();
    Container_Terrain containerTerrian;
    Container_EventCenter eventCenterContainer;

    public override void AfterEventRegist()
    {

    }

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        center.ListenEvent<PlaceEBlockParm>(nameof(EventNames.SetEBlock), SetEBlockObj);
        center.RegistFunc<PlaceEBlockParm,EventCenter>(nameof(EventNames.SetEBlock), SetAndGetEBlockObj);
        center.RegistFunc<Entity_Block, bool>(nameof(EventNames.EBlockCanPlaceAt), IsEBlockCanPlace);
        center.ListenEvent<long>(nameof(EventNames.RemoveEBlock), RemoveEBlockObj);
        center.ListenEvent<long, int>(nameof(EventNames.ChunkUpdate), OnChunkUpdate);//监听container_terrian的加卸载事件
        center.ListenEvent<long, int>(nameof(EventNames.BigChunkLoaded), OnBigChunkLoaded);//同步加载模式下先于chunkupdate
        containerTerrian = ContainerManager.GetContainer<Container_Terrain>();
        eventCenterContainer = ContainerManager.GetContainer<Container_EventCenter>();
    }
    public override void OnLoadGame(MapPrefabsData data, int index)
    {
        base.OnLoadGame(data, index);
        if (index != 1) return;
        //读取数据
        try
        {
            //string eblockdatas = FileSaver.GetEBlockData(center.GetParm<string>(nameof(EventNames.ThisSavePath)));
            terrianRoot = ContainerManager.GetContainer<Container_Terrain>().Chunks;
            if (FileSaver.IsMapLoaded(center.GetParm<string>(nameof(EventNames.ThisSavePath))))
            {
                //nop
            }
            else
            {
                //UnityEngine.Debug.Log("生成自然eblock数据");//在terraingenerator处生成
               //EntityBlockFile[,] efs= generator.GenEBlocks(data);//数据生成阶段,生成chunk数据到文件
                //SaveAllEBlockFile(efs);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("实体方块数据加载失败" + e.Message + "。尝试生成实体方块数据文件");
            try
            {
                //FileSaver.SetEBlockData(center.GetParm<string>(nameof(EventNames.ThisSavePath)), "");
            }
            catch (System.Exception)
            {
                Debug.Log("实体方块数据生成失败");
            }

        }
    }
    /// <summary>
    /// 实体在地形生成完成后再生成
    /// </summary>
    /// <param name="index"></param>
    public override void OnBuildGame(int index)
    {
        if (index != 1) return;
        //以玩家当前位置为中心，从文件中读实体数据,生成实体obj
        
    }
    public override void UnLoadGame(int ind)
    {
        if (ind != 0) return;
        //chunkPos_blockpos_blocks.Clear();
        //UUID_blockEvc.Clear();
        blockPos_UUID.Clear();
        UUID_blockpos.Clear();
        UUID_Data.Clear();
        ChunkPos_eblocks.Clear();
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

    public void OnLoadEBlockDone()//eblock数据加载完成开始生成gobj
    {

    }

    public void SaveBigChunk(long xy,EntityBlockFile f)
    {
        int x = xy.GetX();
        int y = xy.GetY();
        FileSaver.SetEBlockFile(center.GetParm<string>(nameof(EventNames.ThisSavePath)), x, y, f.ToString());
    }
    public void OnBigChunkLoaded(long xy, int stat)//当某个chunks文件被加载时,0加载，1卸载；加载时早于chunkupdate,卸载时晚于chunkupdate
    {//xy是bigchunk坐标
        switch (stat)
        {
            case 0:
                string dat = FileSaver.GetEBlockFile(center.GetParm<string>(nameof(EventNames.ThisSavePath)), xy.GetX(), xy.GetY());
                if (string.IsNullOrEmpty(dat)) return;
                EntityBlockFile f = JsonConvert.DeserializeObject<EntityBlockFile>(dat);
                //Dictionary<long,Dictionary<long,Entity_Block>> ebs = new Dictionary<long, Dictionary<long, Entity_Block>>();//<chunkpos,eblocks>
                for (int i = 0; i < f.blocks.Count; i++)//将eblockfile里的eblock按照chunk坐标分割
                {
                    AddEBlockData(f.blocks[i], f.datas[f.blocks[i].UUID]);
                }
                

                break;
            case 1:
                //找到bigchunk上所有eblock的obj，获取数据后写入eblockfile保存，销毁卸载区域内eblockObj,从chunkpos_eblock，uuid_data中卸载对应bigchunk
                int x = xy.GetX();
                int y = xy.GetY();

                List<long> unloadUUID = new List<long>();
                List<Entity_Block> unloadblocks = new List<Entity_Block>();
                for (int i = 0; i < Container_Terrain.chunkSizePerFile; i++)//获取需要回收的所有eblock UUID
                {
                    for (int j = 0; j < Container_Terrain.chunkSizePerFile; j++)
                    {
                        long temp = XYHelper.ToLongXY(x * Container_Terrain.chunkSizePerFile + i, y * Container_Terrain.chunkSizePerFile + j);
                        if (ChunkPos_eblocks.ContainsKey(temp))
                        {
                            foreach (var item in ChunkPos_eblocks[temp])
                            {
                                unloadUUID.Add(item.Key);
                                unloadblocks.Add(item.Value);
                            }
                        }
                        //unloadblocks.AddRange(ChunkPos_eblocks[temp]);
                        //ChunkPos_eblocks.Remove(temp);
                    }
                }

                EntityBlockFile ebf = new EntityBlockFile();
                ebf.blocks = unloadblocks;
                Dictionary<long, string> tempdata = new Dictionary<long, string>();
                for (int i = 0; i < unloadUUID.Count; i++)
                {
                    if (UUID_blockEvc.ContainsKey(unloadUUID[i]))
                    { tempdata.Add(unloadUUID[i], UUID_blockEvc[unloadUUID[i]].GetParm<string>(nameof(EntityBlockEventNames.saveStr)));
                    }
                    else
                    {
                        tempdata.Add(unloadUUID[i], UUID_Data[unloadUUID[i]]);
                    }
                }
                ebf.datas = tempdata;
                SaveBigChunk(xy, ebf);

                for (int i = 0; i < unloadUUID.Count; i++)
                {
                    RemoveEBlock(unloadUUID[i]);
                    
                }
                break;
            case 2://只保存，不卸载
                int x1 = xy.GetX();
                int y1 = xy.GetY();

                List<long> unloadUUID1 = new List<long>();
                List<Entity_Block> unloadblocks1 = new List<Entity_Block>();
                for (int i = 0; i < Container_Terrain.chunkSizePerFile; i++)//获取需要回收的所有eblock UUID
                {
                    for (int j = 0; j < Container_Terrain.chunkSizePerFile; j++)
                    {
                        long temp = XYHelper.ToLongXY(x1 * Container_Terrain.chunkSizePerFile + i, y1 * Container_Terrain.chunkSizePerFile + j);
                        if (ChunkPos_eblocks.ContainsKey(temp))
                        {
                            foreach (var item in ChunkPos_eblocks[temp])
                            {
                                unloadUUID1.Add(item.Key);
                                unloadblocks1.Add(item.Value);
                            }
                        }
                        //unloadblocks.AddRange(ChunkPos_eblocks[temp]);
                        //ChunkPos_eblocks.Remove(temp);
                    }
                }

                EntityBlockFile ebf1 = new EntityBlockFile();
                ebf1.blocks = unloadblocks1;
                Dictionary<long, string> tempdata1 = new Dictionary<long, string>();
                for (int i = 0; i < unloadUUID1.Count; i++)
                {
                    if (UUID_blockEvc.ContainsKey(unloadUUID1[i]))
                    {
                        tempdata1.Add(unloadUUID1[i], UUID_blockEvc[unloadUUID1[i]].GetParm<string>(nameof(EntityBlockEventNames.saveStr)));
                    }
                    else
                    {
                        tempdata1.Add(unloadUUID1[i], UUID_Data[unloadUUID1[i]]);
                    }
                }
                ebf1.datas = tempdata1;
                SaveBigChunk(xy, ebf1);
                break;
            default:
                break;
        }
    }
    public void OnChunkUpdate(long xy, int stat)//加载时晚于bigchunkupdate，卸载时早于bigchunkload
    {//1加载2卸载3保存
        if (stat != 1 && stat != 2&&stat!=3) return;
        if (!ChunkPos_eblocks.ContainsKey(xy)) return;
        //blockPos_UUID,UUID_blockpos都是常驻不随chunk更新变的，只随bigchunk加卸载变化

        if (stat == 1)//从uuid_data取数据生成eblockobj
        {
            List<long> needGenUUID = new List<long>();
            foreach (var item in ChunkPos_eblocks[xy])
            {
                needGenUUID.Add(item.Key);
            }
            for (int i = 0; i < needGenUUID.Count; i++)
            {
                if (UUID_blockEvc.ContainsKey(needGenUUID[i]))
                {
                    UUID_blockEvc[needGenUUID[i]].SendEvent<bool>("SetActive", true);
                }
                else
                {
                    GameObject temp = center.GetParm<int, GameObject>(nameof(EventNames.GetEBlockObjByID), ChunkPos_eblocks[xy][needGenUUID[i]].typ);
                    temp.transform.SetParent(terrianRoot.transform);
                    //根据ChunkPos_eblocks[xy][needGenUUID[i]]转方向
                    temp.transform.eulerAngles=ChunkPos_eblocks[xy][needGenUUID[i]].dir.GetAngle();
                    EventCenter evc = temp.GetComponent<EventCenter>();
                    UUID_blockEvc.Add(needGenUUID[i], evc);
                    evc.SendEvent<Entity_Block>(nameof(EntityBlockEventNames.ebinit), ChunkPos_eblocks[xy][needGenUUID[i]]);
                    evc.SendEvent<string>(nameof(EntityBlockEventNames.loadStr), UUID_Data[needGenUUID[i]]);
                }
            }

        }
        else if(stat==2)//卸载eblockObj，保存数据到uuid_data
        {
            List<long> needunloadUUID = new List<long>();
            foreach (var item in ChunkPos_eblocks[xy])
            {
                needunloadUUID.Add(item.Key);
            }
            for (int i = 0; i < needunloadUUID.Count; i++)
            {
                if (!UUID_blockEvc.ContainsKey(needunloadUUID[i]) ||UUID_blockEvc[needunloadUUID[i]] == null) continue;//可能此eblockobj已经在bigchunkload方法中被删除了

                string data = UUID_blockEvc[needunloadUUID[i]].GetParm<string>(nameof(EntityBlockEventNames.saveStr));
                if (UUID_Data.ContainsKey(needunloadUUID[i]))
                {
                    UUID_Data[needunloadUUID[i]] = data;
                }
                else
                {
                    UUID_Data.Add(needunloadUUID[i], data);
                }
                //GameObject.Destroy();
                UUID_blockEvc[needunloadUUID[i]].SendEvent<bool>("SetActive", false);
            }
        }
        else if(stat==3)//只保存不卸载
        {
            List<long> needsaveUUIDs = new List<long>();
            foreach (var item in ChunkPos_eblocks[xy])
            {
                needsaveUUIDs.Add(item.Key);
            }
            for (int i = 0; i < needsaveUUIDs.Count; i++)
            {
                if (!UUID_blockEvc.ContainsKey(needsaveUUIDs[i]) || UUID_blockEvc[needsaveUUIDs[i]] == null) continue;//可能此eblockobj已经在bigchunkload方法中被删除了

                string data = UUID_blockEvc[needsaveUUIDs[i]].GetParm<string>(nameof(EntityBlockEventNames.saveStr));
                if (UUID_Data.ContainsKey(needsaveUUIDs[i]))
                {
                    UUID_Data[needsaveUUIDs[i]] = data;
                }
                else
                {
                    UUID_Data.Add(needsaveUUIDs[i], data);
                }
               
            }
        }
    }



    bool IsEBlockCanPlace(Entity_Block eb)
    {
        
        int xyh = center.GetParm<int, int>(nameof(EventNames.GetEBlockSizeByID), eb.typ);
        int[] size = xyh.GetCoord3();
        for (int i = 0; i < size[0]; i++)
        {
            for (int j = 0; j < size[1]; j++)
            {
                for (int k = 0; k < size[2]; k++)
                {
                    long blockpos = XYHelper.ToLongCoord3(eb.posX + i, eb.posY + j, eb.posH + k);
                    if (blockPos_UUID.ContainsKey(blockpos))
                    {
                        if (UUID_Data.ContainsKey(blockPos_UUID[blockpos])) { Debug.Log("此处不可放置" + (eb.posX + i) + ":" + (eb.posY + j) + ":" + (eb.posH + k)); return false; }
                        else
                        {//这里有block但是无效
                            blockPos_UUID.Remove(blockpos);
                        }
                    }
                    else if (!containerTerrian.CanPlaceAt(blockpos))//在地形下放不了东西
                    {
                        //Debug.Log("此处在地形下不可放置" + (blockpos.GetCoord3()[0] + i) + ":" + (blockpos.GetCoord3()[1] + j) + ":" + (blockpos.GetCoord3()[2] + k));
                        return false;
                    }
                }
            }
        }
        return true;
    }
    /// <summary>
    /// 只加数据，不生成obj，不触发方块更新通知事件
    /// </summary>
    /// <param name="eb"></param>
    /// <param name="data"></param>
    void AddEBlockData(Entity_Block eb, string data)//
    {
        if (!IsEBlockCanPlace(eb)) return;
        
        UUID_blockpos.Add(eb.UUID, XYHelper.ToLongCoord3(eb.posX, eb.posY, eb.posH));
        long bchunkpos = Chunk.BlockPosToChunkPos(eb.posX, eb.posY);
        if (ChunkPos_eblocks.ContainsKey(bchunkpos))
        {
            ChunkPos_eblocks[bchunkpos].Add(eb.UUID, eb);
        }
        else
        {
            Dictionary<long, Entity_Block> temp = new Dictionary<long, Entity_Block>();
            temp.Add(eb.UUID, eb);
            ChunkPos_eblocks.Add(bchunkpos, temp);
        }
        int xyh = eb.GetSize();
        int[] size = xyh.GetCoord3();
        //Debug.Log(blockPos_UUID.Count + "个BLOCKPOS_uuid被加载"+xyh);
        for (int i = 0; i < size[0]; i++)
        {
            for (int j = 0; j < size[1]; j++)
            {
                for (int k = 0; k < size[2]; k++)
                {
                    long blockpos = XYHelper.ToLongCoord3(eb.posX + i, eb.posY + j, eb.posH + k);
                    blockPos_UUID.Add(blockpos, eb.UUID);
                }
            }
        }

        UUID_Data.Add(eb.UUID, data);
    }
    /// <summary>
    /// 删除运行时的数据，不删除文件中的eblock,不触发方块更新通知事件
    /// </summary>
    /// <param name="uuid"></param>
    void RemoveEBlock(long uuid)//
    {//移除blockPos_UUID,UUID_blockpos,UUID_blockEvc,ChunkPos_eblocks,UUID_Data
        if (!UUID_blockEvc.ContainsKey(uuid)) { Debug.Log("已被移除"); return; }
        //移除前可能需发个事件通知目标物体断开事件连接
        Entity_Block eblock = UUID_blockEvc[uuid].GetParm<Entity_Block>(nameof(EntityBlockEventNames.geteblock));
        int xyh = center.GetParm<int, int>(nameof(EventNames.GetEBlockSizeByID), eblock.typ);
        //eblock.
        int[] size = xyh.GetCoord3();
        for (int i = 0; i < size[0]; i++)
        {
            for (int j = 0; j < size[1]; j++)
            {
                for (int k = 0; k < size[2]; k++)
                {
                    long blockpos = XYHelper.ToLongCoord3(eblock.posX + i, eblock.posY + j, eblock.posH + k);
                    blockPos_UUID.Remove(blockpos);
                }
            }
        }
        UUID_blockpos.Remove(uuid);
        GameObject.Destroy(UUID_blockEvc[uuid].gameObject);//将来要优化为对象池 
        UUID_blockEvc.Remove(uuid);
        ChunkPos_eblocks[Chunk.BlockPosToChunkPos(eblock.posX, eblock.posY)].Remove(uuid);
        UUID_Data.Remove(uuid);
    }

    public void SaveAllEBlockFile(EntityBlockFile[,] efs)
    {
        string path = center.GetParm<string>(nameof(EventNames.ThisSavePath));
        int x = Mathf.CeilToInt( efs.GetLength(0)*1f/2);
        int y = Mathf.CeilToInt(efs.GetLength(1) * 1f / 2);
        for (int i = 0; i < efs.GetLength(0); i++)
        {
            for (int j = 0; j < efs.GetLength(1); j++)
            {
                FileSaver.SetEBlockFile(path, i-x, j-y, efs[i, j].ToString());
            }
        }
    }

    #region 方块更新通知事件
    public void OnBlockNotifyByTB(Vector3Int pos, Vector3Int target, T_Block tb)
    {
        //检查target有无eblock,有则发送更新事件
        EventCenter evc = GetEBlockEVCByBPos(target);
        if(evc!=null)
            evc.SendEvent<Vector3Int, T_Block>(nameof(TerrianEventName.BlockNotifyByTB),pos,tb);
    }
    public void OnBlockNotifyByBB(Vector3Int pos, Vector3Int target, B_Block bb)
    {
        Debug.Log(target + "BB更新事件");
        EventCenter evc = GetEBlockEVCByBPos(target);
        if (evc != null)
            evc.SendEvent<Vector3Int, B_Block>(nameof(TerrianEventName.BlockNotifyByBB), pos, bb);
    }
    long OnBlockNotifyByEB(Vector3Int pos, Vector3Int target, Entity_Block eb)
    {//2*字典查找+1*evc
        long uuid=GetEBlockUUIDByBPos(target);
        if(uuid==-1)return -1;
        EventCenter evc = GetEVCByUUID(uuid);
        if (evc != null)
            {
                evc.SendEvent<Vector3Int, Entity_Block>(nameof(TerrianEventName.BlockNotifyByEB), pos, eb);
                return uuid;
            }
            return -1;
    }
    
    #endregion

    void EBlockNearByUpdate(Entity_Block eb)
    {
        int[] size=eb.GetSize().GetCoord3();
        HashSet<long> UpdatedUUID=new HashSet<long>();

        Vector3Int source=new Vector3Int(eb.posX,eb.posY,eb.posH);
        Vector3Int pos=new Vector3Int(eb.posX,eb.posY,eb.posH);
        //按-Z面，+X面，+Z面，-X面，+Y面，-Y面的顺序发送更新
        pos.z-=1;
        for (int i = 0; i < size[2]; i++)//y
        {
            for (int j = 0; j < size[0]; j++)//x
            {
                pos.x=source.x+j;
                pos.y=source.y+i;
                long asscessing=GetEBlockUUIDByBPos(pos);
                if(asscessing==-1){containerTerrian.BlockNotifyByEB(source,pos,eb);}
               else if(!UpdatedUUID.Contains(asscessing))
                {
                    OnBlockNotifyByEB(source,pos,eb);
                    UpdatedUUID.Add(asscessing);
                }
            }
        }

        pos=source;//+X面
        pos.x+=size[0];
        for (int i = 0; i < size[2]; i++)//y
        {
            for (int j = 0; j < size[1]; j++)//z
            {
                pos.z=source.z+j;
                pos.y=source.y+i;
                long asscessing=GetEBlockUUIDByBPos(pos);
                if(asscessing==-1){containerTerrian.BlockNotifyByEB(source,pos,eb);}
               else if(!UpdatedUUID.Contains(asscessing))
                {
                    OnBlockNotifyByEB(source,pos,eb);
                    UpdatedUUID.Add(asscessing);
                }
            }
        }

        pos=source;//+Z面
        pos.z+=size[1];
        for (int i = 0; i < size[2]; i++)//y
        {
            for (int j = 0; j < size[0]; j++)//x
            {
                pos.x=source.x+j;
                pos.y=source.y+i;
                 long asscessing=GetEBlockUUIDByBPos(pos);
                if(asscessing==-1){containerTerrian.BlockNotifyByEB(source,pos,eb);}
               else if(!UpdatedUUID.Contains(asscessing))
                {
                    OnBlockNotifyByEB(source,pos,eb);
                    UpdatedUUID.Add(asscessing);
                }
            }
        }

        pos=source;//-X面
        pos.x-=1;
        for (int i = 0; i < size[2]; i++)//y
        {
            for (int j = 0; j < size[1]; j++)//z
            {
                pos.z=source.z+j;
                pos.y=source.y+i;
                 long asscessing=GetEBlockUUIDByBPos(pos);
                if(asscessing==-1){containerTerrian.BlockNotifyByEB(source,pos,eb);}
               else if(!UpdatedUUID.Contains(asscessing))
                {
                    OnBlockNotifyByEB(source,pos,eb);
                    UpdatedUUID.Add(asscessing);
                }
            }
        }

        pos=source;//+Y面
        pos.y+=size[2];
        for (int i = 0; i < size[0]; i++)//x
        {
            for (int j = 0; j < size[1]; j++)//z
            {
                pos.z=source.z+j;
                pos.x=source.x+i;
                 long asscessing=GetEBlockUUIDByBPos(pos);
                if(asscessing==-1){containerTerrian.BlockNotifyByEB(source,pos,eb);}
               else if(!UpdatedUUID.Contains(asscessing))
                {
                    OnBlockNotifyByEB(source,pos,eb);
                    UpdatedUUID.Add(asscessing);
                }
            }
        }

        pos=source;//-Y面
        pos.y-=1;
        for (int i = 0; i < size[0]; i++)//x
        {
            for (int j = 0; j < size[1]; j++)//z
            {
                pos.z=source.z+j;
                pos.x=source.x+i;
                 long asscessing=GetEBlockUUIDByBPos(pos);
                if(asscessing==-1){containerTerrian.BlockNotifyByEB(source,pos,eb);}
               else if(!UpdatedUUID.Contains(asscessing))
                {
                    OnBlockNotifyByEB(source,pos,eb);
                    UpdatedUUID.Add(asscessing);
                }
            }
        }
    }

    #region 外部访问
    /// <summary>
    /// 不检查地形，只检查eblock是否重叠
    /// </summary>
    /// <param name="efs"></param>
    /// <param name="eb"></param>
    /// <returns></returns>
    public static bool IsEBlockCanPlaceAt(EntityBlockFile[,] efs, Entity_Block eb)
    {
        return true;
    }
    public void RemoveEBlockObj(long UUID)//触发eblock更新事件和通知事件
    {
        if (!UUID_blockEvc.ContainsKey(UUID)) return;
        Debug.Log("移除且有" + UUID);
        Entity_Block eb = UUID_blockEvc[UUID].GetParm<Entity_Block>(nameof(EntityBlockEventNames.geteblock));
        RemoveEBlock(UUID);
        EBlockNearByUpdate(eb);
    }
    public void SetEBlockObj(PlaceEBlockParm parm)//触发eblock更新事件和通知事件
    {
        Entity_Block eb = new Entity_Block(parm);

        if (IsEBlockCanPlace(eb))
        {
            long newuuid = center.GetParm<long>(nameof(EventNames.GetMaxUUID));
            eb.UUID = newuuid;
            AddEBlockData(eb, parm.initData);

            Vector3 tempv3 = new Vector3(parm.pos.x * Chunk.BlockSize, parm.pos.y * Chunk.BlockHeight, parm.pos.z * Chunk.BlockSize);
            if (center.GetParm<Vector3, bool>(nameof(EventNames.IsHereLoaded), tempv3))//表现层，在活动区域内才生成obj
            {
                GameObject temp = center.GetParm<int, GameObject>(nameof(EventNames.GetEBlockObjByID), eb.typ);
                temp.transform.SetParent(terrianRoot.transform);//具体位置调整在baseentityblock中进行
                EventCenter evc = temp.GetComponent<EventCenter>();
                UUID_blockEvc.Add(eb.UUID, evc);
                evc.SendEvent<Entity_Block>(nameof(EntityBlockEventNames.ebinit), eb);
                evc.SendEvent<string>(nameof(EntityBlockEventNames.loadStr), parm.initData);
            }

            //触发eblock更新事件和通知事件，发送给container_Terrian和自己
            //查找此eblock边沿所有方块,发送方块更新，
            EBlockNearByUpdate(eb);
        }
    }
    public EventCenter SetAndGetEBlockObj(PlaceEBlockParm parm)//触发eblock更新事件和通知事件
    {
        EventCenter evc = null;
           Entity_Block eb = new Entity_Block(parm);

        if (IsEBlockCanPlace(eb))
        {
            long newuuid = center.GetParm<long>(nameof(EventNames.GetMaxUUID));
            eb.UUID = newuuid;
            AddEBlockData(eb, parm.initData);

            Vector3 tempv3 = new Vector3(parm.pos.x * Chunk.BlockSize, parm.pos.y * Chunk.BlockHeight, parm.pos.z * Chunk.BlockSize);
            if (center.GetParm<Vector3, bool>(nameof(EventNames.IsHereLoaded), tempv3))//表现层，在活动区域内才生成obj
            {
                GameObject temp = center.GetParm<int, GameObject>(nameof(EventNames.GetEBlockObjByID), eb.typ);
                temp.transform.SetParent(terrianRoot.transform);//具体位置调整在baseentityblock中进行
                evc = temp.GetComponent<EventCenter>();
                UUID_blockEvc.Add(eb.UUID, evc);
                evc.SendEvent<Entity_Block>(nameof(EntityBlockEventNames.ebinit), eb);
                evc.SendEvent<string>(nameof(EntityBlockEventNames.loadStr), parm.initData);
            }

            //触发eblock更新事件和通知事件，发送给container_Terrian和自己
            //查找此eblock边沿所有方块,发送方块更新，
            EBlockNearByUpdate(eb);
        }
        return evc;
    }
    


    //都是全局0.0的blockpos
    public long GetEBlockUUIDByBPos(Vector3Int bpos)
    {
        long temp = XYHelper.ToLongCoord3(bpos.x, bpos.z, bpos.y);
        if (blockPos_UUID.ContainsKey(temp))
        {
            if (UUID_Data.ContainsKey(blockPos_UUID[temp]))
            {
                return blockPos_UUID[temp];
            }
        }
        return -1;
    }
    public EventCenter GetEBlockEVCByBPos(Vector3Int bpos)
    {
        long temp = XYHelper.ToLongCoord3(bpos.x, bpos.z, bpos.y);
        if (blockPos_UUID.ContainsKey(temp))
        {
            if (UUID_blockEvc.ContainsKey(blockPos_UUID[temp]))
            {
                return UUID_blockEvc[blockPos_UUID[temp]];
            }
        }
        return null;
    }
    public EventCenter GetEVCByUUID(long uuid)
    {
        if (UUID_blockEvc.ContainsKey(uuid))
            {
                return UUID_blockEvc[uuid];
            }
            return null;
    }
    #endregion
}
public enum TerrianEventName
{
    BlockNotifyByTB, BlockNotifyByBB, BlockNotifyByEB
}