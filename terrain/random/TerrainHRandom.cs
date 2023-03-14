using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// random不承担写入功能,只负责生成如高度场这样简单数据类型的全图数组
/// </summary>
public class TerrainHRandom 
{
    //生成地形高度相关的随机值
    //基底perlin随机
    //terrianForm随机
    //MapFrom随机
    //最终细节随机
    //只需对外提供获取全图float[,]方法
    int width, height;
    public float amplify = 30, smooth = 512, Offset = 10, Seed;

    public float[,] GetMapH(MapPrefabsData data)
    {
        float[,] tblockHeight = new float[data.config.width, data.config.height];
        width = data.config.width;
        height = data.config.height;

        GenBasePerlin(tblockHeight, data);
        GenTerrianForm(tblockHeight, data);
        //GenMapForm(tblockHeight, data);
        randomPerlin(tblockHeight, data);
        return tblockHeight;
    }

   
    float mapPerlin(int bx, int by, MapPrefabsData dt)
    {//bx,by是以地图中心为原点的坐标
        float maskx = bx * 1.0f * 2 / width;
        float masky = by * 1.0f * 2 / height;//归一化为-1~+1

        switch (dt.config.mform)
        {
            case MapForm.island:
                //   -------------  -  形状是最高点为1，最低点为0的半径为地图半径的圆锥
                float f = Mathf.Clamp(Mathf.Sqrt(maskx * maskx + masky * masky), 0f, 1f);
                if ((maskx == 0 && masky == 0) || (maskx == 0.5f && masky == 0.5f))
                {
                    Debug.Log(f);
                }
                return 1 - f;
                break;
            default:
                return 1;
                break;
        }

    }

    /// <summary>
    /// 地形perlin
    /// </summary>
    /// <param name="bx"></param>
    /// <param name="by"></param>
    /// <param name="form"></param>
    /// <returns></returns>
    float terrainPerlin(int bx, int by, TerrainForm form)
    {
        return 1;
    }

    void randomPerlin(float[,] tblockH, MapPrefabsData data)
    {
        //根据mapprefabsdata中指定的随机类型给地形添加随机

    }

    void GenBasePerlin(float[,] highs, MapPrefabsData data)
    {
        for (int i = 0; i < width; i++)//生成基础perlin
        {
            for (int j = 0; j < height; j++)
            {
                highs[i, j] = MathCenter.basePerlin(i - width / 2, j - height / 2, 123.456f,amplify,smooth,Offset);
            }
        }
    }
    void GenTerrianForm(float[,] highs, MapPrefabsData data)
    {
        for (int i = 0; i < width; i++)//terrainform应用
        {
            for (int j = 0; j < height; j++)
            {
                highs[i, j] *= terrainPerlin(i - width / 2, j - height / 2, data.config.tform);
            }
        }
    }
    void GenMapForm(float[,] highs, MapPrefabsData data)
    {
        for (int i = 0; i < width; i++)//mapform应用
        {
            for (int j = 0; j < height; j++)
            {
                float temp = mapPerlin(i - width / 2, j - height / 2, data);
                highs[i, j] *= temp;//中心到边缘起伏递减
                highs[i, j] += (1 - temp) * (data.config.sea - 10);//下降海平面,最深到海平面多少米以下
            }
        }
    }

}
