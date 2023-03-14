using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public enum AnvilMode
{
    plate,tube,stick,ball,cable
}
///铁砧
[Serializable]
[CreateAssetMenu(menuName = "dataEditor/function/铁砧配方")]
public class Anvil_Data:Base_Functioning_Data
{
   public Item mat;
   public Item product;
   public AnvilMode mode;

    public Anvil_Data(){}
    public Anvil_Data(Anvil_Data ad)
    {
        
        mat=ad.mat;
        product=ad.product;
        mode=ad.mode;
    }
}
