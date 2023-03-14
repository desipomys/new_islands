using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPrefabGenerator:BaseTProcessGenerator
{
    //根据mapprefabdata中建筑群信息、场数据定位具体建筑位置
    //会读取建筑生成规则和建筑预设数据

    //生成阶段分为四个，tb,bb,eb,entity，【事件逻辑】阶段
    //每个阶段可细分为三个小阶段：直接写入，简单随机生成，复杂随机生成

    BPG_TBStage tBStage;
    BPG_BBStage bBStage;
    BPG_EBStage eBStage;
    BPG_EntityStage entityStage;


}
class BPG_TBStage
{

}
class BPG_BBStage
{

}
class BPG_EBStage
{

}
class BPG_EntityStage
{

}
