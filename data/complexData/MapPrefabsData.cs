using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Newtonsoft.Json;

[CreateAssetMenu(menuName = "Map/��ͼԤ������")]
[Serializable]
public class MapPrefabsData : Base_SCData
{
    //���ɵĵ�ͼ����Ҫ��ʲô����������������
    public InGameMapGenConfig config = new InGameMapGenConfig();
    public DateTime timeData = new DateTime();//���뵺ʱ�̶���ʱ�䣨0:0-23:59��
    public bool timeMove = true;//ʱ���Ƿ�����
    public MapPointMetaData[] metaDatas;
    public string descript;
    public string mapName = "test";
    public string mapDisplayName;
    public string mapHeadImg;
    public bool generated = false;//�Ƿ�Ԥ�����ͼ

    /// <summary>
    /// ��½������Դ
    /// </summary>
    public Resource_Data land_Resource = new Resource_Data();
    public long maxUUID;

    public Dictionary<MapPrefabsDataType, FP> data;//��Ƽ����Ƶȼ�
    
    public MapPrefabsData() { }
    public MapPrefabsData(string name) { mapName = name; config = new InGameMapGenConfig(256, 256, MapForm.island, TerrainForm.flat); }
    public MapPrefabsData(MapPrefabsData d)
    {
        config = d.config;
        this.timeData = new DateTime(d.timeData.Ticks);
        timeMove = d.timeMove;
        metaDatas = d.metaDatas;
        descript = d.descript;
        mapName = d.mapName;
        mapDisplayName = d.mapDisplayName;
        mapHeadImg = d.mapHeadImg;
        generated = d.generated;
        land_Resource = new Resource_Data(d.land_Resource);
        maxUUID = d.maxUUID;
        data = new Dictionary<MapPrefabsDataType, FP>(d.data);
      
    }
    public long GetMaxUUID() { maxUUID++; return maxUUID; }

    public int GetInt(string nam)
    {
        return GetInt((MapPrefabsDataType)Enum.Parse(typeof(MapPrefabsDataType), nam));
    }
    public float GetFloat(string nam)
    {
        return GetFloat((MapPrefabsDataType)Enum.Parse(typeof(MapPrefabsDataType), nam));
    }
    public string GetStr(string nam)
    {
        return GetStr((MapPrefabsDataType)Enum.Parse(typeof(MapPrefabsDataType), nam));
    }
    public int GetInt(MapPrefabsDataType nam)
    {
        if (data != null && data.ContainsKey(nam))
            return data[nam].Convert<int>();
        else return 0;
    }
    public float GetFloat(MapPrefabsDataType nam)
    {
        if (data != null && data.ContainsKey(nam))
            return data[nam].Convert<float>();
        else return 0;
    }
    public string GetStr(MapPrefabsDataType nam)
    {
        if (data != null && data.ContainsKey(nam))
            return data[nam].Convert<string>();
        else return "";
    }
    public string GetDisplayName()
    {
        if (string.IsNullOrEmpty(mapDisplayName)) return mapName;
        else return mapDisplayName;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, SerializerHelper.setting);
    }
    public static MapPrefabsData FromString(string data)
    {
        return JsonConvert.DeserializeObject<MapPrefabsData>(data);
    }

}