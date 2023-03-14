using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuildingBlock : MonoBehaviour,IEventRegister
{
    MeshFilter mf;
    MeshRenderer mr;
    EventCenter center;
    BoxCollider col;
    public void AfterEventRegist()
    {
        
    }
    public void OnEventRegist(EventCenter e)
    {
        center = e;
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();
        col = GetComponent<BoxCollider>();
    }

    public void Init(B_Block b,int x,int y,int h,EventCenter chunkCenter)
    {
        transform.localPosition = new Vector3(x * Chunk.BlockSize+Chunk.BlockSize/2, h * Chunk.BlockHeight, y * Chunk.BlockSize + Chunk.BlockSize / 2);

        DIR[] ds = b.dir.UnPack();
        Vector3 temp = Vector3.zero;
        foreach (var item in ds)
        {
            switch (item)
            {
                case DIR.none:
                    break;
                case DIR.front:
                    temp.x += 1;
                    break;
                case DIR.back:
                    temp.x -= 1;
                    break;
                case DIR.up:
                    temp.y += 1;
                    break;
                case DIR.down:
                    temp.y -= 1;
                    break;
                case DIR.left:
                    temp.z += 1;
                    break;
                case DIR.right:
                    temp.z += 1;
                    break;
                default:
                    break;
            }
        }
        transform.LookAt(temp+transform.position);
        //center.friend = chunkCenter;
        Show(b);
    }
   

    public void Show(B_Block b)//根据bblock中指定类型及材质去找loader获取mesh,mat，并生成collider
    {
        mf.sharedMesh = EventCenter.WorldCenter.GetParm<int, Mesh>(nameof(EventNames.GetMeshByIndex), b.mesh);
        mr.sharedMaterial = EventCenter.WorldCenter.GetParm<int, Material>(nameof(EventNames.GetMatByIndex), b.mat2);
        Bounds bo = mf.sharedMesh.bounds;
        col.center = bo.center;
        col.size = bo.size;

        if(b.dir.Contain(DIR.left))
        {
            if (b.dir.Contain(DIR.front))//左前
            {
                transform.rotation = Quaternion.Euler(0, -45,0);
            }
            else if (b.dir.Contain(DIR.back))//左后
                transform.rotation = Quaternion.Euler(0, -135,0);
            else transform.rotation = Quaternion.Euler(0, -90, 0);//正左
        }
        else if(b.dir.Contain(DIR.right))
        {
            if (b.dir.Contain(DIR.front))//右前
            {
                transform.rotation = Quaternion.Euler(0, 45, 0);
            }
            else if (b.dir.Contain(DIR.back))//右后
                transform.rotation = Quaternion.Euler(0, 135, 0);
            else transform.rotation = Quaternion.Euler(0, 90, 0);//正右
        }
        else if (b.dir.Contain(DIR.front))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (b.dir.Contain(DIR.back))
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else transform.rotation = Quaternion.Euler(0, 0, 0);
    }

}
