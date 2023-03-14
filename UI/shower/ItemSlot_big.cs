using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// 能检测左中右键三种按键事件
/// </summary>
public class ItemSlot_big:Base_Shower,IPointerClickHandler
{
    UnityEvent leftClick=new UnityEvent();
    UnityEvent middleClick = new UnityEvent();
    UnityEvent rightClick = new UnityEvent();

   
    public int x, y,page;
    public override void ShowerInit(Base_UIComponent father)
    {
        base.ShowerInit(father);

        leftClick.RemoveAllListeners();
        middleClick.RemoveAllListeners();
        rightClick.RemoveAllListeners();

        leftClick.AddListener(new UnityAction(ButtonLeftClick));
        middleClick.AddListener(new UnityAction(ButtonMiddleClick));
        rightClick.AddListener(new UnityAction(ButtonRightClick));
    }
    public override void SetIndex(int h, int w, int page)
    {
        base.SetIndex(h, w, page);
        this.x = w; this.y = h;
    }

    private void ButtonLeftClick()
    {
        father.OnClick(0, x, y, page);
    }

    private void ButtonMiddleClick()
    {
        father.OnClick(1, x, y, page);
    }

    private void ButtonRightClick()
    {
        father.OnClick(2, x, y, page);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            leftClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Middle)
            middleClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            rightClick.Invoke();
    }
}