using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class NewBehaviourScript : MonoBehaviour
{
    public int size,time;
    public float amplify,smooth;

    public float xoffset,yoffset;
    public Gradient g=new Gradient();
    public RawImage img;
    // Start is called before the first frame update
    void Start()
    {
        Texture2D t=new Texture2D(size,size);
        SetTexture(t,GenH(size));
        img.texture=t;
    }
    [Button("生成")]
    public void OnClick()
    {
Texture2D t=new Texture2D(size,size);
        SetTexture(t,GenH(size));
        img.texture=t;
    }

    void SetTexture(Texture2D t,float[,] f)
    {
        for (int i = 0; i < f.GetLength(0); i++)//hid
        {
            for (int j = 0; j < f.GetLength(1); j++)//wid
            {
                t.SetPixel(i,j,g.Evaluate(f[i,j]));
                
            }          
        }
        t.Apply();
        t.filterMode=FilterMode.Point;
    }

    float[,] GenH(int size)
    {
        float[,] ans=new float[size,size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
               ans[i,j]= basePerlin(i, j,xoffset,yoffset, smooth,time)*amplify;
               //Debug.Log(ans[i,j]);
            }
        }

        return ans;
    }

/// <summary>
/// 需根据x,y
/// 返回0-1间数
/// </summary>
/// <param name="bx">可以是0~any</param>
/// <param name="by"></param>
/// <param name="xoffset"></param>
/// <param name="yoffset"></param>
/// <param name="smooth"></param>
/// <param name="stackTime">进行几次叠加</param>
/// <returns></returns>
    public static float basePerlin(float bx, float by,float xoffset,  float yoffset, float smooth,int stackTime)
    {
        //float distance = Mathf.Sqrt((bx ) * (bx ) + (by ) * (by ));

        //cx += (int)(width / Chunk.ChunkSize) / (2 * Chunk.ChunkSize);
        //cy += (int)(height / Chunk.ChunkSize) / (2 * Chunk.ChunkSize); ;
        
      
        float seed = Mathf.PingPong(10.GetHashCode() >> 16, 2);
        float tempamp =1;
        float tempsmooth=smooth;
        float normalizeAmp=1;

        float f = tempamp * (Mathf.PerlinNoise(1 + ((bx+xoffset) * Mathf.PI + seed) / smooth, 1 + ((by+yoffset) * Mathf.PI + seed) / smooth) - 0.5f);
        for (int i = 0; i < stackTime; i++)
        {
            tempsmooth /= 2;
            tempamp /= 2;
            f += tempamp * (Mathf.PerlinNoise(1 + ((bx+xoffset) * Mathf.PI + seed) / tempsmooth, 1 + ((by+yoffset) * Mathf.PI + seed) / tempsmooth) - 0.5f);
            normalizeAmp += tempamp;
            
        }


        return f/normalizeAmp;
    }
}
