using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCOverView : MonoBehaviour,IUIInitReciver
{
    public Camera[] cams;
    public Transform[] NPCGroup;
    public RenderTexture[] npcImgs;

    NPCDataShower_3D[] shower;
    string showerResPath= "Prefabs/player/NPCManModel";

    public void Init()
    {
        GameObject showerRes = Resources.Load<GameObject>(showerResPath);
        shower = new NPCDataShower_3D[cams.Length];
        for (int i = 0; i < cams.Length; i++)
        {
            shower[i] = GameMainManager.CreateGameObject(showerRes).GetComponent<NPCDataShower_3D>();
            shower[i].transform.SetParent(NPCGroup[i]);
            shower[i].transform.localPosition = Vector3.zero;
            shower[i].transform.LookAt(new Vector3(cams[i].transform.position.x,transform.position.y,cams[i].transform.position.z));
            shower[i].gameObject.SetActive(false);
        }
        EventCenter.WorldCenter.ListenEvent<int, NpcData, Action<RenderTexture>>
            (nameof(EventNames.GetNPCBody3DImg), OnRenderTextureRequire);
    }

    void OnRenderTextureRequire(int index, NpcData nd, Action<RenderTexture> func)
    {
        func(Show(index, nd));
    }
    
    public RenderTexture Show(int index,NpcData data)
    {
        if (index >= cams.Length||index<0) return null;
        
            //ÏÔÊ¾NPCÔ¤ÖÆ
        shower[index].Show(data);
        return npcImgs[index];
       
    }

    public void UIInit(UICenter center, BaseUIView view)
    {
        Init();
    }
}
