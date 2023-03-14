using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWareHouse_controller : BaseUIController
{
    public override void Flush()
    {
        base.Flush();
        ItemScrollView temp = ((StartWareHouse_View)view).scrollView;
        temp.Flush();
    }
}
