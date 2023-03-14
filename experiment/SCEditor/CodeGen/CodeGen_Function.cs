using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using System.IO;

[Obsolete]
public class CodeGen_Function
{
    string import =
    @"using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;   
    ";
    string path = "Assets/Scripts/experiment/SCEditor/function/";

    public void Gen()
    {
        ///获取所有basefunction子类
        //对于每个子类获取字段
        //生成scripableobject,内含子类+需要序列化的字段展开成的数组等+
        //  生成get子类方法，获取时将展开的字段写到子类里
        List<Base_Functioning_Data> datas = getAll<Base_Functioning_Data>();
        foreach (var item in datas)
        {
            List<string> codes = new List<string>();
            codes.Add(import);//命名空间

            codes.Add(codeHead(item.GetType()));//scriptableObj类名及注解

            codes.Add("public " + item.GetType().Name + " unit; \n");//functiondata
            List<FieldInfo> fs = new List<FieldInfo>(item.GetType().GetFields());//添加不可在编辑器直接编辑的类型的展开
            //fs.AddRange(item.GetType().BaseType.GetFields());

            foreach (var f in fs)
            {
                codes.AddRange(UnpackVariable(f));
                Debug.Log(f.FieldType.Name);
            }

            codes.Add(BuildGetMethod(item.GetType()));//建造从scripableObject get原始functiondata的方法
            codes.Add(BuildSetMethod(item.GetType()));
            codes.Add("}");
            WriteFile(codes, item.GetType().Name);
            Debug.Log("生成完成");
        }

        List<Base_SCData> SCdatas = getAll<Base_SCData>();
        foreach (var item in SCdatas)
        {
            List<string> codes = new List<string>();
            codes.Add(import);//命名空间

            codes.Add(codeHead(item.GetType()));//scriptableObj类名及注解

            codes.Add("public " + item.GetType().Name + " unit; \n");//functiondata
            List<FieldInfo> fs = new List<FieldInfo>(item.GetType().GetFields());//添加不可在编辑器直接编辑的类型的展开
            //fs.AddRange(item.GetType().BaseType.GetFields());

            foreach (var f in fs)
            {
                codes.AddRange(UnpackVariable(f));
                Debug.Log(f.FieldType.Name);
            }

            codes.Add(BuildGetMethod(item.GetType()));//建造从scripableObject get原始functiondata的方法
            codes.Add(BuildSetMethod(item.GetType()));
            codes.Add("}");
            WriteFile(codes, item.GetType().Name);
            Debug.Log("生成完成");
        }
    }

    void WriteFile(List<string> strs, string classname)
    {
        string fullpath = path + "SC_" + classname + ".cs";
        if (!System.IO.File.Exists(fullpath))
        {
            System.IO.FileStream f = System.IO.File.Create(fullpath);
            f.Close();
            f.Dispose();
        }
        using (StreamWriter sw = new StreamWriter(fullpath))
        {
            foreach (string s in strs)
            {
                sw.WriteLine(s);

            }
        }
    }

    string codeHead(Type t)
    {
        string s = "[CreateAssetMenu(menuName =\"dataEditor/" + t.Name + "\")]\n";
        s += " public class SC_" + t.Name + ":ScriptableObject \n { ";
        return s;
    }
    string BuildSetMethod(Type t)
    {
        Debug.Log("set");
        /*
        原型：
        public void Set(Craft_Data d)
        {
            unit=d;
            [ 
                ingredium_Placement=new XAndY[d.ingredium.items.Length];
                for(int i=0;i<d.ingredium.items.Length;i++)
                {
                    ingredium_Placement[i]=new XAndY(d.ingredium.GetItemLeftUp(i));
                }
            ]
            [
                List<string> keys=new List<string>();
                List<string> values=new List<string>();
                foreach(var item in d.exd)
                {
                    keys.Add(item.Key);
                    values.Add(item.Value);
                }
                exd_Keys=keys.ToArray();
                exd_Values=values.ToArray();
            ]
            [
                Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

              string date=JsonConvert.SerializeObject(Date_date,Newtonsoft.Json.Formatting.None,timeConverter);
            ]
        }

         * */
        string ans = "";

        ans += "public void Set(" + t.Name + " d)\n{";
        ans += "unit=d;";
        FieldInfo[] fs = t.GetFields();
        Dictionary<string, int> apperTimes = new Dictionary<string, int>();

        foreach (var item in fs)
        {
            string fieldname = item.Name;
            string fieldType = item.FieldType.Name;
            if (apperTimes.ContainsKey(item.FieldType.Name))
            {
                apperTimes[item.FieldType.Name] = apperTimes[item.FieldType.Name] + 1;
            }
            else
            {
                apperTimes.Add(item.FieldType.Name, 1);
            }
            switch (fieldType)
            {
                case "ItemPage_Data":
                    
                    ans += $"{fieldname}_Placement=new XAndY[d.{fieldname}.items.Length];\n";
                    ans += $"for(int i=0;i<d.{fieldname}.items.Length;i++)\n";
                    ans += "{\n";
                    ans += $"   {fieldname}_Placement[i]=new XAndY(d.{fieldname}.GetItemLeftUp(i));\n";
                    ans += "}\n";
                    break;
                case "Dictionary`2"://字典类型
                    Type[] typeParameters = item.FieldType.GetGenericArguments();
                    string type1 = typeParameters[0].Name;
                    string type2 = typeParameters[1].Name;

                    ans+="{\n";
                    ans+=$"List<{type1}> keys=new List<{type1}>();\n"; 
                    ans+=$"List<{type2}> values=new List<{type2}>();\n";
                    ans+=$"foreach(var item in d.{fieldname})\n";
                    ans+="{\n";
                    ans+="keys.Add(item.Key);\n";
                    ans+="values.Add(item.Value);\n";
                    ans+="}\n";
                    ans+=$"{fieldname}_Keys=keys.ToArray();\n";
                    ans+=$"{fieldname}_Values=values.ToArray();\n";
                    ans+="}\n";

                    break;
                case "DateTime":
                    if (apperTimes[fieldType] == 1)
                    {
                        ans += " Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();\n timeConverter.DateTimeFormat = \"yyyy-MM-dd HH:mm:ss\";";
                    }
                    ans += $"Date_{fieldname}=JsonConvert.SerializeObject(unit.{fieldname},Newtonsoft.Json.Formatting.None,timeConverter);\n";
                    break;
                default:
                    break;
            }
        }

        ans += "}";
        return ans;
    }
    string BuildGetMethod(Type t)
    {
        string ans = "";
        /*方法原型：
            public functiondata Get()
            {
                [
                unit.ingredium.init();
                if(ingredium_Placement==null||ingredium_Placement.Length==0)
                {
                    Item[] its=unit.ingredium.items;
                    unit.ingredium.items=null;
                    unit.ingredium.AddItems(its);
                }
                else
                {
                    for (int i = 0; i < ingredium_Placement.Length; i++)
                    {
                        int[] size = unit.ingredium.items[i].GetSize();
                        for (int j = 0; j < size[0]; j++)
                        {
                            for (int k = 0; k < size[1]; k++)
                            {
                                unit.ingredium.placement[ingredium_Placement[i].x + j, ingredium_Placement[i].y + k] = i;
                            }
                        }
                    }
                 }
                ]

                [
                    unit.exd=new dictionary<string,string>();
                    for(int i=0;i<exd_Keys.length;i++)
                    {
                        unit.exd.add(exd_Keys[i],exd_Values[i]);
                    }

                ]
                [
                Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                    unit.date=JsonConvert.DeserializeObject<DateTime>(Date_date,timeConverter);
                ]

                return unit;
            }
        */
        Dictionary<string, int> apperTimes = new Dictionary<string, int>();

        ans += " public " + t.Name + " Get() \n{";
        FieldInfo[] fs = t.GetFields();
        foreach (var item in fs)
        {
            string fieldname = item.Name;
            string fieldType = item.FieldType.Name;
            if (apperTimes.ContainsKey(item.FieldType.Name))
            {
                apperTimes[item.FieldType.Name] = apperTimes[item.FieldType.Name] + 1;
            }
            else
            {
                apperTimes.Add(item.FieldType.Name, 1);
            }
            switch (fieldType)
            {
                case "ItemPage_Data":

                    ans += $" unit.{fieldname}.init(); \n ";//生成placement数组
                                                            //ans+=$"unit.{fieldname}.items={fieldname}_Items;\n";
                    ans += $"for(int i =0;i<{fieldname}_Placement.Length;i++)\n";
                    ans += "{\n";
                    ans += $" int[] size=unit.{fieldname}.items[i].GetSize();\n";
                    ans += $"for(int j=0;j<size[0];j++)\n";
                    ans += "{\n";
                    ans += " for(int k=0;k<size[1];k++)\n";
                    ans += "{\n";
                    ans += $"unit.{fieldname}.placement[{fieldname}_Placement[i].x+j,{fieldname}_Placement[i].y+k]=i;\n";
                    ans += @" }
                        }
                    }";
                    break;
                case "Dictionary`2"://字典类型
                    Type[] typeParameters = item.FieldType.GetGenericArguments();
                    string type1 = typeParameters[0].Name;
                    string type2 = typeParameters[1].Name;
                    ans += $"unit.{fieldname}=new Dictionary<{type1},{type2}>();\n";
                    ans += $"  for(int i=0;i<{fieldname}_Keys.Length;i++)\n";
                    ans += " {\n";
                    ans += $"unit.{fieldname}.Add({fieldname}_Keys[i],{fieldname}_Values[i]);\n";
                    ans += "}\n";
                    break;
                case "DateTime":
                    if(apperTimes[fieldType]==1)
                    {
                       ans+=" Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();\n timeConverter.DateTimeFormat = \"yyyy-MM-dd HH:mm:ss\";";
                    }
                    ans += "try{ \n";
                    ans+= $"unit.{fieldname} = JsonConvert.DeserializeObject<DateTime>(Date_{fieldname}, timeConverter);";
                    ans += "}\n";
                    ans += "catch (Exception)\n";
                    ans += "{\n";
                    ans += $"unit.{fieldname} = default(DateTime);\n";
                    ans += "}\n";


                    break;
                default: break;
            }
        }
        ans += " return unit; \n} ";

        return ans;
    }
    string[] UnpackVariable(FieldInfo t)
    {
        List<string> ans = new List<string>();


        string name = t.Name;
        switch (t.FieldType.Name)
        {
            case "ItemPage_Data":
                //ans.Add($"public int[] {name}_Size; \n");
                //ans.Add($"public Item[] {name}_Items; \n");
                ans.Add("[Tooltip(\"每个item的左上角坐标，长度等于paget的item数\")] \n");
                ans.Add($"public XAndY[] {name}_Placement; \n");
                //ans.Add($"public int[] {name}_Placement_Y; \n");
                break;
            case "Dictionary`2":
                Type[] typeParameters = t.FieldType.GetGenericArguments();
                string type1 = typeParameters[0].Name;
                string type2 = typeParameters[1].Name;
                ans.Add($"public {type1}[] {name}_Keys; \n");
                ans.Add($"public {type2}[] {name}_Values; \n");
                break;
            case "DateTime":
                ans.Add("[Tooltip(\"形如yyyy-MM-DD HH24:MI:ss\")]\n");
                ans.Add($"public string Date_{name}; \n");
                break;
            default: break;
        }
        return ans.ToArray();
    }
    public List<T> getAll<T>()
    {
        List<T> datas = new List<T>();
        Assembly assembly = this.GetType().Assembly;
        Type[] types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.IsSubclassOf(typeof(T)) && !type.IsAbstract)
            {
                datas.Add((T)assembly.CreateInstance(type.Name));
            }
        }

        return datas;
    }
}