using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Data;
using ExcelDataReader;

public class Loader_buff : BaseLoader
{
    /// <summary>
    /// resource下的buff scobject不是最终的buff实例，只是buff模板
    /// excel中的同一个buff名称才对应buff实例
    /// key=buff名，value=buff实例
    /// </summary>
    public Dictionary<string, BaseBuff> buffdic = new Dictionary<string, BaseBuff>();
    TwoKeyDictionary<string, int, FP[]> parmList = new TwoKeyDictionary<string, int, FP[]>();
    //如何将excel的配置数据对应到每个buff组件上？
    string path = "";
    string excelPath = "assets/resources/EXCEL/buff.xls";

    public override void OnLoaderInit(int prio)
    {
        if (prio != 0) return;
        buffdic.Clear();
        BaseBuff[] buffs = getBuff();
        ProcessBuff(buffs, GetDataSet(excelPath));
    }
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.RegistFunc<string, BaseBuff>("GetBuffByName", GetBuffByName);
    }

    void ProcessBuff(BaseBuff[] buf,DataSet buffds)
    {
        Dictionary<string, BaseBuff> modula = new Dictionary<string, BaseBuff>();
        for (int i = 0; i < buf.Length; i++)
        {
            modula.Add(buf[i].name, buf[i]);
        }
        buffdic = modula;
        for (int i = 0; i < buffds.Tables[0].Rows.Count; i++)
        {

        }
    }

    BaseBuff[] getBuff()
    {
        return Resources.LoadAll<BaseBuff>(path);
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

    public BaseBuff GetBuffByName(string name)
    {
        return buffdic[name].Copy();//要创建一个新buff实例
    }
}
