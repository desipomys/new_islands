using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveInfoShower:Base_Shower
{
    public RawImage pic;
    public Text createTime,lastTime,playedTime,remainPeople,saveName,isInGame;

    Ticker t = new Ticker(0.5f);
    string index;

   
    public void Show(SaveData data,string index)
    {
        gameObject.SetActive(true);
        this.index = index;

        createTime.text = "创建时间："+data.createTime.ToString();

        TimeSpan ts = new TimeSpan(0, 0, (int)data.playedTime);
        string temp = (ts.Hours==0?"":ts.Hours + "小时")+  (ts.Minutes==0?"":ts.Minutes + "分钟") + ts.Seconds + "秒";
        playedTime.text = "已玩："+temp;

        lastTime.text = "最后启动："+data.lastPlayTime.ToString();
        remainPeople.text = "剩余人数："+data.remainPeople.ToString();
        saveName.text = data.saveName;
        isInGame.text = data.isInGame ? "正在闯关" : "";
        pic.texture = getminmap(data.minmapPath);
    }
    public void OnHit()
    {
        Debug.Log("onhit");
        if (t.IsReady())//之前0.5S没点击
        {
            father.OnEvent(index, GetComponent<RectTransform>());
        }
        else
        {
            father.OnEvent(index);//双击事件
        }
        //EventCenter.WorldCenter.SendEvent<string,Transform>("UI_saveSlotSelect", index,transform);
    }



    Texture getminmap(string path)
    {
        Texture t = Resources.Load<Texture>("Textures/" + path);
        if (t == null) return EventCenter.WorldCenter.GetParm<string, Texture>("StrtoTexture", "gameIcon");
        return t;
    }
}