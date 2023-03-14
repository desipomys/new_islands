using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//挂在静态canvas下
public class MovShell : BaseUIView
{
    public Text resultText;
    public InputField commandText;
    public Dropdown movengines;

    MovScriptEngine engine;

    List<string> history = new List<string>();
    int hisIndex = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnExecute();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)&& commandText.isFocused)
        {
            OnDownArrow();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)&& commandText.isFocused)
        {
            OnUpArrow();
        }
        
            
        
    }

    void OnUpArrow()
    {
        
        if (history.Count == 0) return;
        hisIndex = Mathf.Clamp(hisIndex - 1, 0, history.Count );
        if (hisIndex < 0 || hisIndex >= history.Count) commandText.text = "";
        else commandText.text = history[hisIndex];
        commandText.ActivateInputField();
        commandText.caretPosition=commandText.text.Length;
    }
    void OnDownArrow()
    {
        
        if (history.Count == 0) return;
        hisIndex = Mathf.Clamp(hisIndex + 1, 0, history.Count);
        if (hisIndex < 0 || hisIndex >= history.Count) commandText.text = "";
        else commandText.text = history[hisIndex];
        commandText.ActivateInputField();
        commandText.caretPosition=commandText.text.Length;
    }
    public void OnUIOpen(UICenter center, BaseUIView view)
    {
         commandText.ActivateInputField();
        string[] temp=EventCenter.WorldCenter.GetParm<string[]>(nameof(EventNames.GetAllMovEngineName));
        movengines.ClearOptions();
        movengines.AddOptions(new List<string>(temp));
    }
   
    public void OnMouseOver()
    {
        //显示代码提示
    }
    public void OnTab()
    {
        //代码补全

    }
    public void Clear()
    {
        commandText.text = "";
        commandText.ActivateInputField();
    }
    public void OnDropDown()
    {
        //获取movengine container中的所有engine名字生成dropdown list
        string[] temp=EventCenter.WorldCenter.GetParm<string[]>(nameof(EventNames.GetAllMovEngineName));
        movengines.AddOptions(new List<string>(temp));
    }
    public void OnDropDownSelect(string value)
    {
        //选中dropdown list中某一列时
        engine = EventCenter.WorldCenter.GetParm<string, MovScriptEngine>(nameof(EventNames.GetMovEngineByName), value);
    }
    public void OnExecute()
    {
        //按下回车
        if (check(commandText.text))
        {
            try
            {
                string[] parmAndName=commandText.text.Split(' ');
                if(parmAndName.Length>1)
                {
                    string[] temp=new string[parmAndName.Length-1];
                    Array.Copy(parmAndName,1,temp,0,parmAndName.Length-1);
                    MovScriptCommand.RunFromSimpleStr(parmAndName[0],temp,EventCenter.WorldCenter,engine);
                }else{
                    MovScriptCommand.RunFromSimpleStr(parmAndName[0],null,EventCenter.WorldCenter,engine);
                }

                displayResult(commandText.text, true);
                history.Add(commandText.text);
                hisIndex = history.Count ;
            }
            catch (System.Exception e)
            {
                displayResult(commandText.text, false,e.Message);
                history.Add(commandText.text);
                hisIndex = history.Count ;
            }

        }
        commandText.text = "";
        commandText.ActivateInputField();
    }
    bool check(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return false;//空命令返回
        return true;
    }

    public void OnPressScan()
    {
        //当扫描按键按下，显示屏幕中指向实体evc的UUID
    }

    void displayResult(string command, bool result,string reason="")
    {
        if (result)
        {
            resultText.text +=command + "执行成功"+  "\n" ;
        }
        else
        {
            resultText.text += command + "执行失败,原因为:"+reason+ "\n" ;
        }
    }

    
}