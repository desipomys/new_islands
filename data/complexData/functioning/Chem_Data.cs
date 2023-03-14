using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public enum ChemMode
{
    Compose,//合成
    Electiod,//电解
    Heat//高温
}

///化学台
[Serializable]
[CreateAssetMenu(menuName = "dataEditor/function/化学台配方")]
public class Chem_Data:Base_Functioning_Data
{
   public Item[] mat;
   public Item[] product;
 
    public ChemMode mod;
    public float fuel;//电量


}
