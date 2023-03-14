using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
/// <summary>
/// ���ɽ��족�ű�
/// �����1��������ǰ�����ݣ�����Ѫ��������ӵ�item���洢������2��ɽ���ǰ�����ӣ�ȫ�������滻Ϊĳshader����3�������Ϊ����õ����ӡ�
///1.����ʱ�����ɸ�eblock��gameobj����ʼ�����ɽ��족�ű�����eblock��Ϊ���񣬴�ʱ����ᱣ��ջ�Ĭ����������+�ɽ���ű�������
///2.������������buildcomplite�������������������Ϊʵ�񣬴�ʱ���棺�ջ�Ĭ����������+�տɽ���ű�������
/// </summary>
public class BuildAbleEBlock : MonoBehaviour,IEventRegister,IDataContainer
{

    public float process,endtime;//�ѽ�����Ⱥͽ��������ʱ
    bool inited = false;
    EventCenter center;

    Material virtualmat;
    Dictionary<Renderer, Material[]> renderAndMats = new Dictionary<Renderer, Material[]>();//����õĽ�����ÿ��render�����ڲ��Ĳ��ʵĶ�Ӧ

    public void AfterEventRegist()
    {
       
    }

    public void Init()
    {//�������������߼�
        Renderer[] rds = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rds.Length; i++)
        {
            renderAndMats.Add(rds[i], rds[i].materials);
            Material[] mats = new Material[rds[i].materials.Length];
            for (int j = 0; j < mats.Length; j++)
            {
                mats[j] = virtualmat;
            }
            rds[i].materials=mats;
        }
        inited = true;
        center.SendEvent("StartBuildInited");
    }
    void ChangeToCompleteStat()
    {
        Renderer[] rds = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rds.Length; i++)
        {
            rds[i].materials = renderAndMats[rds[i]];
        }
    }
    public void buildComplete()
    {//��ʵ��
        Debug.Log("�������");
        ChangeToCompleteStat();
        center.SendEvent("buildComplete");
    }

    public void OnEventRegist(EventCenter e)
    {
        virtualmat = Resources.Load<Material>("material/Building/SketchBlock");
        center = e;
    }
    ValueChangeParm<float> fp = new ValueChangeParm<float>();

    public virtual int GetDataCollectPrio => 0;

    void Update()
    {
        if (!inited) return;
        if (process >= endtime) return;
        {
            fp.old = process / endtime;
            process += Time.deltaTime;//����Ҫ�ڽ��ȱ仯ʱ�������������
            fp.now = process / endtime;
            fp.max = endtime;
            center.SendEvent<ValueChangeParm<float>>("processGo",fp);
        }
        if (process >= endtime) buildComplete();

    }

    public void FromObject(object data)
    {
        JArray j = (JArray)data;
        process = j[0].ToObject<float>();
        endtime = j[1].ToObject<float>();
        if (process < endtime) Init();
    }

    public object ToObject()
    {
        return new float[] { process, endtime };
    }
}
