using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_TextShow : MonoBehaviour
{
    Text t;


    void Start()
    {
        t = GetComponent<Text>();
        EventCenter.WorldCenter.ListenEvent<string>("UIDebugInfo", (string s) => { t.text = s; });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
