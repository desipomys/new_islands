using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader_Texture : BaseLoader
{
    public Dictionary<string,Texture> alltextures=new Dictionary<string, Texture>();
    Texture defaultTexture;

    string path= "Textures/";
    string defaultTexturePath = "Textures/default";

    #region Init
    public override void OnEventRegist(EventCenter e)
    {
       center=e;
       e.RegistFunc<string,Texture>(nameof(EventNames.StrtoTexture),StrToTexture);
        
    }

    public override void OnLoaderInit(int prio)
    {
        if(prio!=0)return;
        try
        {
            Texture[] t=Resources.LoadAll<Texture>(path);
        
            for(int i=0;i<t.Length;i++)
            {

                alltextures.Add(t[i].name,t[i]);
                //Debug.Log(t[i].name);
            }
            defaultTexture = Resources.Load<Texture>(defaultTexturePath);
            Debug.Log("textureloader加载完成，有"+t.Length+"个texture");
        }
        catch (System.Exception)
        {
            Debug.Log("texture加载失败");
        }
        
    }
    #endregion

    public Texture StrToTexture(string name)
    {
        if (string.IsNullOrEmpty(name)) { Debug.Log("无此材质"+name); return defaultTexture; }
        Texture t;
        if(alltextures.TryGetValue(name,out t)){
            return t;
        }
        return defaultTexture;
    }
    public Texture BMatToTexture(int bmat)
    {
        if (bmat==0) return defaultTexture;
        Texture t;
        if (alltextures.TryGetValue(((B_Material)bmat).ToString(), out t))
        {
            return t;
        }
        return defaultTexture;
    }
}