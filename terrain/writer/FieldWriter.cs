using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������ɹ����в�����tag
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
    //������д���ȡ���轫tag������Ϊ��
    //Ϊ���г��ṩ�����ȡfloat[,]�͸��������ȡfloat�Ĺ���
    //��������ʱʹ��ȫ�����浽�ڴ�ķ������Ը�����֤�������̵���ȷ��
    //������ҲҪ��chunk�ֿ飬��Ȼ�ֿ����������

    float[,] seaDistance; 
    float[,] waters;//��ʱʹ��ȫ�����浽�ڴ淽��
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
        
        int chunkposOffsetX = maxWid / Chunk.ChunkSize / 2;//ֻҪ��ͼ�ܴ�С��С��64*64�򲻻������
        int chunkposOffsetY = maxHig / Chunk.ChunkSize / 2;//��ͼ���ĵ���Ե��chunk��

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

        int chunkposOffsetX = maxWid / Chunk.ChunkSize / 2;//ֻҪ��ͼ�ܴ�С��С��64*64�򲻻������
        int chunkposOffsetY = maxHig / Chunk.ChunkSize / 2;//��ͼ���ĵ���Ե��chunk��

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

        int chunkposOffsetX = maxWid / Chunk.ChunkSize / 2;//ֻҪ��ͼ�ܴ�С��С��64*64�򲻻������
        int chunkposOffsetY = maxHig / Chunk.ChunkSize / 2;//��ͼ���ĵ���Ե��chunk��

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

        int chunkposOffsetX = maxWid / Chunk.ChunkSize / 2;//ֻҪ��ͼ�ܴ�С��С��64*64�򲻻������
        int chunkposOffsetY = maxHig / Chunk.ChunkSize / 2;//��ͼ���ĵ���Ե��chunk��

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

        int chunkposOffsetX = maxWid / Chunk.ChunkSize / 2;//ֻҪ��ͼ�ܴ�С��С��64*64�򲻻������
        int chunkposOffsetY = maxHig / Chunk.ChunkSize / 2;//��ͼ���ĵ���Ե��chunk��

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
