using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGroupSelectButt : MonoBehaviour
{
    public int id;
    public Text text;
    BaseUIView view;
    string typ;
    public void Init(string typ,int id, BaseUIView view,string groupname)
    {
        this.id = id;
        this.view = view;
        text.text = groupname;
        this.typ = typ;
    }
    public void Init(string typ, BaseUIView view)
    {
        this.view = view;
        this.typ = typ;
    }
    public void onhit()
    {
        view.OnButtonHit(typ,id);
    }
}
