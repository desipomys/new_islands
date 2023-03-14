using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI组件的显示单元管理器
/// </summary>
public class CompShowerManager 
{
    public RectTransform father;//shower父节点，需保证只用来装shower
    Base_Shower template;//不可修改
    Base_UIComponent host;//所属的组件

    Stack<Base_Shower> cachedShower=new Stack<Base_Shower>();
    Dictionary<long, Base_Shower> indexs=new Dictionary<long, Base_Shower>();

    int width, height;//宽和高
    #region private
    void cacheAll()
    {
        List<long> temp = new List<long>(indexs.Keys);
        for (int i = 0; i < temp.Count; i++)
        {
            Base_Shower bs = indexs[temp[i]];
            bs.gameObject.SetActive(false);
            indexs.Remove(temp[i]);
            cachedShower.Push(bs);
        }
    }

    #endregion

    public void Init(Base_Shower b,RectTransform father,Base_UIComponent buc) { 
        template = b; this.father = father; host = buc;
        //Debug.Log("comp shower init"+template.gameObject.name+father.gameObject.name);
    }

    public void SetNum(int wid,int hig)
    {//x是宽，y是高
        if (width == wid && height == hig) return;

        cacheAll();
        if(wid*hig>cachedShower.Count)
        {
            int need = wid*hig - cachedShower.Count;
            for (int i = 0; i < need; i++)
            {
                //生成shower加到cachedShower
                Base_Shower temp = GameObject.Instantiate(template.gameObject, father.position, father.rotation, father).GetComponent<Base_Shower>();
                temp.ShowerInit(host);
                temp.gameObject.SetActive(false);
                cachedShower.Push(temp);
            }
        }

        for (int i = 0; i < hig; i++)//height
        {
            for (int j = 0; j < wid; j++)//width
            {
                Base_Shower temp = cachedShower.Pop();
                temp.gameObject.SetActive(true);
                indexs.Add(XYHelper.ToLongXY(i,j), temp);
                temp.SetIndex(i, j, 0);
                temp.transform.SetSiblingIndex(i * wid + j);
            }
        }
        width = wid;
        height = hig;
    }
    public void SetNum(int num)//不关注shower点击后触发的事件index
    {
        //if (num == indexs.Count) return;

        cacheAll();
        if (num > cachedShower.Count)
        {
            int need = num - cachedShower.Count;
            for (int i = 0; i < need; i++)
            {
                //生成shower加到cachedShower
                GameObject gt = GameObject.Instantiate(template.gameObject, father.position, father.rotation, father);
                Base_Shower temp = gt.GetComponent<Base_Shower>();
                temp.ShowerInit(host);
                temp.gameObject.SetActive(false);
                cachedShower.Push(temp);
            }
                
        }
        //从cache取到indexs,重排列slibin
        for (int i = 0; i < num; i++)
        {
            Base_Shower temp = cachedShower.Pop();
            //temp.gameObject.SetActive(true);
            indexs.Add(i, temp);
            temp.SetIndex(i, 0, 0);
            temp.transform.SetSiblingIndex(i);
        }
        width = 1;
        height = num;
    }
    
    public void Recycle(Base_Shower bs, int h,int w)
    {

    }
    public void Recycle(Base_Shower bs, long index)
    {
        if(indexs.ContainsKey(index))
        {
            Base_Shower bsh = indexs[index];
            bsh.gameObject.SetActive(false);
            indexs.Remove(index);
            cachedShower.Push(bsh);
        }
    }
    public void RecycleAll()
    {
        cacheAll();
    }
    
    /// <summary>
    /// 第w列第h行
    /// </summary>
    /// <param name="h"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public Base_Shower Get(int h,int w)//width,height
    {
        return Get(XYHelper.ToLongXY(h, w));
    }
    public Base_Shower Get(long index)
    {
        try
        {
            return indexs[index];
        }
        catch (System.Exception)
        {
            Debug.Log(index);
            foreach (var item in indexs)
            {
                Debug.Log(item.Key);
            }
            Debug.Log(index.GetX()+","+index.GetY() + "找不到");
            throw;
        }
        
    }
    public Base_Shower GetOrNew(int h, int w)//width,height
    {
        return GetOrNew(XYHelper.ToLongXY(h, w));
    }
    public Base_Shower GetOrNew(long index)//可能要替换掉，manager里只应该注意int-shower的映射，不要管理x,y-shower映射
    {
        if(indexs.ContainsKey(index))
        return indexs[index];
        else
        {
            if (cachedShower.Count <= 0)
            {
                Base_Shower temp = GameObject.Instantiate(template.gameObject, father.position, father.rotation, father).GetComponent<Base_Shower>();
                temp.ShowerInit(host);

                indexs.Add(index, temp);
                temp.transform.SetSiblingIndex(indexs.Count);

                return temp;
            }
            else
            {
                Base_Shower temp = cachedShower.Pop();
                temp.ShowerInit(host);
                temp.gameObject.SetActive(true);
                indexs.Add(index, temp);
                temp.transform.SetSiblingIndex(indexs.Count);

                return temp;
            }
        }
    }
    /// <summary>
    /// 一维数组模式的
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Base_Shower Create(out int index)
    {
        Base_Shower temp;;
        if(cachedShower.Count<=0)temp= GameObject.Instantiate(template.gameObject, father.position, father.rotation, father).GetComponent<Base_Shower>();
        else
        {
            temp = cachedShower.Pop();
            temp.gameObject.SetActive(true);
        }
        temp.ShowerInit(host);

        index = 0;
        foreach (var item in indexs)
        {
            if (index < item.Key) { index = (int)item.Key; }
        }
        index += 1;
        indexs.Add(index, temp);
        temp.transform.SetSiblingIndex(indexs.Count);

        return temp;
    }
    public RectTransform GetRect(int h,int w)
    {
        //Debug.Log("getrect" + h + ":" + w);
        return GetRect(XYHelper.ToLongXY(h, w));
    }
    public RectTransform GetRect(long index)
    {
       return Get(index).GetComponent<RectTransform>();
    }
    public void SetPos(RectTransform rt,long index)
    {
        Get(index).GetComponent<RectTransform>().position=rt.position;

    }
    public void SetPos(RectTransform rt,int h,int w)
    {
        SetPos(rt,XYHelper.ToLongXY(h, w));
    }
}
