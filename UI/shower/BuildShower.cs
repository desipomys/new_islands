using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class BuildShower : Base_Shower, IPointerClickHandler
{
    public RawImage img, star;
    public Text descript;
    //IItemScrollView father;
    int index=0,y=0;
    BuildAble_Data data;
    bool avalible;
    public override void ShowerInit(Base_UIComponent f)
    {
        base.ShowerInit(f);
        this.father = f;
        leftClick.RemoveAllListeners();
        leftClick.AddListener(new UnityAction(ButtonLeftClick));
        middleClick.RemoveAllListeners();
        middleClick.AddListener(new UnityAction(ButtonMiddleClick));
        rightClick.RemoveAllListeners();
        rightClick.AddListener(new UnityAction(ButtonRightClick));
    }
   
    public void Show(BuildAble_Data bud,int x)
    {
        if (bud.isBBlock) Show(bud.bblockTyp, x,true);
        else
        {
            Texture t = EventCenter.WorldCenter.GetParm<string, Texture>(nameof(EventNames.StrtoTexture), bud.img);
            img.texture = t;
            if (!avalible) img.color = Color.black;
            else img.color = Color.white;
            this.avalible = true;
            index = x;
            gameObject.SetActive(true);
        }
        data = bud;
    }
    public void Show(BuildAble_Data bud, int x,bool avalible)
    {
        if (bud.isBBlock) Show(bud.bblockTyp, x, avalible);
        else
        {
            Texture t = EventCenter.WorldCenter.GetParm<string, Texture>(nameof(EventNames.StrtoTexture), bud.img);
            img.texture = t;
            if (!avalible) img.color = Color.black;
            else img.color = Color.white;
            this.avalible = avalible;
            index = x;
            gameObject.SetActive(true);
        }
        data = bud;
    }
    public void Show(Entity_BlockModel eb,int x,bool avalible)//可显示bblock和eblock
    {
        Texture t = EventCenter.WorldCenter.GetParm<string, Texture>(nameof(EventNames.StrtoTexture), eb.strTyp);
        img.texture = t;
        if (!avalible) img.color = Color.black;
        else img.color = Color.white;
        this.avalible = avalible;
        index = x;
        gameObject.SetActive(true);
    }
    public void Show(B_Block bb, int x, bool avalible)
    {
        Texture t = EventCenter.WorldCenter.GetParm<int, Texture>(nameof(EventNames.BMattoTexture), bb.mat2==0?(int)bb.mat:bb.mat2);
        //Mesh m=EventCenter.WorldCenter.GetParm<int, Texture>(nameof(EventNames.BmeshtoMesh), bb.mesh);
        //需根据ID获取模型+材质生成缩略图
        img.texture = t;
        if (!avalible) img.color = Color.black;
        else img.color = Color.white;
        index = bb.mesh;
        y = (int)bb.mat;
        this.avalible = avalible;
        gameObject.SetActive(true);
    }

    UnityEvent leftClick = new UnityEvent();
    UnityEvent middleClick = new UnityEvent();
    UnityEvent rightClick = new UnityEvent();
    private void ButtonLeftClick()
    {
        if(avalible)
        father.OnEvent(data,0);
        else { father.OnEvent(data, -1); }
    }

    private void ButtonMiddleClick()
    {
        if (avalible)
            father.OnEvent(data, 1);
        else { father.OnEvent(data, -1); }
        //father.SlotOnkey(index, y, 1);
    }

    private void ButtonRightClick()
    {
        if (avalible)
            father.OnEvent(data, 2);
        else { father.OnEvent(data, -1); }
        //father.SlotOnkey(index, y, 2);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            leftClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Middle)
            middleClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            rightClick.Invoke();
    }
}
