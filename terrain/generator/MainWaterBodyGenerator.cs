using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainWaterBodyGenerator : BaseTProcessGenerator
{

    //��������H=0����Χ��ˮ
    //���ݳ�����ȷ���Ӻ�ˮ�壬��ˮ��bblockд��chunk�ļ�
    //ֱ�ӽ���terrianGenerator�Ĵ��ε���
    public void GenMainWater()
    {
        int chunkFileWid = width / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);
        int chunkFileHig = height / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);

        int chunkposOffsetX = width / Chunk.ChunkSize / 2;//ֻҪ��ͼ�ܴ�С��С��64*64�򲻻������
        int chunkposOffsetY = height / Chunk.ChunkSize / 2;//��ͼ���ĵ���Ե��chunk��

        for (int i = -Mathf.CeilToInt(chunkFileWid * 1.0f / 2); i < Mathf.CeilToInt(chunkFileWid * 1.0f / 2); i++)//��chunk����
        {
            for (int j = -Mathf.CeilToInt(chunkFileHig * 1.0f / 2); j < Mathf.CeilToInt(chunkFileHig * 1.0f / 2); j++)
            {
                for (int k = 0; k < Container_Terrain.chunkSizePerFile; k++)//smallchunk����
                {
                    for (int o = 0; o < Container_Terrain.chunkSizePerFile; o++)
                    {
                        int chunkposX = i * Container_Terrain.chunkSizePerFile + k;//��0.0Ϊԭ���chunk����
                        int chunkposY = j * Container_Terrain.chunkSizePerFile + o;
                        if (chunkposX >= chunkposOffsetX || chunkposY >= chunkposOffsetY || chunkposX < -chunkposOffsetX || chunkposY < -chunkposOffsetY) continue;
                        
                        float[,] seadistance = fieldWriter.GetChunkSeaDistance(chunkposX, chunkposY);
                        int[,] h=chunkFileWriter.GetChunkH(chunkposX, chunkposY);
                        Dictionary<int, B_Block> bblocks = new Dictionary<int, B_Block>();

                        for (int p = 0; p < Chunk.ChunkSize; p++)
                        {
                            for (int q = 0; q < Chunk.ChunkSize; q++)
                            {
                                if (seadistance[p,q]==0&&h[p,q]<mpd.config.sea)//Ŀǰ��ֻȡС�ں�ƽ���Ϊˮ�����ں�ƽ���Ϊ½��
                                {
                                    for (int t = 0; t < mpd.config.sea-h[p, q]; t++)
                                    {
                                        bblocks.Add(XYHelper.ToCoord3(p, q, t + h[p, q]), new B_Block(B_Material.water,10));
                                    }
                                }
                            }
                        }
                        Debug.Log("������" + bblocks.Count + "��ˮ");
                        chunkFileWriter.ResetBBlockAt(chunkposX, chunkposY, bblocks);
                    }
                }
            }
        }
    }
}
