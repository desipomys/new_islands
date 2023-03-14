using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BasementType
{
    basement
}

//在地图上建立的基地数据
public class BaseIslandData 
{
    public BasementType type;
    public string mapName;
    public string mapSavePath; 
}
