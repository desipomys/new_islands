using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Touch : MonoBehaviour,IInterectable
{
    public int OnInterect(EventCenter source, InteractType type)
    {
        Debug.Log("touch");
        return 1;
    }

   
}
