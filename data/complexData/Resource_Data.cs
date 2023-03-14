using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class Resource_Data 
{//��ԴӦ���Ǹ��������ʵ�ֻ��������
    //���ʡ���Դ����������Ϣ
    /*
    ���ʿɵǵ��ж����ա�������������
    ��Դͬ��
    �����ɸ���ʣ���������лظ�
    ��Ϣ��ͨ���������ʹ���鼮���ǵ��ж��Ǽ��������о�����
    */
    public float material ;
    public float energy ;
    public float manPower ;
    public float information ;
    
    public Resource_Data() { }
    public Resource_Data(float w,float m,float e,float f)
    {
        material = w;
        energy = m;
        manPower = e;
        information = f;
    }
    public Resource_Data(Resource_Data rd)
    {
        material = rd.material;
        energy = rd.energy;
        manPower = rd.manPower;
        information = rd.information;
    }

    /* public float max_resource;
     public float max_build_mat;
     public float max_energy;
     public float max_food;
     public float max_metal;
     public float max_medical;*/
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
    public static Resource_Data FromString(string data)
    {
        return JsonConvert.DeserializeObject<Resource_Data>(data);
    }
    public static Resource_Data operator- (Resource_Data a,Resource_Data b)
    {
        a.energy -= b.energy;
        a.manPower-=b.manPower;
        a.material -= b.material;
        a.information -= b.information;
        return a;
    }
}