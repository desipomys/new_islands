using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// random不承担写入功能
/// </summary>
public class TerrainBiomeMatRandom
{
    //生成地形biome

    //只需根据传入高度、场数据对外提供获取全图biome[,],tblockMat[,]方法，直接返回整个地图的bio[,]
    public T_BIOME[,] GenBiome(float[,] allChunkH, float[,] water, float[,] heat, Vector2[,] magic, Vector2[,] step, MapPrefabsData data)
    {//biome只与地形高度导数和基perlin有关
        T_BIOME[,] tbs = new T_BIOME[allChunkH.GetLength(0), allChunkH.GetLength(1)];
        for (int i = 0; i < allChunkH.GetLength(0); i++)
        {
            for (int j = 0; j < allChunkH.GetLength(1); j++)
            {

                if (allChunkH[i, j] > 8) tbs[i, j] = T_BIOME.mouthian;
                else if (allChunkH[i, j] > 0) tbs[i, j] = T_BIOME.grassland;
                else tbs[i, j] = T_BIOME.beach;
            }
        }
        return tbs;
    }
    public T_Material[,] GenMat(float[,] allChunkH, float[,] water, float[,] heat, Vector2[,] magic, Vector2[,] step, MapPrefabsData data)
    {
        //之后需升级为无参数方法，自己去writer找场数据值

        return null;
    }
    #region 生成方法
    T_BIOME get_BIOME(float h, float water, float heat, Vector2 step, Vector2 mag, MapPrefabsData data)
    {
        if (h > data.config.sea + 10) return T_BIOME.mouthian;
        else if (h > data.config.sea+5) return T_BIOME.grassland;
        else if(h>data.config.sea-4) return T_BIOME.beach;
        else return T_BIOME.undersea;
    }
    /// <summary>
    /// 需从某个loader读取生成规则
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="tb"></param>
    /// <param name="h"></param>
    /// <param name="water"></param>
    /// <param name="heat"></param>
    /// <param name="step"></param>
    /// <param name="mag"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    T_Material get_Material(int x,int y,T_BIOME tb,float h,float water,float heat, Vector2 step, Vector2 mag, MapPrefabsData data)
    {
        float perlin = MathCenter.basePerlin(x, y, 123.456f, 15, 128, 0);
        switch (tb)
        {
            case T_BIOME.beach:
                return T_Material.sand;
                break;
            case T_BIOME.grassland:
                return T_Material.grassdirt;
                break;
            case T_BIOME.mouthian:
                return T_Material.stone;
                break;
            case T_BIOME.desert:
                return T_Material.sand;
                break;
            case T_BIOME.forest:
                return T_Material.grassdirt;
                break;
            case T_BIOME.snow:
                return T_Material.snowdirt;
                break;
            case T_BIOME.savage:
                return T_Material.grassdirt;
                break;
            case T_BIOME.sea:
                break;
            case T_BIOME.river:
                break;
            case T_BIOME.undersea:
                break;
            default:
                break;
        }
        return T_Material.sand;
    }

    #endregion

    #region 高级
    public T_BIOME[,] GenBiome(int wid, int hig, MapPrefabsData data, FieldWriter fWriter, ChunkFileWriter cWriter)
    {
        T_BIOME[,] tbs = new T_BIOME[wid, hig];
        int chunkFileWid = wid / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);
        int chunkFileHig = hig / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);

        int chunkposOffsetX = wid / Chunk.ChunkSize / 2;//只要地图总大小不小于64*64则不会出问题
        int chunkposOffsetY = hig / Chunk.ChunkSize / 2;//地图中心到边缘的chunk数

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
                        int[,] h = cWriter.GetChunkH(chunkposX, chunkposY);
                        float[,] water = fWriter.GetChunkWaterField(chunkposX, chunkposY);
                        float[,] heat = fWriter.GetChunkHeatField(chunkposX, chunkposY);
                        Vector2[,] step = fWriter.GetChunkStepField(chunkposX, chunkposY);
                        Vector2[,] magic = fWriter.GetChunkMagicField(chunkposX, chunkposY);

                        //单独对每个chunk生成
                        for (int p = 0; p < Chunk.ChunkSize; p++)
                        {
                            for (int q = 0; q < Chunk.ChunkSize; q++)
                            {
                                float w = water == null ? 0 : water[p, q];
                                float hea = heat == null ? 0 : heat[p, q];
                                Vector2 ste = step == null ? Vector2.zero : step[p, q];
                                Vector2 mg = magic == null ? Vector2.zero : magic[p, q];

                                try
                                {
                                    tbs[(chunkposX + chunkposOffsetX) * Chunk.ChunkSize + p, (chunkposY + chunkposOffsetY) * Chunk.ChunkSize + q] = get_BIOME(h[p, q], w, hea, ste, mg,data);
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
        return tbs;
    }
    public T_Material[,] GenMat(int wid, int hig, MapPrefabsData data, FieldWriter fWriter, ChunkFileWriter cWriter)
    {
        T_Material[,] tbs = new T_Material[wid, hig];
        int chunkFileWid = wid / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);
        int chunkFileHig = hig / (Chunk.ChunkSize * Container_Terrain.chunkSizePerFile);

        int chunkposOffsetX = wid / Chunk.ChunkSize / 2;//只要地图总大小不小于64*64则不会出问题
        int chunkposOffsetY = hig / Chunk.ChunkSize / 2;//地图中心到边缘的chunk数

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
                        int[,] h = cWriter.GetChunkH(chunkposX, chunkposY);
                        T_BIOME[,] tbios = cWriter.GetChunkBio(chunkposX, chunkposY);
                        float[,] water = fWriter.GetChunkWaterField(chunkposX, chunkposY);
                        float[,] heat = fWriter.GetChunkHeatField(chunkposX, chunkposY);
                        Vector2[,] step = fWriter.GetChunkStepField(chunkposX, chunkposY);
                        Vector2[,] magic = fWriter.GetChunkMagicField(chunkposX, chunkposY);

                        //单独对每个chunk生成
                        for (int p = 0; p < Chunk.ChunkSize; p++)
                        {
                            for (int q = 0; q < Chunk.ChunkSize; q++)
                            {
                                int hc= h == null ? 0 : h[p, q];
                                float w = water == null ? 0 : water[p, q];
                                T_BIOME tb= tbios == null ? 0 : tbios[p, q];
                                float hea = heat == null ? 0 : heat[p, q];
                                Vector2 ste = step == null ? Vector2.zero : step[p, q];
                                Vector2 mg = magic == null ? Vector2.zero : magic[p, q];

                                int xpos = (chunkposX + chunkposOffsetX) * Chunk.ChunkSize + p;
                                int ypos = (chunkposY + chunkposOffsetY) * Chunk.ChunkSize + q;
                                try
                                {
                                    tbs[xpos,ypos] = get_Material(xpos,ypos,tb,hc,w,hea,ste,mg,data);
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
        return tbs;
    }

    #endregion
}
