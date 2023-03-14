using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainWaterBodyGenerator : BaseTProcessGenerator
{

    //首先生成H=0的外围海水
    //根据场数据确定河湖水体，将水体bblock写入chunk文件
    //直接接收terrianGenerator的传参调用
    public void GenMainWater()
    {
        int chunkFileWid = width / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);
        int chunkFileHig = height / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);

        int chunkposOffsetX = width / Chunk.ChunkSize / 2;//只要地图总大小不小于64*64则不会出问题
        int chunkposOffsetY = height / Chunk.ChunkSize / 2;//地图中心到边缘的chunk数

        for (int i = -Mathf.CeilToInt(chunkFileWid * 1.0f / 2); i < Mathf.CeilToInt(chunkFileWid * 1.0f / 2); i++)//大chunk坐标
        {
            for (int j = -Mathf.CeilToInt(chunkFileHig * 1.0f / 2); j < Mathf.CeilToInt(chunkFileHig * 1.0f / 2); j++)
            {
                for (int k = 0; k < Container_Terrain.chunkSizePerFile; k++)//smallchunk遍历
                {
                    for (int o = 0; o < Container_Terrain.chunkSizePerFile; o++)
                    {
                        int chunkposX = i * Container_Terrain.chunkSizePerFile + k;//以0.0为原点的chunk坐标
                        int chunkposY = j * Container_Terrain.chunkSizePerFile + o;
                        if (chunkposX >= chunkposOffsetX || chunkposY >= chunkposOffsetY || chunkposX < -chunkposOffsetX || chunkposY < -chunkposOffsetY) continue;
                        
                        float[,] seadistance = fieldWriter.GetChunkSeaDistance(chunkposX, chunkposY);
                        int[,] h=chunkFileWriter.GetChunkH(chunkposX, chunkposY);
                        Dictionary<int, B_Block> bblocks = new Dictionary<int, B_Block>();

                        for (int p = 0; p < Chunk.ChunkSize; p++)
                        {
                            for (int q = 0; q < Chunk.ChunkSize; q++)
                            {
                                if (seadistance[p,q]==0&&h[p,q]<mpd.config.sea)//目前是只取小于海平面的为水，等于海平面的为陆地
                                {
                                    for (int t = 0; t < mpd.config.sea-h[p, q]; t++)
                                    {
                                        bblocks.Add(XYHelper.ToCoord3(p, q, t + h[p, q]), new B_Block(B_Material.water,10));
                                    }
                                }
                            }
                        }
                        Debug.Log("生成了" + bblocks.Count + "个水");
                        chunkFileWriter.ResetBBlockAt(chunkposX, chunkposY, bblocks);
                    }
                }
            }
        }
    }
}
