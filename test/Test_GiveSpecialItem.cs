using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_GiveSpecialItem : MonoBehaviour
{
    EventCenter center;
    void Start()
    {
        center = GetComponent<EventCenter>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Item t = new Item(1, 1);
            t.level = Random.Range(0, 3072);
            //t.SetContent(ItemContent.OnUIExchange, new string[] { "popmsg -deeee" });
            center.GetParm<Item, int>(nameof(PlayerEventName.giveItem),t);
        }
    }
}
