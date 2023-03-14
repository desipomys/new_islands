using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


///转炉
[Serializable]
[CreateAssetMenu(menuName = "dataEditor/function/转炉配方")]
public class Convert_Data:Base_Functioning_Data
{
   public Item LiquiBase;//液态金属
   public Item[] mat;
   public Item product;
    
}
