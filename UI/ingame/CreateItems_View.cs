using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateItems_View : BaseUIView
{
    public GameObject groupButtonPrefabs;
    public Transform GroupRoot;
    public Button[] groupButtons;
    public ItemScrollView_big groupScrollView;
    public override void UIInit(UICenter center)
    {
        base.UIInit(center);
        //groupButtonPrefabs = ((RectTransform)transform).Find("ItemGroup").gameObject;
        //GroupRoot= ((RectTransform)transform).Find("group");
        //groupScrollView = ((RectTransform)transform).Find("ItemScrollView_big").GetComponent<ItemScrollView_big>();
        ItemGroup[] allgroup = EventCenter.WorldCenter.GetParm<ItemGroup[]>(nameof(EventNames.GetAllItemGroup));
        groupButtons = new Button[allgroup.Length];
        for (int i = 0; i < allgroup.Length; i++)
        {
            GameObject temp = Instantiate(groupButtonPrefabs,GroupRoot);
            temp.SetActive(true);
            groupButtons[i] = temp.GetComponent<Button>();
           
        }
        for (int i = 0; i < allgroup.Length; i++)
        {
            //Debug.Log(i);
            //groupButtons[i].onClick.AddListener(()=> { OnButtonHit(i); });
            groupButtons[i].gameObject.GetComponent<ItemGroupSelectButt>().Init("T",i, this,allgroup[i].ToString());
        }
        groupScrollView.UIInit(center,this);
        groupScrollView.SetPage(0);
        groupScrollView.SetViewSize(6, 7);
        //groupScrollView.Listen(ItemSlotOnHit);
        groupButtonPrefabs.SetActive(false);
    }

    public override void OnUIOpen(int posi = 0)
    {
        base.OnUIOpen(posi);
        //FindAllItems();
        OnButtonHit(0);
       
    }
    void FindAllItems()
    {
        for (int i = 0; i < 5; i++)
        {
           

        }
    }
    
    Item[] items;//当前group页面的item暂存
    public Item GetItemAt(int x,int y)
    {//未测试
        try
        {
            if (x + y * 6 < items.Length)
                return items[x + y * 6];
            else return null;
        }
        catch (System.Exception)
        {
            //Debug.Log("x=" + x + ";y=" + y);
            //throw;
            return null;
        }
        
    }
    public override void OnButtonHit(int id)//group按钮被按下
    {
        base.OnButtonHit(id);
        items = EventCenter.WorldCenter.GetParm<ItemGroup, Item[]>(nameof(EventNames.GetItemsByGroup), (ItemGroup)id);
        if (items.Length == 0)
        {
            groupScrollView.SetItems(new Item[0,0]);
        }
        else
        {
            Item[,] temp = new Item[(int)Mathf.Ceil(items.Length*1.0f / 6),6];//high,wid,
            Debug.Log("temp size="+temp.GetLength(1));
            int sigh = 0;
            for (int i = 0; i < items.Length; i++)
            {
                temp[i / 6,sigh] = items[i];
                sigh = (sigh + 1) % 6;
            }
            groupScrollView.SetItems(temp);
        }
        
    }
    public override void OnButtonHit(string typ, int id)
    {
        switch (typ)
        {
            case "T":
                OnButtonHit(id);
                break;
            default:
                break;
        }
    }
}
