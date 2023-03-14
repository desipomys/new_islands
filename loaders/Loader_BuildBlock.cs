using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader_BuildBlock : BaseLoader
{
    //�����ƻ�ȡ����bblockʵ��
    Dictionary<int, GameObject> allBBlock = new Dictionary<int, GameObject>();
    Dictionary<string, int> name2Index = new Dictionary<string, int>();
    //���ṩ��int��ȡmesh,mat�Ĺ���
    Dictionary<int, Mesh> index2Mesh = new Dictionary<int, Mesh>();
    Dictionary<int, Material> index2Mat = new Dictionary<int, Material>();

    Dictionary<int, B_Material[]> level2Mat = new Dictionary<int, B_Material[]>();

    string path = "Prefabs/terrain/BBlock";
    string Matpath = "Prefabs/terrain/Mat";
    
    GameObject DefaultBBlock;

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.RegistFunc<GameObject>(nameof(EventNames.GetDefaultBBlockObj), GetDefaultBBlockObj);
        e.RegistFunc<int, Mesh>(nameof(EventNames.GetMeshByIndex),GetMeshByIndex);
        e.RegistFunc<int, Material>(nameof(EventNames.GetMatByIndex), GetMatByIndex);
    }
    public override void OnLoaderInit(int prio)
    {
        if (prio != 0) return;
        GameObject[] blocks = Resources.LoadAll<GameObject>(path);
        for (int i = 0; i < blocks.Length; i++)
        {
            string[] temp = blocks[i].name.Split('_');
            int index = int.Parse(temp[1]);
            if (index == 0)
            {
                DefaultBBlock = blocks[i];
            }
            else
            {
                //===============����int-meshӳ�� 
                name2Index.Add(temp[0], index);
                allBBlock.Add(index, blocks[i]);

                index2Mesh.Add(index, blocks[i].GetComponent<MeshFilter>().sharedMesh);
            }
        }
        Debug.Log("����mesh��ɣ�����" + index2Mesh.Count);

        Material[] mats = Resources.LoadAll<Material>(Matpath);
        for (int i = 0; i < mats.Length; i++)
        {
            string[] temp = mats[i].name.Split('_');
            int index = int.Parse(temp[1]);
            index2Mat.Add(index, mats[i]);
        }
        Debug.Log("����material��ɣ�����" + index2Mat.Count);

    }
    Mesh GetMeshByIndex(int index)
    {
        try
        {
 return index2Mesh[index];
        }
        catch (System.Exception)
        {
            Debug.Log(index + "mesh������");
            return null;
        }
       
    }
    Material GetMatByIndex(int index)
    {
        try
        {
            return index2Mat[index];
        }
        catch (System.Exception)
        {
            Debug.Log(index + "mat������");
            return null;
        }
        
    }
    public GameObject GetDefaultBBlockObj() { return DefaultBBlock; }
    public int NameToIndex(string typ)
    {
        if (name2Index.ContainsKey(typ))
            return name2Index[typ];
        else return -1;
    }

}
