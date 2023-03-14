using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.IO;


public class EnumGen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Gen<BuffValueModifyTarget_base>();
        Gen<BuffValueSource_base>();
        Gen<BuffTargetSelector>();
        Gen<buffPrecondition_base>();
        Gen<BuffTargetFilter>();
        Gen<BuffEffector_base>();
    }

    string path = "Assets/Scripts/help/";
    public void Gen<T>()
    {
        Type[] temp = getAll<T>();
        GenEnum(temp, typeof(T));

    }
    public void GenEnum(Type[] tps,Type baseType)
    {
        string s = "public enum Type_"+baseType.Name+"{ none,";

        foreach (var item in tps)
        {
            s += item.Name;
            s += ",";
        }
        s += "}";
        WriteFile(s, "Type_"+baseType.Name);
    }
    void WriteFile(string strs, string classname)
    {
        string fullpath = path + "SC_" + classname + ".cs";
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
    public Type[] getAll<T>()
    {
        List<Type> datas = new List<Type>();
        Assembly assembly = this.GetType().Assembly;
        Type[] types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.IsSubclassOf(typeof(T)) && !type.IsAbstract)
            {
                datas.Add(type);
            }
        }

        return datas.ToArray();
    }
}
