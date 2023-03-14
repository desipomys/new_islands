using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public class Camera_PlayerTracker : MonoBehaviour, IEventRegister
{
    Transform target;
    public Vector3 viewOffset;//视角默认从-z到z
    public LayerMask mask;
    public float rotaSpeed=2,minFOV=90,maxFOV=60,scrollSpeed=1;
    float dirAngle,realDirAngle;

    Vector3 tempViewOffset,realViewOffset=Vector3.zero;
    EventCenter center;
    Camera cam;

    public void OnEventRegist(EventCenter e)//接收摄像机事件中心初始化
    {
        center = e;
        cam = GetComponent<Camera>();
        tempViewOffset = viewOffset;
        e.ListenEvent<Transform>(nameof(EventNames.Track), Track);
        EventCenter.WorldCenter.ListenEvent<int>(nameof(EventNames.ChangeViewDir), changeViewDir);
        EventCenter.WorldCenter.RegistFunc<Vector3>(nameof(EventNames.GetMouseLookAt), GetMouseLookAt);
        EventCenter.WorldCenter.RegistFunc<GameObject>(nameof(EventNames.GetMouseLookObj), GetMouseLookObj);
    }
    public void AfterEventRegist()
    {

    }
    Vector3 GetMouseLookAt()
    {
        if(hitCasted)
        {
            return hit.point;
        }
        if (Physics.Raycast(r, out hit, 1000, mask))
        {
            hitCasted = true;
            return hit.point;
        }
        return Vector3.zero;
    }
    GameObject GetMouseLookObj()
    {
        if(hitCasted)
        {
            if (hit.rigidbody == null) return hit.transform.gameObject;
            return hit.rigidbody.gameObject;
        }
        if (Physics.Raycast(r, out hit, 1000, mask))
        {
            hitCasted = true;
            if (hit.rigidbody == null) return hit.transform.gameObject;
            return hit.rigidbody.gameObject;
        }
        return null;
    }

    void Update()
    {
        RayUpdate();
        adjustView();
        adjustFocus();
    }
    void LateUpdate()
    {
        hitCasted = false;
    }
    Ray r;
    RaycastHit hit;
    bool hitCasted;//当前帧的hit是否有效
    void RayUpdate()
    {
        r = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.Log("rayupdate"+r.direction);
    }
    /// <summary>
    /// 获取玩家当前看的方向（与玩家Y值相同）
    /// </summary>
    /// <returns></returns>
    Vector3 getViewPosi()
    {
        Vector3 temp;
        Vector3 dir = r.direction;
        if (r.direction.y == 0) return Vector3.zero;
        
        float da = (target.position.y - transform.position.y) / dir.y;

        temp.x = da * dir.x + transform.position.x;
        temp.z = da * dir.z + transform.position.z;
        temp.y = target.position.y;
        return temp;
    }
    Vector3 adjustDirOffset()
    {
        /*dirangle角度对应轴关系
            Z
            90
            |
            |
        ---------0 X
            |
            |

        */
        
        realDirAngle=Mathf.Lerp( realDirAngle,dirAngle,Time.deltaTime*rotaSpeed);
        realViewOffset.x=viewOffset.z*Mathf.Sin(Mathf.Deg2Rad* realDirAngle);
        realViewOffset.z=viewOffset.z*Mathf.Cos(Mathf.Deg2Rad*realDirAngle);
        realViewOffset.y = viewOffset.y;
        return realViewOffset;
    }
    void adjustView()
    {
        if (target != null)
        {
            Vector3 temp=adjustDirOffset();
            Vector3 oldPos = transform.position;
            Vector3 targetPos = target.position + temp;//Vector3.Lerp(transform.position,target.position+adjustDirOffset(),Time.deltaTime);
                                                                     //transform.LookAt(target);
            transform.position = Vector3.Lerp(oldPos, targetPos + shakeOffsetByDir()//震屏特效导致的移动
            + mouseOffsetByDir(),0.5f);//鼠标移动导致的摄像机移动

            transform.LookAt(transform.position-(temp+shakeRotaOffset()));
          // transform.Rotate();
        }
    }

    public void Track(Transform tar)
    {
        target = tar;
        //Debug.Log(transform.position+"跟踪前" + tar.position);
        transform.position = target.position + adjustDirOffset();
        transform.LookAt(target);
        //Debug.Log(transform.position+"跟踪" + tar.position);
        //RayUpdate();
        EventCenter.WorldCenter.RegistFunc<Vector3>(nameof(EventNames.GetViewPosi), getViewPosi);
    }
    void adjustFocus()
    {
        return;
        float a = Input.GetAxis("Mouse ScrollWheel");
        Debug.Log(a);
        if(a<0)
        {
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView + Time.deltaTime * scrollSpeed, minFOV, maxFOV);
        }
        else if(a>0)
        {
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - Time.deltaTime * scrollSpeed, minFOV, maxFOV);
        }
    }

    /// <summary>
    /// 改变视角到某个方向，不支持up,down
    /// </summary>
    /// <param name="dir"></param>
    public void changeViewDir(DIR dir)
    {
        if ((dir & DIR.up) != DIR.none || (dir & DIR.down) != DIR.none) return;

        dirAngle = ConvertCenter.DirToAngle(dir);
        Quaternion q = Quaternion.AngleAxis(dirAngle, Vector3.up);// 旋转系数
        tempViewOffset = q * viewOffset;

        //transform.position = target.position + adjustDirOffset() + shakeOffsetByDir()+ mouseOffsetByDir();
        //transform.LookAt(transform.position - tempViewOffset);
    }
    /// <summary>
    /// 当前视角移动N度，只支持90整数倍度
    /// </summary>
    /// <param name="angle"></param>
    public void changeViewDir(int angle)
    {
        if (angle % 90 != 0) return;

        dirAngle = Mathf.Repeat(dirAngle + angle, 360);
        if (dirAngle == 0) realDirAngle -= 360;
        Quaternion q = Quaternion.AngleAxis(dirAngle, Vector3.up);// 旋转系数
        tempViewOffset = q * viewOffset;


        //transform.position = target.position + adjustDirOffset() + shakeOffsetByDir() + mouseOffsetByDir();
        //transform.LookAt(transform.position - tempViewOffset);
    }

    
    Vector3 mouseOffsetByDir()//经过视角度处理的鼠标偏移
    {
        Vector3 temp=center.GetParm<Vector3>("mouseOffset");
       

        Quaternion q = Quaternion.AngleAxis(dirAngle, Vector3.up);// 旋转系数
        temp = q * temp;
        return temp;
    }
    Vector3 shakeOffsetByDir()
    {
        Vector3 temp = center.GetParm<Vector3>("shakeOffset");
        if (temp.y != 0) return temp;//y方向不应用偏移
        Quaternion q = Quaternion.AngleAxis(dirAngle, Vector3.up);// 旋转系数
        temp = q * temp;
        return temp;
    }
    Vector3 shakeRotaOffset()
    {
        Vector3 temp = center.GetParm<Vector3>("ShakeRotaOffset");
        return temp;
    }
}