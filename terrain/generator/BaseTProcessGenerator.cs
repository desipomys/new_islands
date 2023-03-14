using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// generator负责生成复杂数据类型，并自己承担写入功能的调用
/// </summary>
public class BaseTProcessGenerator 
{
    protected ChunkFileWriter chunkFileWriter;
    protected FieldWriter fieldWriter;
    protected EBFileWriter eBFileWriter;
    protected EntityFileWriter entityFileWriter;

    protected int width, height;
    protected MapPrefabsData mpd;
    protected string saveName;
    public virtual void Init(MapPrefabsData data,ChunkFileWriter c,FieldWriter f,EBFileWriter eb,EntityFileWriter en)
    {
        mpd = data;
        width = data.config.width;
        height = data.config.height;
        this.saveName = EventCenter.WorldCenter.GetParm<string>(nameof(EventNames.ThisSavePath));
        chunkFileWriter = c;
        fieldWriter = f;
        eBFileWriter = eb;
        entityFileWriter = en;
    }
    public virtual void DeInit()
    {

    }
}
