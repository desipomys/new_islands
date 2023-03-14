using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// random���е�д�빦��,ֻ����������߶ȳ��������������͵�ȫͼ����
/// </summary>
public class TerrainHRandom 
{
    //���ɵ��θ߶���ص����ֵ
    //����perlin���
    //terrianForm���
    //MapFrom���
    //����ϸ�����
    //ֻ������ṩ��ȡȫͼfloat[,]����
    int width, height;
    public float amplify = 30, smooth = 512, Offset = 10, Seed;

    public float[,] GetMapH(MapPrefabsData data)
    {
        float[,] tblockHeight = new float[data.config.width, data.config.height];
        width = data.config.width;
        height = data.config.height;

        GenBasePerlin(tblockHeight, data);
        GenTerrianForm(tblockHeight, data);
        //GenMapForm(tblockHeight, data);
        randomPerlin(tblockHeight, data);
        return tblockHeight;
    }

   
    float mapPerlin(int bx, int by, MapPrefabsData dt)
    {//bx,by���Ե�ͼ����Ϊԭ�������
        float maskx = bx * 1.0f * 2 / width;
        float masky = by * 1.0f * 2 / height;//��һ��Ϊ-1~+1

        switch (dt.config.mform)
        {
            case MapForm.island:
                //   -------------  -  ��״����ߵ�Ϊ1����͵�Ϊ0�İ뾶Ϊ��ͼ�뾶��Բ׶
                float f = Mathf.Clamp(Mathf.Sqrt(maskx * maskx + masky * masky), 0f, 1f);
                if ((maskx == 0 && masky == 0) || (maskx == 0.5f && masky == 0.5f))
                {
                    Debug.Log(f);
                }
                return 1 - f;
                break;
            default:
                return 1;
                break;
        }

    }

    /// <summary>
    /// ����perlin
    /// </summary>
    /// <param name="bx"></param>
    /// <param name="by"></param>
    /// <param name="form"></param>
    /// <returns></returns>
    float terrainPerlin(int bx, int by, TerrainForm form)
    {
        return 1;
    }

    void randomPerlin(float[,] tblockH, MapPrefabsData data)
    {
        //����mapprefabsdata��ָ����������͸�����������

    }

    void GenBasePerlin(float[,] highs, MapPrefabsData data)
    {
        for (int i = 0; i < width; i++)//���ɻ���perlin
        {
            for (int j = 0; j < height; j++)
            {
                highs[i, j] = MathCenter.basePerlin(i - width / 2, j - height / 2, 123.456f,amplify,smooth,Offset);
            }
        }
    }
    void GenTerrianForm(float[,] highs, MapPrefabsData data)
    {
        for (int i = 0; i < width; i++)//terrainformӦ��
        {
            for (int j = 0; j < height; j++)
            {
                highs[i, j] *= terrainPerlin(i - width / 2, j - height / 2, data.config.tform);
            }
        }
    }
    void GenMapForm(float[,] highs, MapPrefabsData data)
    {
        for (int i = 0; i < width; i++)//mapformӦ��
        {
            for (int j = 0; j < height; j++)
            {
                float temp = mapPerlin(i - width / 2, j - height / 2, data);
                highs[i, j] *= temp;//���ĵ���Ե����ݼ�
                highs[i, j] += (1 - temp) * (data.config.sea - 10);//�½���ƽ��,�����ƽ�����������
            }
        }
    }

}
