using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;


public class ChunkObject : MonoBehaviour, IEventRegister, IPoolable
{
    public static Dictionary<B_Material, Base_BBlockBehavious> BBlockBehavious = new Dictionary<B_Material, Base_BBlockBehavious>();//每种bblock对于不同事件的响应逻辑

    //放在ingame场景，负责显示地形mesh和bblock，少量逻辑转发
    EventCenter center;

    Dictionary<int, BaseBuildingBlock> blocks;

    GameObject defaultBBlock;
    GameObject waterMesh;
    Chunk data;

    public IObjectPool Pool { get { return pool; } set { pool = value; } }
    IObjectPool pool;


    GameObject grassMeshObj4;
    Mesh grassMesh4;
    Material grassmat4;
    Matrix4x4[] grassMatrix4;

    ///===gpuinstance不能超过1024个mesh的临时解决方案
    GameObject grassMeshObj7;
    Mesh grassMesh7;
    Material grassmat7;
    Matrix4x4[] grassMatrix7;

    GameObject grassMeshObj10;
    Mesh grassMesh10;
    Material grassmat10;
    Matrix4x4[] grassMatrix10;

    /// =======

    public int x, y, mapWid, mapHig;//chunk坐标
    int edgeLoaded = 0;

    public void OnEventRegist(EventCenter e)
    {
        InitBBlockBehavious();
        center = e;
        defaultBBlock = EventCenter.WorldCenter.GetParm<GameObject>(nameof(EventNames.GetDefaultBBlockObj));
        waterMesh = transform.GetChild(0).gameObject;
        //throw new NotImplementedException();
    }

    public void AfterEventRegist()
    {
        //throw new NotImplementedException();
    }
    void InitBBlockBehavious()
    {//反射获取所有base_bblockBehavious的子类，组成字典
        if(BBlockBehavious.Count<=0)
        {
            Assembly assembly = this.GetType().Assembly;
            Type[] types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(Base_BBlockBehavious)) && !type.IsAbstract)
                {
                    Base_BBlockBehavious bc = (Base_BBlockBehavious)assembly.CreateInstance(type.Name);
                    bc.Init();
                    BBlockBehavious.Add(bc.mat, bc);
                }
            }
            BBlockBehavious.Add(B_Material.none, new Base_BBlockBehavious());
        }
    }

    public void Init(Chunk c, int x, int y)
    {
        if (data == c && !data.GetDirty()) { return; }
        grassMeshObj4 = Resources.Load<GameObject>("Prefabs/terrain/InstanceGrass_4");
        grassMesh4 = grassMeshObj4.GetComponent<MeshFilter>().sharedMesh;
        grassmat4 = grassMeshObj4.GetComponent<MeshRenderer>().sharedMaterial;

        grassMeshObj7 = Resources.Load<GameObject>("Prefabs/terrain/InstanceGrass_7");
        grassMesh7 = grassMeshObj7.GetComponent<MeshFilter>().sharedMesh;
        grassmat7 = grassMeshObj7.GetComponent<MeshRenderer>().sharedMaterial;

        grassMeshObj10 = Resources.Load<GameObject>("Prefabs/terrain/InstanceGrass_10");
        grassMesh10 = grassMeshObj10.GetComponent<MeshFilter>().sharedMesh;
        grassmat10 = grassMeshObj10.GetComponent<MeshRenderer>().sharedMaterial;


        this.x = x; this.y = y;
        data = c;
        //按数据生成地形和blocks
        long size = EventCenter.WorldCenter.GetParm<long>(nameof(EventNames.GetMapSize));
        mapWid = size.GetX();
        mapHig = size.GetY();

        genMesh();
        genDecoration();

        genLiqud();

        genBBlock();
        /*foreach (var item in blocks)
        {
           EventCenter[] temp= item.Value.GetComponents<EventCenter>();
           for (int i = 0; i < temp.Length; i++)
           {
               temp[i].SetFriendCenter(center);//地形上物体除了自己有事件中心外全部使用chunkobject作为友事件中心
           }
        }*/
    }



    //液体mesh生成
    void genLiqud()
    {

        Mesh m = waterMesh.GetComponent<MeshFilter>().mesh;
        if (m == null || m.vertices == null || m.vertices.Length == 0)
            GenWaterMesh_New();
    }
    void GenWaterMesh_New()
    {
        /*
                 * 先根据水量和水的高度，邻接水量，生成顶面
                 * */
        List<Vector3> vec = new List<Vector3>();//如果已经有mesh则获取现有mesh的vec
        List<int> tri = new List<int>();
        List<Vector2> UVS = new List<Vector2>();
        List<Vector3> NORMAls = new List<Vector3>();

        Dictionary<int, B_Block> copy = new Dictionary<int, B_Block>(data.blocks);
        List<int> temp = new List<int>(copy.Keys);
        for (int i = 0; i < temp.Count; i++)
        {
            if (!copy.ContainsKey(temp[i])) continue;
            if (copy[temp[i]].mat.IsLiqud())
            {
                //上方
                for (int j = 0; j < 32767; j++)
                {
                    int ctemp = temp[i].AddCoord3H(j);
                    if (!copy.ContainsKey(ctemp))
                    {
                        //i+j为当前顶面,temp[0]=x,temp[1]+j=y,temp[2]=h
                        int[] xyz = temp[i].GetCoord3();
                        vec.Add(new Vector3(xyz[0] * Chunk.BlockSize, (xyz[2] + j - 1 + getWaterSurfaceH(ctemp.AddCoord3H(-1))) * Chunk.BlockHeight - 0.02f, (xyz[1]) * Chunk.BlockSize));
                        vec.Add(new Vector3(xyz[0] * Chunk.BlockSize, (xyz[2] + j - 1 + getWaterSurfaceH(ctemp.AddCoord3H(-1))) * Chunk.BlockHeight - 0.02f, (xyz[1] + 1) * Chunk.BlockSize));
                        vec.Add(new Vector3((xyz[0] + 1) * Chunk.BlockSize, (xyz[2] + j - 1 + getWaterSurfaceH(ctemp.AddCoord3H(-1))) * Chunk.BlockHeight - 0.02f, (xyz[1] + 1) * Chunk.BlockSize));
                        vec.Add(new Vector3((xyz[0] + 1) * Chunk.BlockSize, (xyz[2] + j - 1 + getWaterSurfaceH(ctemp.AddCoord3H(-1))) * Chunk.BlockHeight - 0.02f, (xyz[1]) * Chunk.BlockSize));

                        tri.Add(vec.Count - 4);
                        tri.Add(vec.Count - 3);
                        tri.Add(vec.Count - 1);

                        tri.Add(vec.Count - 3);
                        tri.Add(vec.Count - 2);
                        tri.Add(vec.Count - 1);

                        UVS.Add(Vector2.zero);
                        UVS.Add(new Vector2(0, 1f));
                        UVS.Add(new Vector2(1, 1));
                        UVS.Add(new Vector2(1, 0));

                        NORMAls.Add(new Vector3(0, 1, 0));
                        NORMAls.Add(new Vector3(0, 1, 0));
                        NORMAls.Add(new Vector3(0, 1, 0));
                        NORMAls.Add(new Vector3(0, 1, 0));
                        break;
                    }
                    else
                    {
                        copy.Remove(temp[i].AddCoord3H(j));
                    }
                }

                //下方
                for (int k = 0; k < 32767; k++)
                {
                    if (!copy.ContainsKey(temp[i].AddCoord3H(-k)))
                    {
                        //i-k为当前底面
                        break;
                    }
                    else
                    {
                        copy.Remove(temp[i].AddCoord3H(-k));
                    }
                }


                copy.Remove(temp[i]);
            }
        }


        Mesh m = new Mesh();
        m.vertices = vec.ToArray();
        m.triangles = tri.ToArray();
        //m.normals = norm.ToArray();
        m.uv = UVS.ToArray();
        //m.RecalculateNormals();
        m.normals = NORMAls.ToArray();

        m.Optimize();

        waterMesh.GetComponent<MeshFilter>().mesh = m;
        MeshRenderer mr = waterMesh.GetComponent<MeshRenderer>();
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        //waterMesh.AddComponent<MeshCollider>();

        mr.material = Resources.Load<Material>("material/TerrainWater");
    }
    float getWaterSurfaceH(int xyh)
    {
        try
        {
            return data.blocks[xyh].sub * 1.0f / 10;//以10为满水
        }
        catch (Exception)
        {
            return 0;
        }

    }
    /// <summary>
    /// 未优化
    /// </summary>
    void genBBlock()
    {
        /* if (data.blocks.Count > blocks.Count)//需要的basebblock比当前拥有的多
         {
             int need = data.blocks.Count - blocks.Count;
             List<int> dataCoords = new List<int>(data.blocks.Keys);
             int index = 0;
             foreach (var item in blocks)
             {
                 int[] coord3 = dataCoords[index].GetCoord3();
                 item.Value.Init(data.blocks[dataCoords[index]], coord3[0], coord3[1], coord3[2], center);
                 index++;
             }
             for (int i = 0; i < need; i++)//生成新basebblock(bblock的游戏中gobj实例)
             {
                 GameObject g = GameMainManager.CreateGameObject(defaultBBlock, transform.position, transform.rotation, transform);
                 blocks.Add(dataCoords[index+i],g.GetComponent<BaseBuildingBlock>());
                 int[] coord3 = dataCoords[index + i].GetCoord3();
                 g.GetComponent<BaseBuildingBlock>().Init(data.blocks[dataCoords[index + i]], coord3[0], coord3[1], coord3[2], center);
             }
         }
         else
         {
             int flow = blocks.Count - data.blocks.Count;//当前多出的bblock数量
             List<int> bblockCoords = new List<int>(blocks.Keys);
             int index = 0;
             foreach (var item in data.blocks)
             {
                 int[] coord3 = bblockCoords[index].GetCoord3();
                 blocks[bblockCoords[index]].Init(item.Value, coord3[0], coord3[1], coord3[2], center);
                 index++;
             }
         }*/
        if (blocks != null)
        {
            foreach (var item in blocks)
            {
                Destroy(item.Value.gameObject);
            }
            blocks.Clear();
        }
        else
        {
            blocks = new Dictionary<int, BaseBuildingBlock>();
        }
        if (data.blocks != null)
        {
            foreach (var item in data.blocks)
            {
                if (item.Value.mat.IsLiqud()) continue;//临时措施，最后要使用loader+文件来判断是不是液体
                GameObject g = GameMainManager.CreateGameObject(defaultBBlock, transform.position, transform.rotation, transform);
                blocks.Add(item.Key, g.GetComponent<BaseBuildingBlock>());
                int[] coord3 = item.Key.GetCoord3();
                //Debug.Log(coord3[0] + ":" + coord3[1] + ":" + coord3[2]);
                g.GetComponent<BaseBuildingBlock>().Init(item.Value, coord3[0], coord3[1], coord3[2], center);
            }
        }
    }

    void GenMeshEdge()
    {
        long uppos = XYHelper.ToLongXY(0 + x * Chunk.ChunkSize, y * Chunk.ChunkSize + Chunk.ChunkSize);
        int testupH = EventCenter.WorldCenter.GetParm<long, int>(nameof(EventNames.GetTerrainHAt), uppos);
        //if (testupH == int.MaxValue) return;

        uppos = XYHelper.ToLongXY(x * Chunk.ChunkSize + Chunk.ChunkSize, y * Chunk.ChunkSize + 0);
        int testrightH = EventCenter.WorldCenter.GetParm<long, int>(nameof(EventNames.GetTerrainHAt), uppos);//右侧
        if (testupH == int.MaxValue && testrightH == int.MaxValue) { gameObject.AddComponent<MeshCollider>(); return; }

        Mesh m = gameObject.GetComponent<MeshFilter>().sharedMesh;
        //点

        Vector3[] tempnew = m.vertices;
        if (tempnew.Length != 4224)
        {
            tempnew = new Vector3[4224];
            Array.ConstrainedCopy(m.vertices, 0, tempnew, 0, 4096);
        }
        //List<Vector3> newpoint = new List<Vector3>(tempnew);

        //x轴右侧边界
        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            long currentUpBlockPos = XYHelper.ToLongXY(x * Chunk.ChunkSize + Chunk.ChunkSize, y * Chunk.ChunkSize + i);
            int rightH = EventCenter.WorldCenter.GetParm<long, int>(nameof(EventNames.GetTerrainHAt), currentUpBlockPos);
            if (rightH == int.MaxValue) rightH = data.tblocks[Chunk.ChunkSize - 1, i].H;

            tempnew[4096 + 2 * (i) + 0].x = Chunk.BlockSize * Chunk.ChunkSize;
            tempnew[4096 + 2 * (i) + 0].y = rightH * Chunk.BlockHeight;
            tempnew[4096 + 2 * (i) + 0].z = i * Chunk.BlockSize;

            tempnew[4096 + 2 * (i) + 1].x = Chunk.BlockSize * Chunk.ChunkSize;
            tempnew[4096 + 2 * (i) + 1].y = rightH * Chunk.BlockHeight;
            tempnew[4096 + 2 * (i) + 1].z = (i + 1) * Chunk.BlockSize;
            //newpoint.Add(new Vector3( Chunk.BlockSize * Chunk.ChunkSize, rightH * Chunk.BlockHeight, i * Chunk.BlockSize));
            //newpoint.Add(new Vector3( Chunk.BlockSize * Chunk.ChunkSize, rightH * Chunk.BlockHeight, (i + 1) * Chunk.BlockSize));
        }
        //z轴上方边界
        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            //Chunk.BlockPosToWorldPos(new Vector3())
            long currentUpBlockPos = XYHelper.ToLongXY(i + x * Chunk.ChunkSize, y * Chunk.ChunkSize + Chunk.ChunkSize);
            int upH = EventCenter.WorldCenter.GetParm<long, int>(nameof(EventNames.GetTerrainHAt), currentUpBlockPos);
            //if (Mathf.Abs(upH - data.tblocks[i, Chunk.ChunkSize - 1].H) > 1) { Debug.Log("偏差过大" + upH); }
            if (upH == int.MaxValue) upH = data.tblocks[i, Chunk.ChunkSize - 1].H;

            tempnew[4096 + 2 * Chunk.ChunkSize + 2 * (i) + 0].x = i * Chunk.BlockSize;
            tempnew[4096 + 2 * Chunk.ChunkSize + 2 * (i) + 0].y = upH * Chunk.BlockHeight;
            tempnew[4096 + 2 * Chunk.ChunkSize + 2 * (i) + 0].z = Chunk.BlockSize * Chunk.ChunkSize;

            tempnew[4096 + 2 * Chunk.ChunkSize + 2 * (i) + 1].x = (i + 1) * Chunk.BlockSize;
            tempnew[4096 + 2 * Chunk.ChunkSize + 2 * (i) + 1].y = upH * Chunk.BlockHeight;
            tempnew[4096 + 2 * Chunk.ChunkSize + 2 * (i) + 1].z = Chunk.BlockSize * Chunk.ChunkSize;

            //newpoint.Add(new Vector3(i * Chunk.BlockSize, upH * Chunk.BlockHeight, Chunk.BlockSize * Chunk.ChunkSize));
            //newpoint.Add(new Vector3((i + 1) * Chunk.BlockSize, upH * Chunk.BlockHeight, Chunk.BlockSize * Chunk.ChunkSize));
        }

        //三角
        List<int> newtri = new List<int>(m.triangles);
        //z轴上方边界（包括右上角点）
        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            newtri.Add((Chunk.ChunkSize) * (i + 1) * 4 - 4 + 1);
            newtri.Add((Chunk.ChunkSize) * (Chunk.ChunkSize) * 4 + 2 * Chunk.ChunkSize + 2 * i + 0);
            newtri.Add((Chunk.ChunkSize) * (i + 1) * 4 - 4 + 2);

            newtri.Add((Chunk.ChunkSize) * (i + 1) * 4 - 4 + 2);
            newtri.Add((Chunk.ChunkSize) * (Chunk.ChunkSize) * 4 + 2 * Chunk.ChunkSize + 2 * i + 0);
            newtri.Add((Chunk.ChunkSize) * (Chunk.ChunkSize) * 4 + 2 * Chunk.ChunkSize + 2 * i + 1);

            //chunk内部三角
            if (i < Chunk.ChunkSize - 1)
            {
                newtri.Add((Chunk.ChunkSize) * (i + 1) * 4 - 4 + 3);
                newtri.Add((Chunk.ChunkSize) * (i + 1) * 4 - 4 + 2);
                newtri.Add((Chunk.ChunkSize) * (i + 1 + 1) * 4 - 4 + 0);

                newtri.Add((Chunk.ChunkSize) * (i + 1 + 1) * 4 - 4 + 1);
                newtri.Add((Chunk.ChunkSize) * (i + 1 + 1) * 4 - 4 + 0);
                newtri.Add((Chunk.ChunkSize) * (i + 1) * 4 - 4 + 2);

            }
        }
        //x轴右侧边界（包括右上角点）
        for (int i = 0; i < Chunk.ChunkSize; i++)
        {

            newtri.Add((Chunk.ChunkSize) * (Chunk.ChunkSize - 1) * 4 + 4 * i + 2);
            newtri.Add((Chunk.ChunkSize * Chunk.ChunkSize) * 4 + i * 2 + 0);
            newtri.Add((Chunk.ChunkSize) * (Chunk.ChunkSize - 1) * 4 + 4 * i + 3);

            newtri.Add((Chunk.ChunkSize * Chunk.ChunkSize) * 4 + i * 2 + 1);
            newtri.Add((Chunk.ChunkSize * Chunk.ChunkSize) * 4 + i * 2 + 0);
            newtri.Add((Chunk.ChunkSize) * (Chunk.ChunkSize - 1) * 4 + 4 * i + 2);

            if (i < Chunk.ChunkSize - 1)
            {
                newtri.Add((Chunk.ChunkSize) * (Chunk.ChunkSize - 1) * 4 + 4 * i + 1);
                newtri.Add((Chunk.ChunkSize) * (Chunk.ChunkSize - 1) * 4 + 4 * i + 4 + 0);
                newtri.Add((Chunk.ChunkSize) * (Chunk.ChunkSize - 1) * 4 + 4 * i + 2);

                newtri.Add((Chunk.ChunkSize) * (Chunk.ChunkSize - 1) * 4 + 4 * i + 2);
                newtri.Add((Chunk.ChunkSize) * (Chunk.ChunkSize - 1) * 4 + 4 * i + 4 + 0);
                newtri.Add((Chunk.ChunkSize) * (Chunk.ChunkSize - 1) * 4 + 4 * i + 4 + 3);
            }
        }
        //UV
        List<Vector2> UVS = new List<Vector2>();
        Vector2 temp = Vector2.zero;
        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                UVS.Add(temp);
                UVS.Add(temp + new Vector2(0, 1f / Chunk.ChunkSize / 2));
                UVS.Add(temp + new Vector2(1f / Chunk.ChunkSize / 2, 1f / Chunk.ChunkSize / 2));
                UVS.Add(temp + new Vector2(1f / Chunk.ChunkSize / 2, 0));
                temp.y += 1f / Chunk.ChunkSize;
            }
            temp.y = 0;
            temp.x += 1f / Chunk.ChunkSize;
        }

        for (int i = 0; i < Chunk.ChunkSize; i++)//X右边边界
        {
            UVS.Add(new Vector2(1, i * (1f / Chunk.ChunkSize)));
            UVS.Add(new Vector2(1, (i) * (1f / Chunk.ChunkSize) + (1f / Chunk.ChunkSize / 2)));
        }
        for (int i = 0; i < Chunk.ChunkSize; i++)//Z上方边界
        {
            UVS.Add(new Vector2(i * (1f / Chunk.ChunkSize), 1));
            UVS.Add(new Vector2((i) * (1f / Chunk.ChunkSize) + (1f / Chunk.ChunkSize / 2), 1));
        }

        //法线
        List<Vector3> NORMAls = new List<Vector3>();
        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                NORMAls.Add(new Vector3(0, 1, 0));
                NORMAls.Add(new Vector3(0, 1, 0));
                NORMAls.Add(new Vector3(0, 1, 0));
                NORMAls.Add(new Vector3(0, 1, 0));
            }
        }
        for (int j = 0; j < Chunk.ChunkSize; j++)//Z上方边界
        {
            NORMAls.Add(new Vector3(0, 1, 0));
            NORMAls.Add(new Vector3(0, 1, 0));
        }
        for (int j = 0; j < Chunk.ChunkSize; j++)//X右边边界
        {
            NORMAls.Add(new Vector3(0, 1, 0));
            NORMAls.Add(new Vector3(0, 1, 0));
        }
        //Debug.Log(NORMAls.Count + "'" + newpoint.Count+":"+Mathf.Max(newtri.ToArray())+":"+Mathf.Min(newtri.ToArray()));
        m.Clear();
        m = new Mesh();
        m.vertices = tempnew;
        m.triangles = newtri.ToArray();
        //Debug.Log(UVS.Count + ";" + m.vertices.Length);
        m.uv = UVS.ToArray();
        m.normals = NORMAls.ToArray();
        //m.RecalculateNormals();

        //m.Optimize();

        GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", genmat());
        gameObject.GetComponent<MeshFilter>().sharedMesh = m;
        if (gameObject.GetComponent<MeshCollider>() != null)
        {
            Destroy(gameObject.GetComponent<MeshCollider>());
        }
        gameObject.AddComponent<MeshCollider>();

    }


    void GenMesh_New()
    {

        List<Vector3> vec = new List<Vector3>();//如果已经有mesh则获取现有mesh的vec
        List<int> tri = new List<int>();
        List<Vector2> UVS = new List<Vector2>();
        List<Vector3> NORMAls = new List<Vector3>();

        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                vec.Add(new Vector3(i * Chunk.BlockSize, data.tblocks[i, j].H * Chunk.BlockHeight, j * Chunk.BlockSize));
                vec.Add(new Vector3(i * Chunk.BlockSize, data.tblocks[i, j].H * Chunk.BlockHeight, (j + 1) * Chunk.BlockSize));//1,y+
                vec.Add(new Vector3((i + 1) * Chunk.BlockSize, data.tblocks[i, j].H * Chunk.BlockHeight, (j + 1) * Chunk.BlockSize));//2,x+y+
                vec.Add(new Vector3((i + 1) * Chunk.BlockSize, data.tblocks[i, j].H * Chunk.BlockHeight, j * Chunk.BlockSize));//3,x+

            }
        }

        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                tri.Add((i * Chunk.ChunkSize + j) * 4);

                tri.Add((i * Chunk.ChunkSize + j) * 4 + 1);

                tri.Add((i * Chunk.ChunkSize + j) * 4 + 3);


                tri.Add((i * Chunk.ChunkSize + j) * 4 + 1);

                tri.Add((i * Chunk.ChunkSize + j) * 4 + 2);

                tri.Add((i * Chunk.ChunkSize + j) * 4 + 3);

            }
        }
        //竖直面
        for (int i = 0; i < Chunk.ChunkSize - 1; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize - 1; j++)
            {
                //Z上方面
                tri.Add((i * Chunk.ChunkSize + j) * 4 + 1);
                tri.Add(((i) * Chunk.ChunkSize + j + 1) * 4 + 0);
                tri.Add((i * Chunk.ChunkSize + j) * 4 + 2);


                tri.Add(((i) * Chunk.ChunkSize + j + 1) * 4 + 0);
                tri.Add(((i) * Chunk.ChunkSize + j + 1) * 4 + 3);
                tri.Add((i * Chunk.ChunkSize + j) * 4 + 2);

                //X左方面
                tri.Add((i * Chunk.ChunkSize + j) * 4 + 2);
                tri.Add(((i + 1) * Chunk.ChunkSize + j) * 4 + 0);
                tri.Add((i * Chunk.ChunkSize + j) * 4 + 3);


                tri.Add(((i) * Chunk.ChunkSize + j) * 4 + 2);
                tri.Add(((i + 1) * Chunk.ChunkSize + j) * 4 + 1);
                tri.Add(((i + 1) * Chunk.ChunkSize + j) * 4 + 0);
            }
        }

        //UV

        Vector2 temp = Vector2.zero;
        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                UVS.Add(temp);
                UVS.Add(temp + new Vector2(0, 1f / Chunk.ChunkSize / 2));
                UVS.Add(temp + new Vector2(1f / Chunk.ChunkSize / 2, 1f / Chunk.ChunkSize / 2));
                UVS.Add(temp + new Vector2(1f / Chunk.ChunkSize / 2, 0));
                temp.y += 1f / Chunk.ChunkSize;
            }
            temp.y = 0;
            temp.x += 1f / Chunk.ChunkSize;
        }

        //法线

        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                NORMAls.Add(new Vector3(0, 1, 0));
                NORMAls.Add(new Vector3(0, 1, 0));
                NORMAls.Add(new Vector3(0, 1, 0));
                NORMAls.Add(new Vector3(0, 1, 0));
            }
        }
        //Debug.Log(UVS.Count + "dd" + vec.Count);

        Mesh m = new Mesh();
        m.vertices = vec.ToArray();
        m.triangles = tri.ToArray();
        //m.normals = norm.ToArray();
        m.uv = UVS.ToArray();
        //m.RecalculateNormals();
        m.normals = NORMAls.ToArray();
        // m.RecalculateNormals();
        //m.Optimize();

        gameObject.GetComponent<MeshFilter>().mesh = m;
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        //mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        //gameObject.AddComponent<MeshCollider>();

        mr.material = Resources.Load<Material>("material/terrainDefalut");
        //mr.material.SetTextureScale("_MainTex", new Vector2(Chunk.ChunkSize/2f,Chunk.ChunkSize/2f));
        //mr.material.SetTexture("_MainTex", genmat());
    }

    void GenMesh_UseExist()
    {

        Mesh m = GetComponent<MeshFilter>().mesh;
        Vector3[] vec = m.vertices;//如果已经有mesh则获取现有mesh的vec
        int[] tri = m.triangles;
        Vector2[] UVS = m.uv;
        Vector3[] NORMAls = m.normals;

        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                vec[4 * (i * Chunk.ChunkSize + j) + 0].x = i * Chunk.BlockSize;
                vec[4 * (i * Chunk.ChunkSize + j) + 0].y = data.tblocks[i, j].H * Chunk.BlockHeight;
                vec[4 * (i * Chunk.ChunkSize + j) + 0].z = j * Chunk.BlockSize;

                vec[4 * (i * Chunk.ChunkSize + j) + 1].x = i * Chunk.BlockSize;
                vec[4 * (i * Chunk.ChunkSize + j) + 1].y = data.tblocks[i, j].H * Chunk.BlockHeight;
                vec[4 * (i * Chunk.ChunkSize + j) + 1].z = (j + 1) * Chunk.BlockSize;

                vec[4 * (i * Chunk.ChunkSize + j) + 2].x = (i + 1) * Chunk.BlockSize;
                vec[4 * (i * Chunk.ChunkSize + j) + 2].y = data.tblocks[i, j].H * Chunk.BlockHeight;
                vec[4 * (i * Chunk.ChunkSize + j) + 2].z = (j + 1) * Chunk.BlockSize;

                vec[4 * (i * Chunk.ChunkSize + j) + 3].x = (i + 1) * Chunk.BlockSize;
                vec[4 * (i * Chunk.ChunkSize + j) + 3].y = data.tblocks[i, j].H * Chunk.BlockHeight;
                vec[4 * (i * Chunk.ChunkSize + j) + 3].z = j * Chunk.BlockSize;

            }
        }

        for (int i = 4 * (Chunk.ChunkSize * Chunk.ChunkSize); i < vec.Length; i++)
        {
            vec[i].x = 0;
            vec[i].y = 0;
            vec[i].z = 0;
        }

        Vector2 temp = Vector2.zero;
        /*for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                UVS[4 * (i * Chunk.ChunkSize + j) + 0].x = temp.x;
                UVS[4 * (i * Chunk.ChunkSize + j) + 0].y = temp.y;

                UVS[4 * (i * Chunk.ChunkSize + j) + 1].x = temp.x + 0;
                UVS[4 * (i * Chunk.ChunkSize + j) + 1].y = temp.y + (1f / Chunk.ChunkSize / 2);

                UVS[4 * (i * Chunk.ChunkSize + j) + 2].x = temp.x + (1f / Chunk.ChunkSize / 2);
                UVS[4 * (i * Chunk.ChunkSize + j) + 2].y = temp.y + (1f / Chunk.ChunkSize / 2);

                UVS[4 * (i * Chunk.ChunkSize + j) + 3].x = temp.x + (1f / Chunk.ChunkSize / 2);
                UVS[4 * (i * Chunk.ChunkSize + j) + 3].y = temp.y;

                temp.y += 1f / Chunk.ChunkSize;
            }
            temp.y = 0;
            temp.x += 1f / Chunk.ChunkSize;
        }*/
        m.vertices = vec;//需要显式调用vertices=vec才会触发mesh更新，直接设mesh的vertices属性不管用


        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        mr.material.SetTexture("_BaseMap", genmat());
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    void genMesh()
    {
        Mesh m = GetComponent<MeshFilter>().mesh;
        if (m == null || m.vertices == null || m.vertices.Length == 0)
            GenMesh_New();
        else
        {
            GenMesh_UseExist();
        }
    }

    void genDecoration()
    {
        //根据地块数据生成GPUINSTANCE的grass的matrix4x4
        //
        List<Matrix4x4> mats4 = new List<Matrix4x4>();
        List<Matrix4x4> mats7 = new List<Matrix4x4>();
        List<Matrix4x4> mats10 = new List<Matrix4x4>();
        Vector3 currentBlockPos = Vector3.zero;
        for (int i = 0; i < Chunk.ChunkSize; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                currentBlockPos.y = data.tblocks[i, j].H * Chunk.BlockHeight;
                if (data.tblocks[i, j].bio == T_BIOME.grassland)
                {
                    float temp = noise((i + Chunk.PixlePerBlock * x * Chunk.ChunkSize) * 1.0f, (j + Chunk.PixlePerBlock * y * Chunk.ChunkSize) * 1.0f);
                    if (temp > 0.9f)
                    {
                        //大堆草,10颗
                        mats10.AddRange(GetGrassMatrix(1, 0.7f, currentBlockPos));

                    }
                    else if (temp > 0.75f)
                    {
                        //中，7颗
                        mats7.AddRange(GetGrassMatrix(1, 0.7f, currentBlockPos));
                    }
                    else if (temp > 0.5f)
                    {
                        //小4颗
                        mats4.AddRange(GetGrassMatrix(1, 0.7f, currentBlockPos));
                    }
                }
                else if (data.tblocks[i, j].bio == T_BIOME.mouthian || data.tblocks[i, j].bio == T_BIOME.beach)
                {
                    float temp = noise((i + Chunk.PixlePerBlock * x * Chunk.ChunkSize) * 1.0f, (j + Chunk.PixlePerBlock * y * Chunk.ChunkSize) * 1.0f);
                    if (temp > 0.925f)
                    {
                        //中，7颗
                        mats7.AddRange(GetGrassMatrix(1, 0.7f, currentBlockPos));
                    }
                    else if (temp > 0.8f)
                    {
                        //小4颗
                        mats4.AddRange(GetGrassMatrix(1, 0.7f, currentBlockPos));
                    }
                }
                currentBlockPos.z += Chunk.BlockSize;
            }
            currentBlockPos.x += Chunk.BlockSize;
            currentBlockPos.z = 0;
        }
        //grassMatrix = mats.ToArray();
        grassMatrix4 = mats4.ToArray();
        grassMatrix7 = mats7.ToArray();
        grassMatrix10 = mats10.ToArray();
    }
    Matrix4x4[] GetGrassMatrix(float count, float size, Vector3 currentBlockPos)
    {
        List<Matrix4x4> m = new List<Matrix4x4>();
        for (int i = 0; i < count; i++)
        {
            Vector3 rpos = new Vector3(UnityEngine.Random.Range(0f, Chunk.BlockSize), 0, UnityEngine.Random.Range(0f, Chunk.BlockSize)) + currentBlockPos + Chunk.ChunkPosToWorldPos(XYHelper.ToLongXY(x, y));

            Quaternion rrota = Quaternion.LookRotation(UnityEngine.Random.insideUnitSphere + Vector3.left * 5, Vector3.up);
            Vector3 rscale = Vector3.one * UnityEngine.Random.Range(size, 3 * size) + Vector3.up * UnityEngine.Random.Range(0, size);
            m.Add(Matrix4x4.TRS(rpos, rrota, rscale));
        }

        return m.ToArray();
    }

    Texture2D genmat()
    {

        Texture2D t2d = new Texture2D(Chunk.ChunkSize * Chunk.PixlePerBlock, Chunk.ChunkSize * Chunk.PixlePerBlock);

        for (int i = 0; i < Chunk.ChunkSize * Chunk.PixlePerBlock; i++)
        {
            for (int j = 0; j < Chunk.ChunkSize * Chunk.PixlePerBlock; j++)
            {
                t2d.SetPixel(i, j, getcolor(i / Chunk.PixlePerBlock, j / Chunk.PixlePerBlock, noise(
                    (i + Chunk.PixlePerBlock * x * Chunk.ChunkSize) * 1.0f,
                    (j + Chunk.PixlePerBlock * y * Chunk.ChunkSize) * 1.0f
                    )));

            }
        }
        t2d.filterMode = FilterMode.Point;
        t2d.Apply();
        Chunk UpChunk = EventCenter.WorldCenter.GetParm<long, Chunk>(nameof(EventNames.GetChunkData), XYHelper.ToLongXY(x + 1, y));
        Chunk RightChunk = EventCenter.WorldCenter.GetParm<long, Chunk>(nameof(EventNames.GetChunkData), XYHelper.ToLongXY(x, y + 1));
        for (int i = 0; i < Chunk.ChunkSize; i++)//将高一格的材质赋予到方块边缘
        {
            for (int j = 0; j < Chunk.ChunkSize; j++)
            {
                if (i == Chunk.ChunkSize - 1 && j != Chunk.ChunkSize - 1)//取X上方hcunk
                {
                    if (UpChunk != null && data.tblocks[i, j].H < UpChunk.tblocks[0, j].H)//如果当前格高度小于Z方向下一格高度则将边界材质设为当前格
                    {
                        GameObject g = EventCenter.WorldCenter.GetParm<long, GameObject>(nameof(EventNames.GetChunkObj), XYHelper.ToLongXY(x + 1, y));

                        if (g != null)
                        {
                            //Debug.Log("找到" + (x+1) + ":" + y);
                            ChunkObject upchunkObj = g.GetComponent<ChunkObject>();
                            Color[] tempc = new Color[(Chunk.PixlePerBlock / 2) * (Chunk.PixlePerBlock / 2)];
                            for (int tempi = 0; tempi < tempc.Length; tempi++)
                            {
                                tempc[tempi] = upchunkObj.getcolor(0, j, noise(
                         (0 + Chunk.PixlePerBlock * (x + 1) * Chunk.ChunkSize) * 1.0f,
                         (j + Chunk.PixlePerBlock * y * Chunk.ChunkSize) * 1.0f
                         ));
                            }

                            t2d.SetPixels(i * Chunk.PixlePerBlock + (Chunk.PixlePerBlock / 2), (j) * Chunk.PixlePerBlock, Chunk.PixlePerBlock / 2, Chunk.PixlePerBlock / 2,
                                tempc
                                );
                        }
                    }
                    if (data.tblocks[i, j].H < data.tblocks[i, j + 1].H)
                    {
                        t2d.SetPixels(i * Chunk.PixlePerBlock, (j) * Chunk.PixlePerBlock + (Chunk.PixlePerBlock / 2), Chunk.PixlePerBlock / 2, Chunk.PixlePerBlock / 2,
           t2d.GetPixels(i * Chunk.PixlePerBlock, (j + 1) * Chunk.PixlePerBlock, Chunk.PixlePerBlock / 2, Chunk.PixlePerBlock / 2)
           );
                    }
                }
                else if (j == Chunk.ChunkSize - 1 && i != Chunk.ChunkSize - 1)//取Y右方hcunk
                {
                    if (RightChunk != null && data.tblocks[i, j].H < RightChunk.tblocks[i, 0].H)//如果当前格高度大于Z方向下一格高度则将边界材质设为当前格
                    {
                        GameObject g = EventCenter.WorldCenter.GetParm<long, GameObject>(nameof(EventNames.GetChunkObj), XYHelper.ToLongXY(x, y + 1));

                        if (g != null)
                        {
                            //Debug.Log("找到"+x+":"+(y+1));
                            ChunkObject rightchunkObj = g.GetComponent<ChunkObject>();
                            Color[] tempc = new Color[(Chunk.PixlePerBlock / 2) * (Chunk.PixlePerBlock / 2)];
                            for (int tempi = 0; tempi < tempc.Length; tempi++)
                            {
                                tempc[tempi] = rightchunkObj.getcolor(i, 0, noise(
                         (i + Chunk.PixlePerBlock * (x) * Chunk.ChunkSize) * 1.0f,
                         (0 + Chunk.PixlePerBlock * (y + 1) * Chunk.ChunkSize) * 1.0f
                         ));
                            }

                            t2d.SetPixels(i * Chunk.PixlePerBlock, (j) * Chunk.PixlePerBlock + (Chunk.PixlePerBlock / 2), Chunk.PixlePerBlock / 2, Chunk.PixlePerBlock / 2,
                                tempc
                                );
                        }
                    }
                    if (data.tblocks[i, j].H < data.tblocks[i + 1, j].H)
                    {
                        t2d.SetPixels(i * Chunk.PixlePerBlock + (Chunk.PixlePerBlock / 2), (j) * Chunk.PixlePerBlock, Chunk.PixlePerBlock / 2, Chunk.PixlePerBlock / 2,
           t2d.GetPixels((i + 1) * Chunk.PixlePerBlock, (j) * Chunk.PixlePerBlock, Chunk.PixlePerBlock / 2, Chunk.PixlePerBlock / 2)
           );
                    }
                }
                else if (j == Chunk.ChunkSize - 1 && i == Chunk.ChunkSize - 1)
                {
                    if (RightChunk != null && data.tblocks[i, j].H < RightChunk.tblocks[i, 0].H)//如果当前格高度大于Z方向下一格高度则将边界材质设为当前格
                    {

                    }
                    if (UpChunk != null && data.tblocks[i, j].H < UpChunk.tblocks[0, j].H)
                    {

                    }
                }
                else
                {
                    if (data.tblocks[i, j].H < data.tblocks[i, j + 1].H)//如果当前格高度大于Z方向下一格高度则将边界材质设为当前格
                    {
                        t2d.SetPixels(i * Chunk.PixlePerBlock, (j) * Chunk.PixlePerBlock + (Chunk.PixlePerBlock / 2), Chunk.PixlePerBlock / 2, Chunk.PixlePerBlock / 2,
           t2d.GetPixels(i * Chunk.PixlePerBlock, (j + 1) * Chunk.PixlePerBlock, Chunk.PixlePerBlock / 2, Chunk.PixlePerBlock / 2)
           );
                    }
                    if (data.tblocks[i, j].H < data.tblocks[i + 1, j].H)
                    {
                        t2d.SetPixels(i * Chunk.PixlePerBlock + (Chunk.PixlePerBlock / 2), (j) * Chunk.PixlePerBlock, Chunk.PixlePerBlock / 2, Chunk.PixlePerBlock / 2,
           t2d.GetPixels((i + 1) * Chunk.PixlePerBlock, (j) * Chunk.PixlePerBlock, Chunk.PixlePerBlock / 2, Chunk.PixlePerBlock / 2)
           );
                    }
                }


            }
        }
        t2d.Apply();
        return t2d;
        //gameObject.GetComponent<MeshRenderer>().sharedMaterial = Terrain_Gen.mater;

        //gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", t2d);
    }
    void copyTextureArea(Texture2D t2d, Vector2 sourceCoord, Vector2 targetCoord, Vector2 size)
    {

        t2d.SetPixels((int)(targetCoord.x), (int)(targetCoord.y), (int)(size.x), (int)(size.y),
            t2d.GetPixels((int)(sourceCoord.x), (int)(sourceCoord.y), (int)(size.x), (int)(size.y))
            );
    }

    Color mouthian = new Color(1 * 1.0f / 255, 1, 1),
        desert = new Color(1, 1, 1),
        forest = new Color(1, 1, 1);

    float noise(float bx, float by)
    {//bx,by取值范围是关于地图归一化的0-1
        float tempamp = 1, tempsmooth = 16f;
        float seed = Mathf.PingPong(1234.567f, 2);


        float f = tempamp * Mathf.PerlinNoise(1 + ((bx / Chunk.PixlePerBlock) * Mathf.PI + seed) / tempsmooth, 1 + ((by / Chunk.PixlePerBlock) * Mathf.PI + seed) / tempsmooth);
        /*for (int i = 0; i < 3; i++)
        {
            tempsmooth /= 3;
            tempamp /= 3;
            f += tempamp * (Mathf.PerlinNoise(1 + ((bx) * Mathf.PI + seed) / tempsmooth, 1 + ((by) * Mathf.PI + seed) / tempsmooth) - 0.5f);
        }*/

        //f = f / (1 + 1 / 3 + 1 / 9 + 1 / 27);

        return f;
    }

    public Color getcolor(int x, int y, float offset = 0)
    {
        //需从某个loader处读取生成规则，具体是mat,biome和地块颜色的映射规则
        offset = Mathf.Clamp(offset, 0f, 1f);
        x = Mathf.Clamp(x, 0, Chunk.ChunkSize - 1);
        y = Mathf.Clamp(y, 0, Chunk.ChunkSize - 1);
        //Debug.Log(data.tblocks[x, y].bio);
        switch (data.tblocks[x, y].mat)
        {
            case T_Material.sand:
                return (1 - offset) * GameColor.beach + offset * GameColor.deep_beach;
                break;
            case T_Material.grassdirt:
                return (1 - offset) * GameColor.grassland + offset * GameColor.deep_grassland;
                break;
            case T_Material.stone:
                return (1 - offset) * GameColor.moutain + offset * GameColor.deep_moutain;

                break;

            default:
                return Color.grey;
                break;
        }
    }

    #region 逻辑事件转发层
    //更新事件，可以自己建一个缓存队列，将一帧之内的所有更新先立即写入数据，mesh等表现层的更新一帧只执行一次
    public void ChunkBBlockUpdate(B_Block b, Vector3Int pos)
    {
        Debug.Log(b.mat + ":" + pos + "chunk"+x+":"+y+"bblockUpdate");
        if (b == null)
        {
            if (blocks.ContainsKey(pos.ToInt()))
            {
                //删除现有basebuildingblock的gobj
                Destroy(blocks[pos.ToInt()].gameObject);
                blocks.Remove(pos.ToInt());
            }
        }
        else
        {
            if (blocks.ContainsKey(pos.ToInt()))
            {
                Debug.Log(pos+"已有bblock");
                //更新现有basebuildingblock的gobj
                blocks[pos.ToInt()].Show(b);//未优化
            }
            else
            {
               
                //生成gobj，赋值bblock
                GameObject g = GameMainManager.CreateGameObject(defaultBBlock, transform.position, transform.rotation, transform);
                BaseBuildingBlock bb = g.GetComponent<BaseBuildingBlock>();
                Debug.Log(g.transform.position+"sss"+g.transform.parent.gameObject.GetComponent<ChunkObject>().x+":" + g.transform.parent.gameObject.GetComponent<ChunkObject>().y);
                bb.Init(b, pos.x, pos.z, pos.y, center);
                blocks.Add(pos.ToInt(), bb);
            }
        }
    }
    public void ChunkTBlockUpdate(T_Block b, Vector3Int pos)
    {
        GenMesh_New();
        GenMeshEdge();
    }

    //更新通知事件，世界block坐标0.0，已确保target在本chunk内且有此bblock方块
    //收到更新事件不要立即更新，最好延迟一帧后再更新
    public void BlockNotifyByTB(Vector3Int pos, Vector3Int target, T_Block tb)
    {
        Base_BBlockBehavious b;
        B_Block self = data.GetBBlock(target);
        if (!BBlockBehavious.ContainsKey(self.mat)) b = BBlockBehavious[B_Material.none];
        else b = BBlockBehavious[self.mat];

        b.NeighborBlockUpdateTB(self, pos, target, tb);
    }
    public void BlockNotifyByBB(Vector3Int pos, Vector3Int target, B_Block bb)
    {
        Base_BBlockBehavious b;
        B_Block self = data.GetBBlock(target);
        if (self == null) Debug.Log(target+"kond de,有"+data.blocks.Count+"个");
        if (!BBlockBehavious.ContainsKey(self.mat)) b = BBlockBehavious[B_Material.none];
        else b = BBlockBehavious[self.mat];
        Debug.Log("从" + pos + "更新到" + target);
        b.NeighborBlockUpdateBB(self, pos, target, bb);
    }
    public void BlockNotifyByEB(Vector3Int pos, Vector3Int target, Entity_Block eb)
    {//1*字典查找
        Base_BBlockBehavious b;
        B_Block self = data.GetBBlock(target);
        if (!BBlockBehavious.ContainsKey(self.mat)) b = BBlockBehavious[B_Material.none];
        else b = BBlockBehavious[self.mat];

        b.NeighborBlockUpdateEB(self, pos, target, eb);
    }

    public void ChunkStateUpdate(int x, int y, int stat)
    {//active时setactive true，sleep时取消entity活动，cache则setactive false，unload删除chunkobj
        switch (stat)
        {
            case 0:
                //Debug.Log("chunk" + this.x + "," + this.y + "开启" );
                if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
                edgeLoaded = Mathf.Clamp(edgeLoaded + 1, 0, 4);
                if (edgeLoaded == 2)
                {
                    GenMeshEdge();
                    //Debug.Log("2gen at "+x+" "+y);
                    EventCenter.WorldCenter.SendEvent<long, int>(nameof(EventNames.ChunkUpdate), XYHelper.ToLongXY(x, y), 1);
                }
                else EventCenter.WorldCenter.SendEvent<long, int>(nameof(EventNames.ChunkUpdate), XYHelper.ToLongXY(x, y), 0);
                break;
            /* case 1:
                 gameObject.SetActive(true);
                 EventCenter.WorldCenter.SendEvent<long, int>(nameof(EventNames.ChunkUpdate), XYHelper.ToLongXY(x, y), 1);
                 break;*/
            case 2:
                //Debug.Log("chunk" + this.x + "," + this.y + "关闭" );
                gameObject.SetActive(false);
                edgeLoaded = 0;
                data.ResetDirty();
                //genMesh();
                //Debug.Log("尝试回收" + x + "," + y);
                pool.Recycle(gameObject, XYHelper.ToLongXY(x, y));
                EventCenter.WorldCenter.SendEvent<long, int>(nameof(EventNames.ChunkUpdate), XYHelper.ToLongXY(x, y), 2);
                break;
            case 3:
                EventCenter.WorldCenter.SendEvent<long, int>(nameof(EventNames.ChunkUpdate), XYHelper.ToLongXY(x, y), 3);
                Destroy(gameObject);
                //Debug.Log("销毁" + x + "," + y);
                //pool.Recycle(gameObject);
                break;
            default:
                break;
        }
    }
    public void OnBBlockCollid(int x, int y, int h)
    {
        //basebuildingblock只转发不处理碰撞事件
    }

    #endregion


    void Update()
    {
        if (grassMatrix4 != null)
        {
            Graphics.DrawMeshInstanced(grassMesh4, 0, grassmat4, grassMatrix4);
        }
        if (grassMatrix7 != null)
        {
            Graphics.DrawMeshInstanced(grassMesh7, 0, grassmat7, grassMatrix7);
        }
        if (grassMatrix10 != null)
        {
            Graphics.DrawMeshInstanced(grassMesh10, 0, grassmat10, grassMatrix10);
        }
    }

    public void OnPoolInit()
    {
        //throw new NotImplementedException();
    }

    public void OnPoolPush()
    {
        //Debug.Log(x + "," + y + "被回收");
        gameObject.SetActive(false);
        //throw new NotImplementedException();
    }

    public void OnPoolPop(float time)
    {
        //throw new NotImplementedException();
        gameObject.SetActive(true);
    }

    public void OnPoolRecycle()
    {
        //throw new NotImplementedException();
        //Debug.Log(x + "," + y + "被回收");
        //gameObject.SetActive(false);
    }
}