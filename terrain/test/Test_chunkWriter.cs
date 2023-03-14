using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_chunkWriter : MonoBehaviour
{
    ChunkFileWriter cfw=new ChunkFileWriter();
    // Start is called before the first frame update
    void Start()
    {
        EventCenter.WorldCenter.RegistFunc<string>(nameof(EventNames.ThisSavePath), () => { return "owl"; });

        MapPrefabsData mpd = new MapPrefabsData();
        mpd.config.width = 1024;
        mpd.config.height = 1024;
        cfw.Init(mpd);

        float[,] h = new float[mpd.config.width, mpd.config.height];
        for (int i = 0; i < h.GetLength(0); i++)
        {
            for (int j = 0; j < h.GetLength(1); j++)
            {
                h[i, j] = i * h.GetLength(0) + j;
            }
        }
        Debug.Log(h.GetLength(0) + ":" + h.GetLength(1));
        cfw.WriteMapH(h);

       /* int[,] temp1 = cfw.GetChunkH(-8, -8);
        int[,] temp2 = cfw.GetChunkH(-7, -8);
        Debug.Log(temp1[0, 0] - temp2[0, 0]);
        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                if((temp1[i,j]-temp2[i,j])!=(temp1[0,0]-temp2[0,0]))
                {
                    Debug.Log("´íÎó");
                    break;
                }
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
