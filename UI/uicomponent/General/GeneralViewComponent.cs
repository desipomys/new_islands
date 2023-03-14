using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GeneralDataType
{
    Int,Float,Str
}

/// <summary>
/// 挂载于需要显示数据的UI上，要求V没有改变M数据的需求
/// </summary>
public class GeneralViewComponent : Base_UIComponent, IMVConnector
{
    public GeneralDataType dataType;
    public UI_ModelType type;
    public Text text;


    void OnIntChg(int c)
    {
        text.text = c.ToString();
    }
    void OnFloatChg(float c)
    {
        text.text = c.ToString();
    }
    void OnStrChg(string c)
    {
        text.text = c;
    }

    public void BreakMVConnect(string viewname, EventCenter model)
    {
        switch (dataType)
        {
            case GeneralDataType.Int:
                model.UnListenEvent<int>(UpdateEventName, OnIntChg);
                break;
            case GeneralDataType.Float:
                model.UnListenEvent<float>(UpdateEventName, OnFloatChg);
                break;
            case GeneralDataType.Str:
                model.UnListenEvent<string>(UpdateEventName, OnStrChg);
                break;
            default:
                break;
        }
    }

    public void BuildMVConnect(string viewname, EventCenter model)
    {
        switch (dataType)
        {
            case GeneralDataType.Int:
                model.ListenEvent<int>(UpdateEventName,OnIntChg);
                break;
            case GeneralDataType.Float:
                model.ListenEvent<float>(UpdateEventName, OnFloatChg);
                break;
            case GeneralDataType.Str:
                model.ListenEvent<string>(UpdateEventName, OnStrChg);
                break;
            default:
                break;
        }
    }

    public UI_ModelType GetModelType()
    {
        return type;
    }
}
