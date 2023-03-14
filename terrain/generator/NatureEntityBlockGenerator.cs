using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NatureEntityBlockGenerator :BaseTProcessGenerator
{
    //读取随机生成规则、场数据,tag进行自然eblock生成
    //矿，石，树，草，人造物5阶段
    //要把所在点的地形、场数据等传递给生成物，使其改变自身属性再保存

    public override void Init(MapPrefabsData data, ChunkFileWriter c, FieldWriter f, EBFileWriter eb, EntityFileWriter en)
    {
        base.Init(data, c, f, eb, en);
    }
    public override void DeInit()
    {
        base.DeInit();
    }

    public void Generate()
    {
         int chunkFileWid = width / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);
        int chunkFileHig = height / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);

         int chunkposOffsetX = width / Chunk.ChunkSize / 2;//只要地图总大小不小于64*64则不会出问题
        int chunkposOffsetY = height / Chunk.ChunkSize / 2;//地图中心到边缘的chunk数

        
         for (int i = -Mathf.CeilToInt(chunkFileWid * 1.0f / 2); i < Mathf.CeilToInt(chunkFileWid * 1.0f / 2); i++)//大chunk坐标
        {
            for (int j = -Mathf.CeilToInt(chunkFileHig * 1.0f / 2); j < Mathf.CeilToInt(chunkFileHig * 1.0f / 2); j++)
            {
                EntityBlockFile ebf=new EntityBlockFile();
                int coun = 0;
                for (int k = 0; k < Container_Terrain.chunkSizePerFile; k++)//smallchunk遍历
                {
                    for (int o = 0; o < Container_Terrain.chunkSizePerFile; o++)
                    {
                        int chunkposX = i * Container_Terrain.chunkSizePerFile + k;//以0.0为原点的chunk坐标
                        int chunkposY = j * Container_Terrain.chunkSizePerFile + o;
                        if (chunkposX >= chunkposOffsetX || chunkposY >= chunkposOffsetY || chunkposX < -chunkposOffsetX || chunkposY < -chunkposOffsetY) continue;
                        
                        T_BIOME[,] tb=chunkFileWriter.GetChunkBio(chunkposX,chunkposY);//获取当前chunk的biome，高度，水分等数据
                        int[,] h=chunkFileWriter.GetChunkH(chunkposX, chunkposY);
                        float[,] water=fieldWriter.GetChunkWaterField(chunkposX,chunkposY);


                        for (int p = 0; p < Chunk.ChunkSize; p++)//将升级成按规则文件进行生成
                        {
                            //规则文件法：根据biome/stage获取NatureEBlockGenRule,再根据
                            //rule的几率生成eblock
                            for (int q = 0; q < Chunk.ChunkSize; q++)
                            {
                                //沙子上0.004生成椰子树，草地上0.01生成普通树，一格生成了东西就不生成别的了
                                if(tb[p,q]==T_BIOME.beach)
                                {
                                    if(Random.Range(0,1f)<=0.004f)
                                    {
                                       long id= mpd.GetMaxUUID();
                                       Vector3Int pos=new Vector3Int(chunkposX*Chunk.ChunkSize+p,h[p,q],chunkposY*Chunk.ChunkSize+q);
                                       Entity_Block eb=new Entity_Block(pos,4);
                                       eb.UUID=id;
                                       eb.dir=DIR.left;
                                       //没有载入container_entityblock，无法做可放置检测
                                       ebf.Add(eb,"");
                                        coun++;
                                    }
                                    else if (Random.Range(0, 1f) <= 0.002f)
                                    {
                                        //0.002生成小石子
                                        long id = mpd.GetMaxUUID();
                                        Vector3Int pos = new Vector3Int(chunkposX * Chunk.ChunkSize + p, h[p, q], chunkposY * Chunk.ChunkSize + q);
                                        int[] aa = new int[] { 1101, 1102, 1103 };
                                        int rid = aa[Random.Range(0, aa.Length )];
                                        Entity_Block eb = new Entity_Block(pos, rid);
                                        eb.UUID = id;
                                        eb.dir = DIR.left;
                                        //没有载入container_entityblock，无法做可放置检测
                                        ebf.Add(eb, "");
                                        coun++;
                                    }
                                }
                                else if(tb[p,q]==T_BIOME.grassland)
                                {
                                    if(Random.Range(0,1f)<=0.01f)
                                    {
                                        long id= mpd.GetMaxUUID();
                                       Vector3Int pos=new Vector3Int(chunkposX*Chunk.ChunkSize+p,h[p,q],chunkposY*Chunk.ChunkSize+q);
                                       Entity_Block eb=new Entity_Block(pos, Random.Range(2, 4));
                                       eb.UUID=id;
                                       eb.dir=DIR.right;
                                       //没有载入container_entityblock，无法做可放置检测
                                       ebf.Add(eb,"");
                                        coun++;
                                    }
                                    else if (Random.Range(0, 1f) <= 0.01f)
                                    {
                                        //0.002生成灌木
                                        long id = mpd.GetMaxUUID();
                                        Vector3Int pos = new Vector3Int(chunkposX * Chunk.ChunkSize + p, h[p, q], chunkposY * Chunk.ChunkSize + q);
                                        int[] aa = new int[] { 1004, 1005, 1001,1002,1003 };
                                        int rid = aa[Random.Range(0, aa.Length)];
                                        Entity_Block eb = new Entity_Block(pos, rid);
                                        eb.UUID = id;
                                        eb.dir = DIR.left;
                                        //没有载入container_entityblock，无法做可放置检测
                                        ebf.Add(eb, "");
                                        coun++;
                                    }
                                }
                            }
                        }

                    }
                }
                Debug.Log("该eblockfile"+i+":"+j+"生成了" + coun + "个树");
                eBFileWriter.WirteEBAt(i,j,ebf);
            }
        }
    }
    

}
