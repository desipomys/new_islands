using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class EBFileWriter :BaseChunkWriter
{
    //内部缓存eblockfile，要限定缓存大小
    //需提供获取eblock数据方法,访问到没加载的要自行进行加载，

    //待测试
    Dictionary<long,EntityBlockFile> bigchunkEBFCache=new Dictionary<long, EntityBlockFile>();//bigchunk坐标与ebf的对应，0.0为原点
    Queue<long> bigChunkLoadOrder = new Queue<long>();//bigchunkfile加载进来的顺序
   
     public override void Init(MapPrefabsData data)
    {
        base.Init(data);
        this.chunkCacheLimit=64;
    }

   /// <summary>
    /// 根据大chunk坐标检查是否有此chunk文件
    /// </summary>
    /// <param name="x">以0为地图原点的x</param>
    /// <param name="y"></param>
    /// <returns></returns>
    bool ContainEBFile(int x, int y)
    {
        return FileSaver.HasEBlockFile(saveName, x, y);
    }
    bool ContainEBFile(long xy)
    {
        return FileSaver.HasEBlockFile(saveName, xy.GetX(), xy.GetY());
    }
    void SaveLastEBFile()
    {
        long bigxy = bigChunkLoadOrder.Dequeue();
        int bigx = bigxy.GetX();
        int bigy = bigxy.GetY();
        Debug.Log("保存" + bigx + "," + bigy + "chunk");
        EntityBlockFile temp = new EntityBlockFile();
         if (bigchunkEBFCache.ContainsKey(XYHelper.ToLongXY(bigx , bigy )))
            {
                temp = bigchunkEBFCache[XYHelper.ToLongXY(bigx , bigy )];
                bigchunkEBFCache.Remove(XYHelper.ToLongXY(bigx , bigy ));
            }

        FileSaver.SetEBlockFile(saveName, bigx, bigy, JsonConvert.SerializeObject(temp, SerializerHelper.setting));
    }
    void SaveAllEBlockFile()
    {
        int a = bigChunkLoadOrder.Count;
        for (int i = 0; i < a; i++)
        {
            SaveLastEBFile();
        }
        
    }
    /// <summary>
    /// 以0.0为原点
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    void LoadNewEBFileFormFile(int x, int y)//大坐标
    {
        try
        {
            Debug.Log("尝试从" + x + "," + y + "读取eblockfile");
            string s = FileSaver.GetTerrainFile(saveName, x, y);
            EntityBlockFile temp = JsonConvert.DeserializeObject<EntityBlockFile>(s);
            bigChunkLoadOrder.Enqueue(XYHelper.ToLongXY(x, y));
            bigchunkEBFCache.Add(XYHelper.ToLongXY(x, y ), temp);
           
        }
        catch (System.Exception)
        {

            throw;
        }


    }
    void LoadNewEBFileFormFile(long xy)//大坐标
    {
        LoadNewEBFileFormFile(xy.GetX(), xy.GetY());
    }
    /// <summary>
    /// 以0.0为原点
    /// </summary>
    /// <param name="xy"></param>
    void CreateNewEBlockFile(long xy)//大坐标
    {
        bigChunkLoadOrder.Enqueue(xy);
        int bigx = xy.GetX();
        int bigy = xy.GetY();
        Debug.Log("创建" + bigx + "," + bigy+"ebfile");

        EntityBlockFile temp = new EntityBlockFile();

        bigchunkEBFCache.Add(XYHelper.ToLongXY(bigx , bigy ), temp);
            
    }


    /// <summary>
    /// 根据地图0点为中心的bigchunk坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    EntityBlockFile GetEBFileByBigChunkCoord(int x, int y)
    {
        long xy = XYHelper.ToLongXY(x, y);
        long bigxy = xy;
        if (bigchunkEBFCache.ContainsKey(xy))
        {
            return bigchunkEBFCache[xy];
        }
        else
        {
            if (ContainEBFile(bigxy))
            {
                //加载chunk文件，如果活动chunk已经到达上限则先保存最早加载进来的chunk
                if (bigchunkEBFCache.Count >= chunkCacheLimit && LimitCacheSize)
                {
                    //保存最早加载进来的chunkfile
                    SaveLastEBFile();
                }

                //加载chunkfile
                LoadNewEBFileFormFile(bigxy);

                return bigchunkEBFCache[xy];
            }
            else
            {
                if (bigchunkEBFCache.Count >= chunkCacheLimit && LimitCacheSize)
                {
                    //保存最早加载进来的chunkfile
                    SaveLastEBFile();
                }

                //生成一个空的chunkile到内存
                CreateNewEBlockFile(bigxy);

                return bigchunkEBFCache[xy];

            }
        }
    }

    public override void DeInit()
    {
        base.DeInit();
        SaveAllEBlockFile();
    }

    #region 写入
    /// <summary>
    /// 参数是以0.0为原点的bigchunk坐标
    /// </summary>
    /// <param name="bigchunkx"></param>
    /// <param name="bigchunky"></param>
    /// <param name="ebfile"></param>
    public void WirteEBAt(int bigchunkx,int bigchunky,EntityBlockFile ebfile)
    {   
        long xy=XYHelper.ToLongXY(bigchunkx,bigchunky);
        if(bigchunkEBFCache.ContainsKey(xy))//未测试bigChunkLoadOrder中有而bigchunkEBFCache中无的情况
        {
            bigchunkEBFCache[xy]=ebfile;
        }
        else
        {
            
            bigChunkLoadOrder.Enqueue(xy);
            bigchunkEBFCache.Add(xy,ebfile);
        }
    }
    #endregion

    #region 读取

    #endregion
}
