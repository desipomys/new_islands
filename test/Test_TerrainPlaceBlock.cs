using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_TerrainPlaceBlock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        sketchShadow = EventCenter.WorldCenter.GetParm<string, GameObject>(nameof(EventNames.GetEffectPrefabs), "meshEffect");
        sketchShadow.GetComponent<MeshFilter>().sharedMesh = EventCenter.WorldCenter.GetParm<int, Mesh>(nameof(EventNames.GetMeshByIndex), 1);
        sketchShadow.GetComponent<MeshRenderer>().sharedMaterial = EventCenter.WorldCenter.GetParm<int, Material>(nameof(EventNames.GetMatByIndex), 1);
        Bounds bb = sketchShadow.GetComponent<MeshFilter>().sharedMesh.bounds;
        sketchShadow.GetComponent<BoxCollider>().center = bb.center;
        sketchShadow.GetComponent<BoxCollider>().size = bb.size;
        Destroy(sketchShadow.GetComponent<Rigidbody>());

        ct = ContainerManager.GetContainer<Container_Terrain>();
        parm.typ = 1;
    }
    public LayerMask layer;
    Ray r;
    RaycastHit rt;
    GameObject sketchShadow;
    B_Block bb = new B_Block();
    PlaceEBlockParm parm = new PlaceEBlockParm();
    Vector3Int offset;
        int[] eblocksize;

    void Update()
    {
        //testPlaceBBlock();
        //获取指向点vec3
        //转方块坐标送getheight方法
        //设text=返回值

        testPlaceSketchBlock();
        //testPlaceBBlock();
        //testGetTerrianH();
    }
    Container_Terrain ct;
    void testGetTerrianH()
    {
        r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out rt, 1000, layer))
        {
            Vector3Int vi = Chunk.WorldPosToBlockPos(rt.point);
            int a = ct.GetHeight(vi);
            EventCenter.WorldCenter.SendEvent<string>("UIDebugInfo", vi.ToString()+":"+a.ToString());
            
        }
    }
    void testPlaceSketchBlock()
    {
        r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out rt, 1000, layer))
        {
            Vector3Int vi = Chunk.WorldPosToBlockPos(rt.point);
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (offset.x == 0)
                {
                    if (offset.z == 0)
                    {
                        offset.z = -eblocksize[1];
                    }
                    else
                    {
                        offset.x = -eblocksize[0];
                    }
                }
                else
                {
                    if (offset.z == 0)
                    {
                        offset.x = 0;
                    }
                    else
                    {
                        offset.z = 0;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log("指向位置=" + rt.point + ".block位置:" + vi);
                //vi.y += 1;
                parm.typ = 10001;
                parm.pos = vi + offset;
                parm.dir = DIR.none;
                parm.initData = "";
                EventCenter evc = EventCenter.WorldCenter.GetParm<PlaceEBlockParm, EventCenter>(nameof(EventNames.SetEBlock), parm);
                //evc.GetComponent<BuildAbleEBlock>().process = 2;
            }

            Vector3 temp = new Vector3();
            temp.x = vi.x * Chunk.BlockSize + Chunk.BlockSize / 2 + offset.x * Chunk.BlockSize;
            temp.y = (vi.y) * Chunk.BlockHeight + offset.y * Chunk.BlockHeight;
            temp.z = vi.z * Chunk.BlockSize + Chunk.BlockSize / 2 + offset.z * Chunk.BlockSize;

            sketchShadow.transform.position = temp;

        }
    }
   
    void testPlaceEBlock()
    {
        r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out rt, 1000, layer))
        {
            Vector3Int vi = Chunk.WorldPosToBlockPos(rt.point);
            if(Input.GetKeyDown(KeyCode.N))
            {
                int[] all = EventCenter.WorldCenter.GetParm<int[]>(nameof(EventNames.GetAllEBlockID));
                parm.typ = all[Random.Range(0,all.Length)];
                Debug.Log("random=" + parm.typ);
                eblocksize= EventCenter.WorldCenter.GetParm<int,int>(nameof(EventNames.GetEBlockSizeByID),parm.typ).GetCoord3();
                
                offset = Vector3Int.zero;
            }
            if(Input.GetKeyDown(KeyCode.R))
            {
                if(offset.x==0)
                {
                    if(offset.z==0)
                    {
                        offset.z = -eblocksize[1];
                    }
                    else
                    {
                        offset.x = -eblocksize[0];
                    }
                }
                else 
                {
                    if (offset.z == 0)
                    {
                        offset.x = 0;
                    }
                    else
                    {
                        offset.z = 0;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log("指向位置=" + rt.point + ".block位置:" + vi);
                //vi.y += 1;
                parm.pos = vi+offset;
                parm.dir = DIR.none;
                parm.initData = "1";
               EventCenter evc= EventCenter.WorldCenter.GetParm<PlaceEBlockParm,EventCenter>(nameof(EventNames.SetEBlock),parm);
                evc.GetComponent<BuildAbleEBlock>().process = 2;
            }

            Vector3 temp = new Vector3();
            temp.x = vi.x * Chunk.BlockSize + Chunk.BlockSize / 2+offset.x*Chunk.BlockSize;
            temp.y = (vi.y) * Chunk.BlockHeight+offset.y* Chunk.BlockHeight;
            temp.z = vi.z * Chunk.BlockSize + Chunk.BlockSize / 2 + offset.z * Chunk.BlockSize;

            sketchShadow.transform.position = temp;

        }
    }
    void testPlaceBBlock()
    {
        r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out rt, 1000, layer))
        {
            Vector3Int a = Chunk.WorldPosToBlockPos(rt.point);
            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log("指向位置=" + rt.point + ".block位置:" + a);
                Vector3Int vi = new Vector3Int((int)a.x, (int)a.y, (int)a.z);
                Debug.Log(vi);
                bb.mesh = 1;
                bb.mat = B_Material.dirt;
                bb.mat2 = 1;
                bb.sub = 1;
                EventCenter.WorldCenter.SendEvent<Vector3Int, B_Block>(nameof(EventNames.SetBBlock), vi, bb);
            }

            Vector3 temp = new Vector3();
            temp.x = a.x * Chunk.BlockSize + Chunk.BlockSize / 2;
            temp.y = (a.y) * Chunk.BlockHeight;
            temp.z = a.z * Chunk.BlockSize + Chunk.BlockSize / 2;

            sketchShadow.transform.position = temp;

        }
        if (Input.GetKeyDown(KeyCode.N))
        {

        }
    }
}
