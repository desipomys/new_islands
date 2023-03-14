using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Diagnostics;

//可多线程加载chunk数据
public class Container_Terrain : BaseContainer
{
    /// <summary>
    /// 每个chunk文件里有多少*多少个chunk
    /// </summary>
    public static readonly int chunkSizePerFile = 8;
    static float chunkOutOfDateTime = 30f;
    int activeRange = 8, extendRange = 6;//加载范围直径


    int mapWidth, mapHeight, seaLevel;

    TerrainGenerator generator = new TerrainGenerator();

    Dictionary<long, Chunk> activeChunk = new Dictionary<long, Chunk>();//
    //Dictionary<long, Chunk> sleepChunk = new Dictionary<long, Chunk>();//key是小坐标，高32位是x,低32位是y
    Dictionary<long, Chunk[,]> cachedBigChunk = new Dictionary<long, Chunk[,]>();//文件中取出的chunk[,]，key是chunkfile的大坐标

    Dictionary<long, float> bigChunkUsage = new Dictionary<long, float>();//key对应的bigchunk是否在使用中，0则是，非零则为空闲，值为上次检查时间
    GridObjectPool chunkObjPool;

    public GameObject Chunks;
    GameObject Chunkobj;
    Dictionary<long, GameObject> ChunkObjects = new Dictionary<long, GameObject>();
    string path = "Prefabs/terrain/ChunkObj";
    string waterPath = "Prefabs/terrain/TerrainWater";

    public override void OnEventRegist(EventCenter e)
    {
        UnityEngine.Debug.Log("terrian INIT");
        base.OnEventRegist(e);
        e.RegistFunc<Vector3, bool>(nameof(EventNames.IsHereLoaded), IsHereLoaded);//此处是否为活动chunk（其chunkobj是active的）
        e.RegistFunc<long[]>(nameof(EventNames.GetAllActiveChunkFile), () => { return new List<long>(cachedBigChunk.Keys).ToArray(); });
        e.RegistFunc<long, GameObject>(nameof(EventNames.GetChunkObj), (long xy) => { return chunkObjPool.Get(xy); });
        e.ListenEvent(nameof(EventNames.UpdateActiveChunk), UpdateActiveChunk);
        //只负责存储不负责生成地形
    }
    public override void OnLoadSave(SaveData data)
    {
        //mapWidth=data.

        Chunkobj = Resources.Load<GameObject>(path);

        /*Chunks=new GameObject();//以下跳到ingame才执行
        Chunks.transform.position=EventCenter.WorldCenter.transform.position;
        Chunks.transform.SetParent(EventCenter.WorldCenter.transform);*/
    }
    public override void OnLoadGame(MapPrefabsData data, int index)
    {
        if (index != 0) return;
        base.OnLoadGame(data, index);
        Chunks = GameObject.Find("Terrain");
        mapWidth = data.config.width;
        mapHeight = data.config.height;
        seaLevel = data.config.sea;
        chunkObjPool = new GridObjectPool(Chunkobj, EventCenter.WorldCenter.gameObject);
        string thissavepath=center.GetParm<string>(nameof(EventNames.ThisSavePath));
        //读取存档文件的map文件夹，如果里面有map的chunk数据则不动
        if (FileSaver.IsMapLoaded(thissavepath))
        {
            //nop
        }
        else if(data.generated)
        {
            //将gendedMap的对应文件复制到map
            FileSaver.CopyGenedMapToMap(thissavepath,data.mapName);
        }
        else
        {
            

            Stopwatch sw = new Stopwatch();
            sw.Start();

            generator.GenTerrain(data);
            //SaveMapAllChunkFile();//数据生成阶段,生成chunk数据到文件
            //生成eblockfile数据
            //
            sw.Stop();
            UnityEngine.Debug.Log("生成chunk数据用时"+ sw.Elapsed.TotalMilliseconds);
        }

    }
    public override void UnLoadGame(int ind)
    {
        if (ind != 0) return;
        activeChunk.Clear();
        cachedBigChunk.Clear();
        bigChunkUsage.Clear();
        ChunkObjects.Clear();
        chunkObjPool.Clear();
        center.ForceUnRegistFunc<long, int>(nameof(EventNames.GetTerrainHAt));
        center.ForceUnRegistFunc<long, Chunk>(nameof(EventNames.GetChunkData));
        center.UnListenEvent<Vector3Int, B_Block>(nameof(EventNames.SetBBlock), SetBBlock);
    }
    public override void OnBuildGame(int index)
    {
        if (index != 0) return;
        //以玩家当前位置为中心，从文件中读chunk数据,生成chunkobj
        Vector3 v3 = center.GetParm<Vector3>(nameof(EventNames.GetLocalPlayerOfflinePosi));
        long xy = Chunk.WorldPosToChunkPos(v3);
        UnityEngine.Debug.Log(v3.ToString() + "初始加载点" + xy.GetX() + "," + xy.GetY());
        updateActiveChunkFrom(new Vector3[] { v3 });

        buildAirWall();
        //先暂时生成一个平板+四面墙
        buildSea();

        //注册获取地形高度方法，新增监听方法记得在unloadgame取消
        center.RegistFunc<long, int>(nameof(EventNames.GetTerrainHAt), (long a) => { return getBlockHeight(a.GetX(), a.GetY()); });
        center.RegistFunc<long, Chunk>(nameof(EventNames.GetChunkData), (long a) => { return GetChunkData(a); });
        center.ListenEvent<Vector3Int, B_Block>(nameof(EventNames.SetBBlock), SetBBlock);
    }
    //需要map下的proper数据，里面放的是MapPrefabsData
    public override void Save(string path)
    {
        base.Save(path);
        foreach (var item in cachedBigChunk)
        {
            saveChunkFile(item.Key, item.Value);
        }
    }
    public override void OnUpdate()
    {
        if(chunkObjPool!=null)
        chunkObjPool.OnUpdate();
    }


    void buildSea()//临时直接使用平板代替海面
    {
        return;
        GameObject g = Resources.Load<GameObject>(waterPath);
        GameObject t = GameObject.Instantiate(g, Vector3.zero, Quaternion.identity);
        t.transform.localScale = Vector3.one * mapWidth;
        t.transform.position = Vector3.up * (seaLevel * Chunk.BlockHeight - 0.25f * Chunk.BlockHeight);
    }
    void buildAirWall()
    {
        float wallThick = 5f;
        GameObject w1 = new GameObject();
        w1.name = "w1";
        w1.transform.position = new Vector3(0, 0, (mapHeight * Chunk.BlockSize + wallThick) / 2);
        BoxCollider bc = w1.AddComponent<BoxCollider>();
        bc.size = new Vector3(mapWidth * Chunk.BlockSize, 65536, wallThick + wallThick * 2);
        //w1.layer = LayerMask.NameToLayer("Ignore RayCast");

        GameObject w2 = new GameObject();
        w2.name = "w2";
        w2.transform.position = new Vector3(0, 0, -(mapHeight * Chunk.BlockSize + wallThick) / 2);
        bc = w2.AddComponent<BoxCollider>();
        bc.size = new Vector3(mapWidth * Chunk.BlockSize, 65536, wallThick + wallThick * 2);
        //w2.layer = LayerMask.NameToLayer("Ignore RayCast");

        GameObject w3 = new GameObject();
        w3.name = "w3";
        w3.transform.position = new Vector3(-(mapWidth * Chunk.BlockSize + wallThick) / 2, 0, 0);
        bc = w3.AddComponent<BoxCollider>();
        bc.size = new Vector3(wallThick, 65536, mapHeight * Chunk.BlockSize + wallThick * 2);
        //w3.layer = LayerMask.NameToLayer("Ignore RayCast");

        GameObject w4 = new GameObject();
        w4.name = "w4";
        w4.transform.position = new Vector3((mapWidth * Chunk.BlockSize + wallThick) / 2, 0, 0);
        bc = w4.AddComponent<BoxCollider>();
        bc.size = new Vector3(wallThick, 65536, mapHeight * Chunk.BlockSize + wallThick * 2);
        //w4.layer = LayerMask.NameToLayer("Ignore RayCast");
    }
    public void updateActiveChunkFrom(Vector3[] updatePosi)
    {

        HashSet<long> loadcenterChunk = new HashSet<long>();//确定从哪些chunk作为加载中心开始加载

        for (int i = 0; i < updatePosi.Length; i++)
        {
            loadcenterChunk.Add(Chunk.WorldPosToChunkPos(updatePosi[i]));
        }

        HashSet<long> needActiveChunk = new HashSet<long>();
        //HashSet<long> needSleepChunk=new HashSet<long>();
        HashSet<long> needCacheChunk = new HashSet<long>();
        foreach (var item in loadcenterChunk)//确定哪些chunk需要加载、sleep、cache
        {
            for (int i = 0; i < activeRange + extendRange; i++)
            {
                for (int j = 0; j < activeRange + extendRange; j++)//多计算一层加载范围外的chunk
                {

                    long temp = XYHelper.AddXLong(item, (i - (activeRange + extendRange) / 2));
                    temp = XYHelper.AddYLong(temp, (j - (activeRange + extendRange) / 2));
                    if (!IscordInMap(temp)) continue;
                    if (Mathf.Sqrt((i - (activeRange + extendRange) / 2) * (i - (activeRange + extendRange) / 2) + (j - (activeRange + extendRange) / 2) * (j - (activeRange + extendRange) / 2)) <= (activeRange / 2))
                    {
                        //if(!activeChunk.ContainsKey(temp))
                        needActiveChunk.Add(temp);
                        needCacheChunk.Remove(temp);
                    }
                    else /*if (Mathf.Sqrt((i-(activeRange + 2) /2) * (i-(activeRange + 2) /2) + (j-(activeRange + 2) /2) * (j-(activeRange + 2) /2)) <= (activeRange +1))*/
                    {

                        needCacheChunk.Add(temp);
                    }
                }
            }
        }
        foreach (var item in needActiveChunk)
        {
            //Debug.Log("激活" + item.GetX() + "," + item.GetY());
        }
        foreach (var item in needCacheChunk)
        {
            //Debug.Log("关闭" + item.GetX() + "," + item.GetY());
        }

        //将active set 中存在而active chunk中没有的添加到active chunk
        foreach (var item in needActiveChunk)
        {
            if (!activeChunk.ContainsKey(item))
            {
                Chunk c = getChunkInCache(item);
                if (c != null)
                {
                    activeChunk.Add(item, c);
                }
            }
        }
        //清除activechunk中存在而不在needactive hashset中的chunk
        foreach (var item in needCacheChunk)
        {
            if (activeChunk.ContainsKey(item))
            {
                activeChunk.Remove(item);
            }
        }


        //将sleep set 中存在而sleep chunk中没有的添加到sleep chunk
        /*foreach (var item in needSleepChunk)
        {
            if(!sleepChunk.ContainsKey(item))
            {
                sleepChunk.Add(item,getChunkInCache(item));
            }
        }*/


        //更新刚从sleep到cache状态的chunk
        foreach (var item in needCacheChunk)
        {

            updateChunkObject(item, 2);
        }

        //更新active chunk对应chunk object状态
        foreach (var item in needActiveChunk)
        {

            updateChunkObject(item, 0);
        }
        //更新sleep chunk对应chunk object状态
        /*foreach (var item in sleepChunk)
        {
            updateChunkObject(item.Key,1);
        }*/


        checkCacheChunkInUse();
    }
    public void UpdateActiveChunk()
    {
        Vector3[] updatePosi = EventCenter.WorldCenter.GetParm<Vector3[]>(nameof(EventNames.GetAllPlayerPosi));
        /* string a = "";
         for (int i = 0; i < updatePosi.Length; i++)
         {
             a += updatePosi[i].ToString()+" ";
         }*/
        //Debug.Log("生成从" +a+ "开始");
        updateActiveChunkFrom(updatePosi);
    }

    /// <summary>
    /// 向坐标对应chunkobj发送状态更新信息
    /// </summary>
    /// <param name="xy"></param>
    /// <param name="stat">0active,1sleep,2cache,3unload</param>
    void updateChunkObject(long xy, int stat)
    {//active时setactive true，sleep时取消entity活动，cache则setactive false，unload删除chunkobj
        if (!IscordInMap(xy)) { UnityEngine.Debug.Log(xy.GetX() + "," + xy.GetY() + "不在地图内"); return; }
        ChunkObject temp = null;
        switch (stat)
        {
            case 0:
                if (!ChunkObjects.ContainsKey(xy))
                {
                    /*temp = GameObject.Instantiate(Chunkobj, Chunks.transform.position + Chunk.ChunkPosToWorldPos(xy), Chunks.transform.rotation, Chunks.transform).GetComponent<ChunkObject>();*/
                    temp = chunkObjPool.Pop(xy).GetComponent<ChunkObject>();
                    temp.transform.SetParent(Chunks.transform);
                    temp.transform.position = Chunks.transform.position + Chunk.ChunkPosToWorldPos(xy);
                    temp.Init(activeChunk[xy], xy.GetX(), xy.GetY());
                    ChunkObjects.Add(xy, temp.gameObject);//新创建的才需要加入，否则是直接替换
                    //Debug.Log("生成chunkobj于" + xy.GetX() + "," + xy.GetY()+","+xy.ToString("X"));
                }
                else temp = ChunkObjects[xy].GetComponent<ChunkObject>();
                //Debug.Log("加载" + xy.GetX() + "," + xy.GetY());
                break;

            case 2:
                if (!ChunkObjects.ContainsKey(xy)) { return; }
                //Debug.Log("回收" + xy.GetX() + "," + xy.GetY());
                temp = ChunkObjects[xy].GetComponent<ChunkObject>();
                ChunkObjects.Remove(xy);
                break;
            case 3:
                if (!ChunkObjects.ContainsKey(xy)) { return; }
                //Debug.Log("清除" + xy.GetX() + "," + xy.GetY());
                temp = ChunkObjects[xy].GetComponent<ChunkObject>();
                ChunkObjects.Remove(xy);
                break;
            default:
                break;
        }
        if (temp != null)
        {
            temp.ChunkStateUpdate(xy.GetX(), xy.GetY(), stat);

        }
    }

    #region utility
    /// <summary>
    /// 全局chunk坐标转生成的chunk数组坐标,以左下角为0,0，横向增加x
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="mapWid"></param>
    /// <param name="mapHig"></param>
    /// <returns></returns>
    long smallCordToGenChunksCord(int x, int y, int mapWid, int mapHig)
    {
        return XYHelper.ToLongXY(x + (mapWid / Chunk.ChunkSize) / 2, y + (mapWid / Chunk.ChunkSize) / 2);
    }
    long smallCordToIndexInsideFile(long xy)
    {
        int[] xandy = XYHelper.GetLongXY(xy);//小坐标
        //Mathf.FloorToInt((i * 1.0f) / 4)
        int bigx = (int)Mathf.Repeat(xandy[0], chunkSizePerFile);//大坐标
        int bigy = (int)Mathf.Repeat(xandy[1], chunkSizePerFile);//大坐标
        return XYHelper.ToLongXY(bigx, bigy);
    }
    /// <summary>
    /// 该坐标的chunk是否还在地图内
    /// </summary>
    /// <param name="xy"></param>
    bool IscordInMap(long xy)//chunk全局坐标
    {
        if ((mapWidth / 2) < Mathf.Abs(xy.GetX() * Chunk.ChunkSize + 0.1f))
        {
            return false;
        }
        if ((mapHeight / 2) < Mathf.Abs(xy.GetY() * Chunk.ChunkSize + 0.1f)) return false;
        return true;
    }
    /// <summary>
    /// 全局chunk坐标到chunkfile坐标
    /// </summary>
    /// <param name="xy"></param>
    /// <returns></returns>
   public static long smallCordToBigCord(long xy)
    {
        //int[] xandy = XYHelper.GetLongXY(xy);//小坐标
        //Mathf.FloorToInt((i * 1.0f) / 4)
        int bigx = Mathf.FloorToInt((xy.GetX() * 1.0f) / chunkSizePerFile);//大坐标
        int bigy = Mathf.FloorToInt((xy.GetY() * 1.0f) / chunkSizePerFile);//大坐标
        return XYHelper.ToLongXY(bigx, bigy);
    }
    /// <summary>
    /// 参数是小坐标
    /// </summary>
    /// <param name="xy"></param>
    /// <returns></returns>
    bool isChunkLoaded(long xy)
    {
        return cachedBigChunk.ContainsKey(smallCordToBigCord(xy));
    }
    /// <summary>
    /// 获取全局block坐标对应block的高度
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    int getBlockHeight(int x, int y)
    {
        long chunkFileIndex = smallCordToBigCord(Chunk.BlockPosToChunkPos(x, y));
        long globalChunkIndex = Chunk.BlockPosToChunkPos(x, y);
        x = (int)Mathf.Repeat(x, Chunk.ChunkSize);
        y = (int)Mathf.Repeat(y, Chunk.ChunkSize);
        if (cachedBigChunk.ContainsKey(chunkFileIndex))
        {
            if (cachedBigChunk[chunkFileIndex][(int)Mathf.Repeat(globalChunkIndex.GetX(), chunkSizePerFile), (int)Mathf.Repeat(globalChunkIndex.GetY(), chunkSizePerFile)] != null)
            {
                return cachedBigChunk[chunkFileIndex][(int)Mathf.Repeat(globalChunkIndex.GetX(), chunkSizePerFile), (int)Mathf.Repeat(globalChunkIndex.GetY(), chunkSizePerFile)].tblocks[x, y].H;
            }
            else return int.MaxValue;

        }
        else return int.MaxValue;
    }
    /// <summary>
    /// 参数是小坐标
    /// </summary>
    /// <param name="xy"></param>
    /// <returns></returns>
    bool isChunkActive(long xy)
    {
        return activeChunk.ContainsKey(xy);
    }


    /// <summary>
    /// 检查所有不用的chunk，记录到bigchunkusage中
    /// </summary>
    void checkCacheChunkInUse()
    {
        //将每个bigchunk设为空闲，对于每个activechunk和sleepchunk，将其所在bigchunk设为使用中，结束后对仍空闲的bigchunk标记到usage
        List<long> temp = new List<long>(bigChunkUsage.Keys);
        foreach (var item in temp)
        {
            if (bigChunkUsage[item] == 0)
                bigChunkUsage[item] = Time.time;
        }

        foreach (var item in activeChunk)
        {
            long bigxy = smallCordToBigCord(item.Key);
            bigChunkUsage[bigxy] = 0;
        }
        /*foreach (var item in sleepChunk)
        {
            long bigxy = smallCordToBigCord(item.Key);
            bigChunkUsage[bigxy] = 0;
        }*/

    }

    void clearOutOfDateBigChunk()
    {
        List<long> temp = new List<long>();//循环结束后统一unload的chunk的key
        foreach (var item in bigChunkUsage)
        {
            if (Time.time - item.Value > chunkOutOfDateTime)
            {
                temp.Add(item.Key);
            }
        }
        for (int i = 0; i < temp.Count; i++)
        {
            unloadBigChunk(temp[i]);
        }
    }
    void unloadBigChunk(long xy)
    {
        if (!cachedBigChunk.ContainsKey(xy)) return;
        bigChunkUsage.Remove(xy);
        saveChunkFile(xy, cachedBigChunk[xy]);
        for (int i = 0; i < chunkSizePerFile; i++)
        {
            for (int j = 0; j < chunkSizePerFile; j++)
            {
                long temp = XYHelper.AddXLong(xy, i);
                temp = XYHelper.AddYLong(xy, j);
                updateChunkObject(temp, 3);//卸载bigchunk上相应所有chunk object
            }
        }
        cachedBigChunk.Remove(xy);
        center.SendEvent<long, int>(nameof(EventNames.BigChunkLoaded), xy, 1);
    }
    /// <summary>
    /// 从缓存chunk中按坐标获取chunk，缓存中没有则去文件找
    /// </summary>
    /// <param name="xy">chunk坐标，有负数</param>
    /// <returns></returns>
    #endregion


    #region saveLoad
    void loadChunkFile(long xy)//大坐标
    {
        if (cachedBigChunk.ContainsKey(xy)) return;

        Stopwatch sw = new Stopwatch();
        sw.Start();

        string mapPath = EventCenter.WorldCenter.GetParm<string>(nameof(EventNames.ThisSavePath));
        string chunkdata = FileSaver.GetTerrainFile(mapPath, xy.GetX(), xy.GetY());
        if (string.IsNullOrEmpty(chunkdata))//文件损坏或不存在
        {

        }
        else
        {
            cachedBigChunk.Add(xy, JsonConvert.DeserializeObject<Chunk[,]>(chunkdata));
            bigChunkUsage.Add(xy, 0);
            center.SendEvent<long, int>(nameof(EventNames.BigChunkLoaded), xy, 0);//0加载，1卸载
        }
        sw.Stop();

        UnityEngine.Debug.Log("loadchunk" + xy.GetX() + ":" + xy.GetY() + "用时" + sw.Elapsed.TotalMilliseconds);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="xy">大坐标</param>
    /// <param name="chunk"></param>
    void saveChunkFile(long xy, Chunk[,] chunk)//不用在这存实体数据
    {
        //UnityEngine.Debug.Log("保存" + xy.GetX() + "," + xy.GetY() + "的bigchunk数据");
        string mapPath = center.GetParm<string>(nameof(EventNames.ThisSavePath));
        Stopwatch sw = new Stopwatch();
        sw.Start();
        FileSaver.SetTerrainFile(mapPath, xy.GetX(), xy.GetY(), JsonConvert.SerializeObject(chunk, SerializerHelper.setting));
        sw.Stop();

        UnityEngine.Debug.Log("保存" + xy.GetX() + "," + xy.GetY() + "的bigchunk数据" + "用时" + sw.Elapsed.TotalMilliseconds);
    }
    Chunk getChunkInCache(long xy)
    {
        long bigxy = smallCordToBigCord(xy);
        int x = (int)Mathf.Repeat(xy.GetX(), chunkSizePerFile);
        int y = (int)Mathf.Repeat(xy.GetY(), chunkSizePerFile);
        //x += (mapWidth / Chunk.ChunkSize) / 2;
        //y += (mapHeight / Chunk.ChunkSize) / 2;//转成bigchunk内部坐标

        if (!IscordInMap(xy)) return null;//chunk不在地图内返回空
        loadChunkFile(bigxy);

        if (cachedBigChunk.ContainsKey(bigxy) && cachedBigChunk[bigxy].GetLength(0) > x && cachedBigChunk[bigxy].GetLength(1) > y)
            return cachedBigChunk[bigxy][x, y];
        else return null;
    }
    #endregion

    #region 禁用与解禁
    //禁用某些bblock方块只会影响这里不会影响loader
    B_Material[] banedMat;//
    int[] banedMesh;
    
    public void BanMat(B_Material bm)
    {

    }
    public void BanMesh(int mesh)
    {

    }

    #endregion

    #region 外部可用方法
    /// <summary>
    /// 
    /// </summary>
    /// <param name="chunk"></param>
    /// <param name="bpos">世界坐标，以0.0为原点</param>
    /// <returns></returns>
    public static bool CanPlaceAt(Chunk[,] chunk,Vector3Int bpos)
    {
        Chunk.BlockPosToChunkPos(bpos.x, bpos.z);
        return true;
    }
    public bool CanPlaceAt(Vector3Int bpos)
    {
        int h = GetHeight(bpos);
        if (h <= bpos.y) return true;
        else return false;
    }
    public bool CanPlaceAt(long bpos)
    {
        int[] pos = bpos.GetCoord3();
        return CanPlaceAt(new Vector3Int(pos[0], pos[2], pos[1]));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos">世界block坐标</param>
    /// <returns></returns>
    public B_Block GetBBlock(Vector3Int pos)
    {
        try
        {
            if (IsHereLoaded(pos))
            {
                long temp = XYHelper.ToLongXY(pos.x / Chunk.ChunkSize, pos.y / Chunk.ChunkSize);
                if (activeChunk.ContainsKey(temp))//此chunk为active
                {
                    Vector3Int vi = Chunk.GlobalBPosToLocalBPos(pos);
                    return activeChunk[temp].blocks[XYHelper.ToIntXY(vi.x, vi.z)];
                }
            }
        }
        catch (System.Exception)
        {
            return null;

        }

        return null;
    }
    public void SetBBlock(Vector3Int pos, B_Block bb)
    {
        SetBBlock(pos, bb.mesh, bb.dir, bb.mat, bb.mat2, bb.sub);
        /*
        if (!IsHereLoaded(pos)) { UnityEngine.Debug.Log(pos.x / Chunk.ChunkSize + ":" + pos.z / Chunk.ChunkSize + "未加载"); return; }

        long chunkPos = XYHelper.ToLongXY(pos.x / Chunk.ChunkSize, pos.z / Chunk.ChunkSize);
        Vector3Int vi = Chunk.GlobalBPosToLocalBPos(pos);
        if (!activeChunk[chunkPos].CanPlaceAt(vi)) { UnityEngine.Debug.Log(vi + "不可放置"); return; }//如果该chunk中此处不能放置

        activeChunk[chunkPos].SetBBlock(vi, bb);
        UnityEngine.Debug.Log("chunk:" + pos.x / Chunk.ChunkSize + "." + pos.z / Chunk.ChunkSize + ",block:" + vi + "update");
        chunkObjPool.Get(chunkPos).GetComponent<ChunkObject>().ChunkBBlockUpdate(bb, vi);//表现层更新，可考虑使用mvc连接，但有增加复杂性的风险
                                                                                             //需向上下左右前后6方向发送方块更新事件,先向自己发，没有方块再向container_eblock发
                                                                                             */
    }
    public void SetBBlock(Vector3Int pos, int typ, DIR dir, B_Material tmat, int mat2, int sub = 0)//只改数据，触发方块更新事件
    {
        //UnityEngine.Debug.Log("11");
        if (!IsHereLoaded(pos)) { UnityEngine.Debug.Log(pos.x / Chunk.ChunkSize + ":" + pos.z / Chunk.ChunkSize + "未加载"); return; }
        if (GetBBlock(pos) != null)//此处有方块，暂时如下（按照无方块情况）处理，需判断是否液体/地雷方块
        {//如果是地雷方块则在放置后删除地雷，触发爆炸，扣除BBLOCK血量
            long chunkPos = Chunk.BlockPosToChunkPos(pos.x, pos.z);
            Vector3Int vi = Chunk.GlobalBPosToLocalBPos(pos);
            if (!activeChunk[chunkPos].CanPlaceAt(vi)) { UnityEngine.Debug.Log(vi + "不可放置,H="+ activeChunk[chunkPos].GetTH(vi.x,vi.z)); return; }//如果该chunk中此处不能放置
            B_Block bblock = new B_Block();
            bblock.mesh = typ; //LoaderManager.GetLoader<Loader_BuildBlock>().NameToIndex(typ);
            bblock.dir = dir;
            bblock.mat = tmat;
            bblock.mat2 = mat2;
            if (sub != 0) bblock.sub = sub;
            activeChunk[chunkPos].SetBBlock(vi, bblock);
            UnityEngine.Debug.Log(chunkPos + "update1");
            chunkObjPool.Get(chunkPos).GetComponent<ChunkObject>().ChunkBBlockUpdate(bblock, vi);//可考虑使用mvc连接，但有增加复杂性的风险

            Vector3Int temp = pos;
            temp.x += 1;
            if (BlockNotifyByBB(pos, temp, bblock)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByBB(pos, temp, bblock); }
            temp = pos; temp.x -= 1;
            if (BlockNotifyByBB(pos, temp, bblock)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByBB(pos, temp, bblock); }
            temp = pos; temp.z -= 1;
            if (BlockNotifyByBB(pos, temp, bblock)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByBB(pos, temp, bblock); }
            temp = pos; temp.z += 1;
            if (BlockNotifyByBB(pos, temp, bblock)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByBB(pos, temp, bblock); }
            temp = pos; temp.y -= 1;
            if (BlockNotifyByBB(pos, temp, bblock)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByBB(pos, temp, bblock); }
            temp = pos; temp.y += 1;
            if (BlockNotifyByBB(pos, temp, bblock)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByBB(pos, temp, bblock); }
        }
        else//此处无方块，可放置
        {

            long chunkPos = Chunk.BlockPosToChunkPos(pos.x,pos.z);
            Vector3Int vi = Chunk.GlobalBPosToLocalBPos(pos);
            if (!activeChunk[chunkPos].CanPlaceAt(vi)) { UnityEngine.Debug.Log("chunk"+chunkPos.GetX()+","+chunkPos.GetY()+":"+vi + "不可放置,H=" + activeChunk[chunkPos].GetTH(vi.x, vi.z)); return; }//如果该chunk中此处不能放置
            B_Block bblock = new B_Block();
            bblock.mesh = typ; //LoaderManager.GetLoader<Loader_BuildBlock>().NameToIndex(typ);
            bblock.dir = dir;
            bblock.mat = tmat;
            bblock.mat2 = 1;
            if (sub != 0) bblock.sub = sub;
            activeChunk[chunkPos].SetBBlock(vi, bblock);
            UnityEngine.Debug.Log("chunk:" + pos.x / Chunk.ChunkSize + "." + pos.z / Chunk.ChunkSize + ",block:" + vi + "update");
            chunkObjPool.Get(chunkPos).GetComponent<ChunkObject>().ChunkBBlockUpdate(bblock, vi);//表现层更新，可考虑使用mvc连接，但有增加复杂性的风险
                                                                                                 //需向上下左右前后6方向发送方块更新通知事件,先向自己发，没有方块再向container_eblock发
            Vector3Int temp = pos;
            temp.x += 1;
            if( BlockNotifyByBB(pos, temp, bblock)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByBB(pos, temp, bblock); }
            temp = pos; temp.x -=1;
            if (BlockNotifyByBB(pos, temp, bblock)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByBB(pos, temp, bblock); }
            temp = pos; temp.z -= 1;
            if (BlockNotifyByBB(pos, temp, bblock)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByBB(pos, temp, bblock); }
            temp = pos; temp.z += 1;
            if (BlockNotifyByBB(pos, temp, bblock)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByBB(pos, temp, bblock); }
            temp = pos; temp.y -= 1;
            if (BlockNotifyByBB(pos, temp, bblock)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByBB(pos, temp, bblock); }
            temp = pos; temp.y += 1;
            if (BlockNotifyByBB(pos, temp, bblock)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByBB(pos, temp, bblock); }
        }
    }
    /// <summary>
    /// pos是世界block坐标，0.0为原点
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="tb"></param>
    public void SetTBlock(Vector2Int pos, T_Block tb)
    {
        SetTBlock(pos, tb.H, tb.bio, tb.mat, tb.sub);

    }
    public void SetTBlock(Vector2Int pos, int h, T_BIOME bio, T_Material tmat, int sub = 0)
    {
        //如果是调高高度，检测目标高度到当前高度上有无方块,有则不允许更新
        //发送更新通知到源高度上的6格+目标高度上的6格
        int oldH = getBlockHeight(pos.x, pos.y);
        if (h > oldH)
        {
            int len = h - oldH;
            Vector3Int vec3 = new Vector3Int(pos.x,oldH, pos.y);
            for (int i = 1; i <= len; i++)
            {
                vec3.y = i + oldH;
                if(GetBBlock(vec3)!=null)
                {
                    return; 
                }
            }
        }


        Chunk c= GetChunkData(pos);
        Vector2Int v2i = Chunk.GlobalBPosToLocalBPos(pos);
        c.SetTData(v2i.x, v2i.y, h, bio, tmat, sub);

        Vector3Int oldPos = new Vector3Int(pos.x, oldH, pos.y);
        Vector3Int temp = new Vector3Int(pos.x, oldH, pos.y);
        T_Block source = c.GetTBlock(pos.x, pos.y);
        temp.x += 1;
        if (BlockNotifyByTB(oldPos, temp, source)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByTB(oldPos, temp, source); }
        temp = oldPos; temp.x -= 1;
        if (BlockNotifyByTB(oldPos, temp, source)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByTB(oldPos, temp, source); }
        temp = oldPos; temp.z -= 1;
        if (BlockNotifyByTB(oldPos, temp, source)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByTB(oldPos, temp, source); }
        temp = oldPos; temp.z += 1;
        if (BlockNotifyByTB(oldPos, temp, source)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByTB(oldPos, temp, source); }
        temp = oldPos; temp.y -= 1;
        if (BlockNotifyByTB(oldPos, temp, source)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByTB(oldPos, temp, source); }
        temp = oldPos; temp.y += 1;
        if (BlockNotifyByTB(oldPos, temp, source)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByTB(oldPos, temp, source); }

        Vector3Int newPos = new Vector3Int(pos.x, h, pos.y);
        temp = new Vector3Int(pos.x, h, pos.y);
        temp.x += 1;
        if (BlockNotifyByTB(oldPos, temp, source)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByTB(oldPos, temp, source); }
        temp = oldPos; temp.x -= 1;
        if (BlockNotifyByTB(oldPos, temp, source)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByTB(oldPos, temp, source); }
        temp = oldPos; temp.z -= 1;
        if (BlockNotifyByTB(oldPos, temp, source)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByTB(oldPos, temp, source); }
        temp = oldPos; temp.z += 1;
        if (BlockNotifyByTB(oldPos, temp, source)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByTB(oldPos, temp, source); }
        temp = oldPos; temp.y -= 1;
        if (BlockNotifyByTB(oldPos, temp, source)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByTB(oldPos, temp, source); }
        temp = oldPos; temp.y += 1;
        if (BlockNotifyByTB(oldPos, temp, source)) { ContainerManager.GetContainer<Container_EntityBlock>().OnBlockNotifyByTB(oldPos, temp, source); }
    }

    #region 方块更新
    /// <summary>
    /// 由tblock触发的方块更新通知
    /// pos是更新的源位置。这里只转发不处理，返回是否需要通知eblock
    /// </summary>
    /// <param name="pos">世界block坐标0.0</param>
    /// <param name="tb"></param>
    bool BlockNotifyByTB(Vector3Int pos, Vector3Int target, T_Block tb)
    {
        
        long global_chunkpos = Chunk.WorldPosToChunkPos(pos);
        long targetglobal_chunkpos = Chunk.WorldPosToChunkPos(target);
        if (!IscordInMap(global_chunkpos) || !IscordInMap(targetglobal_chunkpos)) return false;
        if (ChunkObjects.ContainsKey(targetglobal_chunkpos))//未加载的chunkobj先不通知
        {
            if (activeChunk[targetglobal_chunkpos].HaveBBlock(target))
            {
                ChunkObjects[targetglobal_chunkpos].GetComponent<ChunkObject>().BlockNotifyByTB(pos, target, tb);
                return false;
            }
            return true;
        }
        else return false;
    }
    bool BlockNotifyByBB(Vector3Int pos, Vector3Int target, B_Block bb)
    {
        
        long global_chunkpos = Chunk.WorldPosToChunkPos(pos);
        long targetglobal_chunkpos= Chunk.WorldPosToChunkPos(target);
        if (!IscordInMap(global_chunkpos) || !IscordInMap(targetglobal_chunkpos)) return false;
        if(ChunkObjects.ContainsKey(targetglobal_chunkpos))//未加载的chunkobj先不通知
        {
            if (activeChunk[targetglobal_chunkpos].HaveBBlock(target))
            { ChunkObjects[targetglobal_chunkpos].GetComponent<ChunkObject>().BlockNotifyByBB(pos, target, bb);
                return false;
            }
            return true;
        }
        else return false;
    }
    /// <summary>
    /// 由container_eblock更新调用
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="target"></param>
    /// <param name="eb"></param>
    /// <returns></returns>
    public bool BlockNotifyByEB(Vector3Int pos, Vector3Int target, Entity_Block eb)
    {//2*字典查找+1*getcomponnent（1*字典查找）
         long global_chunkpos = Chunk.WorldPosToChunkPos(pos);
        long targetglobal_chunkpos= Chunk.WorldPosToChunkPos(target);
        if (!IscordInMap(global_chunkpos) || !IscordInMap(targetglobal_chunkpos)) return false;
        if(ChunkObjects.ContainsKey(targetglobal_chunkpos))//未加载的chunkobj先不通知
        {
            if (activeChunk[targetglobal_chunkpos].HaveBBlock(target))
            { ChunkObjects[targetglobal_chunkpos].GetComponent<ChunkObject>().BlockNotifyByEB(pos, target, eb);
                return false;
            }
            return true;
        }
        else return false;
    }

    #endregion
    /// <summary>
    /// 未测试，可能有xy问题
    /// </summary>
    /// <param name="pos">世界坐标</param>
    /// <returns></returns>
    public int GetHeight(Vector3 pos)
    {
        return GetHeight(Chunk.WorldPosToBlockPos(pos));
    }
    public int GetHeight(Vector3Int pos) //全局方块坐标
    {
        /* if (IsHereLoaded(pos))
         {
             Vector3Int pos1 = pos;

             Vector3Int localPos = Chunk.GlobalBPosToLocalBPos(pos1);

             long chunkxy = Chunk.WorldPosToChunkPos(pos);//chunk坐标
             if (cachedBigChunk.ContainsKey(smallCordToBigCord(chunkxy)))
             {
                 Chunk[,] temp = cachedBigChunk[smallCordToBigCord(chunkxy)];
                 long insidexy = smallCordToIndexInsideFile(chunkxy);
                 int chunkx = insidexy.GetX();
                 int chunky = insidexy.GetY();
                 Chunk temp1 = temp[chunkx, chunky];
                 return temp1.tblocks[localPos.x, localPos.z].H;
             }
             return -65536;
         }
         return -65536;*/
        return getBlockHeight(pos.x, pos.z);
    }
    /// <summary>
    /// 此点是否是已加载chunk的上面
    /// </summary>
    /// <param name="pos">世界坐标</param>
    /// <returns></returns>
    public bool IsHereLoaded(Vector3 pos)
    {
        int x = Mathf.FloorToInt((pos.x / Chunk.BlockSize) / Chunk.ChunkSize);
        int z = Mathf.FloorToInt((pos.z / Chunk.BlockSize) / Chunk.ChunkSize);

        return isChunkActive(Chunk.WorldPosToChunkPos(pos));
    }
    public bool IsHereLoaded(Vector3Int v) { return isChunkActive(XYHelper.ToLongXY(v.x / Chunk.ChunkSize, v.z / Chunk.ChunkSize)); }//以方块坐标为参数
    public bool IsHereLoaded(long xy) { return isChunkActive(xy); }//以chunk坐标为参数

    /// <summary>
    /// 未测试x,y顺序，勿用
    /// 保存刚生成的整个地图的数据,地图中心非[0,0]
    /// </summary>
    /// <param name="data"></param>
    public void SaveMapAllChunkFile(Chunk[,] data)//generator生成后数据交到这里保存
    {

        //int bigx=0,bigy=0;
        int mapBigChunkSizeX = 2 * Mathf.CeilToInt(data.GetLength(0) * 1.0f / (chunkSizePerFile * 2));//从0开始的地图bigchunk大小
        int mapBigChunkSizeY = 2 * Mathf.CeilToInt(data.GetLength(1) * 1.0f / (chunkSizePerFile * 2));
        int globalChunkPos_GenPosOffsetX = (data.GetLength(0)) / 2;
        int globalChunkPos_GenPosOffsetY = (data.GetLength(1)) / 2;
        //Debug.Log(globalChunkPos_GenPosOffsetX + ":" + globalChunkPos_GenPosOffsetY);
        string a = center.GetParm<string>(nameof(EventNames.ThisSavePath));

        for (int bigx = 0; bigx < mapBigChunkSizeX; bigx++)
        {
            for (int bigy = 0; bigy < mapBigChunkSizeY; bigy++)
            {
                Chunk[,] temp = new Chunk[chunkSizePerFile, chunkSizePerFile];
                for (int i = 0; i < chunkSizePerFile; i++)
                {
                    for (int j = 0; j < chunkSizePerFile; j++)
                    {
                        int chunkGenPosX = (bigx - mapBigChunkSizeX / 2) * chunkSizePerFile + i;//当前bigchunk循环中chunk的全局坐标
                        int chunkGenPosY = (bigy - mapBigChunkSizeY / 2) * chunkSizePerFile + j;
                        long inArrayxy = smallCordToGenChunksCord(chunkGenPosX, chunkGenPosY, mapWidth, mapHeight);//转chunk生成坐标
                        chunkGenPosX = inArrayxy.GetX();
                        chunkGenPosY = inArrayxy.GetY();
                        //UnityEngine.Debug.Log(chunkGenPosX + "," + chunkGenPosY + "放在" + (bigx - mapBigChunkSizeX / 2) + "_" + (bigy - mapBigChunkSizeY / 2) + "文件的" + i + "," + j);
                        if (chunkGenPosX < data.GetLength(0) && (chunkGenPosY) < data.GetLength(1) && chunkGenPosX >= 0 && chunkGenPosY >= 0)//如果当前chunk在生成chunk[]中
                        {
                            temp[i, j] = data[chunkGenPosX, chunkGenPosY];
                            //Debug.Log(chunkGenPosX + "," + chunkGenPosY + "放在"+bigx+"_"+bigy+"文件的" + i + "," + j);
                        }
                    }

                }

                FileSaver.SetTerrainFile(a, bigx - mapBigChunkSizeX / 2, bigy - mapBigChunkSizeY / 2, JsonConvert.SerializeObject(temp, SerializerHelper.setting));
                //bigy+=1;
            }
            //bigx+=1;
        }

    }


    public Chunk GetChunkData(long xy)
    {
        if (IsHereLoaded(xy))
        {
            return activeChunk[xy];
        }
        else return null;
    }
    /// <summary>
    /// 根据0.0世界坐标返回chunk
    /// </summary>
    /// <param name="worldCoord"></param>
    /// <returns></returns>
    public Chunk GetChunkData(Vector3Int worldCoord)
    {
        long xy = Chunk.WorldPosToChunkPos(worldCoord);
        return GetChunkData(xy);
    }
    /// <summary>
    /// 根据0.0世界坐标返回chunk
    /// </summary>
    /// <param name="worldCoord"></param>
    /// <returns></returns>
    public Chunk GetChunkData(Vector2Int worldCoord)
    {
        long xy = Chunk.WorldPosToChunkPos(worldCoord);
        return GetChunkData(xy);
    }
    #endregion

    /// <summary>
    /// 根据chunk数据生成一个个chunkobj
    /// </summary>
    void genChunkObj()
    {

    }

    public override string ToString()//没有东西要额外储存
    {
        return base.ToString();
    }
}