using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 要分解为只显示chardata，背包交给单独的itemscrollview
/// </summary>
public class PlayerNPCCharDataShower : MonoBehaviour,IUIInitReciver
{
    public GameObject charData, skillData;
    //public ItemScrollView itemView;//可能外置，不在chardatashower的gobj上
    //public ItemSlot[] equips;
    //public ItemShower[] showers;

    Dictionary<string, Text> skillText = new Dictionary<string, Text>();
    Dictionary<string, Text> charText = new Dictionary<string, Text>();
    
    public void UIInit(UICenter center, BaseUIView view)
    {
        for (int i = 0; i < charData.transform.childCount; i++)
        {
            charText.Add(charData.transform.GetChild(i).name, charData.transform.GetChild(i).GetChild(1).GetComponent<Text>());
        }
        for (int i = 0; i < skillData.transform.childCount; i++)
        {
            skillText.Add(skillData.transform.GetChild(i).name, skillData.transform.GetChild(i).GetChild(1).GetComponent<Text>());
        }
        
    }

    public void ShowChar(Charactor_Data data)
    {
        if (data == null) { ClearChar(); return; }
        charText["health"].text = data.health.ToString()+"/"+data.maxHealth.ToString();
        charText["food"].text = data.food.ToString() + "/" + data.maxFood.ToString();
        charText["power"].text = data.power.ToString() + "/" + data.maxPower.ToString();

        charText["def"].text = data.def.ToString();
        charText["speed"].text = data.speed.ToString();
        charText["healthup"].text = data.baseHealthUpRate.ToString();
        charText["powerup"].text = data.basePowerUpRate.ToString();
    }
    public void ClearChar()
    {
        charText["health"].text = "";
        charText["food"].text = "";
        charText["power"].text = "";

        charText["def"].text = "";
        charText["speed"].text = "";
        charText["healthup"].text = "";
        charText["powerup"].text = "";
    }
    public void ShowSkill(Charactor_Skill_Data data)
    {
        if (data == null) { ClearSkill();return; }
        skillText["power"].text = data.power.ToString();
        skillText["intell"].text = data.knowled.ToString();
        skillText["agile"].text = data.agile.ToString();
        skillText["physi"].text = data.physi.ToString();

        skillText["blade"].text = data.bladeSkill.ToString();
        skillText["hammer"].text = data.hammerSkill.ToString();
        skillText["bow"].text = data.bowSkill.ToString();
        skillText["rifle"].text = data.rifleSkill.ToString();
        skillText["auto"].text = data.autoSkill.ToString();
        skillText["explo"].text = data.exploSkill.ToString();
        skillText["fire"].text = data.fireSkill.ToString();
        skillText["ice"].text = data.iceSkill.ToString();
        skillText["electo"].text = data.electroSkill.ToString();
        skillText["radio"].text = data.radioSkill.ToString();
        skillText["posion"].text = data.posionSkill.ToString();
        skillText["corrup"].text = data.corruptionSkill.ToString();
        skillText["heal"].text = data.healSkill.ToString();
        skillText["nercoman"].text = data.necromanSkill.ToString();
        skillText["build"].text = data.buildSkill.ToString();

    }
    public void ClearSkill()
    {
        skillText["power"].text = "";
        skillText["intell"].text = "";
        skillText["agile"].text = "";
        skillText["physi"].text = "";

        skillText["blade"].text = "";
        skillText["hammer"].text = "";
        skillText["bow"].text = "";
        skillText["rifle"].text = "";
        skillText["auto"].text = "";
        skillText["explo"].text = "";
        skillText["fire"].text = "";
        skillText["ice"].text = "";
        skillText["electo"].text = "";
        skillText["radio"].text = "";
        skillText["posion"].text = "";
        skillText["corrup"].text = "";
        skillText["heal"].text = "";
        skillText["nercoman"].text = "";
        skillText["build"].text = "";
    }
   
   
    public void ShowNpcData(NpcData data)
    {
        if (data == null) return;
        ShowChar(data.char_data);
        ShowSkill(data.char_skill);
        //ShowItems(data.bp);//显示默认的NPC物品
    }

    
    /*public void Start()
{
   UIInit();
   ShowChar(new Charactor_Data());
   ShowSkill(new Charactor_Skill_Data());
}*/
}
