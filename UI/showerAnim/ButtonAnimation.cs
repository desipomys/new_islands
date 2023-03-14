using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ButtonAnimationType
{
    leftRightMove,scale
}

public class ButtonAnimation : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public Vector3 offset;
    public ButtonAnimationType animType;
    public RectTransform button;
    Vector3 origin;
    Ticker t = new Ticker(0.125f);
    private void Start()
    {
        if(button==null)
            button = GetComponent<RectTransform>();
        switch (animType)
        {
            case ButtonAnimationType.leftRightMove:
                origin = button.localPosition;
                offset = origin + offset;
                break;
            case ButtonAnimationType.scale:
                origin = button.localScale;
                Debug.Log(origin);
                offset.x = origin.x * offset.x;
                offset.y = origin.y * offset.y;
                offset.z = origin.z * offset.z;
                Debug.Log(offset);
                break;
            default:
                break;
        }
        
    }
    
    public void OnMouseEnter()
    {
        Debug.Log("anim");
        switch (animType)
        {
            case ButtonAnimationType.leftRightMove:
                button.DOLocalMove(offset, 0.5f);
                break;
            case ButtonAnimationType.scale:
                button.DOScale(offset, 0.5f);
                break;
            default:
                break;
        }
        
    }
    public void OnMouseExit()
    {
        Debug.Log("animDone");
        switch (animType)
        {
            case ButtonAnimationType.leftRightMove:
                button.DOLocalMove(origin, 0.5f);
                break;
            case ButtonAnimationType.scale:
                button.DOScale(origin, 0.5f);
                break;
            default:
                break;
        }
        //button.DOMove(origin, 0.5f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(t.IsReady())
        OnMouseEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (t.IsReady())
            OnMouseExit();
    }
}
