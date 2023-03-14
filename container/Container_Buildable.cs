using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 提供根据当前角色、playerdata科技树、地图设置获取可建造建筑的功能
/// 可从配置中读取角色、playerdata、地图设置 到 可建造建筑 的映射
/// </summary>
public class Container_Buildable : BaseContainer
{
    ///可通过命令系统对某些EBLOCK/BBLOCK，材质、bblock材质、等级 实现运行时禁用、解禁
    ///地图可限制建筑等级，
    ///
    List<BuildAble_Data> datas;
    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
        e.RegistFunc<GetBuildAbleRequestArg, BuildAble_Data[]>(nameof(EventNames.GetBuildAbleData), GetBuildAble_Datas);
    }
    public override void OnLoadGame(MapPrefabsData data, int index)
    {
        base.OnLoadGame(data, index);
        Load();
    }
    public override void UnLoadGame(int ind)
    {
        if (ind != 0) return;
        base.UnLoadGame(ind);

    }
    void Load()
    {
        BuildAble_Data[] bds= Resources.LoadAll<BuildAble_Data>(path);
        datas = new List<BuildAble_Data>(bds);
        Finetune();
        Debug.Log(datas.Count + "个buildable_data加载完成");
    }
    /// <summary>
    /// 从真正的loader_eblock中取eblock数据替换buildabldata中的eblocktype
    /// </summary>
    void Finetune()
    {
        for (int i = 0; i < datas.Count; i++)
        {
            if (!datas[i].isBBlock)
            { 
             }
        }
    }

    string path = "SC/BuildAble";

    public BuildAble_Data[] GetBuildAble_Datas(GetBuildAbleRequestArg arg)
    {
        if (arg == null) throw new System.Exception();
        return GetBuildAble_Datas(arg.playerBuildLevel, arg.research, arg.map, arg.selectlevel, arg.selectType);
    }

    public BuildAble_Data[] GetBuildAble_Datas(int playerBuildLevel,ResearchData research,MapPrefabsData map,int selectlevel,int selectType)
    {
        int mapmax = map.GetInt(MapPrefabsDataType.MaxTech);
        int levelMax = Mathf.Min(playerBuildLevel, mapmax);//取玩家建筑等级、地图允许等级中最小的作为等级上限
        if (mapmax !=0&&selectlevel > levelMax) return null;

        //返回所有research中已解锁的BuildAble_Data
        List<BuildAble_Data> ans=new List<BuildAble_Data>();
        for (int i = 0; i < datas.Count; i++)
        {
            if((datas[i].level==selectlevel&&datas[i].type==selectType))//等级为0默认可以建造
            {
                if(datas[i].relyOnTech==0||research.ContainUnlock(datas[i].relyOnTech) )//已解锁或不需要科技
                {
                    ans.Add(datas[i]);
                }
            }
        }
        return ans.ToArray();
    }
    
}
public class GetBuildAbleRequestArg:BaseEventArg
{
    public int playerBuildLevel;
    public ResearchData research;
    public MapPrefabsData map;
    public int selectlevel;
    public int selectType;
}


