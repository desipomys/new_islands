using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_itemGiver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if (!transform.parent.GetComponent<Static_View>().shellActive)
            {
                Item i = new Item(EventCenter.WorldCenter.GetParm<Item>("GetRandomItem"));
                i.num = 1;
                EventCenter.WorldCenter.SendEvent<Item>("SetMouseItem", i);
            }
        }
        
    }
}
