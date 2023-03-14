using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShower : Base_Shower
{
    public RawImage img,star,bg;
    public Text num,exd,level;
    public Slider subid;
    public bool fixedSize=false;
    protected Item cache;

    RectTransform myrect;
    RectTransform MyRect { get { if (myrect == null)
            { myrect = GetComponent<RectTransform>(); return myrect; }
            else return myrect; } }

    public override void ShowerInit(Base_UIComponent b)//不涉及数据的init
    {
        //获取rawimage等
        //bg = GetComponent<RawImage>();
        if(myrect==null)
        myrect = GetComponent<RectTransform>();
    }
    public void UIInit(bool fix)//不涉及数据的init
    {
        //获取rawimage等
        fixedSize=fix;
        MyRect.pivot = new Vector2(0.5f, 0.5f);
    }
    public void ShowWithFixSize(Item i)
    {
        fixedSize = true;
        Show(i, false);
    }
    public void Show(Item i)
    {
        Show(i, false);
    }
    public void Show(Item i,bool islock)
    {
        if (cache == null) cache = new Item();
        gameObject.SetActive(true);
       
        if(Item.IsNullOrEmpty(i))
        {
            Debug.Log("设空");
            num.text="";
            bg.enabled = false;
            img.gameObject.SetActive(false);
            star.gameObject.SetActive(false);
            subid.gameObject.SetActive(false);
            exd.text = "";
            if(level!=null)level.text = "";
            return;
        }

         if(i.rota)
        {
            img.transform.rotation=Quaternion.Euler(0,0,90);//未测试
            //MyRect.pivot=new Vector2(1,1);
        }
        else{
            img.transform.rotation=Quaternion.Euler(0,0,0);
            if(!fixedSize)
                MyRect.pivot=new Vector2(0,1);
        }

        bg.enabled = true;
        if (islock) { bg.color = GameColor.Transparent; }
        else { bg.color = GameColor.QuadBlack; }
        img.gameObject.SetActive(true);
        //if (cache.num != i.num)
        //{
            if (i.num == 1) num.text = "";
            else num.text = i.num.ToString();
        //}
        //if(cache.id!=i.id)
            img.texture=EventCenter.WorldCenter.GetParm<int,Texture>("ItemtoTexture",i.id);
        if (i.IsExdEmpty()) exd.text = "";
        else exd.text = "*";

        //if (cache.level != i.level)
        //{
            Texture starTexture;
            switch (i.level/1024)
            {
                case 1:
                    starTexture = EventCenter.WorldCenter.GetParm<string, Texture>("StrtoTexture", "star");
                    star.texture = starTexture;
                    star.gameObject.SetActive(true);
                    break;
                case 2:
                    starTexture = EventCenter.WorldCenter.GetParm<string, Texture>("StrtoTexture", "star");
                    star.color = GameColor.gold;
                    star.texture = starTexture;
                    star.gameObject.SetActive(true);
                    break;
                default:
                    star.gameObject.SetActive(false);
                    break;
            }
       if(level!=null) level.text = i.level.ToString();
        //}

        if (i.subid == 0) subid.gameObject.SetActive(false);
        else
        {
            if(!subid.gameObject.activeInHierarchy)subid.gameObject.SetActive(true);
            //if (cache.subid != i.subid)
                subid.value = i.subid * 1.0f / i.GetMaxSub();
        }

        //if (cache.id != i.id) 
        adjustSize(i);
        //cache.CopyFrom(i,ItemCompareMode.excludeNumExd);
    }

    public void SetCenterPosi(RectTransform target)
    {
        try
        {
            if (gameObject.activeInHierarchy)
            {
                //StartCoroutine(synPosiWait(target));
                Vector2 temp = GetComponent<RectTransform>().sizeDelta/2;
                GetComponent<RectTransform>().position = target.position;
                    //+ new Vector3(-temp.x,temp.y,0);
                //transform.position = target.position;
                gameObject.SetActive(false);
                gameObject.SetActive(true);
            }
        }
        catch (System.Exception)
        {

            Debug.Log("itemshower未激活");
        }
    }
    public void SetPosi(RectTransform target)
    {
        //transform.SetParent(target);
        try
        {
            if (gameObject.activeInHierarchy)
            {
                //StartCoroutine(synPosiWait(target));
                GetComponent<RectTransform>().position = target.position;
                //transform.position = target.position;
                if (fixedSize)
                {
                    float xscale = (target.sizeDelta.x / (Loader_Item.ItemUISize * 2));
                    float yscale = (target.sizeDelta.y / (Loader_Item.ItemUISize * 2));
                    transform.localScale = new Vector3(xscale, yscale, 1);
                }

                gameObject.SetActive(false);
                gameObject.SetActive(true);
            }
        }
        catch (System.Exception)
        {

            Debug.Log("itemshower未激活");
        }
        
    }
    IEnumerator synPosiWait(Transform t)
    {
        yield return null;
        transform.position = t.position;
        if (fixedSize)
        {
            float xscale = (((RectTransform)t).sizeDelta.x / (Loader_Item.ItemUISize * 2));
            float yscale = (((RectTransform)t).sizeDelta.y / (Loader_Item.ItemUISize * 2));
            transform.localScale = new Vector3(xscale, yscale, 1);
        }
    }
    public void SetBG(bool v)
    {
        bg.enabled=v;
    }
    public void SetEnable(bool b)
    {
        img.color = b ? Color.black : Color.white;
    }
    public void adjustSize(Item i)
    {
        if(!fixedSize)
        {
            int[] temp = i.GetSize();

            if (i.rota)
            { img.rectTransform.sizeDelta = new Vector2(Loader_Item.ItemUISize * temp[1], Loader_Item.ItemUISize * temp[0]);
            }
            else
            {
                img.rectTransform.sizeDelta = new Vector2(Loader_Item.ItemUISize * temp[0], Loader_Item.ItemUISize * temp[1]);
            }
            subid.GetComponent<RectTransform>().sizeDelta = new Vector2(Loader_Item.ItemUISize * temp[0], 16);
            MyRect.sizeDelta = new Vector2(Loader_Item.ItemUISize * temp[0]+2, Loader_Item.ItemUISize * temp[1]+2);
        }
        else
        {
            MyRect.pivot = new Vector2(0.5f, 0.5f);
            int[] temp = i.GetSize();
            //Debug.Log(i.id + "的大小是"+temp[0]+":"+temp[1]);
            int max = Mathf.Max(temp);
            float x =temp[0]* (2*1f/max);
            float y= temp[1] *(2*1f / max);
            if (!i.rota)
            { img.rectTransform.sizeDelta = new Vector2(Loader_Item.ItemUISize * x, Loader_Item.ItemUISize * y); }
            else
            {
                img.rectTransform.sizeDelta = new Vector2(Loader_Item.ItemUISize * y, Loader_Item.ItemUISize * x);
            }
            subid.GetComponent<RectTransform>().sizeDelta = new Vector2(Loader_Item.ItemUISize * x, 16);
            MyRect.sizeDelta = new Vector2(Loader_Item.ItemUISize * x+2, Loader_Item.ItemUISize * y+2);

        }
    }
}