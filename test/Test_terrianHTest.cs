using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

//可多线程加载chunk数据
public class Test_terrainHtest : MonoBehaviour
{
    private void Start()
    {
        
    }
    public Text t;
    Ray r;
    RaycastHit rt;
    void Update()
    {
        r=Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(r,out rt))
        {
            Vector3Int a=Chunk.WorldPosToBlockPos(rt.point);
            int ans=EventCenter.WorldCenter.GetParm<long,int>(nameof(EventNames.GetTerrainHAt),XYHelper.ToLongXY((int)a.x,(int)a.y));
            t.text=ans.ToString();
        }
        //获取指向点vec3
        //转方块坐标送getheight方法
        //设text=返回值
    }
}