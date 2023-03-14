using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������������ű�
/// </summary>
public class SketchBlock : MonoBehaviour
{
    bool oldAvalidFlag = false;//�ϴ��ǿ��û��ǲ�����

    Renderer[] rends; 
    Material m;

    public void Init()
    {
        Collider[] cols= GetComponentsInChildren<Collider>();
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].enabled = false;
        }

        m = Resources.Load<Material>("material/Building/SketchBlock");

        rends= GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rends.Length; i++)
        {
            Material[] mats = new Material[rends[i].materials.Length];
            for (int j = 0; j < rends[i].materials.Length; j++)
            {
                mats[j] = m;
            }
            rends[i].materials = mats;
        }
        
        //�ر���ײ�壬�ϲ�mesh���ʣ���ɫͳһ����
    }
    public void SetAvalible(bool stat)
    {
        if(oldAvalidFlag!=stat)
        {
            oldAvalidFlag = stat;
            if(stat)//�Ӳ����ñ�Ϊ���ã���ת��
            {
                m.SetColor("_Color", Color.green);
            }
            else//�ӿ��ñ�Ϊ�����ã���ת��
            {
                m.SetColor("_Color", Color.red);
            }
        }
    }
}
