using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NatureEntityBlockGenerator :BaseTProcessGenerator
{
    //��ȡ������ɹ��򡢳�����,tag������Ȼeblock����
    //��ʯ�������ݣ�������5�׶�
    //Ҫ�����ڵ�ĵ��Ρ������ݵȴ��ݸ������ʹ��ı����������ٱ���

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

         int chunkposOffsetX = width / Chunk.ChunkSize / 2;//ֻҪ��ͼ�ܴ�С��С��64*64�򲻻������
        int chunkposOffsetY = height / Chunk.ChunkSize / 2;//��ͼ���ĵ���Ե��chunk��

        
         for (int i = -Mathf.CeilToInt(chunkFileWid * 1.0f / 2); i < Mathf.CeilToInt(chunkFileWid * 1.0f / 2); i++)//��chunk����
        {
            for (int j = -Mathf.CeilToInt(chunkFileHig * 1.0f / 2); j < Mathf.CeilToInt(chunkFileHig * 1.0f / 2); j++)
            {
                EntityBlockFile ebf=new EntityBlockFile();
                int coun = 0;
                for (int k = 0; k < Container_Terrain.chunkSizePerFile; k++)//smallchunk����
                {
                    for (int o = 0; o < Container_Terrain.chunkSizePerFile; o++)
                    {
                        int chunkposX = i * Container_Terrain.chunkSizePerFile + k;//��0.0Ϊԭ���chunk����
                        int chunkposY = j * Container_Terrain.chunkSizePerFile + o;
                        if (chunkposX >= chunkposOffsetX || chunkposY >= chunkposOffsetY || chunkposX < -chunkposOffsetX || chunkposY < -chunkposOffsetY) continue;
                        
                        T_BIOME[,] tb=chunkFileWriter.GetChunkBio(chunkposX,chunkposY);//��ȡ��ǰchunk��biome���߶ȣ�ˮ�ֵ�����
                        int[,] h=chunkFileWriter.GetChunkH(chunkposX, chunkposY);
                        float[,] water=fieldWriter.GetChunkWaterField(chunkposX,chunkposY);


                        for (int p = 0; p < Chunk.ChunkSize; p++)//�������ɰ������ļ���������
                        {
                            //�����ļ���������biome/stage��ȡNatureEBlockGenRule,�ٸ���
                            //rule�ļ�������eblock
                            for (int q = 0; q < Chunk.ChunkSize; q++)
                            {
                                //ɳ����0.004����Ҭ�������ݵ���0.01������ͨ����һ�������˶����Ͳ����ɱ����
                                if(tb[p,q]==T_BIOME.beach)
                                {
                                    if(Random.Range(0,1f)<=0.004f)
                                    {
                                       long id= mpd.GetMaxUUID();
                                       Vector3Int pos=new Vector3Int(chunkposX*Chunk.ChunkSize+p,h[p,q],chunkposY*Chunk.ChunkSize+q);
                                       Entity_Block eb=new Entity_Block(pos,4);
                                       eb.UUID=id;
                                       eb.dir=DIR.left;
                                       //û������container_entityblock���޷����ɷ��ü��
                                       ebf.Add(eb,"");
                                        coun++;
                                    }
                                    else if (Random.Range(0, 1f) <= 0.002f)
                                    {
                                        //0.002����Сʯ��
                                        long id = mpd.GetMaxUUID();
                                        Vector3Int pos = new Vector3Int(chunkposX * Chunk.ChunkSize + p, h[p, q], chunkposY * Chunk.ChunkSize + q);
                                        int[] aa = new int[] { 1101, 1102, 1103 };
                                        int rid = aa[Random.Range(0, aa.Length )];
                                        Entity_Block eb = new Entity_Block(pos, rid);
                                        eb.UUID = id;
                                        eb.dir = DIR.left;
                                        //û������container_entityblock���޷����ɷ��ü��
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
                                       //û������container_entityblock���޷����ɷ��ü��
                                       ebf.Add(eb,"");
                                        coun++;
                                    }
                                    else if (Random.Range(0, 1f) <= 0.01f)
                                    {
                                        //0.002���ɹ�ľ
                                        long id = mpd.GetMaxUUID();
                                        Vector3Int pos = new Vector3Int(chunkposX * Chunk.ChunkSize + p, h[p, q], chunkposY * Chunk.ChunkSize + q);
                                        int[] aa = new int[] { 1004, 1005, 1001,1002,1003 };
                                        int rid = aa[Random.Range(0, aa.Length)];
                                        Entity_Block eb = new Entity_Block(pos, rid);
                                        eb.UUID = id;
                                        eb.dir = DIR.left;
                                        //û������container_entityblock���޷����ɷ��ü��
                                        ebf.Add(eb, "");
                                        coun++;
                                    }
                                }
                            }
                        }

                    }
                }
                Debug.Log("��eblockfile"+i+":"+j+"������" + coun + "����");
                eBFileWriter.WirteEBAt(i,j,ebf);
            }
        }
    }
    

}
