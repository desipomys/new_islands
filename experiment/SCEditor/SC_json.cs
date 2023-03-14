using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class a
{
    public int dd;
}
public class b : a
{
    public int uu;
}

[System.Obsolete]
public class SC_json : MonoBehaviour
{
    public string itemPath, mapprefabPath, mapGraphPath;
    // Start is called before the first frame update
    void Start()
    {
        Base_Functioning_Data db = new Base_Functioning_Data();
        FieldInfo[] ps = db.GetType().GetFields();
        string s = "";
        for (int i = 0; i < ps.Length; i++)
        {
            s += ps[i].FieldType.Name+"  ";
        }
        Debug.Log(s);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
