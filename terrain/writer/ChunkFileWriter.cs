using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class BaseChunkWriter
{
    protected int maxWid, maxHig;
    protected int chunkCacheLimit = 1024;
    public bool LimitCacheSize;
    protected string saveName;
    public virtual void Init(MapPrefabsData data)
    {
        maxWid = data.config.width;
        maxHig = data.config.height;
        this.saveName = EventCenter.WorldCenter.GetParm<string>(nameof(EventNames.ThisSavePath));
    }
    public virtual void DeInit()
    {
        
    }
}

public class ChunkFileWriter:BaseChunkWriter
{
    //只在地图数据生成阶段使用
    //内部缓存chunkfile，要限定缓存大小
    //需提供获取block数据方法,访问到没加载的要自行进行加载，访问到没生成的则要自行生成
    //提供获取地图格tag的方法，地图格tag文件是与chunkfile同步加卸载的
   
    Dictionary<long, Chunk> chunkCache = new Dictionary<long, Chunk>();//访问是以小chunk坐标进行，读取保存是以大chunk坐标进行
    Queue<long> bigChunkLoadOrder = new Queue<long>();//chunkfile加载进来的顺序

    public override void DeInit()
    {
        base.DeInit();
        SaveAllChunkFile();
    }

    /// <summary>
    /// 根据大chunk坐标检查是否有此chunk文件
    /// </summary>
    /// <param name="x">以0为地图原点的x</param>
    /// <param name="y"></param>
    /// <returns></returns>
    bool ContainChunkFile(int x, int y)
    {
        return FileSaver.HasTerrainFile(saveName, x, y);
    }
    bool ContainChunkFile(long xy)
    {
        return FileSaver.HasTerrainFile(saveName, xy.GetX(), xy.GetY());
    }
    void SaveLastChunkFile()
    {
        long bigxy = bigChunkLoadOrder.Dequeue();
        int bigx = bigxy.GetX();
        int bigy = bigxy.GetY();
        Debug.Log("保存" + bigx + "," + bigy + "chunk");
        Chunk[,] temp = new Chunk[Container_Terrain.chunkSizePerFile, Container_Terrain.chunkSizePerFile];
        for (int i = 0; i < Container_Terrain.chunkSizePerFile; i++)
        {
            for (int j = 0; j < Container_Terrain.chunkSizePerFile; j++)
            {
                if (chunkCache.ContainsKey(XYHelper.ToLongXY(bigx * Container_Terrain.chunkSizePerFile + i, bigy * Container_Terrain.chunkSizePerFile + j)))
                {
                    temp[i, j] = chunkCache[XYHelper.ToLongXY(bigx * Container_Terrain.chunkSizePerFile + i, bigy * Container_Terrain.chunkSizePerFile + j)];
                    chunkCache.Remove(XYHelper.ToLongXY(bigx * Container_Terrain.chunkSizePerFile + i, bigy * Container_Terrain.chunkSizePerFile + j));
                }
            }
        }

        FileSaver.SetTerrainFile(saveName, bigx, bigy, JsonConvert.SerializeObject(temp, SerializerHelper.setting));
    }
    void SaveAllChunkFile()
    {
        int a = bigChunkLoadOrder.Count;
        for (int i = 0; i < a; i++)
        {
            SaveLastChunkFile();
        }
        
    }
    /// <summary>
    /// 以0.0为原点
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    void LoadNewChunksFormFile(int x, int y)//大坐标
    {
        try
        {
            Debug.Log("尝试从" + x + "," + y + "读取chunk");
            string s = FileSaver.GetTerrainFile(saveName, x, y);
            Chunk[,] temp = JsonConvert.DeserializeObject<Chunk[,]>(s);
            bigChunkLoadOrder.Enqueue(XYHelper.ToLongXY(x, y));
            for (int i = 0; i < Container_Terrain.chunkSizePerFile; i++)
            {
                for (int j = 0; j < Container_Terrain.chunkSizePerFile; j++)
                {
                    chunkCache.Add(XYHelper.ToLongXY(x * Container_Terrain.chunkSizePerFile + i, y * Container_Terrain.chunkSizePerFile + j), temp[i, j]);
                }
            }
        }
        catch (System.Exception)
        {

            throw;
        }


    }
    void LoadNewChunksFormFile(long xy)//大坐标
    {
        LoadNewChunksFormFile(xy.GetX(), xy.GetY());
    }
    /// <summary>
    /// 以0.0为原点
    /// </summary>
    /// <param name="xy"></param>
    void CreateNewChunkFile(long xy)//大坐标
    {
        bigChunkLoadOrder.Enqueue(xy);
        int bigx = xy.GetX();
        int bigy = xy.GetY();
        Debug.Log("创建" + bigx + "," + bigy+"chunk");

        Chunk[,] temp = new Chunk[Container_Terrain.chunkSizePerFile, Container_Terrain.chunkSizePerFile];
        for (int i = 0; i < Container_Terrain.chunkSizePerFile; i++)
        {
            for (int j = 0; j < Container_Terrain.chunkSizePerFile; j++)
            {
                temp[i, j] = new Chunk();//还需加判断：如果该chunk在地图外则无需初始化
                temp[i, j].Init();
                chunkCache.Add(XYHelper.ToLongXY(bigx * Container_Terrain.chunkSizePerFile + i, bigy * Container_Terrain.chunkSizePerFile + j), temp[i, j]);
            }
        }
    }


    /// <summary>
    /// 根据地图0点为中心的chunk坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    Chunk GetChunkBySmallChunkCoord(int x, int y)
    {
        long xy = XYHelper.ToLongXY(x, y);
        long bigxy = Container_Terrain.smallCordToBigCord(xy);
        if (chunkCache.ContainsKey(xy))
        {
            return chunkCache[xy];
        }
        else
        {
            if (ContainChunkFile(bigxy))
            {
                //加载chunk文件，如果活动chunk已经到达上限则先保存最早加载进来的chunk
                if (chunkCache.Count >= chunkCacheLimit && LimitCacheSize)
                {
                    //保存最早加载进来的chunkfile
                    SaveLastChunkFile();
                }

                //加载chunkfile
                LoadNewChunksFormFile(bigxy);

                return chunkCache[xy];
            }
            else
            {
                if (chunkCache.Count >= chunkCacheLimit && LimitCacheSize)
                {
                    //保存最早加载进来的chunkfile
                    SaveLastChunkFile();
                }

                //生成一个空的chunkile到内存
                CreateNewChunkFile(bigxy);

                return chunkCache[xy];

            }
        }
    }


    #region 写入方法

    public void WriteMapH(float[,] h)
    {
        int wid = h.GetLength(0);
        int hig = h.GetLength(1);

        if (wid > maxWid || hig > maxHig) { Debug.Log("传入wid,hig太大"+maxWid+":"+maxHig); return; }
        if (wid <= 0 || hig <= 0) { Debug.Log("传入wid,hig太小" + wid + ":" + hig); return; }

        int chunkFileWid = wid / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);
        int chunkFileHig = hig / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);

        int chunkposOffsetX = wid/ Chunk.ChunkSize / 2;//只要地图总大小不小于64*64则不会出问题
        int chunkposOffsetY = hig/ Chunk.ChunkSize / 2;//地图中心到边缘的chunk数

        Debug.Log(chunkFileWid + "," + chunkFileWid);

        for (int i = -Mathf.CeilToInt(chunkFileWid*1.0f / 2); i < Mathf.CeilToInt(chunkFileWid * 1.0f / 2); i++)//大chunk坐标
        {
            for (int j = -Mathf.CeilToInt(chunkFileHig*1.0f / 2); j < Mathf.CeilToInt(chunkFileHig * 1.0f / 2); j++)
            {
                for (int k = 0; k < Container_Terrain.chunkSizePerFile; k++)//smallchunk遍历
                {
                    for (int o = 0; o < Container_Terrain.chunkSizePerFile; o++)
                    {
                        int chunkposX = i * Container_Terrain.chunkSizePerFile + k;//以0.0为原点的chunk坐标
                        int chunkposY = j * Container_Terrain.chunkSizePerFile + o;
                        if (chunkposX >= chunkposOffsetX || chunkposY >= chunkposOffsetY|| chunkposX < -chunkposOffsetX || chunkposY < -chunkposOffsetY) continue;
                        //Debug.Log(chunkposX + "," + chunkposY);
                        Chunk temp = GetChunkBySmallChunkCoord(chunkposX, chunkposY);
                        for (int p = 0; p < Chunk.ChunkSize; p++)
                        {
                            for (int q = 0; q < Chunk.ChunkSize; q++)
                            {
                                try
                                {
                                    temp.SetTH(p,q, (int)h[(chunkposX+chunkposOffsetX) * Chunk.ChunkSize + p, (chunkposY+ chunkposOffsetY) * Chunk.ChunkSize + q]);
                                }
                                catch (System.Exception)
                                {
                                    Debug.Log(chunkposX + "," + chunkposY);
                                    Debug.Log(p + "," + q);
                                    Debug.Log(chunkposOffsetX + "," + chunkposOffsetY);
                                    Debug.Log(((chunkposX + chunkposOffsetX) * Chunk.ChunkSize +  + p) + "," + ((chunkposY+ chunkposOffsetY) * Chunk.ChunkSize  + q));
                                    throw;
                                }

                            }
                        }
                    }
                }
            }
        }
        SaveAllChunkFile();
    }
    
    public void WriteMapBio(T_BIOME[,] bio)
    {
       

        if(bio==null)return;
        int wid = bio.GetLength(0);
        int hig = bio.GetLength(1);

        if (wid > maxWid || hig > maxHig) return;
        if (wid <= 0 || hig <= 0) return;

        int chunkFileWid = wid / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);
        int chunkFileHig = hig / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);

        int chunkposOffsetX = wid / Chunk.ChunkSize / 2;
        int chunkposOffsetY = hig / Chunk.ChunkSize / 2;

        for (int i = -Mathf.CeilToInt(chunkFileWid * 1.0f / 2); i < Mathf.CeilToInt(chunkFileWid * 1.0f / 2); i++)//大chunk坐标
        {
            for (int j = -Mathf.CeilToInt(chunkFileHig * 1.0f / 2); j < Mathf.CeilToInt(chunkFileHig * 1.0f / 2); j++)
            {
                for (int k = 0; k < Container_Terrain.chunkSizePerFile; k++)//smallchunk遍历
                {
                    for (int o = 0; o < Container_Terrain.chunkSizePerFile; o++)
                    {
                        int chunkposX = i * Container_Terrain.chunkSizePerFile + k;
                        int chunkposY = j * Container_Terrain.chunkSizePerFile + o;
                        if (chunkposX >= chunkposOffsetX || chunkposY >= chunkposOffsetY || chunkposX < -chunkposOffsetX || chunkposY < -chunkposOffsetY) continue;
                        Chunk temp = GetChunkBySmallChunkCoord(chunkposX, chunkposY);
                        for (int p = 0; p < Chunk.ChunkSize; p++)
                        {
                            for (int q = 0; q < Chunk.ChunkSize; q++)
                            {
                                try
                                {
                                    temp.SetTBio(p,q, bio[(chunkposX + chunkposOffsetX) * Chunk.ChunkSize + p, (chunkposY + chunkposOffsetY) * Chunk.ChunkSize + q]);
                                    
                                }
                                catch (System.Exception)
                                {
                                    Debug.Log(chunkposX + "," + chunkposY);
                                    Debug.Log(p + "," + q);
                                    Debug.Log(chunkposOffsetX + "," + chunkposOffsetY);
                                    Debug.Log(((chunkposX + chunkposOffsetX) * Chunk.ChunkSize + +p) + "," + ((chunkposY + chunkposOffsetY) * Chunk.ChunkSize + q));
                                    throw;
                                }

                            }
                        }
                    }
                }
            }
        }
        SaveAllChunkFile();
    }

    public void WriteMapMat(T_Material[,] mats)
    {
        if(mats==null)return; 
        int wid = mats.GetLength(0);
        int hig = mats.GetLength(1);

        if (wid > maxWid || hig > maxHig) return;
        if (wid <= 0 || hig <= 0) return;

        int chunkFileWid = wid / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);
        int chunkFileHig = hig / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);

        int chunkposOffsetX = wid / Chunk.ChunkSize / 2;
        int chunkposOffsetY = hig / Chunk.ChunkSize / 2;

        for (int i = -Mathf.CeilToInt(chunkFileWid * 1.0f / 2); i < Mathf.CeilToInt(chunkFileWid * 1.0f / 2); i++)//大chunk坐标
        {
            for (int j = -Mathf.CeilToInt(chunkFileHig * 1.0f / 2); j < Mathf.CeilToInt(chunkFileHig * 1.0f / 2); j++)
            {
                for (int k = 0; k < Container_Terrain.chunkSizePerFile; k++)//smallchunk遍历
                {
                    for (int o = 0; o < Container_Terrain.chunkSizePerFile; o++)
                    {
                        int chunkposX = i * Container_Terrain.chunkSizePerFile + k;
                        int chunkposY = j * Container_Terrain.chunkSizePerFile + o;
                        if (chunkposX >= chunkposOffsetX || chunkposY >= chunkposOffsetY || chunkposX < -chunkposOffsetX || chunkposY < -chunkposOffsetY) continue;
                        Chunk temp = GetChunkBySmallChunkCoord(chunkposX, chunkposY);
                        for (int p = 0; p < Chunk.ChunkSize; p++)
                        {
                            for (int q = 0; q < Chunk.ChunkSize; q++)
                            {
                                try
                                {
                                    temp.SetTMat(p,q,mats[(chunkposX + chunkposOffsetX) * Chunk.ChunkSize + p, (chunkposY + chunkposOffsetY) * Chunk.ChunkSize + q]);
                                }
                                catch (System.Exception)
                                {
                                    Debug.Log(chunkposX + "," + chunkposY);
                                    Debug.Log(p + "," + q);
                                    Debug.Log(chunkposOffsetX + "," + chunkposOffsetY);
                                    Debug.Log(((chunkposX + chunkposOffsetX) * Chunk.ChunkSize + +p) + "," + ((chunkposY + chunkposOffsetY) * Chunk.ChunkSize + q));
                                    throw;
                                }

                            }
                        }
                    }
                }
            }
        }
        SaveAllChunkFile();
    }
    
    public void AddBBlockAt(int x,int y ,Dictionary<int,B_Block> data)
    {
        if (data == null) return;
        if (data.Count <= 0) return;

        int chunkposOffsetX = maxWid / Chunk.ChunkSize / 2;//只要地图总大小不小于64*64则不会出问题
        int chunkposOffsetY = maxHig / Chunk.ChunkSize / 2;//地图中心到边缘的chunk数

        if (x >= chunkposOffsetX || y >= chunkposOffsetY) return;
        if (x < -chunkposOffsetX || y <-chunkposOffsetY) return;

        Chunk temp = GetChunkBySmallChunkCoord(x, y);
        foreach (var item in data)
        {
            if(temp.CanPlaceAt(item.Key))
            {
                temp.SetBBlock(item.Key, item.Value);
            }
        }
    }
    public void ResetBBlockAt(int x, int y, Dictionary<int, B_Block> data)
    {
        int chunkposOffsetX = maxWid / Chunk.ChunkSize / 2;//只要地图总大小不小于64*64则不会出问题
        int chunkposOffsetY = maxHig / Chunk.ChunkSize / 2;//地图中心到边缘的chunk数

        if (x >= chunkposOffsetX || y >= chunkposOffsetY) return;
        if (x < -chunkposOffsetX || y < -chunkposOffsetY) return;

        Chunk temp = GetChunkBySmallChunkCoord(x, y);
        temp.SetBBlocks(data);

    }
    #endregion
    #region 读取方法
    /// <summary>
    /// 参数是以0.0为原点的chunk坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int[,] GetChunkH(int x, int y)
    {
        if (x >= (maxWid / Chunk.ChunkSize / 2) || y >= (maxHig / Chunk.ChunkSize / 2)) return null;
        if (x < -(maxWid / Chunk.ChunkSize / 2) || y < -(maxHig / Chunk.ChunkSize / 2)) return null;
        int[,] h = new int[Chunk.ChunkSize, Chunk.ChunkSize];
        Chunk temp = GetChunkBySmallChunkCoord(x, y);
        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                h[i, j] = temp.tblocks[i, j].H;
            }
        }
        return h;
    }
    /// <summary>
    /// 参数是以0.0为原点的chunk坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public T_BIOME[,] GetChunkBio(int x, int y)
    {
        if (x >= (maxWid / Chunk.ChunkSize / 2) || y >= (maxHig / Chunk.ChunkSize / 2)) return null;
        if (x < -(maxWid / Chunk.ChunkSize / 2) || y < -(maxHig / Chunk.ChunkSize / 2)) return null;
        T_BIOME[,] bio = new T_BIOME[Chunk.ChunkSize, Chunk.ChunkSize];
        Chunk temp = GetChunkBySmallChunkCoord(x, y);
        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                bio[i, j] = temp.tblocks[i, j].bio;
            }
        }
        return bio;
    }
    /// <summary>
    /// 参数是以0.0为原点的chunk坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public T_Material[,] GetChunkMat(int x, int y)
    {
        if (x >= (maxWid / Chunk.ChunkSize / 2) || y >= (maxHig / Chunk.ChunkSize / 2)) return null;
        if (x < -(maxWid / Chunk.ChunkSize / 2) || y < -(maxHig / Chunk.ChunkSize / 2)) return null;
        T_Material[,] mat = new T_Material[Chunk.ChunkSize, Chunk.ChunkSize];
        Chunk temp = GetChunkBySmallChunkCoord(x, y);
        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                mat[i, j] = temp.tblocks[i, j].mat;
            }
        }
        return mat;
    }
    #endregion
}
/// <summary>
/// 计划有分块写入功能的writer都使用此类,目前各种writer无法抽象出共同点,先不用
///chunk的分块写入与float等的分块写入有本质不同，不可混用
/// </summary>
/// <typeparam name="T"></typeparam>
class DataChunkLoader<T>
{
    public delegate void WriteArray<P>(P[,] data);
    public delegate void Wirte<T,P>(P dat);
    public delegate void CreateNewDataChunkFile(int x,int y);
    public delegate string LoadNewDataChunksFormFile(int x,int y);
    public delegate void SaveDataChunkFile(string saveName,string data, int x,int y);
    public delegate  bool ContainDataChunkFile(int x, int y);

    CreateNewDataChunkFile createNewDataChunkFile;
    LoadNewDataChunksFormFile loadNewDataChunksFormFile;
    SaveDataChunkFile saveDataChunkFile;
    ContainDataChunkFile containDataChunkFile;

    int maxWid, maxHig;
    int chunkCacheLimit = 1024;
    Dictionary<long, T> chunkCache = new Dictionary<long, T>();//访问是以小chunk坐标进行，读取保存是以大chunk坐标进行
    Queue<long> bigChunkLoadOrder = new Queue<long>();//chunkfile加载进来的顺序

    public bool LimitCacheSize;
    string saveName;
    public void Init(MapPrefabsData data)
    {
        maxWid = data.config.width;
        maxHig = data.config.height;
        this.saveName = EventCenter.WorldCenter.GetParm<string>(nameof(EventNames.ThisSavePath));
    }
    void SaveLastChunkFile()
    {
        long bigxy = bigChunkLoadOrder.Dequeue();
        int bigx = bigxy.GetX();
        int bigy = bigxy.GetY();
        Debug.Log("保存" + bigx + "," + bigy + "chunk");
        T[,] temp = new T[Container_Terrain.chunkSizePerFile, Container_Terrain.chunkSizePerFile];
        for (int i = 0; i < Container_Terrain.chunkSizePerFile; i++)
        {
            for (int j = 0; j < Container_Terrain.chunkSizePerFile; j++)
            {
                if (chunkCache.ContainsKey(XYHelper.ToLongXY(bigx * Container_Terrain.chunkSizePerFile + i, bigy * Container_Terrain.chunkSizePerFile + j)))
                {
                    temp[i, j] = chunkCache[XYHelper.ToLongXY(bigx * Container_Terrain.chunkSizePerFile + i, bigy * Container_Terrain.chunkSizePerFile + j)];
                    chunkCache.Remove(XYHelper.ToLongXY(bigx * Container_Terrain.chunkSizePerFile + i, bigy * Container_Terrain.chunkSizePerFile + j));
                }
            }
        }

        saveDataChunkFile(saveName, JsonConvert.SerializeObject(temp, SerializerHelper.setting), bigx, bigy);
        //FileSaver.SetTerrainFile(saveName, bigx, bigy, JsonConvert.SerializeObject(temp, SerializerHelper.setting));
    }

    void SaveAllChunkFile()
    {
        int a = bigChunkLoadOrder.Count;
        for (int i = 0; i < a; i++)
        {
            SaveLastChunkFile();
        }

    }

    T GetChunkBySmallChunkCoord(int x, int y)
    {
        long xy = XYHelper.ToLongXY(x, y);
        long bigxy = Container_Terrain.smallCordToBigCord(xy);
        if (chunkCache.ContainsKey(xy))
        {
            return chunkCache[xy];
        }
        else
        {
            if (containDataChunkFile(bigxy.GetX(),bigxy.GetY()))
            {
                //加载chunk文件，如果活动chunk已经到达上限则先保存最早加载进来的chunk
                if (chunkCache.Count >= chunkCacheLimit && LimitCacheSize)
                {
                    //保存最早加载进来的chunkfile
                    SaveLastChunkFile();
                }

                //加载chunkfile
                loadNewDataChunksFormFile(bigxy.GetX(),bigxy.GetY());

                return chunkCache[xy];
            }
            else
            {
                if (chunkCache.Count >= chunkCacheLimit && LimitCacheSize)
                {
                    //保存最早加载进来的chunkfile
                    SaveLastChunkFile();
                }

                //生成一个空的chunkile到内存
                createNewDataChunkFile(bigxy.GetX(),bigxy.GetY());

                return chunkCache[xy];

            }
        }
    }
    public T GetByChunkCoord(int x,int y)
    {
        return chunkCache[XYHelper.ToLongXY(x, y)];
    }
}