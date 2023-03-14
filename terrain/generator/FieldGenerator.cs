using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : BaseTProcessGenerator
{
    //根据高度数据生成各种场数据，会被临时保存到文件中以防生成中卡死
    //高度导数场,水分场，热场，魔力场,盐场，光场  
    public float[,] GenSeaDistance(float[,] hights, MapPrefabsData data)
    {
        float[,] f = new float[data.config.width, data.config.height];
        Queue<long> coordQuene = new Queue<long>();
        HashSet<long> coorset = new HashSet<long>();
        for (int i = 0; i < hights.GetLength(0); i++)
        {
            for (int j = 0; j < hights.GetLength(1); j++)
            {
                if (hights[i, j] < data.config.sea)
                {
                    f[i, j] = 0;
                    if ((i - 1 >= 0 && i - 1 < data.config.width) && (hights[i - 1, j] >= data.config.sea) && !coorset.Contains(XYHelper.ToLongXY(i - 1, j)))
                    {
                        coordQuene.Enqueue(XYHelper.ToLongXY(i - 1, j));
                        coorset.Add(XYHelper.ToLongXY(i - 1, j));
                    }
                    if ((i + 1 >= 0 && i + 1 < data.config.width) && (hights[i + 1, j] >= data.config.sea) && !coorset.Contains(XYHelper.ToLongXY(i + 1, j)))
                    {
                        coordQuene.Enqueue(XYHelper.ToLongXY(i + 1, j));
                        coorset.Add(XYHelper.ToLongXY(i + 1, j));
                    }
                    if ((j - 1 >= 0 && j - 1 < data.config.height) && (hights[i, j - 1] >= data.config.sea) && !coorset.Contains(XYHelper.ToLongXY(i, j - 1)))
                    {
                        coordQuene.Enqueue(XYHelper.ToLongXY(i, j - 1));
                        coorset.Add(XYHelper.ToLongXY(i, j - 1));
                    }
                    if ((j + 1 >= 0 && j + 1 < data.config.height) && (hights[i, j + 1] >= data.config.sea) && !coorset.Contains(XYHelper.ToLongXY(i, j + 1)))
                    {
                        coordQuene.Enqueue(XYHelper.ToLongXY(i, j + 1));
                        coorset.Add(XYHelper.ToLongXY(i, j + 1));
                    }
                }
                else f[i, j] = -1;
            }
        }
        Debug.Log(coordQuene.Count + "队列大小");
        while (coordQuene.Count > 0)//广度优先计算曼哈顿距离场
        {
            long temp = coordQuene.Dequeue();
            int i = temp.GetX();
            int j = temp.GetY();
            //曼哈顿距离法，
            try
            {
                float left = int.MaxValue;
                if (i - 1 >= 0)
                {
                    left = f[i - 1, j];
                    if (left == -1)
                    {
                        if (!coorset.Contains(XYHelper.ToLongXY(i - 1, j)))
                        {
                            coordQuene.Enqueue(XYHelper.ToLongXY(i - 1, j));
                            coorset.Add(XYHelper.ToLongXY(i - 1, j));
                        }
                        left = int.MaxValue;
                    }
                }
                else left = f[i, j];

                float right = int.MaxValue;
                if (i + 1 >= 0)
                {
                    right = f[i + 1, j];
                    if (right == -1)
                    {
                        if (!coorset.Contains(XYHelper.ToLongXY(i + 1, j)))
                        {
                            coordQuene.Enqueue(XYHelper.ToLongXY(i + 1, j));
                            coorset.Add(XYHelper.ToLongXY(i + 1, j));
                        }
                        right = int.MaxValue;
                    }
                }
                else right = f[i, j];

                float up = int.MaxValue;
                if (j + 1 >= 0)
                {
                    up = f[i, j + 1];
                    if (up == -1)
                    {
                        if (!coorset.Contains(XYHelper.ToLongXY(i, j + 1)))
                        {
                            coordQuene.Enqueue(XYHelper.ToLongXY(i, j + 1));
                            coorset.Add(XYHelper.ToLongXY(i, j + 1));
                        }

                        up = int.MaxValue;
                    }
                }
                else up = f[i, j];

                float down = int.MaxValue;
                if (j - 1 >= 0)
                {
                    down = f[i, j - 1];
                    if (down == -1)
                    {
                        if (!coorset.Contains(XYHelper.ToLongXY(i, j - 1)))
                        {
                            coordQuene.Enqueue(XYHelper.ToLongXY(i, j - 1));
                            coorset.Add(XYHelper.ToLongXY(i, j - 1));
                        }
                        down = int.MaxValue;
                    }
                }
                else down = f[i, j];

                f[i, j] = Mathf.Min(left, right, up, down) + 1;
            }
            catch (System.Exception)
            {
                Debug.Log(coordQuene.Count + "队列大小");
                throw;
            }

            //要换直线距离法，找最大（九格中最小的不在对角的两个值平方和再开方，最小4相邻块+1）

        }
        return f;
    }

    public Vector2[,] GenStepNew(float[,] highs, int radio)
    {
        Vector2[,] steps = new Vector2[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                List<Vector2> temp = new List<Vector2>();
                for (int k = -radio; k < radio + 1; k++)
                {
                    for (int r = -radio; r < radio + 1; r++)
                    {
                        if (i + k >= 0 && i + k < width)
                        {
                            if (j + r >= 0 && j + r < height)
                            {
                                Vector2 t = new Vector2(k, r);
                                temp.Add(t.normalized * (highs[i, j] - highs[i + k, j + r]) * (1f / t.magnitude));
                            }
                        }
                    }
                }
                Vector2 sum = Vector2.zero;
                foreach (var item in temp)
                {
                    sum += item;
                }
                steps[i, j] = sum;
            }
        }
        //stepField = steps;
        return steps;
    }
    public float[,] GenWaterField(float[,] highs, MapPrefabsData data, FieldWriter fWriter)
    {
        float[,] temp = new float[data.config.width, data.config.height];
        
        int chunkFileWid = data.config.width / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);
        int chunkFileHig = data.config.height / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);

        int chunkposOffsetX = data.config.width / Chunk.ChunkSize / 2;//只要地图总大小不小于64*64则不会出问题
        int chunkposOffsetY = data.config.height / Chunk.ChunkSize / 2;//地图中心到边缘的chunk数

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

                        float[,] sead = fWriter.GetChunkSeaDistance(chunkposX, chunkposY);
                        //单独对每个chunk生成
                        for (int p = 0; p < Chunk.ChunkSize; p++)
                        {
                            for (int q = 0; q < Chunk.ChunkSize; q++)
                            {
                                float sd = sead == null ? 0 : sead[p, q];
                                try
                                {
                                    temp[(chunkposX + chunkposOffsetX) * Chunk.ChunkSize + p, (chunkposY + chunkposOffsetY) * Chunk.ChunkSize + q] = sd==0?100:Mathf.Clamp((100f-sd)/2,0,int.MaxValue);
                                }
                                catch (System.Exception)
                                {
                                    Debug.Log(chunkposX + "," + chunkposY);
                                    Debug.Log(p + "," + q);
                                    Debug.Log(chunkposOffsetX + "," + chunkposOffsetY);
                                    Debug.Log(((chunkposX + chunkposOffsetX) * Chunk.ChunkSize + +p) + "," + ((chunkposY + chunkposOffsetY) * Chunk.ChunkSize + q));
                                    throw;
                                }
                            }
                        }

                    }
                }
            }
        }
        //Debug.Log(tbs[0,0]);
        return temp;

        return temp;
    }
    public float[,] GenHeatField(float[,] highs, MapPrefabsData data, FieldWriter fWriter)
    {//需部分外部参数控制
        float[,] temp = new float[data.config.width, data.config.height];
        for (int i = 0; i < data.config.width; i++)
        {
            for (int j = 0; j < data.config.height; j++)
            {
                float perlin = MathCenter.basePerlin(i, j, 123.456f, 20, 512, 10);
                temp[i, j] = perlin + Mathf.Clamp(highs[i, j], 0, 100) * -0.4f + 20;
            }
        }

        return temp;
    }
    public Vector2[,] GenMagicField(float[,] highs, MapPrefabsData data, FieldWriter fWriter)
    {
        return null;
    }
}
