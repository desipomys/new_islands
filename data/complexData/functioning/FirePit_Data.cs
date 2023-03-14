using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


[Serializable]
[CreateAssetMenu(menuName = "dataEditor/function/火堆配方")]
public class FirePit_Data:Base_Functioning_Data
{
   public Item mat;
   public Item product;

    public float fuel;

}
