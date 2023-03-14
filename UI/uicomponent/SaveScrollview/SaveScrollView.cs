using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveScrollView : Base_UIComponent,IMVConnector
{
    public RectTransform selectbox;
    public RectTransform content;
    public UI_ModelType modelType;
    public RectTransform saveSlots;

    string showerPath = "Prefabs/UI/inGame/unit/DefaultSave";
    string currentSave = "";
    CompShowerManager showerManager = new CompShowerManager();

    /// <summary>
    /// �浵·����showermanager�ڲ�index��ӳ��
    /// </summary>
    Dictionary<string, int> savepath_index = new Dictionary<string, int>();

    public override void UIInit(UICenter center, BaseUIView view)
    {
        base.UIInit(center, view);
        Base_Shower bs = Resources.Load<GameObject>(showerPath).GetComponent<Base_Shower>();
        showerManager.Init(bs,saveSlots,this);
        
    }

    public override void OnUIOpen(UICenter center, BaseUIView view)
    {
        base.OnUIOpen(center, view);

    }

    public override void OnEvent(object p1, object p2)
    {
        base.OnEvent(p1, p2);//���������Ե�ǰ
        currentSave = (string)p1;
        selectbox.position = ((RectTransform)p2).position;
    }
    public override void OnEvent(object parm)
    {
        base.OnEvent(parm);
        //˫��,����浵
        if (currentSave == (string)parm)
        {
            ((UICompCTRL_saveScrollView)controll).EnterSave((string)parm);
        }
    }

    public UI_ModelType GetModelType()
    {
        return modelType;
    }

    public void BuildMVConnect(string viewname, EventCenter model)
    {
        //����Loader_SaveData�Ĵ浵������ɾ���¼�
        //��Loader_SaveData��ȡ���д浵
        GenSaveSlots();

    }
    

    public void BreakMVConnect(string viewname, EventCenter model)
    {
        selectbox.gameObject.SetActive(false);
        savepath_index.Clear();
        showerManager.RecycleAll();
        currentSave = null;
    }

    public string GetSelectingSave()
    {
        return currentSave;
    }
    public void Flush()
    {
        selectbox.gameObject.SetActive(false);
        savepath_index.Clear();
        showerManager.RecycleAll();
        currentSave = null;
        GenSaveSlots();
    }
    void GenSaveSlots()
    {
        Dictionary<string, SaveData> allsaves = EventCenter.WorldCenter.GetParm<Dictionary<string, SaveData>>("GetAllSaveData");
        List<string> sortedSaves = new List<string>();
        foreach (var item in allsaves)
        {
            sortedSaves.Add(item.Key);
        }
        sortedSaves.Sort();
        //Debug.Log(allsaves.Length);
        adjustSlotScrollViewSize(allsaves.Count);
        //showerManager.SetNum(allsaves.Count);
        savepath_index.Clear();
        int tempindex = 0;

        foreach (var item in sortedSaves)
        {
            if (tempindex == 0) { currentSave = item; StartCoroutine(setSlotPosWait()); }//Ĭ��ѡ��һ���浵
            savepath_index.Add(item, tempindex);
            SaveInfoShower sif= (SaveInfoShower)showerManager.GetOrNew(tempindex);
            sif.Show(allsaves[item], item);
            tempindex++;
        }

    }
    IEnumerator setSlotPosWait()
    {
        yield return null;
        selectbox.gameObject.SetActive(true);
        selectbox.position = showerManager.GetRect(0).position;
    }
    /// <summary>
    /// �����浵�������С
    /// </summary>
    /// <param name="size"></param>
    void adjustSlotScrollViewSize(int size)
    {
        content.sizeDelta = new Vector2(0, size * (256 + 5) + 5);

    }
}
