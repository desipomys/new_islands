using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// �ṩ���ݵ�ǰ��ɫ��playerdata�Ƽ�������ͼ���û�ȡ�ɽ��콨���Ĺ���
/// �ɴ������ж�ȡ��ɫ��playerdata����ͼ���� �� �ɽ��콨�� ��ӳ��
/// </summary>
public class Container_Buildable : BaseContainer
{
    ///��ͨ������ϵͳ��ĳЩEBLOCK/BBLOCK�����ʡ�bblock���ʡ��ȼ� ʵ������ʱ���á����
    ///��ͼ�����ƽ����ȼ���
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
        Debug.Log(datas.Count + "��buildable_data�������");
    }
    /// <summary>
    /// ��������loader_eblock��ȡeblock�����滻buildabldata�е�eblocktype
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
        int levelMax = Mathf.Min(playerBuildLevel, mapmax);//ȡ��ҽ����ȼ�����ͼ����ȼ�����С����Ϊ�ȼ�����
        if (mapmax !=0&&selectlevel > levelMax) return null;

        //��������research���ѽ�����BuildAble_Data
        List<BuildAble_Data> ans=new List<BuildAble_Data>();
        for (int i = 0; i < datas.Count; i++)
        {
            if((datas[i].level==selectlevel&&datas[i].type==selectType))//�ȼ�Ϊ0Ĭ�Ͽ��Խ���
            {
                if(datas[i].relyOnTech==0||research.ContainUnlock(datas[i].relyOnTech) )//�ѽ�������Ҫ�Ƽ�
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


