using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
/// <summary>
/// “可建造”脚本
/// 负责把1建筑建造前的数据（工地血量和已添加的item）存储起来、2变成建造前的样子（全部材质替换为某shader）、3从虚像变为建造好的样子。
///1.建造时先生成该eblock的gameobj，初始化“可建造”脚本将该eblock变为虚像，此时保存会保存空或默认主体数据+可建造脚本的数据
///2.进度走完后调用buildcomplite方法，将建筑从虚像变为实像，此时保存：空或默认主体数据+空可建造脚本的数据
/// </summary>
public class BuildAbleEBlock : MonoBehaviour,IEventRegister,IDataContainer
{

    public float process,endtime;//已建造进度和建造完成用时
    bool inited = false;
    EventCenter center;

    Material virtualmat;
    Dictionary<Renderer, Material[]> renderAndMats = new Dictionary<Renderer, Material[]>();//建造好的建筑里每个render及其内部的材质的对应

    public void AfterEventRegist()
    {
       
    }

    public void Init()
    {//单纯变虚像，无逻辑
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
    {//变实像
        Debug.Log("建造完毕");
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
            process += Time.deltaTime;//将来要在进度变化时生成世界进度条
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
