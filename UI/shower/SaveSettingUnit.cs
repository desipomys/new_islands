using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class SaveSettingUnit:Base_Shower
{
    public Text label,valuetext;
    public Slider value;
    public void Show(FieldInfo f,Action<object> callback)
    {

    }
    public object GetValue()
    {
        return value.value;
    }
}