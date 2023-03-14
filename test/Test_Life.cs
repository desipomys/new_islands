using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Life : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //Debug.Log("onawake");
        /*testIntCoord3(1, 2, 3);
        Debug.Log("------------------------");
        testIntCoord3(0,1,0);
        Debug.Log("------------------------");
        testIntCoord3(1, 0, 0);
        Debug.Log("------------------------");
        testIntCoord3(0, 0, 1);
        Debug.Log("------------------------");
        testIntCoord3(int.MaxValue, int.MinValue, int.MinValue);*/
        testvec3Toint();
    }
    void testvec3Toint()
    {
        /*int a = XYHelper.ToCoord3(0, -1, 0);
        Debug.Log(a.GetCoord3()[0] + "," + a.GetCoord3()[1] + "," + a.GetCoord3()[2]);
        a = XYHelper.ToCoord3(-1, 2, -1);
        Debug.Log(a.GetCoord3()[0] + "," + a.GetCoord3()[1] + "," + a.GetCoord3()[2]);
        a = XYHelper.ToCoord3(126, -126, 32765);
        Debug.Log(a.GetCoord3()[0] + "," + a.GetCoord3()[1] + "," + a.GetCoord3()[2]);
        a = XYHelper.ToCoord3(-127, 126, -32765);
        Debug.Log(a.GetCoord3()[0] + "," + a.GetCoord3()[1] + "," + a.GetCoord3()[2]);*/
        Vector3Int vi = new Vector3Int(0, 0, 0);
        int a = vi.ToInt();
        Debug.Log(a.GetCoord3()[0] + "," + a.GetCoord3()[1] + "," + a.GetCoord3()[2]);
        vi=new Vector3Int(127,32767, 127 );
        a = vi.ToInt();
        Debug.Log(a.GetCoord3()[0] + "," + a.GetCoord3()[1] + "," + a.GetCoord3()[2]);
        vi = new Vector3Int(-127, -32767, -127);
        a = vi.ToInt();
        Debug.Log(a.GetCoord3()[0] + "," + a.GetCoord3()[1] + "," + a.GetCoord3()[2]);
        vi = new Vector3Int(1, 1, 1);
        a = vi.ToInt();
        Debug.Log(a.GetCoord3()[0] + "," + a.GetCoord3()[1] + "," + a.GetCoord3()[2]);
        vi = new Vector3Int(-1, -1, -1);
        a = vi.ToInt();
        Debug.Log(a.GetCoord3()[0] + "," + a.GetCoord3()[1] + "," + a.GetCoord3()[2]);
    }
    void testworldToLocal()
    {
        Vector3Int temp = new Vector3Int(0, 1, 2);
        Debug.Log(Chunk.GlobalBPosToLocalBPos(temp));
        temp = new Vector3Int(-1, 32, -2);
        Debug.Log(Chunk.GlobalBPosToLocalBPos(temp));
        temp = new Vector3Int(-32, 31, -31);
        Debug.Log(Chunk.GlobalBPosToLocalBPos(temp));
        
    }
    void testIntCoord3(int x,int y,int z)
    {
        int ans = XYHelper.ToCoord3(x, y, z);
        Debug.Log(ans);

        Debug.Log(x+"."+y+"."+z+"ans:"+ans.GetCoord3()[0] + "," + ans.GetCoord3()[1] + "," + ans.GetCoord3()[2]);
    }
    
}
