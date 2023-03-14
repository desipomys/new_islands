using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Data;
using ExcelDataReader;
//using SAEA.Common.Serialization;
using Newtonsoft.Json;
using UnityEditor;


public static class ItemExcelReader 
{
    [Header("读取item.xls")]
    public static string fileNamePath = "E:\\U3D开发\\remotenewcode\\数据excel表\\items.xls";

    public static DataSet GetDataSet(string name)
    {
        IExcelDataReader excelReader = null;
        DataSet dataSet = null;
        using (var stream = File.Open(name, FileMode.Open, FileAccess.Read))
        {

            string extension = Path.GetExtension(name);
            if (extension.ToUpper() == ".XLS")
            {
                if (stream == null) Debug.Log("null");
                excelReader = ExcelReaderFactory.CreateReader(stream);
            }
            else if (extension.ToUpper() == ".XLSX")
            {
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            else
            {
                Debug.Log("格式错误");
                //return null;

            }
            //dataSet = excelReader.AsDataSet();//第一行当作数据读取
            dataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });//第一行当作列名读取
            excelReader.Close();


            /*for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                Debug.Log(dataSet.Tables[i].TableName);
            }   */
            return dataSet;
        }
    }

    public static void processDataSetForItem(DataSet ds)
    {
        for (int i = 0; i < ds.Tables.Count; i++)
        {
            for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
            {
                DataRow dr = ds.Tables[i].Rows[j];
                int id = 0;
                if (int.TryParse(dr["id"].ToString(), out id))
                {
                    string name = dr["名称"].ToString();
                    string textureName= dr["图标"].ToString();
                    if (id!=0&&!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(textureName))
                    {
                        Item_Warper temp = dataRow2Item(dr);
                        SaveItemSC(temp);
                    }
                }



            }
        }
        AssetDatabase.Refresh();
    }
    public static Item_Warper dataRow2Item(DataRow dr)
    {
        int id = int.Parse(dr["id"].ToString());
        string name = dr["名称"].ToString();
        string textureName = dr["图标"].ToString();
        int w = 1;
        int h = 1;
        int.TryParse(dr["宽度x"].ToString(), out w);
        int.TryParse(dr["长度y"].ToString(), out h);
        int max = 20;
        int.TryParse(dr["最大数量"].ToString(), out max);
        float material = 0;
        float energy = 0;
        float manpower = 0;
        float infomation = 0;
        float.TryParse(dr["material"].ToString(), out material);
        float.TryParse(dr["energy"].ToString(), out energy);
        float.TryParse(dr["manPower"].ToString(), out manpower);
        float.TryParse(dr["information"].ToString(), out infomation);

        Item_Warper temp = new Item_Warper();
        temp.item = new Item(id, 1);
        temp.nam = name;
        temp.texture = textureName;
        temp.w = w;
        temp.h = h;
        temp.max = max;
        temp.group = getIG(dr["类型"].ToString());
        temp.descript = dr["描述"].ToString();
        temp.typeValues = new Resource_Data(material, energy, manpower, infomation);

        return temp;
    }
    public static ItemGroup getIG(string s)
    {
        switch (s)
        {
            case "资源":
                return ItemGroup.resource;
                break;
            //资源,工具,武器,装备,配件,食物,皮肤,其他
            case "工具":
                return ItemGroup.tool;
                break;
            case "武器":
                return ItemGroup.weapon;
                break;
            case "装备":
                return ItemGroup.equip;
                break;
            case "配件":
                return ItemGroup.part;
                break;
            case "食物":
                return ItemGroup.food;
                break;
            case "皮肤":
                return ItemGroup.skin;
                break;
            case "其他":
                return ItemGroup.other;
                break;
            default:
                return ItemGroup.resource;
                break;
        }
    }

    static string tuneName(string s)
    {
        if(s.StartsWith("."))
        {
            return "_" + s;
        }

        return s;
    }
    public static void SaveItemSC(Item_Warper sc)
    {
        string temp = itemSCPath;
        switch (sc.group)
        {
            case ItemGroup.hide:
                break;
            case ItemGroup.resource:
                break;
            case ItemGroup.tool:
                
                //break;
            case ItemGroup.weapon:
                
                //break;
            case ItemGroup.gun:
                //break;
            case ItemGroup.equip:
                //break;
            case ItemGroup.part:
                //break;
            case ItemGroup.food:
                //break;
            case ItemGroup.skin:
                //break;
            case ItemGroup.other:
                //break;
            case ItemGroup.block:
                temp += "/"+sc.group.ToString().ToLower();
                break;
            default:
                break;
        }
        temp += "/"+tuneName(sc.nam);
        
        Item_Warper target = Resources.Load<Item_Warper>(temp);
        
        if(target!=null)
            AssetDatabase.DeleteAsset(temp + ".asset");
        try
        {
            AssetDatabase.CreateAsset(sc, temp+".asset");
        }
        catch (System.Exception)
        {
            AssetDatabase.CreateFolder(itemSCPath,sc.group.ToString().ToLower());
            AssetDatabase.CreateAsset(sc, temp+".asset");
        }
        
    }

   static string itemSCPath = "Assets/Resources/SC/ITEM";

    [MenuItem("newIsland/读取itemExcel")]
    // Start is called before the first frame update
   static void run()
    {

        
        //FileStream stream = null;
        DataSet ds = GetDataSet(fileNamePath);
        processDataSetForItem(ds);
        Debug.Log("读取完成");
    }

}
