using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_terrianSystem : MonoBehaviour
{
    public GameObject block;
    public void Start()
    {
        // GetComponent<EventCenter>().RegistFunc<string>(nameof(EventNames.ThisSavePath),thisSavePath);
        //test();
        //GetComponent<ChunkObject>().Init(new Chunk(), 0, 0);


    }

    public Text t;
    Ray r;
    RaycastHit rt;
    void Update()
    {
        r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out rt))
        {
            Vector3Int a = Chunk.WorldPosToBlockPos(rt.point);
            int ans = EventCenter.WorldCenter.GetParm<long, int>(nameof(EventNames.GetTerrainHAt), XYHelper.ToLongXY((int)a.x, (int)a.y));
            t.text = ans.ToString();
        }
        //获取指向点vec3
        //转方块坐标送getheight方法
        //设text=返回值
    }


    private void test()
    {
        MapPrefabsData data=new MapPrefabsData();
        data.config=new InGameMapGenConfig();
        data.config.height=128;
        data.config.width=128;
        data.config.mform=MapForm.island;
        data.config.tform=TerrainForm.oneMountain;
        
      Chunk[,] allchunk= GetComponent<TerrainGenerator>().GenTerrain(data);
       Vector3 bigX=Vector3.zero;
       Vector3 bigY=Vector3.zero;
       Vector3 X=Vector3.zero;
       Vector3 Y=Vector3.zero;
        for (int i = 0; i < allchunk.GetLength(0); i++)
        {
            for (int j = 0; j < allchunk.GetLength(1); j++)
            {
                for (int k = 0; k < Chunk.ChunkSize; k++)
                {
                    for (int p = 0; p < Chunk.ChunkSize; p++)
                    {
                        Vector3 h=new Vector3(0,allchunk[i,j].tblocks[k,p].H*Chunk.BlockHeight,0);
                        Instantiate(block,bigX+bigY+X+Y+h,transform.rotation);
                        Y-=new Vector3(0,0,Chunk.BlockSize);
                    }
                    Y=Vector3.zero;
                    X-=new Vector3(Chunk.BlockSize,0,0);
                }
                Y=Vector3.zero;
                X=Vector3.zero;
                bigY-=new Vector3(0,0,Chunk.ChunkSize*Chunk.BlockSize);
            }
             Y=Vector3.zero;
                X=Vector3.zero;
                bigY=Vector3.zero;
            bigX-=new Vector3(Chunk.ChunkSize*Chunk.BlockSize,0,0);
        }
    }
    // Start is called before the first frame update
    public string thisSavePath()
    {
        return "1";
    }
}
/*
 * 除震动效果不太好，视角切换太生硬外没什么问题
 * 
 * 
 * 
 */ 