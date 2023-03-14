using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_chgeq : MonoBehaviour
{
    /// <summary>
    /// ���gobj�Ĺ���
    /// </summary>
    public GameObject skeleton;
    /// <summary>
    /// ���gobj
    /// </summary>
    public GameObject playerobj;//����Ԥ��

    public GameObject cloth1;//ֻ���������͵�һ��װ����Ԥ��
    //public GameObject mycloth;//ֻ���������͵�һ��װ����Ԥ��
    // Start is called before the first frame update
    Loader_EquipAndSkin eqloader = new Loader_EquipAndSkin();
    void Start()
    {
        InitLoader();
        //��cloth1��mesh�赽mycloth��skinmeshrenderer��
        combine(GetMeshObjByItems(), skeleton.transform,ref playerobj);


    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            clearCloth();
            string[] s = new string[]
            {
                "femal_body","kizuna_hair_low","kizuna_hair_up","stander_leg","miku_handEQ","stander_head","stander_hand","miku_bodyEQ"
            };
            Item[] its = new Item[] {new Item(400001),new Item(400002),new Item(400003) };
            combine(GetMeshObjByItems(its), skeleton.transform, ref playerobj);
        }
        
    }

    void InitLoader()
    {
        eqloader.OnLoaderInit(1);
        eqloader.OnEventRegist(GetComponent<EventCenter>());
    }

    GameObject[] GetMeshObjByItems(Item[] temp=null)
    {
        EventCenter evctemp = GetComponent<EventCenter>();
        if (temp==null)
        {
            Item[] its = new Item[] { new Item(400004), new Item(400005), new Item(400006) };
            return evctemp.GetParm<Item[], GameObject[]>(nameof(EventNames.GetEQObjByItem), its);
        }
        else
        {
           return evctemp.GetParm<Item[], GameObject[]>(nameof(EventNames.GetEQObjByItem),temp);
        }
    }

    void clearCloth()
    {
        SkinnedMeshRenderer[] smr = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var item in smr)
        {
            Destroy(item.gameObject);
        }
    }

    GameObject[] testGetCloth(string[] strs =null)
    {
        if(strs==null) strs=new string[] {"femal_body","miku_hair_low","miku_hair_up","stander_leg","miku_handEQ","stander_head","stander_hand","miku_bodyEQ" };
        Debug.Log(eqloader.getEQObj(strs).Length);
        return eqloader.getEQObj(strs);
    }
    //��Ŀ��prefabs�е����gobj������skinmeshrenderer
    GameObject[] getAllCloth(GameObject gobj)
    {
        List<GameObject> gs = new List<GameObject>();
        SkinnedMeshRenderer[] skin = gobj.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var item in skin)
        {
            gs.Add(item.gameObject);
        }
        return gs.ToArray();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="allcloth">��Ҫ�ϲ�������cloth��gobj�����Բ���ʵ��</param>
    /// <param name="boneRoot">��������ҵ�transform</param>
    /// <param name="smrPos">��������ҵ�bone</param>
    private void combine(GameObject[] allcloth, Transform boneRoot,ref GameObject smrPos)
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
        }

       
        foreach (var item in gobjs)
        {
            DestroyImmediate(item);
        }
    }



}
