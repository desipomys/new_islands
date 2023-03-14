using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Start_CreateNewSave : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(onclick);
    }

    public void onclick()
    {
       string temp= EventCenter.WorldCenter.GetParm<string>(nameof(EventNames.CreateSaveAndGetName));

        Debug.Log("进入存档");
        SaveData sd = EventCenter.WorldCenter.GetParm<string, SaveData>(nameof(EventNames.GetSaveDataByIndex), temp);
        EventCenter.WorldCenter.SendEvent<SaveData>(nameof(EventNames.LoadSave), sd);

        UICenter.UIWorldCenter.ShowView("continue");
        UICenter.UIWorldCenter.ShowView("menu");
    }
}
