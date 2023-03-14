using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI�������ʾ��Ԫ������
/// </summary>
public class CompShowerManager 
{
    public RectTransform father;//shower���ڵ㣬�豣ֻ֤����װshower
    Base_Shower template;//�����޸�
    Base_UIComponent host;//���������

    Stack<Base_Shower> cachedShower=new Stack<Base_Shower>();
    Dictionary<long, Base_Shower> indexs=new Dictionary<long, Base_Shower>();

    int width, height;//��͸�
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
    {//x�ǿ�y�Ǹ�
        if (width == wid && height == hig) return;

        cacheAll();
        if(wid*hig>cachedShower.Count)
        {
            int need = wid*hig - cachedShower.Count;
            for (int i = 0; i < need; i++)
            {
                //����shower�ӵ�cachedShower
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
    public void SetNum(int num)//����עshower����󴥷����¼�index
    {
        //if (num == indexs.Count) return;

        cacheAll();
        if (num > cachedShower.Count)
        {
            int need = num - cachedShower.Count;
            for (int i = 0; i < need; i++)
            {
                //����shower�ӵ�cachedShower
                GameObject gt = GameObject.Instantiate(template.gameObject, father.position, father.rotation, father);
                Base_Shower temp = gt.GetComponent<Base_Shower>();
                temp.ShowerInit(host);
                temp.gameObject.SetActive(false);
                cachedShower.Push(temp);
            }
                
        }
        //��cacheȡ��indexs,������slibin
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
    /// ��w�е�h��
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
            Debug.Log(index.GetX()+","+index.GetY() + "�Ҳ���");
            throw;
        }
        
    }
    public Base_Shower GetOrNew(int h, int w)//width,height
    {
        return GetOrNew(XYHelper.ToLongXY(h, w));
    }
    public Base_Shower GetOrNew(long index)//����Ҫ�滻����manager��ֻӦ��ע��int-shower��ӳ�䣬��Ҫ����x,y-showerӳ��
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
    /// һά����ģʽ��
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
