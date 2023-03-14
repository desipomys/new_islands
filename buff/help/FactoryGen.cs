using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.IO;

//Ã²ËÆÎÞ±ØÒª
public class FactoryGen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    string GetContent<T>()
    {
        Type tt=typeof(T);
        string head="public class BuffValueModifyTargetFactory\n{";
        string funcname="public static "+tt.Name+" GetInstance(SC_Type_"+tt.Name+" type)\n{";
        string switchstr="switch(type){\n";
        string[] nameList=System.Enum.GetNames(tt);
        foreach (var item in nameList)
        {
            switchstr+="case "+tt.Name+"."+item+":return ";
        }

        string switchend = "default:return null;break;}";

        string tail="}\n}";
        return null;
    }

    string path = "Assets/Scripts/help/";
    void WriteFile(string strs, string filename)
    {
        string fullpath = path + filename + ".cs";
        if (!System.IO.File.Exists(fullpath))
        {
            System.IO.FileStream f = System.IO.File.Create(fullpath);
            f.Close();
            f.Dispose();
        }
        using (StreamWriter sw = new StreamWriter(fullpath))
        {sw.WriteLine(strs);
            
        }
    }
}
