using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_UISynPosi : MonoBehaviour
{
    public Transform t;
    // Start is called before the first frame update
    void Start()
    {
        //transform.position = t.position;
        
    }
    IEnumerator ii(ItemScrollView_big ib)
    {
        yield return new WaitForSeconds(1);
 

        Item[,] t= new Item[3, 2];
        t[0, 0] = new Item(1, 1);
        ib.SetItems(t);

        StartCoroutine(wait());
    }

   IEnumerator wait()
    {
        yield return new WaitForSeconds(1);
       

        StartCoroutine(idd());
    }
    IEnumerator idd()
    {
        yield return new WaitForSeconds(1);
       
    }
    IEnumerator fff()
    {
        yield return new WaitForSeconds(1);
        
    }
}
