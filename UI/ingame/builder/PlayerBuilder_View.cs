using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerBuilder_View : BaseUIView
{//����ֻ�ܷ���eblock
    public Transform GroupRoot;
    public BuildScrollView view;
    public bool InVirtualMode = false;

    GameObject sketchShadow;
    

    int currentTech = 0;
    int currentClass = 0;

    /// <summary>
    /// ��ǰѡ��Ľ��������index
    /// </summary>
    int currentSelected =0;

    public override void UIInit(UICenter center)
    {
        base.UIInit(center);

        //view.UIInit(0);
        //view.Listen(onItemScrollSlotHit);
   
    }
    public override void OnUIOpen(int posi = 0)
    {
        
        BuildMVConnect(UIName, EventCenter.WorldCenter.GetParm<EventCenter>(nameof(EventNames.GetLocalPlayer)),EventCenter.WorldCenter);
        base.OnUIOpen(posi);
        uiCenter.PausePlayer(true);
        UpdateCurrent();
    }
    public override void OnUIClose()
    {
        base.OnUIClose();
        BreakMVConnect(UIName, EventCenter.WorldCenter.GetParm<EventCenter>(nameof(EventNames.GetLocalPlayer)), EventCenter.WorldCenter);
        InVirtualMode = false;
        uiCenter.PausePlayer(false);
        Debug.Log("�رս������");
    }

    /// <summary>
    /// ��ȡ���еȼ���eblock/bblock��������UI��ť
    /// ��������ʱ������ҽ����ȼ���ʾ���õĽ��������������õĽ���
    /// 
    /// </summary>
    /// <param name="viewname"></param>
    /// <param name="model"></param>
    public override void BuildMVConnect(string viewname, EventCenter model, EventCenter target)
    {
        base.BuildMVConnect(viewname, model,target);
        
    }
    
   
    public override bool OnESC()
    {
        InVirtualMode = false;
        return true;
    }

    void UpdateCurrent()
    {
        //�Ե�ǰT�����ȡҪ��ʾ��eblock��bblock
        if (currentClass == 2)//����
        {

        }
        else    
        {//Debug.Log("�ȼ������"+currentTech + ":" + currentClass);
           /* Entity_BlockModel[] ebs = EventCenter.WorldCenter.GetParm<int, int, Entity_BlockModel[]>(nameof(EventNames.GetEBlockByLvAndClass), currentTech, currentClass);
            Debug.Log(ebs.Length + "��");
            view.SetBuildings(ebs);*/
        }
    }
    
    public void SetScrollViewVisable(bool stat)
    {
        view.gameObject.SetActive(stat);
    }
   
    
}
