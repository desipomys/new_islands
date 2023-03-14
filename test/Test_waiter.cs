using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_waiter : MonoBehaviour
{
    Waiter w = new Waiter();

    // Start is called before the first frame update
    void Start()
    {
        int[] temp = new int[32];
        int[] temp1 = new int[32];
        //w.AddWaitTime(1);
        for (int i = -16; i < 16; i += 1)
        {
            temp[i+16] = i / 4;
                temp1[i+16] = i % 4;
            for (int j = -16; j < 16; j += 1)
            {
                /*long temp = XYHelper.ToLongXY(i, j);

                                Debug.Log(i + ","+j +"="+ temp.ToString("X"));
                Debug.Log(temp.ToString("X") + "=" + temp.GetX() + "," + temp.GetY());*/



                //long temp = Chunk.WorldPosToChunkPos(new Vector3(i, 0, j));
                //Debug.Log(i + "," + j + "=" + temp.GetX()+","+temp.GetY());
                //long temp = XYHelper.ToLongXY(i, j);
                //Debug.Log(i + "," + j + "=" + IscordInMap(temp));
                
            }

        }

    }
    bool IscordInMap(long xy)//chunk全局坐标
    {
        if ((128 / 2) < Mathf.Abs(xy.GetX() * Chunk.ChunkSize + 0.1f))
        {
            return false;
        }
        if ((128 / 2) < Mathf.Abs(xy.GetY() * Chunk.ChunkSize + 0.1f)) return false;
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        //if (w.IsReady()) { Debug.Log("OK");w.AddWaitTime(0.15F); }
    }
}
