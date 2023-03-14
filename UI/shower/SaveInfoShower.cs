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

        createTime.text = "����ʱ�䣺"+data.createTime.ToString();

        TimeSpan ts = new TimeSpan(0, 0, (int)data.playedTime);
        string temp = (ts.Hours==0?"":ts.Hours + "Сʱ")+  (ts.Minutes==0?"":ts.Minutes + "����") + ts.Seconds + "��";
        playedTime.text = "���棺"+temp;

        lastTime.text = "���������"+data.lastPlayTime.ToString();
        remainPeople.text = "ʣ��������"+data.remainPeople.ToString();
        saveName.text = data.saveName;
        isInGame.text = data.isInGame ? "���ڴ���" : "";
        pic.texture = getminmap(data.minmapPath);
    }
    public void OnHit()
    {
        Debug.Log("onhit");
        if (t.IsReady())//֮ǰ0.5Sû���
        {
            father.OnEvent(index, GetComponent<RectTransform>());
        }
        else
        {
            father.OnEvent(index);//˫���¼�
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