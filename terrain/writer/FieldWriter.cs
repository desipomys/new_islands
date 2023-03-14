using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地形生成过程中产生的tag
/// </summary>
public enum TerrainGenBlockTag
{
    
}

class AllField
{
    public float[,] waters;
    public float[,] heats;
    public Vector2[,] step;
    public Vector2[,] magics;
    public float[,] lights;
    public float[,] salts;
    public float[,] seaDistance; 
    Dictionary<TerrainGenBlockTag, float>[,] tags;
}

public class FieldWriter :BaseChunkWriter
{
    //场数据写入读取，需将tag数据视为场
    //为所有场提供整体获取float[,]和根据坐标获取float的功能
    //可以先临时使用全部保存到内存的方案，以辅助验证整体流程的正确性
    //场数据也要按chunk分块，不然分块加载无意义

    float[,] seaDistance; 
    float[,] waters;//临时使用全部保存到内存方法
    float[,] heats;
    Vector2[,] step;
    Vector2[,] magics;
    Dictionary<TerrainGenBlockTag,float>[,] tags;



   

    public void WriteSeaDistanceField(float[,]  f)
    {
        seaDistance = f;
    }
    public void WriteWaterField(float[,] f)
    {
        waters = f;
    }
    public void WriteHeatField(float[,] h)
    {
        heats = h;
    }
    public void WriteStepField(Vector2[,] s)
    {
        step = s;
    }
    public void WriteMagicField(Vector2[,] m)
    {
        magics = m;
    }
    public void WriteTagField(Dictionary<TerrainGenBlockTag,float>[,] t)
    {
        tags = t;
    }
    public float[,] GetChunkSeaDistance(int x,int y)
    {
        float[,] temp = new float[Chunk.ChunkSize, Chunk.ChunkSize];
        
        int chunkposOffsetX = maxWid / Chunk.ChunkSize / 2;//只要地图总大小不小于64*64则不会出问题
        int chunkposOffsetY = maxHig / Chunk.ChunkSize / 2;//地图中心到边缘的chunk数

        if (x >= chunkposOffsetX || y >= chunkposOffsetY) {  return null; }
        if (x < -chunkposOffsetX || y < -chunkposOffsetY) {  return null; }

        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                temp[i, j] = seaDistance[(x + chunkposOffsetX) * Chunk.ChunkSize + i, (y + chunkposOffsetY) * Chunk.ChunkSize + j];
            }
        }
        return temp;
    }
    public float[,] GetChunkWaterField(int x,int y)
    {
        float[,] temp = new float[Chunk.ChunkSize, Chunk.ChunkSize];

        int chunkposOffsetX = maxWid / Chunk.ChunkSize / 2;//只要地图总大小不小于64*64则不会出问题
        int chunkposOffsetY = maxHig / Chunk.ChunkSize / 2;//地图中心到边缘的chunk数

        if (x >= chunkposOffsetX || y >= chunkposOffsetY) return null;
        if (x < -chunkposOffsetX || y < -chunkposOffsetY) return null;

        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                temp[i, j] = waters[(x + chunkposOffsetX) * Chunk.ChunkSize + i, (y + chunkposOffsetY) * Chunk.ChunkSize + j];
            }
        }
        return temp;
    }
    public float[,] GetChunkHeatField(int x,int y)
    {
        float[,] temp = new float[Chunk.ChunkSize, Chunk.ChunkSize];

        int chunkposOffsetX = maxWid / Chunk.ChunkSize / 2;//只要地图总大小不小于64*64则不会出问题
        int chunkposOffsetY = maxHig / Chunk.ChunkSize / 2;//地图中心到边缘的chunk数

        if (x >= chunkposOffsetX || y >= chunkposOffsetY) return null;
        if (x < -chunkposOffsetX || y < -chunkposOffsetY) return null;

        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                temp[i, j] = heats[(x + chunkposOffsetX) * Chunk.ChunkSize + i, (y + chunkposOffsetY) * Chunk.ChunkSize + j];
            }
        }
        return temp;
    }
    public Vector2[,] GetChunkStepField(int x,int y)
    {
        Vector2[,] temp = new Vector2[Chunk.ChunkSize, Chunk.ChunkSize];

        int chunkposOffsetX = maxWid / Chunk.ChunkSize / 2;//只要地图总大小不小于64*64则不会出问题
        int chunkposOffsetY = maxHig / Chunk.ChunkSize / 2;//地图中心到边缘的chunk数

        if (x >= chunkposOffsetX || y >= chunkposOffsetY) return null;
        if (x < -chunkposOffsetX || y < -chunkposOffsetY) return null;

        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                temp[i, j] = step[(x + chunkposOffsetX) * Chunk.ChunkSize + i, (y + chunkposOffsetY) * Chunk.ChunkSize + j];
            }
        }
        return temp;
    }
    public Vector2[,] GetChunkMagicField(int x,int y)
    {
        if (magics == null) return null;
        Vector2[,] temp = new Vector2[Chunk.ChunkSize, Chunk.ChunkSize];

        int chunkposOffsetX = maxWid / Chunk.ChunkSize / 2;//只要地图总大小不小于64*64则不会出问题
        int chunkposOffsetY = maxHig / Chunk.ChunkSize / 2;//地图中心到边缘的chunk数

        if (x >= chunkposOffsetX || y >= chunkposOffsetY) return null;
        if (x < -chunkposOffsetX || y < -chunkposOffsetY) return null;

        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                temp[i, j] = magics[(x + chunkposOffsetX) * Chunk.ChunkSize + i, (y + chunkposOffsetY) * Chunk.ChunkSize + j];
            }
        }
        return temp;
    }
    public Dictionary<TerrainGenBlockTag,float>[,] GetChunkTagField(int x,int y)
    {
        return null;
    }
}
