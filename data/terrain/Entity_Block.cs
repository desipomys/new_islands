using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Text;



[Serializable]
public class Entity_BlockModel//固定的预制数据
{
    public byte x;
    public byte y;
    public byte h;
    public int typ;
    /// <summary>
    /// lv0以下代表自然物品，0以上代表人造物品级别，-1矿，-2石，-3树，-4草
    /// 只是生成顺序不是代表这个eblock真实是草还是树
    /// </summary>
    public int lv;
    /// <summary>
    /// 能否被玩家建造
    /// </summary>
    public bool canBuild;
    /// <summary>
    /// str的名字
    /// </summary>
    public string strTyp;
    /// <summary>
    /// 类别，-1其他，0生产，1工事，3物流、存储
    /// </summary>
    public int classTyp;

    public int[] GetSize(){return new int[3] {Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(h) };}
    public void SetSize(int x,int y,int h) { this.x=(byte)Mathf.Abs(x); this.y = (byte)Mathf.Abs(y); this.h = (byte)Mathf.Abs(h); }
}
/// <summary>
/// posx,y,z代表eblock包围盒左下角的坐标
/// </summary>
[Serializable]
public class Entity_Block//运行时数据,不能继承scriptobj
{
    public int posX;
    public int posY;
    public short posH;
    public int typ;
    public DIR dir;
    public long UUID;//container中保存blockpos到uuid+uuid到evc组件数据映射

    int sizeCache;
    
    public Entity_Block() { }
    public Entity_Block(int t) { typ = t; }
    public Entity_Block(Vector3Int bpos,int typ) { posX = bpos.x;posY = bpos.z;posH = (short)bpos.y;this.typ = typ; }
    public Entity_Block(PlaceEBlockParm parm)
    {
        posX = parm.pos.x; posY = parm.pos.z; posH = (short)parm.pos.y; this.typ = parm.typ;
        dir = parm.dir;
    }

    public long GetChunkPos()
    {
        return Chunk.BlockPosToChunkPos(posX, posY);
    }
    public int GetSize()
    {
        if(sizeCache==0)sizeCache= EventCenter.WorldCenter.GetParm<int, int>(nameof(EventNames.GetEBlockSizeByID),typ);
       return sizeCache;
    }
}