using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 显示单元基类
/// </summary>
public class Base_Shower : MonoBehaviour
{
    public Base_UIComponent father;
    public virtual void ShowerInit(Base_UIComponent f)
    {
        father = f;
    }
    /// <summary>
    /// h在一维时是index
    /// </summary>
    /// <param name="h"></param>
    /// <param name="w"></param>
    /// <param name="page"></param>
    public virtual void SetIndex(int h,int w,int page)
    {

    }
}
