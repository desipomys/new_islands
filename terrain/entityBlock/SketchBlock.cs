using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 跟随鼠标的虚像脚本
/// </summary>
public class SketchBlock : MonoBehaviour
{
    bool oldAvalidFlag = false;//上次是可用还是不可用

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
        
        //关闭碰撞体，合并mesh材质，颜色统一调控
    }
    public void SetAvalible(bool stat)
    {
        if(oldAvalidFlag!=stat)
        {
            oldAvalidFlag = stat;
            if(stat)//从不可用变为可用，红转绿
            {
                m.SetColor("_Color", Color.green);
            }
            else//从可用变为不可用，绿转红
            {
                m.SetColor("_Color", Color.red);
            }
        }
    }
}
