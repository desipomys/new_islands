using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

[System.Obsolete]
[CustomEditor(typeof(SC_json))]
public class SC_ToJson : Editor
{
    public override void OnInspectorGUI() 
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("生成sc"))
        {
            //Logic

            SC_json ctr = target as SC_json;
            CodeGen_Function c = new CodeGen_Function();
            c.Gen();
        }
        if (GUILayout.Button("写入itemwarper到json文件"))
        {
            //Logic
            
            SC_json ctr = target as SC_json;
            ToItemJson(ctr.itemPath);
        }
        if(GUILayout.Button("写入mapprefabs到json文件"))
        {
            //Logic
            
            SC_json ctr = target as SC_json;
            ToMapprefabJson(ctr.mapprefabPath);
        }
        if(GUILayout.Button("写入mapgraph到json文件"))
        {
            //Logic
            
            SC_json ctr = target as SC_json;
            ToMapGraphJson(ctr.mapGraphPath);
        }
        if (GUILayout.Button("从eblockmodel的txt转scobj"))
        {
            //Logic

            SC_json ctr = target as SC_json;
            ToScObj_EblockModel();
        }
    }
    void ToMapGraphJson(string p)
    {
        //string p="SCMAPGRAPH/";
        //SC_MAPGRAPH[] scs=Resources.LoadAll<SC_MAPGRAPH>(p);
        
        //string s=JsonConvert.SerializeObject(scs);
        //FileSaver.SaveFile("mapGraph.txt",s);
        Debug.Log("写入完成");
    }
    void ToMapprefabJson(string p)
    {
        
    }
    string modelPath = "JsonData/EBlockModel";
    void ToScObj_EblockModel()
    {
        //string p="SCMAPGRAPH/";
        string modelEb = Resources.Load<TextAsset>(modelPath).text;
        Entity_BlockModel[] scs = JsonConvert.DeserializeObject<Entity_BlockModel[]>(modelEb);
        string s = @"Assets/Resources/SC/EBLOCKMODLE/";
        for (int i = 0; i < scs.Length; i++)
        {
            SC_Entity_BlockModel cubeAsset = ScriptableObject.CreateInstance<SC_Entity_BlockModel>();
            cubeAsset.model.x = scs[i].x;
            cubeAsset.model.y = scs[i].y;
            cubeAsset.model.typ = scs[i].typ;
            cubeAsset.model.strTyp = scs[i].strTyp;
            cubeAsset.model.lv = scs[i].lv;
            cubeAsset.model.h = scs[i].h;
            cubeAsset.model.canBuild = scs[i].canBuild;
            cubeAsset.model.classTyp = scs[i].classTyp;
            Debug.Log(cubeAsset.model.h);

            AssetDatabase.CreateAsset(cubeAsset, s+ cubeAsset.model.typ.ToString()+ ".asset");
            AssetDatabase.SaveAssets();
        }

       
        AssetDatabase.Refresh();

        Debug.Log("写入完成");
    }

    void ToItemJson(string path)
    {
        //string path="SCObj/";
        //读取路径下所有scriptobj生成itemwarper[]，转json后保存到txt
      /*SC_ITEMWARPER[] scs=Resources.LoadAll<SC_ITEMWARPER>(path);
        
        List<Item_Warper> iws=new List<Item_Warper>();
        for (int i = 0; i < scs.Length; i++)
        {
            iws.Add(scs[i].toWarper());
        }
        string s=JsonConvert.SerializeObject(iws);
        FileSaver.SaveFile(path,s);*/
        Debug.Log("写入完成");
    }
}
