using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// �򻯵���ť����ʱ���ֿ�ѡ����������ť��ѡ�������㰴ť��
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
