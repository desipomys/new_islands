using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEquip : MonoBehaviour, IEventRegister
{
    Transform skeloton;
    List<GameObject> equips;

    void InitObj()
    {
        //�ҹ�����obj
        skeloton = transform.Find("mixamorig_Hips");
    }


    public void AfterEventRegist()
    {
        
    }

    public void OnEventRegist(EventCenter e)
    {
        InitObj();

    }
    GameObject[] GetMeshObjByItems(Item[] temp = null)
    {
        EventCenter evctemp = EventCenter.WorldCenter;
        if (temp == null)
        {
            Item[] its = new Item[] { new Item(400004), new Item(400005), new Item(400006) };
            return evctemp.GetParm<Item[], GameObject[]>(nameof(EventNames.GetEQObjByItem), its);
        }
        else
        {
            return evctemp.GetParm<Item[], GameObject[]>(nameof(EventNames.GetEQObjByItem), temp);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="allcloth">��Ҫ�ϲ�������cloth��gobj�����Բ���ʵ��</param>
    /// <param name="boneRoot">��������ҵ�transform</param>
    /// <param name="smrPos">��������ҵ�bone</param>
    protected GameObject[] combine(GameObject[] allcloth, Transform boneRoot,  GameObject smrPos)
    {
        List<Material> mats = new List<Material>();
        List<CombineInstance> combines = new List<CombineInstance>();
        List<Transform> bones = new List<Transform>();
        List<GameObject> gobjs = new List<GameObject>();
        List<GameObject> newEQInstant = new List<GameObject>();

        List<Transform> transforms = new List<Transform>();
        transforms.AddRange(boneRoot.GetComponentsInChildren<Transform>(true));

        for (int i = 0; i < allcloth.Length; i++)
        {
            gobjs.Add(Instantiate(allcloth[i]));
        }
        foreach (var item in gobjs)
        {

            SkinnedMeshRenderer smr = item.GetComponentInChildren<SkinnedMeshRenderer>();
            //if (targetCloth.name == item.name) {smr=targetCloth.GetComponent<SkinnedMeshRenderer>(); }//�����Ŀ���·���Ӧ�Ĳ����Ͳ���Ŀ���·�
            mats.AddRange(smr.materials);//��ȡ����
            for (int i = 0; i < smr.sharedMesh.subMeshCount; i++)//��ȡ�����·���mesh
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

            //if (smrPos.GetComponent<SkinnedMeshRenderer>() != null) { DestroyImmediate//(smrPos.GetComponent<SkinnedMeshRenderer>()); }
            GameObject temp = new GameObject(smr.sharedMesh.name);
            temp.transform.SetParent(smrPos.transform);
            SkinnedMeshRenderer bsmr = temp.AddComponent<SkinnedMeshRenderer>();
            bsmr.sharedMesh = smr.sharedMesh;
            //bsmr.sharedMesh.CombineMeshes(combines.ToArray(), true, false);
            //bsmr.sharedMesh.RecalculateBounds();
            bsmr.bones = bones.ToArray();
            bsmr.rootBone = boneRoot;
            bsmr.materials = smr.materials;

            combines.Clear();
            bones.Clear();
            newEQInstant.Add(temp);
        }
        foreach (var item in gobjs)
        {
            DestroyImmediate(item);
        }
        return newEQInstant.ToArray();
    }
}
