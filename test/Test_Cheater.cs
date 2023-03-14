using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Cheater : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            bool b = EventCenter.WorldCenter.GetParm<bool>(nameof(EventNames.IsCheatMode));
            Debug.Log("作弊模式设为" + !b);
            EventCenter.WorldCenter.SendEvent<bool>(nameof(EventNames.SetCheat), !b);

            GetComponent<EventCenter>().SendEvent<bool>("set_isCheat", !b);
        }
    }
}
