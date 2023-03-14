using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEParm : BaseEventArg
{
    public string type;
    public Damage dam;
    public BaseTool tool;
    public long sourceUUID;

    public PhyBullet phyBehav;

    public float time;
    public Shape3D shape;
    public float[] shapeArgs;
    public Vector3 pos;
    public Vector3 targetPos;

    public Dictionary<string,object> parms;

    /*
     *�ӵ�Ԥ�������˺����˺����͡����乤�����͡�������
���ֵ�Ԫ��
����ʱ�䡢AOE��״����С����[]������λ�á�����Ŀ��λ�á�
obj[]�������� 
     */
}
