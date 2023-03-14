//读取预制的建筑数据
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;

public class Loader_BuildingPrefabsData : BaseLoader
{
    Dictionary<string,BuildingPrefabsData> allbuildings=new Dictionary<string, BuildingPrefabsData>();

    #region Init
    public override void OnEventRegist(EventCenter e)
    {
       center=e;
       //e.RegistFunc<string,Texture>("StrtoTexture",StrToTexture);
        //Debug.Log("textureloaderinit");
    }

    string path= "JsonData/BuildingPrefabs";
    public override void OnLoaderInit(int prio)
    {
        if(prio!=0)return;
        try
        {
            
            string t=Resources.Load<TextAsset>(path).text;
        
            for(int i=0;i<t.Length;i++)
            {

                //alltextures.Add(t[i].name,t[i]);
                //Debug.Log(t[i].name);
            }
        }
        catch (System.Exception)
        {

            Debug.Log("buildprefabs loader 加载失败");
        }
        
        
    }
#endregion
}

public enum ScatterType
{
    random,normalDis
}
public enum ReFreshType
{
    OnGen,
    OnLoad//当所在chunk被加载
}
public class BuildingGenRule
{
    public string BuildingName;

}
public class EntityGenRule
{
    public string EntityName;
    public string EntityData;
    public string[] EntityCondition;
    public float Rate;
    public int NumLimt;
    public ScatterType sType;
    public ReFreshType rType;
}
public class EntityRandomArea
{
    public Shape3D shape;
    public int width, height, tall;
    public EntityGenRule[] rules;
    
}
/// <summary>
/// 建筑的预制数据
/// </summary>
public class BuildingPrefabsData
{
    /// <summary>
    /// 建筑方块id及其对应位置，位置是相对于自身中心的,vector3中全部使用整数代表方块坐标(block坐标)
    /// eblockfile中uuid都是从0开始
    /// </summary>
    public int width,height,tall;
    
    //public Dictionary<int,Vector3Int[]> bblockPosi;

    public EntityBlockFile eblocks;

    public Chunk[,] chunks;//地形高度偏移+bblock

    //实体组

    //建筑自身内部事件

    /// <summary>
    /// 该建筑在以center为中心时，在chunkpos代表的chunk中是否有方块
    /// </summary>
    /// <param name="center">block坐标</param>
    /// <param name="chunkPos">chunk坐标</param>
    /// <returns></returns>
    public bool NeedLoad(Vector3 center,Vector3 chunkPos)
    {/*
        bool need=false;
        foreach (var item in bblockPosi)
        {
            for (int i = 0; i < item.Value.Length; i++)
            {
                Vector3 temp=center+item.Value[i];
                temp.y=0;
                if(temp.x<chunkPos.x*Chunk.ChunkSize+Chunk.ChunkSize/2&&temp.x>chunkPos.x*Chunk.ChunkSize-Chunk.ChunkSize/2)
                {
                    if(temp.z<chunkPos.z*Chunk.ChunkSize+Chunk.ChunkSize/2&&temp.z>chunkPos.z*Chunk.ChunkSize-Chunk.ChunkSize/2)
                    {
                        need=true;
                        return need;
                    }
                }
            }
        }
        */
        return false;
    }
}
