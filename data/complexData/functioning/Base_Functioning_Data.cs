using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Sirenix.OdinInspector;


[Serializable]
public class Base_TechData:SerializedScriptableObject
{
    public int uuid;
    /// <summary>
    /// 依赖于哪些科技，默认必须都满足
    /// </summary>
    public int[] relyOnTech;
    public int level;
   public string descript;
    public float time;

    public Dictionary<string,string> processCode;//可读的指令，
    
}

[Serializable]
public class Base_Functioning_Data:Base_TechData
{
   
    public ItemCompareMode[] matRestrict;
    public bool passLevel=true,addrandom=true;//产物是否继承原料的level,合成时等级是否添加幸运随机，优先于指令执行

    //prod[x].subid=ing[x].num*5+ing[x+1].num*5


    
}
[Serializable]
public class Base_SCData:SerializedScriptableObject
{
    public string[] tags;
}