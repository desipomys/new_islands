using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

///土高炉
[Serializable]
[CreateAssetMenu(menuName = "dataEditor/function/土高炉配方")]
public class Furnance_Data:Base_Functioning_Data
{
   public Item mat1,mat2;
   public Item moduel;
   public Item product;

   

}
