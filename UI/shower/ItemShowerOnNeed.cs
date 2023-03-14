using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShowerOnNeed : ItemShower
{
    public Text needNum;
    public void ShowWithFixSize(Item need, Item now)
    {
        fixedSize = true;
        
        Show(need, now);
    }
    public void ClearNum(int need)
    {

        needNum.text = "<color=red>" + "0" + "</color>" + "/" + need.ToString();
    }
    public void SetNumWithFixSize(Item item,int neednum)
    {
        
            fixedSize = true;
            if (item.num >= neednum)
            {
                needNum.text = item.num.ToString() + "/" + neednum.ToString();
            }
            else
            {
                needNum.text = "<color=red>" + item.num.ToString() + "</color>" + "/" + neednum.ToString();
            }
     
    }
    public void Show(Item need,Item now)
    {
        if(need.Compare(now,ItemCompareMode.excludeNum)||Item.IsNullOrEmpty(now))
        {
            Debug.Log(need);
            Show(need, false);
            if (!Item.IsNullOrEmpty(now)&&now.num >= need.num)
            {
                int nownum = 0;
                if (!Item.IsNullOrEmpty(now)) nownum = now.num;
                needNum.text = nownum.ToString()+"/" + need.num.ToString();
            }
            else
            {
                int nownum = 0;
                if (!Item.IsNullOrEmpty(now)) nownum = now.num;
                needNum.text="<color=red>"+ nownum.ToString()+"</color>" + "/" + need.num.ToString();
            }
        }
    }
}
