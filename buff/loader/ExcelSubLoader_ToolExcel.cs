using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using System.IO;
using System.Data;
using ExcelDataReader;

public class ExcelSubLoader_ToolExcel : excelSubLoader
{
    /// <summary>
    /// itemid,level,列的key,值
    /// </summary>
    /// <typeparam name="int"></typeparam>
    /// <typeparam name="int"></typeparam>
    /// <typeparam name="string"></typeparam>
    /// <typeparam name="FP"></typeparam>
    /// <returns></returns>
    public ThreeKeyDictionary<int, int, string, FP> datas = new ThreeKeyDictionary<int, int, string, FP>();
    public TwoKeyDictionary<int, int, string> item_prefabs = new TwoKeyDictionary<int, int, string>();
    int count = 0;

    public override void OnLoaderInit()
    {
        base.OnLoaderInit();
        base.excelName = "EXCEL/ToolParms.xls";
    }
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.RegistFunc<Item, Dictionary<string, FP>>(nameof(EventNames.GetToolExcelData), GetData);
        e.RegistFunc<Item, string>(nameof(EventNames.GetBaseToolNameByItem), GetToolPrefabsName);
    }

    public override void Run(DataSet ds)
    {
        item_prefabs.Clear();
        datas.Clear();
     
            DataTable dt = Filter(ds.Tables[0]);
            foreach (DataRow item in dt.Rows)
            {
                Preprocess(item);

            }
            ReadToDic(dt);
        

        Debug.Log("有" + count + "条tool参数被加载");
    }
    public override void ReadToDic(DataTable dt)
    {
        /**/
        foreach (DataRow item in dt.Rows)
        {
            int itemid = int.Parse(item["itemID"].ToString());
            int level = int.Parse(item["level"].ToString());

            item_prefabs.Add(itemid, level, item[3].ToString());
            for (int i = 4; i < dt.Columns.Count; i++)
            {
                datas.Add(itemid, level, dt.Columns[i].ColumnName, new FP(item[i]));

            }
            count++;
        }
    }
    public override bool CheckRowAvaliable(DataRow dr)
    {
        if (dr["itemID"] == null || dr["level"] == null) return false;
        int itemID, level;
        if (!int.TryParse(dr["itemID"].ToString(), out itemID)) return false;
        if (!int.TryParse(dr["level"].ToString(), out level)) return false;

        return true;
    }
    public override DataTable Filter(DataTable dt)
    {
        return base.Filter(dt);

    }
    public override void Preprocess(DataRow ds)
    {
        base.Preprocess(ds);
        //if (string.IsNullOrEmpty(ds["弹头数量"].ToString())) ds["弹头数量"] = 1;
    }

    public Dictionary<string, FP> GetData(Item it)
    {
        if(datas.ContainsKey(it.id))
            if(datas[it.id].ContainsKey(it.GetTrueLevel))
                return datas[it.id][it.GetTrueLevel];
        return null;
    }
    public string GetToolPrefabsName(Item it)
    {
        if(item_prefabs.ContainsKey(it.id))
            if(item_prefabs[it.id].ContainsKey(it.GetTrueLevel))
                return item_prefabs[it.id][it.GetTrueLevel];
        return null;
    }
}