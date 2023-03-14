using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapSelectView_DragMap : MonoBehaviour, IUIInitReciver,IDragHandler,IBeginDragHandler,IPointerEnterHandler,IPointerExitHandler
{
    //landButtonmask是点击按钮后出现的框,mapmask是隐藏尚未解锁的节点的mask
    public RectTransform mapPic,islandGroup,landButtonMask,mapMask,shippos;
    RectTransform myrect;
    public float scaleSpeed=1;
    float sizeFactor = 1;
    Vector3 pos,nowPicPosition;
    Vector3 originPicSize;
    Vector3 picBound,mapPosInSceneLeft, mapPosInSceneRight, mapPosInSceneUp, mapPosInSceneDown;

    bool isEnter=false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEnter = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(mapPic, eventData.position, Camera.main, out pos);
        pos.z = 0;
        nowPicPosition = mapPic.position-pos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(mapPic, eventData.position, Camera.main, out pos);
        pos.z = 0;
        Vector3 temp = new Vector3();
        temp = pos+nowPicPosition;
        
        temp.x = Mathf.Clamp(temp.x, -picBound.x-Mathf.Abs( mapPosInSceneRight.x), picBound.x+ Mathf.Abs(mapPosInSceneLeft.x)) ;
        temp.y = Mathf.Clamp(temp.y, -picBound.y - Mathf.Abs(mapPosInSceneRight.y), picBound.y + Mathf.Abs(mapPosInSceneLeft.y)) ;
        //Debug.Log(temp + ">" + picBound);

        mapPic.position = temp;//需限制移动，使图片不能移出地图框的范围
        islandGroup.position = temp;
        mapMask.position = temp;
        shippos.position = temp;
    }
    public void OnUIOpen(UICenter center, BaseUIView view,MapNodeShower ms)
    {
        landButtonMask.position = ms.GetComponent<RectTransform>().position;
    }

    public void UIInit(UICenter center, BaseUIView view)
    {
        myrect = GetComponent<RectTransform>();
        RectTransform rt = view.GetComponent<RectTransform>();
        originPicSize = mapPic.sizeDelta;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, new Vector3(Mathf.Abs(originPicSize.x) / 2,Mathf.Abs(originPicSize.y ) / 2 ,0), Camera.main, out picBound);

        RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, new Vector3(Screen.width,Screen.height,0)/2 , Camera.main, out mapPosInSceneRight);
        Vector3 v3;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, new Vector3(myrect.sizeDelta.x, myrect.sizeDelta.y / 2, 0), Camera.main, out v3);
        mapPosInSceneRight -= v3;
        mapPosInSceneRight -= myrect.position;
        //screen-(myrect.x,myrect.y/2+myrecrt.pos)
        Vector3 v31;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, new Vector3(-Screen.width, -Screen.height, 0) / 2, Camera.main, out mapPosInSceneLeft);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, new Vector3(0, -myrect.sizeDelta.y / 2, 0), Camera.main, out v31);
        mapPosInSceneLeft -= v31;
        mapPosInSceneLeft -= myrect.position;

        Debug.Log("ans="+ mapPosInSceneLeft+":"+mapPosInSceneRight);
    }
    

    public void OnMapNodeClick(RectTransform rt)
    {
        landButtonMask.position = rt.position;
    }
    // Update is called once per frame
    void Update()
    {
       if(!isEnter) return;
        float x = Input.GetAxis("Mouse ScrollWheel");
        if (x != 0)
        {
            //需以指向位置为中心进行缩放
            Vector3 temp;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(mapPic, Input.mousePosition, Camera.main, out temp);
            temp.z = 0;
            if (x > 0)
            {
                sizeFactor = Mathf.Clamp(sizeFactor + Time.deltaTime * scaleSpeed, Mathf.Max(myrect.sizeDelta.x, myrect.sizeDelta.y) / Mathf.Max(originPicSize.x, originPicSize.y), 1f);
            }
            else
            {
                sizeFactor = Mathf.Clamp(sizeFactor - Time.deltaTime * scaleSpeed, Mathf.Max(myrect.sizeDelta.x, myrect.sizeDelta.y) / Mathf.Max(originPicSize.x, originPicSize.y), 1f);
            }
            //Debug.Log(sizeFactor+":"+temp);
            //mapPic.anchoredPosition
            mapPic.localScale = Vector3.one * sizeFactor;
            islandGroup.localScale = Vector3.one * sizeFactor;
            mapMask.localScale = Vector3.one * sizeFactor;
            shippos.localScale = Vector3.one * sizeFactor;
        }
    }

    
}
