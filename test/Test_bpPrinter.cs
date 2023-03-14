using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_bpPrinter : MonoBehaviour
{
    BaseBackPack bp;
    int index=0;
    void Awake()
    {
        bp = GetComponent<BaseBackPack>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            string ans = "";
            int[,] placements = bp.itemPages[index].GetPlacements();
            for (int i = 0; i < placements.GetLength(0); i++)
            {
                for (int j = 0; j < placements.GetLength(1); j++)
                {
                    ans += placements[i,j].ToString() + " ";
                }
                ans += "\n";
            }
            Debug.Log(ans);
        }
        if(Input.GetKeyDown(KeyCode.I))
        {
            Item[] its = bp.itemPages[index].GetItems();
            for (int i = 0; i < its.Length; i++)
            {
                Debug.Log(its[i]);
            }
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            index = (int)Mathf.Repeat(index + 1, bp.itemPages.Count);
            Debug.Log("当前背包page=" + index);
        }
    }
}
