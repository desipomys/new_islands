using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class EBFileWriter :BaseChunkWriter
{
    //�ڲ�����eblockfile��Ҫ�޶������С
    //���ṩ��ȡeblock���ݷ���,���ʵ�û���ص�Ҫ���н��м��أ�

    //������
    Dictionary<long,EntityBlockFile> bigchunkEBFCache=new Dictionary<long, EntityBlockFile>();//bigchunk������ebf�Ķ�Ӧ��0.0Ϊԭ��
    Queue<long> bigChunkLoadOrder = new Queue<long>();//bigchunkfile���ؽ�����˳��
   
     public override void Init(MapPrefabsData data)
    {
        base.Init(data);
        this.chunkCacheLimit=64;
    }

   /// <summary>
    /// ���ݴ�chunk�������Ƿ��д�chunk�ļ�
    /// </summary>
    /// <param name="x">��0Ϊ��ͼԭ���x</param>
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
        Debug.Log("����" + bigx + "," + bigy + "chunk");
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
    /// ��0.0Ϊԭ��
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    void LoadNewEBFileFormFile(int x, int y)//������
    {
        try
        {
            Debug.Log("���Դ�" + x + "," + y + "��ȡeblockfile");
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
    void LoadNewEBFileFormFile(long xy)//������
    {
        LoadNewEBFileFormFile(xy.GetX(), xy.GetY());
    }
    /// <summary>
    /// ��0.0Ϊԭ��
    /// </summary>
    /// <param name="xy"></param>
    void CreateNewEBlockFile(long xy)//������
    {
        bigChunkLoadOrder.Enqueue(xy);
        int bigx = xy.GetX();
        int bigy = xy.GetY();
        Debug.Log("����" + bigx + "," + bigy+"ebfile");

        EntityBlockFile temp = new EntityBlockFile();

        bigchunkEBFCache.Add(XYHelper.ToLongXY(bigx , bigy ), temp);
            
    }


    /// <summary>
    /// ���ݵ�ͼ0��Ϊ���ĵ�bigchunk����
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
                //����chunk�ļ�������chunk�Ѿ������������ȱ���������ؽ�����chunk
                if (bigchunkEBFCache.Count >= chunkCacheLimit && LimitCacheSize)
                {
                    //����������ؽ�����chunkfile
                    SaveLastEBFile();
                }

                //����chunkfile
                LoadNewEBFileFormFile(bigxy);

                return bigchunkEBFCache[xy];
            }
            else
            {
                if (bigchunkEBFCache.Count >= chunkCacheLimit && LimitCacheSize)
                {
                    //����������ؽ�����chunkfile
                    SaveLastEBFile();
                }

                //����һ���յ�chunkile���ڴ�
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

    #region д��
    /// <summary>
    /// ��������0.0Ϊԭ���bigchunk����
    /// </summary>
    /// <param name="bigchunkx"></param>
    /// <param name="bigchunky"></param>
    /// <param name="ebfile"></param>
    public void WirteEBAt(int bigchunkx,int bigchunky,EntityBlockFile ebfile)
    {   
        long xy=XYHelper.ToLongXY(bigchunkx,bigchunky);
        if(bigchunkEBFCache.ContainsKey(xy))//δ����bigChunkLoadOrder���ж�bigchunkEBFCache���޵����
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

    #region ��ȡ

    #endregion
}
