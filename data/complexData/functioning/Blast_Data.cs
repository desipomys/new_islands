using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


///高炉
[Serializable]
[CreateAssetMenu(menuName = "dataEditor/function/高炉配方")]
public class Blast_Data:Base_Functioning_Data
{
   public Item[] mat;
   public Item product;
 
    public bool canLiqu;//能否产出液体
}
