using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

using System;
using System.IO;
using System.Data;
using ExcelDataReader;

public class Loader_Excel : BaseLoader
{

    string BuffPath = "EXCEL/buff.xls";

    //string ToolItemPath = "EXCEL/itemTool.xls";
    string AmmoParmPath = "EXCEL/AmmoParms.xls";
    string ToolParmPath = "EXCEL/ToolParms.xls";

    public excelSubLoader[] loader;

   /* 
   /// <summary>
    /// buffname,level,parms[]
    /// </summary>
    TwoKeyDictionary<string, int, FP[]> parmList = new TwoKeyDictionary<string, int, FP[]>();
    /// <summary>
    /// itemid,level/1024,basetoolName,
    /// </summary>
    TwoKeyDictionary<int, int, string> ToolList = new TwoKeyDictionary<int, int, string>();
    /// <summary>
    /// itemid,level/1024,model��,
    /// </summary>
    TwoKeyDictionary<int, int, string> ToolModelNameList = new TwoKeyDictionary<int, int, string>();
    /// <summary>
    /// Ԥ������itemlevel,����
    /// </summary>
    TwoKeyDictionary<string, int, FP[]> ToolParmList = new TwoKeyDictionary<string, int, FP[]>();

    TwoKeyDictionary<int, int, FP[]> AmmoParmList = new TwoKeyDictionary<int, int, FP[]>();*/

    public override void OnLoaderInit(int prio)
    {
        if (prio != 0) return;
        loader=GetAllLoader();
        foreach (var item in loader)
        {
            item.OnLoaderInit();
        }

        /*DataSet ds = GetDataSet(BuffPath);//item��xsl·��
        ProcessBuffDataSet(ds);

        DataSet ds1 = GetDataSet(ToolParmPath);//item��xsl·��
        ProcessToolParmsDataSet(ds1);

        DataSet ds2 = GetDataSet(AmmoParmPath);//item��xsl·��
        ProcessAmmoParmsDataSet(ds2);*/
        //����ToolParmPath������excel�ӵ�ToolParmList
    }
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);



        foreach (var item in loader)
        {
            DataSet ds=GetDataSet(item.excelName);
            item.Run(ds);
            item.OnEventRegist(e);
        }

        /*e.RegistFunc<BaseBuff, FP[]>(nameof(EventNames.GetBuffExcelData), GetBuffData);
        e.RegistFunc<Item, BaseTool>(nameof(EventNames.GetBaseToolByItem), GetBaseToolByItem);
        e.RegistFunc<BaseTool, FP[]>(nameof(EventNames.GetToolExcelData), GetToolData);
        e.RegistFunc<Item, FP[]>(nameof(EventNames.GetAmmoExcelData), GetAmmoData);*/
    }

    public excelSubLoader[] GetAllLoader()
    {
        List<excelSubLoader> all=new List<excelSubLoader>();
        Assembly assembly = this.GetType().Assembly;
        Type[] types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.IsSubclassOf(typeof(excelSubLoader)) && !type.IsAbstract)
                {
                excelSubLoader bc = (excelSubLoader)assembly.CreateInstance(type.Name);
                   all.Add(bc);
                }
        }
        return all.ToArray();
    }

    /*FP[] GetBuffData(BaseBuff bf)
    {
        string name = bf.buffname;
        int level = bf.level;
        if (parmList.ContainsKey(name))
        {
            if (parmList[name].ContainsKey(level))
            { return parmList[name][level]; }
            else
            {
                return parmList[name][0];
            }
        }
        else return null;
    }

    BaseTool GetBaseToolByItem(Item it)
    {
        if (!ToolList.ContainsKey(it.id)) { return EventCenter.WorldCenter.GetParm<BaseTool>(nameof(EventNames.GetBareHand)); }

        int trueLevel = it.GetTrueLevel;
        if (ToolList[it.id].Count == 1)
        {//�����item���м���ֻ��һ��Ĭ��tool���򷵻ش�
            trueLevel = new List<int>(ToolList[it.id].Keys)[0];

        }
        BaseTool temp = center.GetParm<string, BaseTool>(nameof(EventNames.GetToolByName), ToolList[it.id][trueLevel]);
        BaseTool newtool = DataExtension.DeepCopy(temp);
        newtool.ModelName = ToolModelNameList[it.id][trueLevel];
        return newtool;
    }

    FP[] GetToolData(BaseTool bt)
    {
        string name = bt.name;
        int level = bt.toolItem.GetTrueLevel;
        if (ToolParmList.ContainsKey(name))
        {
            if (ToolParmList[name].ContainsKey(level))
            { return ToolParmList[name][level]; }
            else
            {
                return ToolParmList[name][0];
            }
        }
        else return null;
    }
    FP[] GetAmmoData(Item it)
    {
        int id =it.id;
        int level =it.GetTrueLevel;
        if (AmmoParmList.ContainsKey(id))
        {
            if (AmmoParmList[id].ContainsKey(level))
            { return AmmoParmList[id][level]; }
            else
            {
                return AmmoParmList[id][0];
            }
        }
        else return null;
    }

    void ProcessBuffDataSet(DataSet ds)
    {
        DataTable dt = ds.Tables[0];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string bfname = dt.Rows[i]["buff����"].ToString();
            int level = int.Parse(dt.Rows[i]["�ȼ�"].ToString());
            FP[] datas = new FP[dt.Columns.Count-4];
            for (int j = 0; j < datas.Length; j++)
            {
                if(dt.Rows[i][j + 4]!=null)
                    datas[j] = new FP(dt.Rows[i][j + 4]);
            }
            parmList.Add(bfname, level, datas);
        }
        Debug.Log("��" + parmList.Count + "��buff���ݱ�����");
    }

   
    void ProcessToolParmsDataSet(DataSet ds)
    {
        DataTable dt = ds.Tables[0];
        for (int i = 0; i < dt.Rows.Count; i++)
        {   int level;int itemID=0;
            if(!int.TryParse(dt.Rows[i]["level"].ToString(),out level)) { continue; }
            if (!int.TryParse(dt.Rows[i]["itemID"].ToString(), out itemID)) { continue; }
            string name = dt.Rows[i]["Ԥ����"].ToString();
            string modName = dt.Rows[i]["ģ����"].ToString();

            FP[] parms = new FP[20];
            for (int j = 0; j < 20; j++)
            {
                parms[j]= dt.Rows[i][j + 2].ToString();
            }
            ToolParmList.Add(name, level, parms);
            ToolList.Add(itemID, level, name);
            ToolModelNameList.Add(itemID, level, modName);
        }

    }

    void ProcessAmmoParmsDataSet(DataSet ds)
    {
        DataTable dt = ds.Tables[0];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            int level; int id = 0;
            if (!int.TryParse(dt.Rows[i]["level"].ToString(), out level)) { continue; }
            if (!int.TryParse(dt.Rows[i]["itemID"].ToString(), out id)) { continue; }

            FP[] parms = new FP[20];
            for (int j = 0; j < 20; j++)
            {
                parms[j] = dt.Rows[i][j + 4].ToString();
            }
            AmmoParmList.Add(id, level, parms);
        }

    }*/

    public DataSet GetDataSet(string name)
    {
        name = "Resources/" + name;
        IExcelDataReader excelReader = null;
        DataSet dataSet = null;
        using (var stream = File.Open(Path.Combine(Application.dataPath ,name), FileMode.Open, FileAccess.Read))
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
                Debug.Log("��ʽ����");
                //return null;

            }
            //dataSet = excelReader.AsDataSet();//��һ�е������ݶ�ȡ
            dataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });//��һ�е���������ȡ
            excelReader.Close();


            /*for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                Debug.Log(dataSet.Tables[i].TableName);
            }   */
            return dataSet;
        }
    }
}
/// <summary>
/// Ϊÿ������excelдһ������������loaderexcel����
/// </summary>
public class excelSubLoader
{
    public string excelName;
    //�����twokeydic
    public virtual void Run(DataSet ds)
    {
        DataTable dt= Filter(ds.Tables[0]);
        foreach (DataRow item in dt.Rows)
        {
            Preprocess(item);
            
        }
        ReadToDic(dt);
    }
    public virtual void ReadToDic(DataTable dr)
    {

    }
    public virtual void Preprocess(DataRow dr)
    {
        
    }
    public virtual DataTable Filter(DataTable dt)
    {
        for (int i = dt.Rows.Count-1; i >=0 ; i--)
        {
            if(!CheckRowAvaliable(dt.Rows[i]))
            {
                dt.Rows.RemoveAt(i);
            }
        }
        return dt;
    }
    public virtual bool CheckRowAvaliable(DataRow dr)
    {
        return false;
    }
    
    public virtual void OnEventRegist(EventCenter e)
    {

    }
    /// <summary>
    /// ����oneventreg
    /// </summary>
    public virtual void OnLoaderInit()
    {

    }
}