using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Test_Equip : MonoBehaviour
{
    //[Tooltip("挂载于完整装备的人物上")]
    SkinnedMeshRenderer[] meshs;
    [Tooltip("纯骨骼预制")]
    //public GameObject skeleton;
    List<Material> materials=new List<Material>();
    List<CombineInstance> combineInstances = new List<CombineInstance>();
    List<Transform> bones = new List<Transform>();

    public bool combineMesh;
    bool stat=false;

    void Start()
    {
        ChangeEquipment(0, "kirie_hair");

    }
    void change()
    {
        /*meshs = GetComponentsInChildren<SkinnedMeshRenderer>();
        List<Transform> transforms = new List<Transform>();
        foreach (Transform item in skeleton.transform)
        {
            transforms.Add(item);
        } 

        for (int i = 0; i < meshs.Length; i++)
        {
            SkinnedMeshRenderer smr = meshs[i];

            for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = smr.sharedMesh;
                ci.subMeshIndex = sub;
                combineInstances.Add(ci);
            }
            for (int j = 0; j < smr.bones.Length; j++)
            {
                int tBase = 0;
                for (tBase = 0; tBase < transforms.Count; tBase++)
                {
                    if (smr.bones[j].name.Equals(transforms[tBase].name))
                    {
                        bones.Add(transforms[tBase]);
                        break;
                    }
                }
            }

            SkinnedMeshRenderer oldSKinned = skeleton.GetComponent<SkinnedMeshRenderer>();
            if (oldSKinned != null)
            {
                GameObject.DestroyImmediate(oldSKinned);
            }
            SkinnedMeshRenderer r = skeleton.AddComponent<SkinnedMeshRenderer>();
            r.sharedMesh = new Mesh();
            r.sharedMesh.CombineMeshes(combineInstances.ToArray(), false, false);// Combine meshes
            r.bones = bones.ToArray();// Use new bones
           
            
        }*/
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            string a = stat ? "umr_hair" : "kirie_hair";
            ChangeEquipment(0, a, combineMesh);
            stat = !stat;
            
            Debug.Log("done");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ChangeEquipment(1, "kirie_bodyeq", combineMesh);
            
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
           
            ChangeEquipment(2, "kirie_handeq", combineMesh);
            
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            
            ChangeEquipment(3, "kirie_footeq", combineMesh);
        }
    }

    string skeleton;
     string equipment_head= "kirie_hair";
     string equipment_chest= "kirie_bodyeq";
     string equipment_hand= "kirie_handeq";
     string equipment_feet= "kirie_footeq";
    /// <summary>
    /// 新生成的骨骼预制
    /// </summary>
    public GameObject Instance = null;
    public void ChangeEquipment(int index, string equipment, bool combine = false)
    {
        switch (index)
        {

            case 0:
                equipment_head = equipment;
                break;
            case 1:
                equipment_chest = equipment;
                break;
            case 2:
                equipment_hand = equipment;
                break;
            case 3:
                equipment_feet = equipment;
                break;
        }

        //装备prefabs以此形式组织：
        /*
         *gobj
         *  |-骨骼
         *  |-mesh
         */

        string[] equipments = new string[4];
        equipments[0] = equipment_head;
        equipments[1] = equipment_chest;
        equipments[2] = equipment_hand;
        equipments[3] = equipment_feet;

        Object res = null;
        SkinnedMeshRenderer[] meshes = new SkinnedMeshRenderer[4];
        GameObject[] objects = new GameObject[4];
        for (int i = 0; i < equipments.Length; i++)
        {//生成所有的装备obj，获取其skinmeshrenderer

            res = Resources.Load("Prefabs/playerEQ/test/" + equipments[i]);
            objects[i] = GameObject.Instantiate(res) as GameObject;
            meshes[i] = objects[i].GetComponentInChildren<SkinnedMeshRenderer>();
        }

        CombineObject(Instance, meshes, combine);//合并所有装备mesh

        //立即删除临时生成的装备gobj
        for (int i = 0; i < objects.Length; i++)
        {

            GameObject.DestroyImmediate(objects[i].gameObject);
        }
    }

    private const int COMBINE_TEXTURE_MAX = 512;
    private const string COMBINE_DIFFUSE_TEXTURE = "_MainTex";
    /// <summary>
    /// Combine SkinnedMeshRenderers together and share one skeleton.
    /// Merge materials will reduce the drawcalls, but it will increase the size of memory. 
    /// </summary>
    /// <param name="skeleton">combine meshes to this skeleton(a gameobject) 生成的骨骼</param>
    /// <param name="meshes">meshes need to be merged 装备的mesh</param>
    /// <param name="combine">merge materials or not </param>
	public void CombineObject(GameObject skeleton, SkinnedMeshRenderer[] meshes, bool combine = false)
    {

        // Fetch all bones of the skeleton
        List<Transform> transforms = new List<Transform>();
        transforms.AddRange(skeleton.GetComponentsInChildren<Transform>(true));

        List<Material> materials = new List<Material>();//the list of materials
        List<CombineInstance> combineInstances = new List<CombineInstance>();//the list of meshes
        List<Transform> bones = new List<Transform>();//the list of bones

        // Below informations only are used for merge materilas(bool combine = true)
        List<Vector2[]> oldUV = null;
        Material newMaterial = null;
        Texture2D newDiffuseTex = null;

        // Collect information from meshes遍历生成的装备mesh
        for (int i = 0; i < meshes.Length; i++)
        {
            SkinnedMeshRenderer smr = meshes[i];
            materials.AddRange(smr.materials); // Collect materials
                                               // Collect meshes
            for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = smr.sharedMesh;
                ci.subMeshIndex = sub;
                combineInstances.Add(ci);
            }
            // Collect bones
            for (int j = 0; j < smr.bones.Length; j++)
            {
                int tBase = 0;
                for (tBase = 0; tBase < transforms.Count; tBase++)
                {
                    if (smr.bones[j].name.Equals(transforms[tBase].name))
                    {
                        bones.Add(transforms[tBase]);
                        break;
                    }
                }
            }
        }

        // merge materials
        if (combine)
        {
            newMaterial = new Material(Shader.Find("Mobile/Diffuse"));
            oldUV = new List<Vector2[]>();
            // merge the texture
            List<Texture2D> Textures = new List<Texture2D>();
            for (int i = 0; i < materials.Count; i++)
            {
                Textures.Add(materials[i].GetTexture(COMBINE_DIFFUSE_TEXTURE) as Texture2D);
            }

            newDiffuseTex = new Texture2D(COMBINE_TEXTURE_MAX, COMBINE_TEXTURE_MAX, TextureFormat.RGBA32, true);
            Rect[] uvs = newDiffuseTex.PackTextures(Textures.ToArray(), 0);
            newMaterial.mainTexture = newDiffuseTex;

            // reset uv
            Vector2[] uva, uvb;
            for (int j = 0; j < combineInstances.Count; j++)
            {
                uva = (Vector2[])(combineInstances[j].mesh.uv);
                uvb = new Vector2[uva.Length];
                for (int k = 0; k < uva.Length; k++)
                {
                    uvb[k] = new Vector2((uva[k].x * uvs[j].width) + uvs[j].x, (uva[k].y * uvs[j].height) + uvs[j].y);
                }
                oldUV.Add(combineInstances[j].mesh.uv);
                combineInstances[j].mesh.uv = uvb;
            }
        }

        // Create a new SkinnedMeshRenderer
        SkinnedMeshRenderer oldSKinned = skeleton.GetComponent<SkinnedMeshRenderer>();
        if (oldSKinned != null)
        {

            GameObject.DestroyImmediate(oldSKinned);
        }
        SkinnedMeshRenderer r = skeleton.AddComponent<SkinnedMeshRenderer>();
        r.sharedMesh = new Mesh();
        r.sharedMesh.CombineMeshes(combineInstances.ToArray(), combine, false);// Combine meshes

        GameObject tempg = Instantiate(new GameObject());
        MeshFilter mf = tempg.AddComponent<MeshFilter>();
        MeshRenderer mtr=tempg.AddComponent<MeshRenderer>();
        mf.sharedMesh = new Mesh();
        mf.sharedMesh.CombineMeshes(combineInstances.ToArray(), combine, true);


        r.bones = bones.ToArray();// Use new bones
        if (combine)
        {
            r.material = newMaterial;
            for (int i = 0; i < combineInstances.Count; i++)
            {
                combineInstances[i].mesh.uv = oldUV[i];
            }
        }
        else
        {
            r.materials = materials.ToArray();
        }
        
    }
}
