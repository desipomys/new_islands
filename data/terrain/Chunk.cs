using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Text;
using System.IO;

//世界由地形、地形方块、实体方块、实体构成
//地形方块只占一格且不携带额外数据，实体方块可占多格
//建筑方块能放在自身和地形上，大小固定
//每个地形格子0.5倍自身长度高
/// <summary>
/// 数据类
/// </summary>
public class Chunk
{
    public const float BlockSize = 0.5f;//一个block0.5m
    public const int ChunkSize = 32;//32个方块一个chunk
    public const float BlockHeight = 0.25f;//方块高度
    public const int PixlePerBlock = 4;//每格地块几个像素

    #region utility
    /// <summary>
    /// 方块坐标转文件坐标，已测试
    /// </summary>
    /// <param name="cpos"></param>
    /// <returns></returns>
    public static long ChunkPosToBigChunkPos(long cpos)
    {
        int x = cpos.GetX();
        int y = cpos.GetY();
        x = Mathf.FloorToInt(x*1.0f / Container_Terrain.chunkSizePerFile);
        y = Mathf.FloorToInt(y*1.0f / Container_Terrain.chunkSizePerFile);
        return XYHelper.ToLongXY(x, y);
    }
    /// <summary>
    /// 世界坐标转方块坐标(以0,0为原点的世界方块坐标),世界坐标在方块内部即为该方块坐标
    /// </summary>
    /// <param name="worldPosi"></param>
    /// <returns></returns>
    public static Vector3Int WorldPosToBlockPos(Vector3 worldPosi)
    {
        Vector3Int temp=new Vector3Int();
        temp.x=Mathf.FloorToInt(worldPosi.x/BlockSize) ;
        temp.y=Mathf.FloorToInt(worldPosi.y/BlockHeight);
        temp.z=Mathf.FloorToInt(worldPosi.z/BlockSize);
        return temp;
    }
    public static Vector3Int WorldPosToBlockPos(Vector3 worldPosi,bool excludeBound)
    {
        Vector3Int temp = new Vector3Int();
       

        temp.x = Mathf.FloorToInt(worldPosi.x / BlockSize);
        temp.y = Mathf.FloorToInt(worldPosi.y / BlockHeight);
        temp.z = Mathf.FloorToInt(worldPosi.z / BlockSize);
        return temp;
    }
    /// <summary>
    /// 方块坐标转世界坐标，取方块底面中心,如0,0,0转为0.25,0,0.25;1,1,1转为0.75,0.25,0.75
    /// </summary>
    /// <param name="blockPosi"></param>
    /// <returns></returns>
    public static Vector3 BlockPosToWorldPos(Vector3Int blockPosi)
    {
        Vector3 temp = new Vector3();
        temp.x=blockPosi.x*BlockSize+0.5f*BlockSize;
        temp.y=blockPosi.y*BlockHeight;
        temp.z=blockPosi.z*BlockSize+0.5f*BlockSize;
        return temp;
    }

    public static Vector3Int GlobalBPosToLocalBPos(Vector3Int gbpos)
    {
        gbpos.x =(int)Mathf.Repeat( gbpos.x , Chunk.ChunkSize);
        
        //gbpos.y= (int)Mathf.Repeat(gbpos.y, Chunk.ChunkSize);
        gbpos.z = (int)Mathf.Repeat(gbpos.z, Chunk.ChunkSize);
        return gbpos;
    }
    public static Vector2Int GlobalBPosToLocalBPos(Vector2Int gbpos)
    {
        gbpos.x = (int)Mathf.Repeat(gbpos.x, Chunk.ChunkSize);

        //gbpos.y= (int)Mathf.Repeat(gbpos.y, Chunk.ChunkSize);
        gbpos.y = (int)Mathf.Repeat(gbpos.y, Chunk.ChunkSize);
        return gbpos;
    }
    /// <summary>
    /// 世界坐标转当前block高度
    /// </summary>
    /// <param name="worldPosi"></param>
    /// <returns></returns>
    public static float WorldPosToBlockH(Vector3 worldPosi)
    {
        //if(isbl)
        Vector3Int v = Chunk.WorldPosToBlockPos(worldPosi);
        int h= EventCenter.WorldCenter.GetParm<long, int>(nameof(EventNames.GetTerrainHAt), XYHelper.ToLongXY((int)v.x, (int)v.z));
        if (h == int.MaxValue) return 0;
        else return h*Chunk.BlockHeight;
    }
    public static Vector3 ChunkAndBlockPosToBlockPos(int cx,int cy,int bx,int by)
    {
        Vector3 temp=new Vector3((bx + cx * Chunk.ChunkSize),0,(by + cy * Chunk.ChunkSize));
        return temp;
    }
    /// <summary>
    /// 已测试
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static long WorldPosToChunkPos(Vector3 pos)//
    {
        int x=Mathf.FloorToInt(pos.x/(BlockSize*ChunkSize));
        int y=Mathf.FloorToInt(pos.z/(BlockSize*ChunkSize));
        return XYHelper.ToLongXY(x,y);
    }
    public static long WorldPosToChunkPos(Vector2 pos)//
    {
        int x = Mathf.FloorToInt(pos.x / (BlockSize * ChunkSize));
        int y = Mathf.FloorToInt(pos.y / (BlockSize * ChunkSize));
        return XYHelper.ToLongXY(x, y);
    }
    public static Vector3 ChunkPosToWorldPos(long xy)
    {
        Vector3 temp=new Vector3(xy.GetX()*(BlockSize*ChunkSize),0,xy.GetY()*(BlockSize*ChunkSize));
        return temp;
    }
    public static long BlockPosToChunkPos(int x,int y)
    {
         //Debug.Log(x*1.0f / ( ChunkSize)+":"+y*1.0f / ( ChunkSize));
         x = Mathf.FloorToInt(x*1.0f / ( ChunkSize));
         y = Mathf.FloorToInt(y*1.0f / ( ChunkSize));
        return XYHelper.ToLongXY(x, y);
    }

    #endregion

    bool dirtyFlag = false;
    //地图限高32767
    public T_Block[,] tblocks;
    //public Dictionary<int,int> blockIndex;
    public Dictionary<int, B_Block> blocks = new Dictionary<int, B_Block>();//key值xxxxxxxx|zzzzzzzz|yyyyyyyyyyyyyyyyy前8位为x，中8位为z，其他为y，改版后chun只存占据单个位置的方块
    //索引与bblock,因为存在一个bblock位于多个格子的情况因此需另设索引完成多对一
    //public Dictionary<int,T_Decoration> decos;
    //public long[] entitys;//真正的实体数据不在这里存储

    public bool CanPlaceAt(int xyh)
    {
        int[] temp = xyh.GetCoord3();
        return CanPlaceAt(temp[0], temp[1], temp[2]);
    }
    public bool CanPlaceAt(int x,int y,int h)
    {
        if(x<0||x>=Chunk.ChunkSize)return false;
        if(y<0||y>=Chunk.ChunkSize)return false;
        if(h<-32767||h>32767)return false;
        if (h < tblocks[x, y].H) return false;//在地形底下
        if(blocks!=null)
            if(blocks.ContainsKey(x<<24+y<<16+h))return false;
        return true;
    }
    public bool CanPlaceAt(Vector3Int pos)
    {
        return CanPlaceAt(pos.x, pos.z, pos.y);
    }
    /// <summary>
    /// 仅测试chunk内部是否能放下，不考虑跨chunk方块的情况
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="h"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public bool InnerCanPalceAt(int x,int y,int h,Vector3 size)
    {
        if(x<0||x>=Chunk.ChunkSize)return false;
        if(y<0||y>=Chunk.ChunkSize)return false;
        if(h<-32767||h>32767)return false;

        int sizex=(int)size.x;
        int sizey=(int)size.y;
        int sizez=(int)size.z;
        for (int i = 0; i < sizex; i++)//不考虑位于两个chunk之间的情况
        {
            for (int j = 0; j < sizey; j++)
            {
                for (int k = 0; k < sizez; k++)
                {
                    if(blocks.ContainsKey((x+i)<<24+(y+k)<<16+(h+j)))return false;
                }
            }
        }
        return true;
    }

    //地图限高32767

        public bool HaveBBlock(int x,int y,int h)
    {
        return blocks.ContainsKey(XYHelper.ToCoord3(x, y, h));
    }
    public bool HaveBBlock(Vector3Int globalBlockpos)
    {
        globalBlockpos = GlobalBPosToLocalBPos(globalBlockpos);
        return HaveBBlock(globalBlockpos.x, globalBlockpos.z, globalBlockpos.y);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">方块内部坐标(左下为0,0)</param>
    /// <param name="y">同X</param>
    /// <param name="z">以坐标0为0的高度</param>
    /// <returns></returns>
    public B_Block GetBBlock(int x,int y,int h)
    {
        int xyz= XYHelper.ToCoord3(x,y,h);
        return GetBBlock(xyz);
    }
    public B_Block GetBBlock(Vector3Int vi)
    {
        return GetBBlock(vi.x, vi.z, vi.y);
    }
    public B_Block GetBBlock(int xyz)
    {
        
        if (blocks.ContainsKey(xyz))
        {
            return blocks[xyz];
        }
        return null;
    }
    public T_Block GetTBlock(int x,int y)
    {
        if (x >= 0 && y >= 0 && x < Chunk.ChunkSize && y < Chunk.ChunkSize)return tblocks[x, y]; 
        return null;
    }
    public int GetTH(int x,int y)
    {
        if (x >= 0 && y >= 0 && x < Chunk.ChunkSize && y < Chunk.ChunkSize) return tblocks[x, y].H;
        return -32767;
    }
    public T_BIOME GetTBio(int x,int y)
    {
        if (x >= 0 && y >= 0 && x < Chunk.ChunkSize && y < Chunk.ChunkSize) return tblocks[x, y].bio;
        return T_BIOME.none;
    }
    public T_Material GetTMat(int x,int y)
    {
        if (x >= 0 && y >= 0 && x < Chunk.ChunkSize && y < Chunk.ChunkSize) return tblocks[x, y].mat;
        return T_Material.none;
    }
    public bool GetDirty() { return dirtyFlag; }
    public void ResetDirty() { dirtyFlag = false; }

    public void SetBBlock(Vector3Int vi,B_Block bb)
    {
        
        SetBBlock(vi.x, vi.z, vi.y, bb);
    }
    public void SetBBlock(int x, int y, int h,B_Block bb)
    {
        int xyz = XYHelper.ToCoord3(x, y, h);
        SetBBlock(xyz,bb);
    }
    public void SetBBlock(int xyz,B_Block bb)
    {
        if (bb == null) blocks.Remove(xyz);
        else
        {
            if (blocks != null)
            {
                if (blocks.ContainsKey(xyz))
                    blocks[xyz] = bb;
                else blocks.Add(xyz, bb);
            }
            else
            {
                blocks = new Dictionary<int, B_Block>();
                blocks.Add(xyz, bb);
            }
        }
        
    }

    public void SetBBlocks(Dictionary<int, B_Block> dat)
    {
        blocks = dat;
    }
    public void SetTBlocks(T_Block[,] tb)
    {
        if (tb == null) return;
        if (tb.GetLength(0) != Chunk.ChunkSize || tb.GetLength(1) != Chunk.ChunkSize) return;
        dirtyFlag = true;
        tblocks = tb;
    }
    public void SetTH(int x,int y,int h)
    {
        if (x >= 0 && y >= 0 && x < Chunk.ChunkSize && y < Chunk.ChunkSize)
        {
            dirtyFlag = true;
            tblocks[x, y].H = h;
        }
    }
    public void SetTBio(int x, int y, T_BIOME b)
    {
        if (x >= 0 && y >= 0 && x < Chunk.ChunkSize && y < Chunk.ChunkSize)
        {
            dirtyFlag = true;
            tblocks[x, y].bio = b;
        }
    }
    public void SetTMat(int x, int y, T_Material m)
    {
        if (x >= 0 && y >= 0 && x < Chunk.ChunkSize && y < Chunk.ChunkSize)
        {
            dirtyFlag = true;
            tblocks[x, y].mat = m;
        }
    }
    public void SetTData(int x,int y,int h,T_BIOME tb,T_Material tm,int sub)
    {
        if (x >= 0 && y >= 0 && x < Chunk.ChunkSize && y < Chunk.ChunkSize)
        {
            dirtyFlag = true;
            tblocks[x, y].H=h;
            tblocks[x, y].bio = tb;
            tblocks[x, y].mat = tm;
            tblocks[x, y].sub = sub;
        }
    }

   

        public void Init()
    {
        tblocks = new T_Block[Chunk.ChunkSize, ChunkSize];
        for (int i = 0; i < ChunkSize; i++)
        {
            for (int j = 0; j < ChunkSize; j++)
            {
                tblocks[i, j] = new T_Block();
            }
        }
    }


    #region 序列化
    //尚不可用
    public override string ToString()
    {
        DataWarper temp=new DataWarper(this);
        return temp.ToString();
    }
    public static Chunk FromString(string data)//尚不可用
    {
        try
        {
            return (Chunk)DataWarper.FromString(data).UnLoad();
        }
        catch (System.Exception)
        {
            Debug.Log(data+"   \n解析chunk错误");
            return null;
        }
        
    }
    #endregion
}
public enum T_BIOME:byte
{
    none,beach, grassland, mouthian, desert,forest,snow,savage,sea,river,undersea
}
public enum T_Material:byte
{//地形的特性与此材质绑定
    none,sand, stone, dirt,wood,redwood, grassdirt,snowdirt,graval,lavarock,
    brick,cerment,  farmland, iron,copper,lead, gold, alloy,
    ore_iron, ore_lead, ore_copper, ore_gold, ore_crystal, ore_coal, ore_salt,
    flesh,
    crystal,

}
public enum B_Material:byte
{
    none=0,
    sand,   grassdirt, graval, lavarock,ore_iron, ore_lead, ore_copper, ore_gold, ore_crystal, ore_coal, ore_salt,//nature
    wood=50,stone,dirt,grass,cloth,bone,//T0
    brick=100, cerment,redwood, iron, gold,lead,copper,sliver, flesh,shell,//T1
    steel=150,steelcerment,mthystl,dragon,//T2
    alloy=180,//T3
    
    

    water=200, blood, lava,clorium,sulfacid,oil
}

[JsonConverter(typeof(TBlockConverter))]
public class T_Block//地形方块，不含坐标数据
{
    public int H, sub;//subid 血量
    public T_BIOME bio;
    public T_Material mat;

    public T_Block(){}
    public T_Block(int h){H=h;}
    public byte[] ToByte()
    {
        byte[] b = new byte[10];
        byte[] Hb = BitConverter.GetBytes(H);//还是会产生几个new的byte数组垃圾
        Array.ConstrainedCopy(Hb, 0, b, 0, Hb.Length);
        byte[] subb = BitConverter.GetBytes(sub);
        Array.ConstrainedCopy(subb, 0, b, Hb.Length, subb.Length);
        byte[] biob = new byte[]{(byte)bio};
        Array.ConstrainedCopy(biob, 0, b, Hb.Length+ subb.Length, 1);
        byte[] matb =new byte[]{(byte)mat};
        Array.ConstrainedCopy(matb, 0, b, Hb.Length + subb.Length+ 1, 1);

        return b;
    }
    public static T_Block FromByte(Byte[] b)
    {
        if (b.Length < 10) { throw new Exception("用于反序列化t_block的byte数组长度不够"); }
        T_Block t = new T_Block();
        //byte[] temp = new byte[4];
        //Array.ConstrainedCopy(b, 0, temp, 0, temp.Length);
        t.H = BitConverter.ToInt32(b,0);
        t.sub = BitConverter.ToInt32(b, 4);
        t.bio=(T_BIOME)b[8];
        t.mat=(T_Material)b[9];

        return t;
    }
}
public enum DIR:byte//建筑方块朝向
{//0无，1左边，2右边，3=0
//00|00|00,代表前后，上下，左右，00为中心，01为前，10为左，11为中心
    none=0, front = 1,back=2, up = 4,down=8,left = 16,right=32
    
}

[JsonConverter(typeof(BBlockConverter))]
[System.Serializable]
public class B_Block//建筑方块运行时数据，含坐标，属于小方块
{
    //public int x, y, z;//全局坐标
    public DIR dir;
    /// <summary>
    /// 真实材质
    /// </summary>
    public B_Material mat;//
    public int sub,max;//当前血量和总血量
    /// <summary>
    /// 真实方块mesh类型
    /// </summary>
    public int mesh;
    /// <summary>
    /// 显示材质
    /// </summary>
    public int mat2;//

    bool[] nearby=new bool[6];//六面上有无bblock（会忽略液体）,用于坍塌运算
    
    public B_Block() { }
    public B_Block(B_Material bm) { mat = bm; }
    public B_Block(B_Material bm,int m) { mat = bm; max = m;sub = m; }

    public byte[] ToByte()
    {
        
        byte[] dirb = new byte[]{(byte)dir};//还是会产生几个new的byte数组垃圾
        byte[] matb = new byte[]{(byte)mat};
        byte[] subb = BitConverter.GetBytes(sub);
        byte[] maxb = BitConverter.GetBytes(max);
        byte[] meshb = BitConverter.GetBytes(mesh);
        byte[] mat2b = BitConverter.GetBytes(mat2);
        byte[] b = new byte[dirb.Length+matb.Length+subb.Length+maxb.Length+meshb.Length+mat2b.Length];

        int ind=0;
        Array.ConstrainedCopy(dirb, 0, b, ind, dirb.Length);
        ind+=dirb.Length;
        Array.ConstrainedCopy(matb, 0, b, ind, matb.Length);
        ind+=matb.Length;
        Array.ConstrainedCopy(subb, 0, b, ind, subb.Length);
        ind+=subb.Length;
        Array.ConstrainedCopy(maxb, 0, b, ind, maxb.Length);
        ind+=maxb.Length;
        Array.ConstrainedCopy(meshb, 0, b, ind, meshb.Length);
        ind+=meshb.Length;
        Array.ConstrainedCopy(mat2b, 0, b, ind, mat2b.Length);
        ind+=mat2b.Length;

        return b;
    }
    public static B_Block FromByte(byte[] b)
    {
        try
        {
            if (b.Length < 18) { throw new Exception("用于反序列化b_block的byte数组长度不够"); }
            B_Block t = new B_Block();
            t.dir=(DIR)b[0];
            t.mat=(B_Material)b[1];
            t.sub=BitConverter.ToInt32(b,2);
            t.max=BitConverter.ToInt32(b,6);
            t.mesh=BitConverter.ToInt32(b,10);
            t.mat2=BitConverter.ToInt32(b,14);

           
          
            return t;
        }
        catch (System.Exception)
        {
            Debug.Log(b+"   \n解析B_Block错误");
            return null;
        }
        
    }
}
[Obsolete]
public class E_Block
{
    public string nam;
    public string pic;
    public int size;
    public string des;

}

[Obsolete]
public class T_Decoration
{//树，草等属于此类
    //public int x, y;//全局坐标
    public int type;
    public DIR dir;
    public int subid;
}

/// <summary>
/// 施工中，尝试自定义占用空间更少的chunk序列化/反序列化器
/// </summary>
public class JsonChunkConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        StringBuilder sb = new StringBuilder();
        if(value is T_Block temp)
        {
            byte[] b = temp.ToByte();
        }
        writer.WriteValue($"[{sb.ToString().Trim(',')}]");
    }
}