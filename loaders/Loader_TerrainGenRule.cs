using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader_TerrainGenRule : BaseLoader
{

    string path = "SC/NATUREEBLOCK_GENRULE";

    public override void OnEventRegist(EventCenter e)
    {
        base.OnEventRegist(e);
    }
    public override void OnLoaderInit(int prio)
    {
        if (prio != 0) return;
        NatureEBlockGenRule[] rules=Resources.LoadAll<NatureEBlockGenRule>(path);


    }
}
