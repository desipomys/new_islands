using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 简化当按钮按下时出现框选，点其他按钮框选跳到被点按钮上
/// </summary>
public class UIButtonBounder : MonoBehaviour
{
    public Texture BoundTexture;
    Texture oldText;
    public Image img;

   public void SetBound()
    {
        //img.overrideSprite = BoundTexture as Sprite;
    }
    public void ClearBound()
    {

    }
}
