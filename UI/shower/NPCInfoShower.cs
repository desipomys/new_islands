using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

/// <summary>
/// 只能用于列表的头像显示
/// </summary>
public class NPCInfoShower:Base_Shower, IPointerEnterHandler,IPointerExitHandler
{
    public int index;
    public RawImage pic;
    public Text level,npcName;
    /// <summary>

    public Slider hp;
    //public IItemScrollView father;

    RectTransform rect;
    public bool allowClick;

    BaseUIView UIview;
   
    public void UnRecycle()
    {
        SetSelected(false);
    }
    public override void ShowerInit(Base_UIComponent f)
    {
        base.ShowerInit(f);
        //allowClick = ;
        level.gameObject.SetActive(false);
        pic.texture = null;
        npcName.gameObject.SetActive(false);
        if (hp != null) hp.gameObject.SetActive(false);
        //GetComponent<Image>().color = new Color(0, 0, 0, 0.0f);
        gameObject.SetActive(false);
        GetComponent<Button>().onClick.AddListener(OnHit);
    }
    public void Show(NpcData data,bool locked=false)
    {
        if (data == null) {
            //Debug.Log("空的");
            allowClick = !locked;
            level.gameObject.SetActive(false);
            pic.texture = null;//EventCenter.WorldCenter.GetParm<string, Texture>(nameof(EventNames.StrtoTexture), "UI_Icon_LockLocked");
            //if (view != null) view.gameObject.SetActive(false);
            npcName.gameObject.SetActive(false);
            if (hp != null) hp.gameObject.SetActive(false);
            //GetComponent<Image>().color = new Color(0, 0, 0, 0.0f);
            gameObject.SetActive(false);
            return;
        }
        //Debug.Log("非空");
        gameObject.SetActive(true);
        level.text=data.level.ToString();
        npcName.text=data.npcName;
        allowClick = !locked;

            pic.texture = EventCenter.WorldCenter.GetParm<NpcProfession, Texture>("GetNPCHeadImg", data.profession);
        if(locked)
        {
            level.gameObject.SetActive(false);
            pic.texture = EventCenter.WorldCenter.GetParm<string, Texture>(nameof(EventNames.StrtoTexture), "UI_Icon_LockLocked");
            //if (view != null) view.gameObject.SetActive(false);
            npcName.gameObject.SetActive(false);
            if (hp != null) hp.gameObject.SetActive(false);
        }
        else
        {
            level.gameObject.SetActive(true);
            pic.gameObject.SetActive(true);
            //if (view != null) view.gameObject.SetActive(true);
            npcName.gameObject.SetActive(true);
            if (hp != null) hp.gameObject.SetActive(true);
            //GetComponent<Image>().color = new Color(1, 1, 1, 1.0f);
        }
    }
    [Obsolete]
    public void SetSelected(bool select)
    {
        //将背景设为淡蓝
        if (select)
            GetComponent<Image>().color = GameColor.lightBlue;
        else GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
    }
   
    public void OnHit()//点击
    {
        Debug.Log("点击");
        //UIview.OnButtonHit(65536+index);
        if(allowClick)
            ((IItemScrollView)father).SlotOnkey(index, 0, 0);
    }
    public void OnViewHit()
    {
        //显示详细chardata信息到鼠标当前位置
        if (!allowClick) { return; }
            //UIview.OnButtonHit(index);
        if (father!=null)
            ((IItemScrollView)father).SlotOnkey(index, 0, 1);
        
    }
    public override void SetIndex(int h, int w, int page)
    {
        base.SetIndex(h, w, page);
        index = h;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        UICenter.UIWorldCenter.SendEvent<RectTransform, int, string>("OnNPCMouseEnter", rect, index, null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        UICenter.UIWorldCenter.SendEvent<RectTransform, int, string>("OnNPCMouseExit", rect, index, null);
    }
}