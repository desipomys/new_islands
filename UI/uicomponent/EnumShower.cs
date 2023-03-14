using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class EnumShower : MonoBehaviour
{
    public string enumName;
    List<string> enumValueName=new List<string>();
    

    public void UIInit()
    {
        getEnumValues();
        GetComponent<Dropdown>().onValueChanged.AddListener(OnValueChange);
    }

    void getEnumValues()
    {
        var fields = Type.GetType(enumName).GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach (var fi in fields)
            enumValueName.Add(fi.Name);
    }

    public void OnValueChange(int p)
    {

    }
    public void Show(object o)
    {

    }
}