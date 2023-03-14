using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


///电炉
[Serializable]
[CreateAssetMenu(menuName = "dataEditor/function/电弧炉配方")]
public class ElectriFurnance_Data:Base_Functioning_Data
{
   public Item LiquiBase;//液态金属，但可有可无
   public Item[] mat;
   public Item product;
    
}
