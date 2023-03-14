using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public class Camera_MouseFollow : MonoBehaviour, IEventRegister
{
    public float viewRange;

    EventCenter center;
    
    
    public void OnEventRegist(EventCenter e)//接收世界事件中心初始化
    {
        center = e;
        e.RegistFunc<Vector3>("mouseOffset", GetMouseOffset);
        //e.ListenEvent("ArriveInGameScene", arraiveTest);
        

    }
    void arraiveTest()
    {
        Debug.Log("camera arrive");
    }
    

    public void AfterEventRegist()
    {

    }
    public Vector3 mouseoffset;
    Vector3 GetMouseOffset()
    {
        /* 0.5 y
         * |-------  0.5,0.5
         * |     
         * |     
         * |-------   0.5  x
         * -0.5
         * 中心是0,0
         */

        Vector3 normalmouseposi;
        normalmouseposi = Input.mousePosition;
        normalmouseposi.x /= Screen.width;
        normalmouseposi.y /= Screen.height;
        normalmouseposi.z = normalmouseposi.y;

        normalmouseposi.y = 0;

        normalmouseposi.x -= 0.5f;
        normalmouseposi.z -= 0.5f;

       

        return normalmouseposi*viewRange;
    }
}