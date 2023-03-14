using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerEquipPos
{
    hat,
    hair_up,
    hair_low,
    head,
    glass,
    body,
    arm,
    leg,
    bodyeq_in,
    bodyeq_out,
    armeq,
    legeq,
    pant
}

[CreateAssetMenu(menuName = "dataEditor/Equipment")]
public class SC_Equipment : ScriptableObject
{
    public int itemID;
    public string abstractName;
    public PlayerEquipPos[] partPos;
    public string[] partname;
    public Texture[] partTexture;
    [Tooltip("默认urplit")]
    public Material[] partMat;
}
