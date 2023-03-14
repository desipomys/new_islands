﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 换装概念：找出所有已有装备，获取其mesh、mat、bone，根据要换的装备替换已有对应装备的mesh,mat,bone，再进行合并mesh，mat，
/// 绑定bone的操作
/// </summary>

public class Test_ChangeCloth : MonoBehaviour
{
    // 目标物体（必须是骨骼的父物体，不然蒙皮失效）
    public GameObject target;
    //要换的装备
    public GameObject clothToChg;
    // 最终材质（合并所有模型后使用的材质）
    public Material material;
    public GameObject[] defaultEquipPartPaths = new GameObject[1];


    // 物体所有的部分
    private GameObject[] targetParts = new GameObject[9];

    
    public GameObject smrPos;
    public GameObject[] clothPart2;
    public GameObject[] clothPart;
    public GameObject targetCloth;


    public GameObject temp1,temp2;
    private void Start()
    {
        //combine(clothPart,smrPos.transform);
    }
    void change()
    {//只把某个特定的装备组件的mesh换成另一个mesh
        SkinnedMeshRenderer smr = Instantiate(temp2).GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer targetsmr = temp1.GetComponent<SkinnedMeshRenderer>();
        targetsmr.sharedMesh = smr.sharedMesh;
        targetsmr.materials = smr.materials;
        Destroy(smr.gameObject);
    }
    private void combine(GameObject[] allcloth,Transform boneRoot)
    {
        List<Material> mats = new List<Material>();
        List<CombineInstance> combines = new List<CombineInstance>();
        List<Transform> bones = new List<Transform>();
        List<GameObject> gobjs = new List<GameObject>();

        List<Transform> transforms = new List<Transform>();
        transforms.AddRange(boneRoot.GetComponentsInChildren<Transform>(true));

        for (int i = 0; i < allcloth.Length; i++)
        {
            gobjs.Add(Instantiate( allcloth[i]));
        }
        foreach (var item in gobjs)
        {
            
            SkinnedMeshRenderer smr =item.GetComponentInChildren<SkinnedMeshRenderer>();
            //if (targetCloth.name == item.name) {smr=targetCloth.GetComponent<SkinnedMeshRenderer>(); }//如果是目标衣服对应的部件就采用目标衣服
            mats.AddRange(smr.materials);//获取材质
            for (int i = 0; i < smr.sharedMesh.subMeshCount; i++)//获取所有衣服的mesh
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = smr.sharedMesh;
                ci.subMeshIndex = i;
                combines.Add(ci);
            }

            for (int i = 0; i < smr.bones.Length; i++)//
            {
                foreach (Transform tr in transforms)
                {
                    if (tr.name != smr.bones[i].name) continue;
                    bones.Add(tr);
                    break;
                }
            }

        }

        if (boneRoot.GetComponent<SkinnedMeshRenderer>() != null) { DestroyImmediate(boneRoot.GetComponent<SkinnedMeshRenderer>()); }
        SkinnedMeshRenderer bsmr = smrPos.AddComponent<SkinnedMeshRenderer>();
        bsmr.sharedMesh = new Mesh();
        bsmr.sharedMesh.CombineMeshes(combines.ToArray(),true,false);
        bsmr.bones = bones.ToArray();
        bsmr.materials = mats.ToArray();

        foreach (var item in gobjs)
        {
            DestroyImmediate(item);
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            //test();
            //combine(clothPart2, smrPos.transform);
            change();
        }
    }
    #region mychangecloth

    Material[] mats;

    void GetClothBones()
    {

    }
    void GetClothMaterial()
    {

    }


    #endregion


    void change2()
    {
        // 把FBX的模型按部件分别放入Resources下对应的文件夹里，可以留空，模型需要蒙皮，而且所有模型使用同一骨骼
        // 最后的M是Fbx的模型，需要的Unity3D里设置好材质和贴图，部件贴图要勾选Read/Write Enabled
       /* defaultEquipPartPaths[0] = "Assets/Models/Player/Test/part/boot.fbx";
        defaultEquipPartPaths[1] = "Model/Player/GirlPlayer/Face/Face0000/M";
        defaultEquipPartPaths[2] = "Model/Player/GirlPlayer/Hair/Hair0000/M";
        defaultEquipPartPaths[3] = "";
        defaultEquipPartPaths[4] = "Model/Player/GirlPlayer/Body/Body0000/M";
        defaultEquipPartPaths[5] = "Model/Player/GirlPlayer/Leg/Leg0000/M";
        defaultEquipPartPaths[6] = "Model/Player/GirlPlayer/Hand/Hand0000/M";
        defaultEquipPartPaths[7] = "Model/Player/GirlPlayer/Foot/Foot0000/M";
        defaultEquipPartPaths[8] = "Model/Player/GirlPlayer/Wing/Wing0001/M";*/

        Destroy(target.GetComponent<SkinnedMeshRenderer>());
        for (int i = 0; i < defaultEquipPartPaths.Length; i++)
        {
            GameObject o = defaultEquipPartPaths[i];
            if (o)
            {
                GameObject go = Instantiate(o) as GameObject;
                go.transform.parent = target.transform;
                go.transform.localPosition = new Vector3(0, -1000, 0);
                go.transform.localRotation = new Quaternion();
                targetParts[i] = go;
            }
        }

        StartCoroutine(DoCombine());
    }

    /// <summary>
    /// 使用延时，不然某些GameObject还没有创建
    /// </summary>
    /// <returns></returns>
    IEnumerator DoCombine()
    {
        yield return null;
        Combine(target.transform);
    }


    /// <summary>
    /// 合并蒙皮网格，刷新骨骼
    /// 注意：合并后的网格会使用同一个Material
    /// </summary>
    /// <param name="root">角色根物体</param>
    private void Combine(Transform root)
    {
        float startTime = Time.realtimeSinceStartup;

        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Transform> boneList = new List<Transform>();
        Transform[] transforms = root.GetComponentsInChildren<Transform>();
        List<Texture2D> textures = new List<Texture2D>();

        int width = 0;
        int height = 0;

        int uvCount = 0;

        List<Vector2[]> uvList = new List<Vector2[]>();

        // 遍历所有蒙皮网格渲染器，以计算出所有需要合并的网格、UV、骨骼的信息
        foreach (SkinnedMeshRenderer smr in root.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            
            for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = smr.sharedMesh;
                ci.subMeshIndex = sub;
                combineInstances.Add(ci);
            }

            uvList.Add(smr.sharedMesh.uv);
            uvCount += smr.sharedMesh.uv.Length;

            if (smr.material.mainTexture != null)
            {
                textures.Add(smr.GetComponent<Renderer>().material.mainTexture as Texture2D);
                width += smr.GetComponent<Renderer>().material.mainTexture.width;
                height += smr.GetComponent<Renderer>().material.mainTexture.height;
            }

            foreach (Transform bone in smr.bones)//将目标装备的骨骼从目标骨骼转到源骨骼（源是玩家，目标是要换的装备）
            {
                foreach (Transform item in transforms)
                {
                    if (item.name != bone.name) continue;
                    boneList.Add(item);
                    break;
                }
            }
        }

        // 获取并配置角色所有的SkinnedMeshRenderer
        SkinnedMeshRenderer tempRenderer = root.gameObject.GetComponent<SkinnedMeshRenderer>();
        if (!tempRenderer)
        {
            tempRenderer = root.gameObject.AddComponent<SkinnedMeshRenderer>();
        }

        tempRenderer.sharedMesh = new Mesh();

        // 合并网格，刷新骨骼，附加材质
        tempRenderer.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, false);
        tempRenderer.bones = boneList.ToArray();
        tempRenderer.material = material;

        Texture2D skinnedMeshAtlas = new Texture2D(Mathf.NextPowerOfTwo(width), Mathf.NextPowerOfTwo(height));
        Rect[] packingResult = skinnedMeshAtlas.PackTextures(textures.ToArray(), 0);
        Vector2[] atlasUVs = new Vector2[uvCount];

        // 因为将贴图都整合到了一张图片上，所以需要重新计算UV
        int j = 0;
        for (int i = 0; i < uvList.Count; i++)
        {
            foreach (Vector2 uv in uvList[i])
            {
                if (i < packingResult.Length)
                {
                    atlasUVs[j].x = Mathf.Lerp(packingResult[i].xMin, packingResult[i].xMax, uv.x);
                    atlasUVs[j].y = Mathf.Lerp(packingResult[i].yMin, packingResult[i].yMax, uv.y);
                    j++;
                }
            }
        }

        // 设置贴图和UV
        tempRenderer.material.mainTexture = skinnedMeshAtlas;
        //tempRenderer.sharedMesh.uv = atlasUVs;

        // 销毁所有部件
        foreach (GameObject goTemp in targetParts)
        {
            if (goTemp)
            {
                Destroy(goTemp);
            }
        }

        Debug.Log("合并耗时 : " + (Time.realtimeSinceStartup - startTime) * 1000 + " ms");
    }


    /// <summary>
    /// 获取最接近输入值的2的N次方的数，最大不会超过1024，例如输入320会得到512
    /// </summary>
    private int get2Pow(int into)
    {
        int outo = 1;
        for (int i = 0; i < 10; i++)
        {
            outo *= 2;
            if (outo > into)
            {
                break;
            }
        }

        return outo;
    }

}
