using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest_View : BaseUIView
{
    public Image bg;
    public ItemScrollView bp;
    public override void UIInit(UICenter center)
    {
        base.UIInit(center);
        bg = transform.Find("BG").GetComponent<Image>();
    }
    //buildMVC由chestOBJ上的脚本驱动
    public override void BuildMVConnect(string viewname, EventCenter modelSource, EventCenter modelTarget)//source是chestOBJ，target是打开人
    {
        base.BuildMVConnect(viewname, modelSource, modelTarget);
        bg.rectTransform.sizeDelta = bp.GetComponent<RectTransform>().sizeDelta+ new Vector2(8,8);

        //生成page列表按钮
    }
}
