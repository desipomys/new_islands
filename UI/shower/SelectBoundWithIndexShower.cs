using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectBoundWithIndexShower : Base_Shower
{
    public Text Index;

    /// <summary>
    /// h在一维时是index
    /// </summary>
    /// <param name="h"></param>
    /// <param name="w"></param>
    /// <param name="page"></param>
    public override void SetIndex(int h,int w,int page)
    {
        Index.text=h.ToString();
    }

}