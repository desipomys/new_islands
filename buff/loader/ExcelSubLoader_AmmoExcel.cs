using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using System.IO;
using System.Data;
using ExcelDataReader;

public class ExcelSubLoader_AmmoExcel:excelSubLoader
{
    /// <summary>
    /// itemid,level,列的key,值
    /// </summary>
    /// <typeparam name="int"></typeparam>
    /// <typeparam name="int"></typeparam>
    /// <typeparam name="string"></typeparam>
    /// <typeparam name="FP"></typeparam>
    /// <returns></returns>
    public ThreeKeyDictionary<int,int,string,FP> datas=new ThreeKeyDictionary<int, int, string, FP>();
    int count = 0;

    public override void OnLoaderInit()
    {
        base.OnLoaderInit();
        base.excelName= "EXCEL/AmmoParms.xls";
    }
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.RegistFunc<Item, Dictionary<string, FP>>(nameof(EventNames.GetAmmoExcelData), GetData);
    }
    
    public override void Run(DataSet ds)
    {
        datas.Clear();
        for (int i = 0; i < ds.Tables.Count; i++)
        {
            DataTable dt= Filter(ds.Tables[i]);
            foreach (DataRow item in dt.Rows)
            {
                Preprocess(item);
                
            }
            ReadToDic(dt);
        }

        Debug.Log("有" + count + "条ammo参数被加载");
    }
     public override void ReadToDic(DataTable dt)
    {
        /**/
        foreach (DataRow item in dt.Rows)
        {
            int itemid = int.Parse(item["itemID"].ToString());
            int level = int.Parse(item["level"].ToString());

            for (int i = 3; i < dt.Columns.Count; i++)
            {
                datas.Add(itemid, level, dt.Columns[i].ColumnName, new FP(item[i]));
                
            }
            count++;
        }
    }
    public override bool CheckRowAvaliable(DataRow dr)
    {
        if(dr["itemID"]==null||dr["level"]==null)return false;
        int itemID,level;
        if(!int.TryParse(dr["itemID"].ToString(),out itemID)) return false;
        if (!int.TryParse(dr["level"].ToString(),out level))return false;
        
        return true;
    }
    public override void Preprocess(DataRow ds)
    {
        base.Preprocess(ds);
        if(string.IsNullOrEmpty(ds["弹头数量"].ToString()))ds["弹头数量"]=1;
    }

    public Dictionary<string,FP> GetData(Item it)
    {
        return datas[it.id][it.GetTrueLevel];
    }
}