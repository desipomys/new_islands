using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ItemSlot : Base_Shower, IPointerClickHandler
{
   
    public int x, y,page;
    public int index = -1;
    //public Base_UIComponent father;
    public Image myimag;
    public override void ShowerInit (Base_UIComponent father)
    {
        
        this.father = father;
        myimag = GetComponent<Image>();

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


    bool isover = false;
    Color over = new Color(0, 0, 0, 0.5f);
    Color noover = new Color(1, 1, 1, 0.5f);
    Color canplace = new Color(0, 1, 0, 0.5f);
    Color notplace = new Color(1, 0, 0, 0.5f);

    UnityEvent leftClick = new UnityEvent();
    UnityEvent middleClick = new UnityEvent();
    UnityEvent rightClick = new UnityEvent();
    private void ButtonLeftClick()
    {if(Input.GetKey(KeyCode.LeftShift))
        {
            father.OnClick(10, x, y, page);
        }
    else
        father.OnClick(0,x,y,page);
    }

    private void ButtonMiddleClick()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            father.OnClick(11, x, y, page);
        }
        else
            father.OnClick(1, x, y, page);
    }

    private void ButtonRightClick()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            father.OnClick(12, x, y, page);
        }
        else
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