using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public class MapWeather
{
    public int wet;
    public int cold;
    public int windy;

}
public enum MapForm
{
    island, islandGroup, costeLine, land,flat
}
public enum TerrainForm
{
    /// <summary>
    /// 山脉
    /// </summary>
    mountainRange,
    /// <summary>
    /// 平原
    /// </summary>
    flat,
    /// <summary>
    /// 一座山
    /// </summary>
    oneMountain
}
public enum TerrianRandomType
{
    none,
    /// <summary>
    /// 陨石型，随机挖球形坑
    /// </summary>
    metero,
    /// <summary>
    /// 随机凹凸一个圆柱
    /// </summary>
    bump
}

/// <summary>
/// 某个地图的地形地貌特征
/// </summary>
[System.Serializable]
public class InGameMapGenConfig
{
    public int width = 256, height = 256;//地图大小的方块数
    /// <summary>
    /// 海平面高度
    /// </summary>
    public int sea = -5;
    //public int size;
    /// <summary>
    /// 采用那种地形生成器
    /// </summary>
    public MapForm mform;
    public TerrainForm tform;
    public MapWeather weather;
    public TerrianRandomType ranType;//只会对地形高度进行加减
    


    public InGameMapGenConfig() { }
    public InGameMapGenConfig(int x, int y, MapForm fromm, TerrainForm fromt)
    {
        width = (x / Chunk.ChunkSize) * Chunk.ChunkSize;
        height = (y / Chunk.ChunkSize) * Chunk.ChunkSize;//限定xy为chunk大小的整数倍
        mform = fromm;
        tform = fromt;
    }
}
/// <summary>
/// 地形其上植被等生成规则，各种生物群系上某种物件的生成几率
/// </summary>
public class NatureGenRule
{
    //包括树、草、石、矿在各种biome下的生成几率
    //“在石头附近草的生成率上升”如何实现？只能继续分阶段，按矿、石、树、草的阶段生成eblock
    public float[,] rate;
    public string[] rowNames;
    public T_BIOME[] colBios;
    public string natureName;

}

/// <summary>
/// 只生成到水阶段，
/// </summary>
public class TerrainGenerator
{

    int width = 512, height = 512;//地图大小的方块数
    int genstep = 0;//当前生成到第几步

    
    ChunkFileWriter chunkFileWriter=new ChunkFileWriter();
    EBFileWriter eBFileWriter=new EBFileWriter();
    EntityFileWriter entityFileWriter=new EntityFileWriter();
    FieldWriter fieldWriter=new FieldWriter();

    TerrainHRandom thrandom=new TerrainHRandom();
    FieldGenerator fieldGenerator=new FieldGenerator();
    TerrainBiomeMatRandom tbrandom=new TerrainBiomeMatRandom();
    MainWaterBodyGenerator waterBodyGenerator=new MainWaterBodyGenerator();
    MapMetaGenerator metaGenerator=new MapMetaGenerator();//建筑群、建筑数据生成

    BuildingPrefabGenerator bpGenerator=new BuildingPrefabGenerator();

    NatureBuildingBlockGenerator bbgen=new NatureBuildingBlockGenerator();
    NatureEntityBlockGenerator ebgen=new NatureEntityBlockGenerator();
    NatureEntityGenerator egen=new NatureEntityGenerator();
    

    //只负责生成数据不负责存储地形
    public void OnEventRegist(EventCenter e)
    {
        //throw new NotImplementedException();
    }

    public void AfterEventRegist()
    {
        //throw new NotImplementedException();
    }
    public void Init(MapPrefabsData data)
    {//读取生成规则文件，这里是地图生成总入口，不要在这里读取生成规则
        Debug.Log(data.config.width + "," + data.config.height);
        chunkFileWriter.Init(data);
        fieldWriter.Init(data);
        eBFileWriter.Init(data);
        entityFileWriter.Init(data);

        fieldGenerator.Init(data,chunkFileWriter,fieldWriter,eBFileWriter,entityFileWriter);
        waterBodyGenerator.Init(data, chunkFileWriter, fieldWriter, eBFileWriter, entityFileWriter);
        metaGenerator.Init(data, chunkFileWriter, fieldWriter, eBFileWriter, entityFileWriter);
        bpGenerator.Init(data, chunkFileWriter, fieldWriter, eBFileWriter, entityFileWriter);
        bbgen.Init(data, chunkFileWriter, fieldWriter, eBFileWriter, entityFileWriter);
        ebgen.Init(data, chunkFileWriter, fieldWriter, eBFileWriter, entityFileWriter);
        egen.Init(data, chunkFileWriter, fieldWriter, eBFileWriter, entityFileWriter);
    }
    void DeInit()
    {
        chunkFileWriter.DeInit();
        fieldWriter.DeInit();
        eBFileWriter.DeInit();
        entityFileWriter.DeInit();

        fieldGenerator.DeInit();
        waterBodyGenerator.DeInit();
        metaGenerator.DeInit();
        bpGenerator.DeInit();
        bbgen.DeInit();
        ebgen.DeInit();
        egen.DeInit();
    }
    public Chunk[,] GenTerrain(MapPrefabsData data)//已进入关卡，开始生成地形数据
    {
        //
        //地形-
        //  基础perlin
        //  乘terrainform,mapform模板
        //biome生成
        // 生成metadata 确定建筑、矿物、重要节点位置
        // 生成道路
        // 生成树草等自然物体
        //生成建筑
        //生成固定npc
        Init(data);

        height = data.config.height;//块数
        width = data.config.width;

       /* EntityBlockFile[,] ebf; 不是本阶段要做的事
        ebf = new EntityBlockFile[data.config.width / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile), data.config.height / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile)];
        */

        Chunk[,] allChunk = new Chunk[width / Chunk.ChunkSize, height / Chunk.ChunkSize];
        for (int i = 0; i < width / Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < height / Chunk.ChunkSize; j++)
            {
                allChunk[i, j] = new Chunk();
            }
        }

        float[,] tblockHeight = thrandom.GetMapH(data);
        Debug.Log("H数组"+tblockHeight.GetLength(0) + ":" + tblockHeight.GetLength(1));
        chunkFileWriter.WriteMapH(tblockHeight);
        genstep++;

        fieldWriter.WriteSeaDistanceField(fieldGenerator.GenSeaDistance(tblockHeight, data));

        fieldWriter.WriteStepField(fieldGenerator.GenStepNew(tblockHeight, 4));//GenStep(allChunk);,临时将场数据当作局部变量存储，
        genstep++;
        //到时要让fieldwriter自己管理场数据

        fieldWriter.WriteWaterField(fieldGenerator.GenWaterField(tblockHeight, data,fieldWriter));
        genstep++;

        fieldWriter.WriteHeatField(fieldGenerator.GenHeatField(tblockHeight, data,fieldWriter));
        genstep++;

        fieldWriter.WriteMagicField(fieldGenerator.GenMagicField(tblockHeight, data,fieldWriter));
        genstep++;

        //FineTuneHight(tblockHeight,waters,heats,magics,data);
        genstep++;

       

        chunkFileWriter.WriteMapBio(tbrandom.GenBiome(width,height,data,fieldWriter,chunkFileWriter));
        genstep++;

        chunkFileWriter.WriteMapMat(tbrandom.GenMat(width, height, data, fieldWriter, chunkFileWriter));
        genstep++;

        waterBodyGenerator.GenMainWater();
        genstep++;

        //建筑群位置确定

        //根据建筑群位置生成具体建筑位置，平整地形

        //生成道路




        //生成自然植物（eblock）
        ebgen.Generate();

        //生成自然动物

        //eBFileWriter.w
        //GenBlockMaterial(allChunk, data);
        //GenWaterBlock(allChunk, step, waters, heats, magics, data);

        /*  不是本阶段要做的事
         *  GenLocation(allChunk, step, data);
          InnerBuildingPosi[] buildingPosi= GenBuildingLocation(allChunk, step, data);
          UnpackBuildingBBlock(allChunk, buildingPosi);
          UnpackBuildingEBlock(allChunk,ebf, buildingPosi);
          */
        //GenBuilding(allChunk, data);
        // GenNatureEBlocks(allChunk,ebf, step, waters, heats, magics, data);
        //SaveChunkFile(allChunk);

        DeInit();

        return allChunk;
    }




    
    Vector2[,] GenStep(Chunk[,] allChunk)//生成每个点高度的导数
    {
        Vector2[,] steps = new Vector2[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int iMinus1 = Mathf.Clamp(i - 1, 0, width - 1);
                int jMinus1 = Mathf.Clamp(j - 1, 0, height - 1);
                int iPlus1 = Mathf.Clamp(i + 1, 0, width - 1);
                int jPlus1 = Mathf.Clamp(j + 1, 0, height - 1);
                T_Block[] tt = new T_Block[9];//与邻接方块高度差
                try
                {
                    tt[0] = allChunk[iMinus1 / Chunk.ChunkSize, jMinus1 / Chunk.ChunkSize].tblocks[(int)Mathf.Repeat(iMinus1, Chunk.ChunkSize), (int)Mathf.Repeat(jMinus1, Chunk.ChunkSize)];
                    tt[1] = allChunk[iMinus1 / Chunk.ChunkSize, (j) / Chunk.ChunkSize].tblocks[iMinus1 % Chunk.ChunkSize, (j) % Chunk.ChunkSize];
                    tt[2] = allChunk[iMinus1 / Chunk.ChunkSize, jPlus1 / Chunk.ChunkSize].tblocks[iMinus1 % Chunk.ChunkSize, jPlus1 % Chunk.ChunkSize];
                    tt[3] = allChunk[(i) / Chunk.ChunkSize, jMinus1 / Chunk.ChunkSize].tblocks[(i) % Chunk.ChunkSize, jMinus1 % Chunk.ChunkSize];
                    tt[4] = allChunk[(i) / Chunk.ChunkSize, (j) / Chunk.ChunkSize].tblocks[(i) % Chunk.ChunkSize, (j) % Chunk.ChunkSize];
                    tt[5] = allChunk[(i) / Chunk.ChunkSize, jPlus1 / Chunk.ChunkSize].tblocks[(i) % Chunk.ChunkSize, jPlus1 % Chunk.ChunkSize];
                    tt[6] = allChunk[iPlus1 / Chunk.ChunkSize, jMinus1 / Chunk.ChunkSize].tblocks[iPlus1 % Chunk.ChunkSize, jMinus1 % Chunk.ChunkSize];
                    tt[7] = allChunk[iPlus1 / Chunk.ChunkSize, (j) / Chunk.ChunkSize].tblocks[iPlus1 % Chunk.ChunkSize, (j) % Chunk.ChunkSize];
                    tt[8] = allChunk[iPlus1 / Chunk.ChunkSize, jPlus1 / Chunk.ChunkSize].tblocks[iPlus1 % Chunk.ChunkSize, jPlus1 % Chunk.ChunkSize];

                }
                catch (Exception)
                {
                    Debug.Log("于减：" + iMinus1 / Chunk.ChunkSize + "," + jMinus1 / Chunk.ChunkSize + ",加：" + iPlus1 / Chunk.ChunkSize + "," + jPlus1 / Chunk.ChunkSize + "处出错");
                    throw;
                }

                float[] ft = new float[9];
                for (int k = 0; k < 9; k++)
                {
                    ft[k] = tt[k].H - tt[4].H;
                }

                Vector2 ans = Vector2.zero;
                Vector2[] temp = new Vector2[9]{new Vector2(-1,-1).normalized,-Vector2.up,new Vector2(-1,1).normalized,
                Vector2.left,Vector2.zero,-Vector2.left,
                new Vector2(1,-1).normalized,Vector2.up,new Vector2(1,1).normalized};
                for (int k = 0; k < 9; k++)
                {
                    ans += temp[k] * ft[k];
                }

                steps[i, j] = ans;
            }
        }
        return steps;
    }

   
    void FineTuneHight(float[,] highs,float[,] water,float[,] heat,Vector2[,] magic,MapPrefabsData data)
    {

    }

   
    void GenBlockMaterial(Chunk[,] allChunk, MapPrefabsData data)//生成方块具体的材质,后续生成建筑时还会修改，这里只考虑biome和高度影响下的材质
    {

    }
    void GenWaterBlock(Chunk[,] chunks,Vector2[,] step, float[,] water,float[,] heat,Vector2[,] magic, MapPrefabsData data)
    {

    }

    void GenLocation(Chunk[,] chunks,Vector2[,] step,MapPrefabsData data)//确定特殊点位置
    {

    }
    InnerBuildingPosi[] GenBuildingLocation(Chunk[,] chunks, Vector2[,] step, MapPrefabsData data)//确定建筑位置，平整地形
    {
        return null;
    }

    void UnpackBuildingBBlock(Chunk[,] allchunk,InnerBuildingPosi[] buildingPos)
    {

    }
    Dictionary<long,long> UnpackBuildingEBlock(Chunk[,] allchunk, EntityBlockFile[,] ebf, InnerBuildingPosi[] buildingPos)
    {
        //返回全局的eblock占位字典<pos,uuid>
        return null;
    }

    public void GenNatureEBlocks(Chunk[,] allChunk, EntityBlockFile[,] ebf, Vector2[,] step, float[,] water, float[,] heat, Vector2[,] magic, MapPrefabsData mpd)
    {
        
        //会生成eblock和eblockfile、保存eblockfile
        //按矿、石、树、草的顺序进行自然eblock生成
        for (int i = 0; i < allChunk.GetLength(0); i++)//矿
        {
            for (int j = 0; j < allChunk.GetLength(1); j++)
            {
                for (int m = 0; m < Chunk.ChunkSize; m++)
                {
                    for (int n = 0; n < Chunk.ChunkSize; n++)
                    {

                        switch (allChunk[i, j].tblocks[m, n].bio)
                        {
                            case T_BIOME.beach:
                                break;
                            case T_BIOME.grassland:
                                break;
                            case T_BIOME.mouthian:
                                break;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < allChunk.GetLength(0); i++)//石
        {
            for (int j = 0; j < allChunk.GetLength(1); j++)
            {
                for (int m = 0; m < Chunk.ChunkSize; m++)
                {
                    for (int n = 0; n < Chunk.ChunkSize; n++)
                    {

                        switch (allChunk[i, j].tblocks[m, n].bio)
                        {
                            case T_BIOME.beach:
                                break;
                            case T_BIOME.grassland:
                                break;
                            case T_BIOME.mouthian:
                                break;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < allChunk.GetLength(0); i++)//树
        {
            for (int j = 0; j < allChunk.GetLength(1); j++)
            {
                for (int m = 0; m < Chunk.ChunkSize; m++)
                {
                    for (int n = 0; n < Chunk.ChunkSize; n++)
                    {

                        switch (allChunk[i, j].tblocks[m, n].bio)
                        {
                            case T_BIOME.beach:
                                break;
                            case T_BIOME.grassland:
                                break;
                            case T_BIOME.mouthian:
                                break;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < allChunk.GetLength(0); i++)//草
        {
            for (int j = 0; j < allChunk.GetLength(1); j++)
            {
                for (int m = 0; m < Chunk.ChunkSize; m++)
                {
                    for (int n = 0; n < Chunk.ChunkSize; n++)
                    {

                        switch (allChunk[i, j].tblocks[m, n].bio)
                        {
                            case T_BIOME.beach:
                                break;
                            case T_BIOME.grassland:
                                break;
                            case T_BIOME.mouthian:
                                break;
                        }
                    }
                }
            }
        }

        ContainerManager.GetContainer<Container_EntityBlock>().SaveAllEBlockFile(ebf);
    }

[Obsolete]
    void GenBuilding(Chunk[,] allChunk, MapPrefabsData data)//建筑预制是一个eblockfile+dic<pos,bblock>+buildingconfig形式的类文件，其中buildingconfig是预制属性
    {//根据预设建筑选定位置，可能会改写地形高度和材质、群系。写入mapprefabsdata,写入mapprefabsdata文件
     //可能会移除、改变一些自然eblock

        if(data.metaDatas!=null)
        foreach (var item in data.metaDatas)
        {

        }

    }
    void SaveChunkFile(Chunk[,] c)
    {
        ContainerManager.GetContainer<Container_Terrain>().SaveMapAllChunkFile(c);
    }

    /// <summary>
    /// 内部类，用以标识某个将要生成的具体建筑的位置
    /// </summary>
    class InnerBuildingPosi
    {
        public int type;
        public Vector3Int worldPos;//中心的世界坐标
        public DIR dir;
    }
}