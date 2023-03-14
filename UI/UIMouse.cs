using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMouse : Base_UIComponent
{
    Canvas canvas;
    RectTransform rectTransform;
    Vector2 pos;
    Camera _camera;
    bool state = false;
    RectTransform canvasRectTransform;

    ItemShower shower;

    void Awake()
    {
        //Debug.Log("鼠标awake");


        //Debug.Log(canvas.renderMode);
    }
    public override void UIInit(UICenter center, BaseUIView view)
    {
        rectTransform = transform as RectTransform;
        canvas = transform.parent.GetComponent<Canvas>();//staticView的canvas
        _camera = canvas.GetComponent<Camera>();
        canvasRectTransform = canvas.transform as RectTransform;
        //不在这里监听鼠标持有物变化事件，要在进入存档后才监听
        //EventCenter.WorldCenter.ListenEvent("LoadSaveDone", LoadSaveDone);
        EventCenter.WorldCenter.ListenEvent<Item>("MouseItemChg", OnItemChange);
        //EventCenter.WorldCenter.ListenEvent("QuitStartScene", OnQuitStartScene);
        //EventCenter.WorldCenter.ListenEvent("ArriveInGameScene", OnArriveInGameScene);
        //EventCenter.WorldCenter.ListenEvent("QuitInGameScene", OnQuitInGameScene);
        //EventCenter.WorldCenter.ListenEvent("ArriveStartScene", OnArriveStartScene);


        shower = GetComponentInChildren<ItemShower>();
        shower.ShowerInit(this);
        Item i = EventCenter.WorldCenter.GetParm<Item>("GetMouseItem");
        Debug.Log("鼠标在uiinit刷新了" + (i == null ? 0 : i.id));
        OnItemChange(i);
    }
    void LoadSaveDone()
    {
        //EventCenter.WorldCenter.ListenEvent<Item>("MouseItemChg", OnItemChange);
    }
    public void OnQuitStartScene()
    {
        //EventCenter.WorldCenter.UnListenEvent("LoadSaveDone", LoadSaveDone);
        //EventCenter.WorldCenter.UnListenEvent("QuitStartScene", OnQuitStartScene);
        //EventCenter.WorldCenter.UnListenEvent("ArriveInGameScene", OnArriveInGameScene);
        //EventCenter.WorldCenter.UnListenEvent("QuitInGameScene", OnQuitInGameScene);
        //EventCenter.WorldCenter.UnListenEvent("ArriveStartScene", OnArriveStartScene);
        EventCenter.WorldCenter.UnListenEvent<Item>("MouseItemChg", OnItemChange);
    }
    public void OnQuitInGameScene()
    {
        //EventCenter.WorldCenter.UnListenEvent("ArriveInGameScene", OnArriveInGameScene);
        //EventCenter.WorldCenter.UnListenEvent("QuitInGameScene", OnQuitInGameScene);
        //EventCenter.WorldCenter.UnListenEvent("ArriveStartScene", OnArriveStartScene);
        EventCenter.WorldCenter.UnListenEvent<Item>("MouseItemChg", OnItemChange);
    }
    public void OnArriveInGameScene()
    {

    }
    public void OnArriveStartScene()
    {
        Debug.Log("start");
    }

    // Update is called once per frame
    void Update()
    {
        //if (canvasRectTransform == null || _camera == null)
        //  return;
        try
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {

                    EventCenter playerevc = EventCenter.WorldCenter.GetParm<EventCenter>(nameof(EventNames.GetLocalPlayer));
                    Item mouseitem = EventCenter.WorldCenter.GetParm<Item>(nameof(EventNames.GetMouseItem));
                    if (!Item.IsNullOrEmpty(mouseitem))
                    {
                        bool b;
                        Debug.Log("手不空，丢出物体");
                        playerevc.SendEvent<Item>(nameof(PlayerEventName.dropItem), mouseitem, out b);//以玩家为中心丢出物体
                        Debug.Log("aaaaaa" + b);
                        EventCenter.WorldCenter.SendEvent<Item>(nameof(EventNames.SetMouseItem), Item.Empty, out b);
                        Debug.Log("减少手持物" + b);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Item mouseitem = EventCenter.WorldCenter.GetParm<Item>(nameof(EventNames.GetMouseItem));
                if (!Item.IsNullOrEmpty(mouseitem))
                {
                    mouseitem.rota = !mouseitem.rota;
                    bool b;
                    EventCenter.WorldCenter.SendEvent<Item>(nameof(EventNames.SetMouseItem), mouseitem, out b);
                }
            }
            rectTransform.position = Input.mousePosition;
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, Camera.main, out pos);

            // canvas.GetComponent<Camera>() 1.ScreenSpace -Overlay 
            /*if (RenderMode.ScreenSpaceCamera == canvas.renderMode)
            {
                       }
            else if (RenderMode.ScreenSpaceOverlay == canvas.renderMode)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, _camera, out pos);
            }
            else
            {
                Debug.Log("请选择正确的相机模式!");
            }*/
            //rectTransform.anchoredPosition = pos;
        }
        catch (System.Exception)
        {

            return;
        }

    }

    public void OnItemChange(Item parm)
    {
        if (parm == null) parm = Item.Empty;
        Debug.Log("手显示" + parm.id);
        shower.Show(parm);
    }
}
