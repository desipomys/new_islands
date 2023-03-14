using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[Serializable]
//���ڿ�ʼ������ʾ��ͼ���ӹ�ϵ�������ڵ�ͼ���ƶ��ɱ�
[CreateAssetMenu(menuName = "Map/��ͼ���ӹ�ϵ")]
public class MapGraphData :Base_SCData
{
    public string mapName;
   
    public int x, y;//�ڵ��ڵ�ͼ�е�����
    public int landTime;//��½����
    public MapEdgeData[] edges;//��ýڵ������ı�

    public MapExtraData exd;
}
[Serializable]
/// <summary>
/// ��ͼ���������ڵ��һ����
/// </summary>
public class MapEdgeData
{
   
    public string target;
    public Resource_Data moveCost;//ֻ��ȥĿ�ĵصĳɱ������صĳɱ�������һ��graphData��
    public float moveTimeCost;//�ƶ���ʱ
    public float distance;//���룬�Ժ��ƶ����ĺ��ƶ���ʱ���������
    [DictionaryDrawerSettings()]
    [ShowInInspector]
    public Dictionary<string,float> edgeTag;

}
[Serializable]
public class MapExtraData
{
    public int danger;

}
