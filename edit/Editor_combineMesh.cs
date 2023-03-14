using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Editor_combineMesh : MonoBehaviour
{
    public GameObject[] gobjs;
    void Start()
    {
        GameObject g = GameObject.Find("playerBone");

    }

    private void combine(GameObject[] allcloth, Transform boneRoot)
    {
        List<Material> mats = new List<Material>();
        List<CombineInstance> combines = new List<CombineInstance>();
        List<Transform> bones = new List<Transform>();
        List<GameObject> gobjs = new List<GameObject>();

        List<Transform> transforms = new List<Transform>();
        transforms.AddRange(boneRoot.GetComponentsInChildren<Transform>(true));

        for (int i = 0; i < allcloth.Length; i++)
        {
            gobjs.Add(Instantiate(allcloth[i]));
        }
        foreach (var item in gobjs)
        {

            SkinnedMeshRenderer smr = item.GetComponentInChildren<SkinnedMeshRenderer>();
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
        SkinnedMeshRenderer bsmr = boneRoot.gameObject.AddComponent<SkinnedMeshRenderer>();
        bsmr.sharedMesh = new Mesh();
        bsmr.sharedMesh.CombineMeshes(combines.ToArray(), true, false);
        bsmr.bones = bones.ToArray();
        bsmr.materials = mats.ToArray();

        foreach (var item in gobjs)
        {
            DestroyImmediate(item);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
