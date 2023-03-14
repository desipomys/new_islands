using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
/// <summary>
/// 用于显示单个NPC信息，显示为NPC全身图
/// </summary>
public class NPCfullInfoView : Base_UIComponent, IMVConnector
{
    public UI_ModelType type;
    public RawImage pic, job;
    public Text npcname, level;
    public Slider hp;
    /// <summary>
    /// UIINDEX
    /// </summary>
    public int index;
    public Button viewButton;
    public GameObject father;

    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);
        GetComponent<Button>().onClick.AddListener(OnClick);
       if(viewButton!=null) viewButton.onClick.AddListener(OnViewClick);
    }
    public override void SetPage(int pag)
    {
        base.SetPage(pag);
        index = pag;
        Flush();
    }

    public void Flush()
    {
        if (index == -1) { SetLock(true);return; }
        NpcData nd = EventCenter.WorldCenter.GetParm<int, NpcData>(nameof(Container_PlayerData_EventNames.GetNPCByUIIndex), index);
        Show(nd);

    }
    void OnClick()
    {
        Debug.Log("NPC点击");
        fatherView.OnButtonHit("select", index);
    }
    void OnViewClick()
    {
        Debug.Log("NPC详情");
        fatherView.OnButtonHit("viewNPC", index);
    }

    public void Show(NpcData data)
    {
        int[] select = EventCenter.WorldCenter.GetParm<int[]>(nameof(Container_PlayerData_EventNames.GetNPCSelectedIndexs));
        if (select == null)
        {
            if (father != null) father.SetActive(false);
            Debug.LogWarning("select为空");
            return;
        }
        if(father!=null)father.SetActive(true);
        Debug.Log(select.Length + "and" + index);
        if (select.Length <= index) { SetLock(true); }
        else
        {
            SetLock(false);
            if (data != null)
            {
                Debug.Log("npc非空");
                npcname.text = data.npcName;
                level.text = "LV:" + data.level.ToString();
                hp.gameObject.SetActive(true);
                hp.value = data.char_data.health;

                pic.gameObject.SetActive(true);
                EventCenter.WorldCenter.SendEvent<int, NpcData, Action<RenderTexture>>(nameof(EventNames.GetNPCBody3DImg), index, data, SetNPCPic);//以后要改为获取人物三维模型的rendertexture

                job.gameObject.SetActive(true);
                if(viewButton!=null)viewButton.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("npc空");
                npcname.text = "";
                level.text = "";
                hp.value = 1;
                hp.gameObject.SetActive(false);

                //job.texture = null;
                job.gameObject.SetActive(false);
                //pic.texture = null;
                pic.gameObject.SetActive(false);
                if (viewButton != null) viewButton.gameObject.SetActive(false);
                //gameObject.SetActive(false);
            }
        }
    }
    void SetNPCPic(RenderTexture rt)
    {
        pic.texture = rt;
    }
    public void OnNPCChg(NPCDataChangeParm parm)
    {
        if (parm.UIindex == index)
            Show(parm.data);
    }
    public void OnSelectChg(int[] selects)
    {
        if (selects == null) return;
        if (selects.Length <= index)
        {
            //当前槽位不可用，显示锁定
            SetLock(true);
        }
        else if (selects[index] < 0) Show(null);//当前槽位可用但是为空
        else
        {
            Flush();
        }
    }
    /// <summary>
    /// 隐藏
    /// </summary>
    /// <param name="stat"></param>
    void SetLock(bool stat)
    {
        gameObject.SetActive(!stat);
    }

    public void BreakMVConnect(string viewname, EventCenter model)
    {
        EventCenter.WorldCenter.UnListenEvent<NPCDataChangeParm>(nameof(Container_PlayerData_EventNames.NPCDataChg), OnNPCChg);
    }

    public void BuildMVConnect(string viewname, EventCenter model)
    {
        NpcData nd = EventCenter.WorldCenter.GetParm<int, NpcData>(nameof(Container_PlayerData_EventNames.GetNPCByUIIndex), index);
        Show(nd);
        EventCenter.WorldCenter.ListenEvent<NPCDataChangeParm>(nameof(Container_PlayerData_EventNames.NPCDataChg), OnNPCChg);
        EventCenter.WorldCenter.ListenEvent<int[]>(nameof(Container_PlayerData_EventNames.NPCSelectChg), OnSelectChg);
    }

    public UI_ModelType GetModelType()
    {
        return type;
    }
}
