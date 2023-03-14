using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_perlin : MonoBehaviour
{
    public GameObject g;
    // Start is called before the first frame update
    void Start()
    {
        int tem= XYHelper.ToCoord3(1, 2, 3);
        tem = tem.AddCoord3H(1);
        Debug.Log(tem.GetCoord3()[0] + "£¬" + tem.GetCoord3()[1] + "£¬" + tem.GetCoord3()[2]);

        tem = XYHelper.ToCoord3(-1, -1, -1);
        tem = tem.AddCoord3H(-1);
        Debug.Log(tem.GetCoord3()[0]+"£¬"+ tem.GetCoord3()[1] + "£¬" + tem.GetCoord3()[2]);
        tem = XYHelper.ToCoord3(-1, -1, -1);
        tem = tem.AddCoord3H(2);
        Debug.Log(tem.GetCoord3()[0] + "£¬" + tem.GetCoord3()[1] + "£¬" + tem.GetCoord3()[2]);
        tem = XYHelper.ToCoord3(-1, -1, 1);
        tem = tem.AddCoord3H(-3);
        Debug.Log(tem.GetCoord3()[0] + "£¬" + tem.GetCoord3()[1] + "£¬" + tem.GetCoord3()[2]);
        /*float[,] fs = new float[128, 128];
        for (int i = 0; i < fs.GetLength(0); i++)
        {
            for (int j = 0; j < fs.GetLength(1); j++)
            {
                fs[i,j] = MathCenter.basePerlin(i, j, 123.456f, 20, 128, 0);
                Instantiate(g, new Vector3(i, fs[i, j], j), transform.rotation, transform);
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
