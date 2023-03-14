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

public class functionExcelReader : MonoBehaviour
{
    string itemPath = "E:\\U3D开发\\remotenewcode\\数据excel表\\items.xls"; 
    Dictionary<string,int> itemName2ID=new Dictionary<string,int>();
    void BuildItem()
    {
        DataSet ds=GetDataSet(itemPath);//item的xsl路径

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
                       itemName2ID.Add(name,id);
                    }
                }



            }
        }
        Debug.Log("共有" + itemName2ID.Count + "个item");
        foreach (var item in itemName2ID)
        {
            Debug.Log(item.Key + ":" + item.Value);
        }
    }


    public DataSet GetDataSet(string name)
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

#region craft
    public void processDataSetForCraft(DataSet ds)
    {
        Debug.Log(itemName2ID["木板"]);
        int i=0;
        for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
        {
            DataRow dr = ds.Tables[i].Rows[j];
            int id = 0;
            if (int.TryParse(dr["id"].ToString(), out id))
            {
                string productnum = dr["产物数量"].ToString();
                string product= dr["产物"].ToString();
                if (id!=0&&!string.IsNullOrEmpty(productnum) && !string.IsNullOrEmpty(product))
                {
                    try
                    {
                        if (itemName2ID.ContainsKey(product))
                        {
                            Debug.Log(product + "存在");
                            int productid = itemName2ID[product];
                            if (productid != 0)
                            {
                                Craft_Data temp = dataRow2Craft(dr);
                                if(temp != null)
                                SaveCraftSC(temp, product);
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        throw;
                        //Debug.LogError(product + ":item找不到");
                    }
                   
                }
            }



        }
        
        AssetDatabase.Refresh();
    }
    public Craft_Data dataRow2Craft(DataRow dr)
    {
        int uuid=0;
        int.TryParse(dr["id"].ToString(), out uuid);
        int productid = itemName2ID[dr["产物"].ToString()] ;
        int productnum=0;
        int.TryParse(dr["产物数量"].ToString(), out productnum);
        int relyTech=0;
        int.TryParse(dr["依赖"].ToString(), out relyTech);
        int time=1;
        int.TryParse(dr["用时"].ToString(), out time);

        int[] matid = new int[5];
        int[] matnum= new int[5];
        for (int i = 0; i < 5; i++)
        {
            string n="原料";
            string m="数量";
            try
            {
                if (!itemName2ID.ContainsKey(dr[n + (i + 1).ToString()].ToString()))//如果配方中的物品不存在
                    return null;
                matid[i]=itemName2ID[dr[n+(i+1).ToString()].ToString()] ;
                matnum[i]=int.Parse(dr[m+(i+1).ToString()].ToString()) ;
            }
            catch (System.Exception)
            {
                matid[i]=0;
                matnum[i]=0;
            }
        } 
        int matsize=0;
        for (int i = 0; i < 5; i++)
        {
            if(matid[i]!=0&&matnum[i]!=0)matsize++;
        }

        Craft_Data temp = new Craft_Data();
        temp.uuid=uuid;
        temp.product = new Item(productid, productnum);
        temp.ingredium=new Item[matsize];
        int index=0;
        for (int i = 0; i < 5; i++)
        {
            if(matid[i]!=0&&matnum[i]!=0)
            {
                temp.ingredium[index]=new Item(matid[i],matnum[i]);
                index++;
            }
        }
        temp.relyOnTech=new int[]{relyTech};//依赖科技
        temp.time=time;

        return temp;
    }

    public void SaveCraftSC(Craft_Data sc,string productname)
    {
        string temp = CraftSCPath;
        /*switch (sc.unit.group)
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
                temp += "/"+sc.unit.group.ToString().ToLower();
                break;
            default:
                break;
        }*/
        temp += "/"+productname;
        
        Craft_Data target = Resources.Load<Craft_Data>(temp);
        
        if(target!=null)
            AssetDatabase.DeleteAsset(temp + ".asset");
        AssetDatabase.CreateAsset(sc, temp+".asset");
    }
#endregion

#region firePit
    public void processDataSetForFirePit(DataSet ds)
    {
        
        int i=1;
        for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
        {
            DataRow dr = ds.Tables[i].Rows[j];
            int id = 0;
            if (int.TryParse(dr["id"].ToString(), out id))
            {
                string productnum = dr["产物数量"].ToString();
                string product= dr["产物"].ToString();
                if (id!=0&&!string.IsNullOrEmpty(productnum) && !string.IsNullOrEmpty(product))
                {
                    try
                    {
                        if (itemName2ID.ContainsKey(product))
                        {
                            Debug.Log(product + "存在");
                            int productid = itemName2ID[product];
                            if (productid != 0)
                            {
                                FirePit_Data temp = dataRow2FirePit(dr);
                                if(temp!=null)
                                SaveFirePitSC(temp, product);
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        throw;//bsa_icut改制造命令字段长度
                        //Debug.LogError(product+":item找不到");
                    }
                    
                }
            }
        }
        
        AssetDatabase.Refresh();
    }
    public FirePit_Data dataRow2FirePit(DataRow dr)
    {
        int uuid=0;
        int.TryParse(dr["id"].ToString(), out uuid);
        int productid = 0;
        try
        {
            productid = itemName2ID[dr["产物"].ToString()];
        }
        catch (System.Exception)
        {
            Debug.LogError(dr["产物"].ToString() + ":item不存在"); return null;
        }
        
        int productnum=0;
        int.TryParse(dr["产物数量"].ToString(), out productnum);
        int relyTech=0;
        int.TryParse(dr["依赖"].ToString(), out relyTech);
        float time=1;
        float.TryParse(dr["用时"].ToString(), out time);
        float fuel=1;
        float.TryParse(dr["燃料"].ToString(), out fuel);
        int matid = 0;
        try{
            matid = itemName2ID[dr["原料1"].ToString()];
        }
        catch{ Debug.LogError(dr["原料1"].ToString()+":item不存在"); return null; }
        int matnum=0;
        int.TryParse(dr["数量1"].ToString(), out matnum);
        

        FirePit_Data temp = new FirePit_Data();
        temp = new FirePit_Data();
        temp.uuid=uuid;
        temp.product = new Item(productid, productnum);
        temp.mat=new Item(matid,matnum);
       
        temp.relyOnTech=new int[]{relyTech};//依赖科技
        temp.time=time;
        temp.fuel=fuel;

        return temp;
    }

    public void SaveFirePitSC(FirePit_Data sc,string productname)
    {
        string temp = FirePitSCPath;
        temp += "/"+productname;
        
        FirePit_Data target = Resources.Load<FirePit_Data>(temp);
        
        if(target!=null)
            AssetDatabase.DeleteAsset(temp + ".asset");
        AssetDatabase.CreateAsset(sc, temp+".asset");
    }
#endregion

#region furnance

#endregion

    string CraftSCPath = "Assets/Resources/SC/FUNCTIONDATA/CRAFT";
    string FirePitSCPath = "Assets/Resources/SC/FUNCTIONDATA/FIREPIT";
    string FurnanceSCPath = "Assets/Resources/SC/FUNCTIONDATA/FURNANCE";
    //string CraftSCPath = "Assets/Resources/SC/FUNCTION/CRAFT";
    // Start is called before the first frame update
    void CreateFolder()
    {
        if (!Directory.Exists(Path.Combine(System.Environment.CurrentDirectory, FirePitSCPath)))
        {
            Directory.CreateDirectory(Path.Combine(System.Environment.CurrentDirectory, FirePitSCPath));
        }
        if (!Directory.Exists(Path.Combine(System.Environment.CurrentDirectory, FurnanceSCPath)))
        {
            Directory.CreateDirectory(Path.Combine(System.Environment.CurrentDirectory, FurnanceSCPath));
        }
        
    }
    void Start()
    {
        CreateFolder();
        BuildItem();

        string fileNamePath = "E:\\U3D开发\\remotenewcode\\数据excel表\\craft.xls";
        //FileStream stream = null;
        DataSet ds = GetDataSet(fileNamePath);
        processDataSetForCraft(ds);
        processDataSetForFirePit(ds);

        Debug.Log("完成");
    }

}
